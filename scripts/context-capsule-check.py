#!/usr/bin/env python3
"""Fail-closed validator for Accepted-Contract Capsule v1."""
from __future__ import annotations

import argparse
import copy
import glob
import hashlib
import json
from pathlib import Path
import re
import sys
import tempfile

SCHEMA = "arkus.accepted-contract-capsule@1"
INDEX_SCHEMA = "arkus.accepted-contract-capsule-index@1"
AUTHORITY = "NON_AUTHORITATIVE_NAVIGATION_ONLY"
SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)

class CapsuleError(ValueError):
    pass

def load_json(path: Path) -> dict:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except FileNotFoundError as exc:
        raise CapsuleError(f"missing JSON: {path}") from exc
    except json.JSONDecodeError as exc:
        raise CapsuleError(f"malformed JSON: {path}: {exc}") from exc
    if not isinstance(data, dict):
        raise CapsuleError(f"expected object root: {path}")
    return data

def git_blob_sha(data: bytes) -> str:
    return hashlib.sha1(f"blob {len(data)}\0".encode("utf-8") + data).hexdigest()

def read_bound_source(repo_root: Path, source: dict, *, label: str) -> bytes:
    if not isinstance(source, dict):
        raise CapsuleError(f"{label} must be an object")
    path = source.get("path")
    expected = source.get("git_blob_sha")
    if not isinstance(path, str) or not path:
        raise CapsuleError(f"{label}.path must be non-empty")
    if not isinstance(expected, str) or not SHA_RE.fullmatch(expected):
        raise CapsuleError(f"{label}.git_blob_sha must be exact 40-char git blob SHA")
    if "kind" in source and not isinstance(source["kind"], str):
        raise CapsuleError(f"{label}.kind must be a string when present")
    full = repo_root / path
    try:
        data = full.read_bytes()
    except FileNotFoundError as exc:
        raise CapsuleError(f"{label} missing authoritative source: {path}") from exc
    actual = git_blob_sha(data)
    if actual.lower() != expected.lower():
        raise CapsuleError(
            f"{label} source fingerprint mismatch for {path}: "
            f"capsule={expected.lower()} actual={actual.lower()}; reconstruct from authoritative source"
        )
    return data

def strip_md(value: str) -> str:
    return " ".join(value.replace("**", "").replace("`", "").split()).strip()

def parse_completion(text: str) -> dict:
    if not re.search(r"(?mi)^Status:\s*\**COMPLETE\**\s*$", text):
        raise CapsuleError("identity source is not COMPLETE")
    candidate_patterns = [
        r"(?mi)^Accepted candidate:\s*`?([0-9a-f]{40})`?",
        r"(?mi)^\s*-\s*Reviewed candidate SHA:\s*`?([0-9a-f]{40})`?",
        r"(?mi)^Reviewed candidate SHA:\s*`?([0-9a-f]{40})`?",
    ]
    merge_patterns = [
        r"(?mi)^Merged:.*?merge commit\s*`?([0-9a-f]{40})`?",
        r"(?mi)^\s*-\s*Merge SHA:\s*`?([0-9a-f]{40})`?",
        r"(?mi)^Merge SHA:\s*`?([0-9a-f]{40})`?",
    ]
    inline_pass_review = re.search(
        r"(?mi)^Independent review:\s*(?:\*\*)?PASS(?:\*\*)?\s*,?.*?review\s*`?#?(\d+)`?",
        text,
    )
    review_id = inline_pass_review.group(1).lower() if inline_pass_review else None
    if review_id is None:
        has_pass_verdict = re.search(
            r"(?mi)^\s*(?:-\s*)?(?:Independent\s+)?Reviewer verdict:\s*`?(?:\*\*)?PASS(?:\*\*)?`?\s*$",
            text,
        )
        if not has_pass_verdict:
            raise CapsuleError("identity source missing independent PASS review verdict")
        review_patterns = [
            r"(?mi)^\s*-\s*Reviewer evidence:\s*PR review\s*`?#?(\d+)`?",
            r"(?mi)^Reviewer evidence:\s*PR review\s*`?#?(\d+)`?",
        ]
        for pattern in review_patterns:
            match = re.search(pattern, text)
            if match:
                review_id = match.group(1).lower()
                break
        if review_id is None:
            raise CapsuleError("identity source missing independent PASS review id")
    def first(patterns, name):
        for pattern in patterns:
            match = re.search(pattern, text)
            if match:
                return match.group(1).lower()
        raise CapsuleError(f"identity source missing {name}")
    return {
        "state": "ACCEPTED",
        "reviewed_candidate_sha": first(candidate_patterns, "reviewed/accepted candidate SHA"),
        "merge_sha": first(merge_patterns, "merge SHA"),
        "review_id": review_id,
    }

def parse_disposition_table(text: str, spec: dict) -> dict[str, str]:
    section = spec.get("section")
    key_column = spec.get("key_column")
    status_column = spec.get("status_column")
    if not isinstance(section, str) or not section.startswith("## "):
        raise CapsuleError("disposition_source.section must be an exact level-2 heading")
    if not isinstance(key_column, int) or not isinstance(status_column, int):
        raise CapsuleError("disposition_source key/status columns must be integers")
    lines = text.splitlines()
    try:
        start = next(i for i, line in enumerate(lines) if line.strip() == section)
    except StopIteration as exc:
        raise CapsuleError(f"structured source section not found: {section}") from exc
    rows: dict[str, str] = {}
    for line in lines[start + 1:]:
        if line.startswith("## "):
            break
        s = line.strip()
        if not (s.startswith("|") and s.endswith("|")):
            continue
        cells = [c.strip() for c in s[1:-1].split("|")]
        if not cells:
            continue
        if all(re.fullmatch(r":?-{3,}:?", c.replace(" ", "")) for c in cells):
            continue
        if max(key_column, status_column) >= len(cells):
            continue
        key = strip_md(cells[key_column])
        status = strip_md(cells[status_column])
        if key.lower() in {"id", "relationship/mechanism"} or status.lower() in {"status", "disposition"}:
            continue
        if not key or not status:
            continue
        if key in rows:
            raise CapsuleError(f"duplicate disposition source key in {section}: {key}")
        rows[key] = status
    if not rows:
        raise CapsuleError(f"no disposition rows parsed from {section}")
    return rows

def validate_basic_shape(capsule: dict) -> None:
    if capsule.get("schema") != SCHEMA:
        raise CapsuleError("unexpected capsule schema")
    if capsule.get("authority") != AUTHORITY:
        raise CapsuleError("capsule must remain NON_AUTHORITATIVE_NAVIGATION_ONLY")
    cid = capsule.get("capsule_id")
    if not isinstance(cid, str) or not cid:
        raise CapsuleError("capsule_id required")
    if capsule.get("track") not in {"H0", "H1", "CITY", "PA", "CTX"}:
        raise CapsuleError(f"{cid}: unsupported track")
    if capsule.get("content_mode") not in {"boundary_summary", "structured_disposition"}:
        raise CapsuleError(f"{cid}: content_mode must be boundary_summary or structured_disposition")
    identity = capsule.get("accepted_identity")
    if not isinstance(identity, dict):
        raise CapsuleError(f"{cid}: accepted_identity required")
    required_identity = {"reviewed_candidate_sha", "merge_sha", "review_id"}
    if set(identity) != required_identity:
        raise CapsuleError(f"{cid}: accepted_identity must contain exactly reviewed_candidate_sha, merge_sha, review_id")
    for key in ("reviewed_candidate_sha", "merge_sha"):
        value = identity.get(key)
        if not isinstance(value, str) or not SHA_RE.fullmatch(value):
            raise CapsuleError(f"{cid}: accepted_identity.{key} must be exact SHA")
    if not isinstance(identity.get("review_id"), str) or not identity["review_id"].isdigit():
        raise CapsuleError(f"{cid}: accepted_identity.review_id must be numeric string")
    if not isinstance(capsule.get("mandatory_source_reads"), list):
        raise CapsuleError(f"{cid}: mandatory_source_reads must be a list")
    if "consumer_hints" in capsule:
        hints = capsule["consumer_hints"]
        if not isinstance(hints, list) or any(not isinstance(item, str) or not item.strip() for item in hints):
            raise CapsuleError(f"{cid}: consumer_hints must contain only non-empty strings")
    for field in ("exported_guarantees", "exclusions_nonclaims", "reopen_conditions", "escalate_if"):
        value = capsule.get(field)
        if not isinstance(value, list) or not value:
            raise CapsuleError(f"{cid}: {field} must be a non-empty list")
    for field in ("reopen_conditions", "escalate_if"):
        for i, entry in enumerate(capsule[field]):
            if not isinstance(entry, str) or not entry.strip():
                raise CapsuleError(f"{cid}: {field}[{i}] must be a non-empty string")
    for field in ("exported_guarantees", "exclusions_nonclaims"):
        ids = set()
        for entry in capsule[field]:
            if not isinstance(entry, dict) or not isinstance(entry.get("id"), str) or not entry["id"]:
                raise CapsuleError(f"{cid}: {field} entries need id")
            if entry["id"] in ids:
                raise CapsuleError(f"{cid}: duplicate {field} id {entry['id']}")
            ids.add(entry["id"])
            if not isinstance(entry.get("statement"), str) or not entry["statement"].strip():
                raise CapsuleError(f"{cid}: {field} entry {entry['id']} needs statement")
            if "source_pointer" in entry and not isinstance(entry["source_pointer"], str):
                raise CapsuleError(f"{cid}: {field} entry {entry['id']} source_pointer must be a string")
    if "disposition_source" in capsule and not isinstance(capsule["disposition_source"], dict):
        raise CapsuleError(f"{cid}: disposition_source must be an object when present")
    if "dispositions" in capsule and not isinstance(capsule["dispositions"], list):
        raise CapsuleError(f"{cid}: dispositions must be a list when present")
    if "directional_semantics" in capsule and not isinstance(capsule["directional_semantics"], list):
        raise CapsuleError(f"{cid}: directional_semantics must be a list when present")

def validate_identity(repo_root: Path, capsule: dict, live_state: dict | None) -> dict:
    cid = capsule["capsule_id"]
    data = read_bound_source(repo_root, capsule.get("identity_source"), label=f"{cid}.identity_source")
    parsed = parse_completion(data.decode("utf-8"))
    expected = capsule["accepted_identity"]
    for key in ("reviewed_candidate_sha", "merge_sha", "review_id"):
        if parsed[key].lower() != str(expected[key]).lower():
            raise CapsuleError(f"{cid}: accepted identity mismatch for {key}: capsule={expected[key]} completion={parsed[key]}")
    live_checked = False
    if live_state is not None:
        entries = live_state.get("capsules")
        if not isinstance(entries, dict) or cid not in entries:
            raise CapsuleError(f"{cid}: live accepted-state input has no capsule entry")
        live = entries[cid]
        if not isinstance(live, dict):
            raise CapsuleError(f"{cid}: live accepted-state entry malformed")
        if live.get("state") != "ACCEPTED":
            raise CapsuleError(f"{cid}: live accepted state is {live.get('state')!r}, not ACCEPTED; capsule unusable and predecessor must be reconstructed/reopened")
        for key in ("reviewed_candidate_sha", "merge_sha"):
            if str(live.get(key, "")).lower() != expected[key].lower():
                raise CapsuleError(f"{cid}: live exact-SHA mismatch for {key}; capsule unusable and authoritative reconstruction required")
        live_checked = True
    return {"completion_identity_checked": True, "live_state_checked": live_checked}

def validate_sources(repo_root: Path, capsule: dict) -> None:
    cid = capsule["capsule_id"]
    sources = capsule.get("authoritative_sources")
    if not isinstance(sources, list) or not sources:
        raise CapsuleError(f"{cid}: authoritative_sources must be non-empty")
    seen = set()
    for i, source in enumerate(sources):
        read_bound_source(repo_root, source, label=f"{cid}.authoritative_sources[{i}]")
        path = source["path"]
        if path in seen:
            raise CapsuleError(f"{cid}: duplicate authoritative source {path}")
        seen.add(path)

def validate_mandatory_reads(repo_root: Path, capsule: dict) -> None:
    cid = capsule["capsule_id"]
    for i, source in enumerate(capsule["mandatory_source_reads"]):
        label = f"{cid}.mandatory_source_reads[{i}]"
        if not isinstance(source, dict):
            raise CapsuleError(f"{label} must be an object")
        if source.get("noncompressible") is not True:
            raise CapsuleError(f"{label}.noncompressible must be true")
        reason = source.get("reason")
        if not isinstance(reason, str) or not reason.strip():
            raise CapsuleError(f"{label}.reason must be a non-empty string")
        read_bound_source(repo_root, source, label=label)

def validate_dispositions(repo_root: Path, capsule: dict) -> None:
    cid = capsule["capsule_id"]
    spec = capsule.get("disposition_source")
    dispositions = capsule.get("dispositions")
    if spec is None and dispositions is None:
        return
    if not isinstance(spec, dict) or not isinstance(dispositions, list):
        raise CapsuleError(f"{cid}: disposition_source and dispositions must appear together")
    data = read_bound_source(repo_root, spec, label=f"{cid}.disposition_source")
    source_rows = parse_disposition_table(data.decode("utf-8"), spec)
    capsule_rows: dict[str, str] = {}
    for row in dispositions:
        if not isinstance(row, dict):
            raise CapsuleError(f"{cid}: disposition row must be object")
        key = row.get("source_key")
        status = row.get("status")
        if not isinstance(key, str) or not key.strip() or not isinstance(status, str) or not status.strip():
            raise CapsuleError(f"{cid}: disposition row requires non-empty source_key/status")
        if key in capsule_rows:
            raise CapsuleError(f"{cid}: duplicate capsule disposition key {key}")
        capsule_rows[key] = strip_md(status)
    if capsule_rows != source_rows:
        missing = sorted(set(source_rows) - set(capsule_rows))
        extra = sorted(set(capsule_rows) - set(source_rows))
        changed = sorted(key for key in set(source_rows) & set(capsule_rows) if source_rows[key] != capsule_rows[key])
        detail = []
        if missing:
            detail.append("missing=" + repr(missing))
        if extra:
            detail.append("extra=" + repr(extra))
        if changed:
            detail.append("changed=" + repr({key: {"source": source_rows[key], "capsule": capsule_rows[key]} for key in changed}))
        raise CapsuleError(f"{cid}: structured disposition coverage is lossy: " + "; ".join(detail))

def validate_directionality(capsule: dict) -> None:
    cid = capsule["capsule_id"]
    items = capsule.get("directional_semantics", [])
    for i, item in enumerate(items):
        if not isinstance(item, dict):
            raise CapsuleError(f"{cid}: directional_semantics[{i}] must be an object")
        if item.get("must_remain_distinct") is not True:
            raise CapsuleError(f"{cid}: directional semantic must declare must_remain_distinct=true")
        semantic_id = item.get("id")
        forward_raw = item.get("forward")
        reverse_raw = item.get("reverse")
        if not isinstance(semantic_id, str):
            raise CapsuleError(f"{cid}: directional semantic id must be a string")
        if not isinstance(forward_raw, str) or not isinstance(reverse_raw, str):
            raise CapsuleError(f"{cid}: directional semantic forward/reverse must be strings")
        forward = strip_md(forward_raw)
        reverse = strip_md(reverse_raw)
        if not forward or not reverse or forward == reverse:
            raise CapsuleError(f"{cid}: asymmetric semantics collapsed: {forward!r} vs {reverse!r}")

def validate_city_noncompressible(repo_root: Path, capsule: dict) -> None:
    if capsule["track"] != "CITY":
        return
    if capsule.get("content_mode") != "boundary_summary":
        raise CapsuleError(f"{capsule['capsule_id']}: CITY capsule must be boundary_summary")
    reads = capsule["mandatory_source_reads"]
    if not reads:
        raise CapsuleError(f"{capsule['capsule_id']}: CITY boundary capsule must preserve mandatory non-compressible source reads")

def validate_capsule(repo_root: Path, capsule: dict, live_state: dict | None = None) -> dict:
    validate_basic_shape(capsule)
    identity_result = validate_identity(repo_root, capsule, live_state)
    validate_sources(repo_root, capsule)
    validate_mandatory_reads(repo_root, capsule)
    validate_dispositions(repo_root, capsule)
    validate_directionality(capsule)
    validate_city_noncompressible(repo_root, capsule)
    return {
        "capsule_id": capsule["capsule_id"],
        "result": "VALID_NAVIGATION_ONLY",
        "authority": AUTHORITY,
        **identity_result,
        "semantic_authority_granted": False,
        "on_any_mismatch": "RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES",
    }

def workpack_is_complete(path: Path) -> bool:
    try:
        text = path.read_text(encoding="utf-8")
    except FileNotFoundError:
        return False
    return bool(re.search(r"(?mi)^Status:\s*\**COMPLETE\**\s*$", text))

def validate_pa_chain(repo_root: Path, index: dict, capsules: dict[str, dict]) -> dict:
    rule = index.get("coverage_rules", {}).get("pa_accepted_result_chain")
    if not isinstance(rule, dict):
        raise CapsuleError("index missing pa_accepted_result_chain coverage rule")
    result_glob = rule.get("result_glob")
    wp_template = rule.get("workpack_template")
    if not isinstance(result_glob, str) or not isinstance(wp_template, str):
        raise CapsuleError("invalid PA chain discovery rule")
    accepted_results = []
    for result_path_str in sorted(glob.glob(str(repo_root / result_glob))):
        result_path = Path(result_path_str)
        match = re.fullmatch(r"PA-(\d\d)\.md", result_path.name)
        if not match:
            continue
        num = match.group(1)
        wp_path = repo_root / wp_template.replace("{NN}", num)
        if workpack_is_complete(wp_path):
            accepted_results.append((num, result_path.relative_to(repo_root).as_posix()))
    missing = []
    for num, result_path in accepted_results:
        cid = f"WP-PA-{num}"
        capsule = capsules.get(cid)
        if capsule is None:
            missing.append(cid)
            continue
        if capsule.get("track") != "PA":
            raise CapsuleError(f"{cid}: accepted PA chain capsule must declare track=PA")
        if capsule.get("content_mode") != "structured_disposition":
            raise CapsuleError(f"{cid}: accepted PA chain capsule must use content_mode=structured_disposition")
        if not isinstance(capsule.get("disposition_source"), dict):
            raise CapsuleError(f"{cid}: accepted PA chain capsule requires disposition_source")
        if not isinstance(capsule.get("dispositions"), list) or not capsule["dispositions"]:
            raise CapsuleError(f"{cid}: accepted PA chain capsule requires non-empty dispositions")
        paths = {s.get("path") for s in capsule.get("authoritative_sources", []) if isinstance(s, dict)}
        if result_path not in paths:
            raise CapsuleError(f"{cid}: capsule does not point to accepted PA result {result_path}")
    if missing:
        raise CapsuleError("accepted PA result-chain coverage gap: " + ", ".join(missing) + "; missing capsule forces authoritative source reconstruction and DocSync cannot claim capsule coverage complete")
    return {"accepted_pa_results_discovered": [f"WP-PA-{num}" for num, _ in accepted_results], "coverage": "COMPLETE"}

def audit_index(repo_root: Path, index_path: Path, live_state: dict | None = None) -> dict:
    index = load_json(index_path)
    if index.get("schema") != INDEX_SCHEMA:
        raise CapsuleError("unexpected capsule index schema")
    if index.get("authority") != AUTHORITY:
        raise CapsuleError("capsule index must remain NON_AUTHORITATIVE_NAVIGATION_ONLY")
    entries = index.get("entries")
    if not isinstance(entries, list) or not entries:
        raise CapsuleError("capsule index entries must be non-empty")
    capsules = {}
    results = []
    for entry in entries:
        if not isinstance(entry, dict) or not isinstance(entry.get("path"), str):
            raise CapsuleError("capsule index entry requires path")
        capsule = load_json(repo_root / entry["path"])
        cid = capsule.get("capsule_id")
        if entry.get("capsule_id") != cid:
            raise CapsuleError(f"index/capsule id mismatch for {entry['path']}")
        if cid in capsules:
            raise CapsuleError(f"duplicate capsule id in index: {cid}")
        capsules[cid] = capsule
        results.append(validate_capsule(repo_root, capsule, live_state))
    return {
        "result": "PASS",
        "authority": AUTHORITY,
        "capsules": results,
        "pa_chain": validate_pa_chain(repo_root, index, capsules),
        "semantic_authority_granted": False,
    }

def expect_failure(fn, needle: str) -> None:
    try:
        fn()
    except CapsuleError as exc:
        if needle not in str(exc):
            raise AssertionError(f"expected failure containing {needle!r}, got {exc!r}") from exc
    else:
        raise AssertionError(f"expected failure containing {needle!r}")

def run_self_test() -> None:
    with tempfile.TemporaryDirectory() as td:
        repo = Path(td)
        (repo / "Docs/workpacks/PA").mkdir(parents=True)
        (repo / "Docs/research/living-world/results").mkdir(parents=True)
        (repo / "Docs/production").mkdir(parents=True)
        wp_text = """# WP-PA-03\n\nStatus: **COMPLETE**\nAccepted candidate: `1111111111111111111111111111111111111111`\nIndependent review: **PASS**, review `12345`\nMerged: PR `#1`, merge commit `2222222222222222222222222222222222222222`\n"""
        result_text = """# PA-03\n\n## 4. Minimal relationship vocabulary recommendation\n\n| Relationship/mechanism | Status | Juego2 recommendation |\n|---|---|---|\n| directed trust | **ADOPT** | material decision input |\n| synthetic opinion | **LATER / non-authoritative** | readability only |\n| default global/N-hop social traversal to discover targets | **REJECT** | inherited bounded-discovery guarantee |\n\n## 5. Next\n"""
        wp = repo / "Docs/workpacks/PA/WP-PA-03.md"
        result = repo / "Docs/research/living-world/results/PA-03.md"
        city_spec = repo / "Docs/production/CITY_PRODUCT_SEED.md"
        wp.write_text(wp_text, encoding="utf-8")
        result.write_text(result_text, encoding="utf-8")
        city_spec.write_text("exact spatial spec", encoding="utf-8")
        source = {"path": "Docs/research/living-world/results/PA-03.md", "git_blob_sha": git_blob_sha(result.read_bytes())}
        capsule = {
            "schema": SCHEMA,
            "authority": AUTHORITY,
            "capsule_id": "WP-PA-03",
            "track": "PA",
            "content_mode": "structured_disposition",
            "accepted_identity": {"reviewed_candidate_sha": "1"*40, "merge_sha": "2"*40, "review_id": "12345"},
            "identity_source": {"path": "Docs/workpacks/PA/WP-PA-03.md", "git_blob_sha": git_blob_sha(wp.read_bytes())},
            "authoritative_sources": [source],
            "exported_guarantees": [{"id": "g1", "statement": "directed relationship matters"}],
            "exclusions_nonclaims": [{"id": "x1", "statement": "no global discovery"}],
            "reopen_conditions": ["concrete contradictory evidence"],
            "escalate_if": ["material exact semantics needed"],
            "disposition_source": {**source, "section": "## 4. Minimal relationship vocabulary recommendation", "key_column": 0, "status_column": 1},
            "dispositions": [
                {"source_key": "directed trust", "status": "ADOPT"},
                {"source_key": "synthetic opinion", "status": "LATER / non-authoritative"},
                {"source_key": "default global/N-hop social traversal to discover targets", "status": "REJECT"},
            ],
            "directional_semantics": [{"id": "pair", "forward": "A -> B", "reverse": "B -> A", "must_remain_distinct": True}],
            "mandatory_source_reads": [],
        }
        validate_capsule(repo, capsule)
        stale = copy.deepcopy(capsule)
        stale["accepted_identity"]["reviewed_candidate_sha"] = "3"*40
        expect_failure(lambda: validate_capsule(repo, stale), "accepted identity mismatch")
        missing_positive = copy.deepcopy(capsule)
        missing_positive["exported_guarantees"] = []
        expect_failure(lambda: validate_capsule(repo, missing_positive), "exported_guarantees")
        missing_mode = copy.deepcopy(capsule)
        missing_mode.pop("content_mode")
        expect_failure(lambda: validate_capsule(repo, missing_mode), "content_mode")
        missing_reads = copy.deepcopy(capsule)
        missing_reads.pop("mandatory_source_reads")
        expect_failure(lambda: validate_capsule(repo, missing_reads), "mandatory_source_reads")
        malformed_reopen = copy.deepcopy(capsule)
        malformed_reopen["reopen_conditions"] = [None]
        expect_failure(lambda: validate_capsule(repo, malformed_reopen), "reopen_conditions[0] must be a non-empty string")
        blank_escalation = copy.deepcopy(capsule)
        blank_escalation["escalate_if"] = ["   "]
        expect_failure(lambda: validate_capsule(repo, blank_escalation), "escalate_if[0] must be a non-empty string")
        bad_direction = copy.deepcopy(capsule)
        bad_direction["directional_semantics"][0]["forward"] = None
        expect_failure(lambda: validate_capsule(repo, bad_direction), "forward/reverse must be strings")
        fail_review_text = wp_text.replace("**PASS**", "**FAIL**")
        wp.write_text(fail_review_text, encoding="utf-8")
        fail_review = copy.deepcopy(capsule)
        fail_review["identity_source"]["git_blob_sha"] = git_blob_sha(wp.read_bytes())
        expect_failure(lambda: validate_capsule(repo, fail_review), "independent PASS review verdict")
        wp.write_text(wp_text, encoding="utf-8")
        missing_reject = copy.deepcopy(capsule)
        missing_reject["dispositions"] = missing_reject["dispositions"][:-1]
        expect_failure(lambda: validate_capsule(repo, missing_reject), "structured disposition coverage is lossy")
        later_collapsed = copy.deepcopy(capsule)
        later_collapsed["dispositions"][1]["status"] = "ADOPT"
        expect_failure(lambda: validate_capsule(repo, later_collapsed), "structured disposition coverage is lossy")
        collapsed_direction = copy.deepcopy(capsule)
        collapsed_direction["directional_semantics"][0]["reverse"] = "A -> B"
        expect_failure(lambda: validate_capsule(repo, collapsed_direction), "asymmetric semantics collapsed")
        live_reopen = {"capsules": {"WP-PA-03": {"state": "REOPENED", "reviewed_candidate_sha": "1"*40, "merge_sha": "2"*40}}}
        expect_failure(lambda: validate_capsule(repo, capsule, live_reopen), "not ACCEPTED")
        live_stale = {"capsules": {"WP-PA-03": {"state": "ACCEPTED", "reviewed_candidate_sha": "4"*40, "merge_sha": "2"*40}}}
        expect_failure(lambda: validate_capsule(repo, capsule, live_stale), "live exact-SHA mismatch")
        result.write_text(result_text + "\nchanged", encoding="utf-8")
        expect_failure(lambda: validate_capsule(repo, capsule), "source fingerprint mismatch")
        result.write_text(result_text, encoding="utf-8")
        city = copy.deepcopy(capsule)
        city["capsule_id"] = "WP-CITY-03"
        city["track"] = "CITY"
        city["content_mode"] = "boundary_summary"
        city.pop("disposition_source", None)
        city.pop("dispositions", None)
        city.pop("directional_semantics", None)
        city["mandatory_source_reads"] = []
        expect_failure(lambda: validate_capsule(repo, city), "mandatory non-compressible source reads")
        city["mandatory_source_reads"] = [{"path": "Docs/production/CITY_PRODUCT_SEED.md", "git_blob_sha": git_blob_sha(city_spec.read_bytes()), "noncompressible": True, "reason": "execution specification"}]
        validate_capsule(repo, city)
        bad_reason = copy.deepcopy(city)
        bad_reason["mandatory_source_reads"][0]["reason"] = ""
        expect_failure(lambda: validate_capsule(repo, bad_reason), "reason must be a non-empty string")
    print("context-capsule self-test: PASS")

def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--repo-root", default=".")
    parser.add_argument("--capsule")
    parser.add_argument("--index", default="Docs/engineering/context-capsules/index.json")
    parser.add_argument("--accepted-state")
    parser.add_argument("--require-live-state", action="store_true")
    parser.add_argument("--audit-index", action="store_true")
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()
    try:
        if args.self_test:
            run_self_test()
            return 0
        repo = Path(args.repo_root).resolve()
        live = load_json(Path(args.accepted_state)) if args.accepted_state else None
        if args.require_live_state and live is None:
            raise CapsuleError("--require-live-state requires --accepted-state")
        if args.audit_index:
            result = audit_index(repo, repo / args.index, live)
        elif args.capsule:
            result = validate_capsule(repo, load_json(repo / args.capsule), live)
        else:
            raise CapsuleError("provide --self-test, --audit-index, or --capsule")
        if args.require_live_state:
            rows = result.get("capsules", [result])
            if any(not row.get("live_state_checked") for row in rows):
                raise CapsuleError("live-state confirmation required but not completed")
        print(json.dumps(result, indent=2, sort_keys=True))
        return 0
    except CapsuleError as exc:
        print(f"context-capsule: FAIL: {exc}", file=sys.stderr)
        return 2

if __name__ == "__main__":
    raise SystemExit(main())