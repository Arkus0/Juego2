#!/usr/bin/env python3
"""Local, ChatGPT-subscription-only driver for one Juego2 WP lifecycle.

GitHub remains the source of truth. Local logs/locks are never PASS evidence.
"""

from __future__ import annotations

import argparse
import asyncio
import importlib.util
import json
import os
import re
import subprocess
import sys
import time
import uuid
from contextlib import contextmanager
from dataclasses import dataclass
from datetime import datetime
from functools import lru_cache
from pathlib import Path
from typing import Any, Self

REPO = "Arkus0/Juego2"
WP_RE = re.compile(r"^(?:WP-)?([A-Z][A-Z0-9]*-[0-9A-Z]+)$")
SHA_RE = re.compile(r"^[0-9a-f]{40}$")
REVIEW_ID_RE = re.compile(r"^[0-9a-f]{32}$")
TOKEN_ENV = ("OPENAI_API_KEY", "OPENROUTER_API_KEY", "ANTHROPIC_API_KEY",
             "AZURE_OPENAI_API_KEY", "CODEX_API_KEY", "OPENAI_BASE_URL")
CONTINUE_OFFER_TTL = 21000


class StopFlow(Exception):
    def __init__(self, message: str, *, notified: bool = False) -> None:
        super().__init__(message)
        self.notified = notified


def clean_env() -> dict[str, str]:
    env = os.environ.copy()
    for name in TOKEN_ENV:
        env.pop(name, None)
    return env


def run(*args: str, cwd: Path | None = None, input_data: str | None = None) -> str:
    result = subprocess.run(args, cwd=cwd, env=clean_env(), text=True,
                            encoding="utf-8", errors="replace", input=input_data,
                            capture_output=True, check=False)
    if result.returncode:
        raise StopFlow(f"{args[0]} failed ({result.returncode}): {result.stderr[-600:]}")
    return result.stdout.strip()


def gh_json(*args: str) -> Any:
    return json.loads(run("gh", *args))


def gh_pages(path: str) -> list[dict[str, Any]]:
    pages = gh_json("api", "--paginate", "--slurp", path)
    if not isinstance(pages, list) or any(not isinstance(page, list) for page in pages):
        raise StopFlow(f"Unexpected paginated GitHub response for {path}")
    return [row for page in pages for row in page]


def require_repo(root: Path) -> None:
    remote = run("git", "remote", "get-url", "origin", cwd=root).rstrip("/")
    if not remote.endswith(("github.com/Arkus0/Juego2.git", "github.com:Arkus0/Juego2.git")):
        raise StopFlow(f"Unexpected Git origin: {remote}")
    owner = json.loads(run("gh", "repo", "view", "--json", "nameWithOwner", cwd=root))["nameWithOwner"]
    if owner != REPO:
        raise StopFlow(f"Unexpected GitHub repository: {owner}")
    actor = run("gh", "api", "user", "--jq", ".login", cwd=root)
    if actor != "Arkus0":
        raise StopFlow(f"GitHub actor must be repository owner Arkus0, got {actor}")


def normalize_wp(value: str) -> str:
    match = WP_RE.fullmatch(value.strip().upper())
    if not match:
        raise StopFlow(f"Invalid WP id: {value!r}")
    return match.group(1)


def fields(body: str) -> dict[str, str]:
    out: dict[str, str] = {}
    for line in body.splitlines():
        if ":" in line:
            key, value = line.split(":", 1)
            out[key.strip().strip("*_#` ").lower()] = value.strip().strip("`*_ .")
    return out


def markers(pr: int) -> list[dict[str, str]]:
    data = gh_pages(f"repos/{REPO}/issues/{pr}/comments?per_page=100")
    out = []
    for row in data:
        body = row.get("body") or ""
        if "ARKUS_AUTOMATION_V2" not in body:
            continue
        actor = (row.get("user") or {}).get("login")
        parsed = fields(body)
        state = parsed.get("state")
        if state == "DOCSYNC_COMPLETE" and actor != "Arkus0":
            continue
        if state in {"REVIEW_READY", "REPAIR_REQUIRED", "PASS_PREFLIGHT_GREEN"} and actor != "github-actions[bot]":
            continue
        if actor not in {"Arkus0", "github-actions[bot]"}:
            continue
        out.append(dict(parsed, _created_at=row.get("created_at") or ""))
    return out


def local_markers(pr: int) -> list[dict[str, str]]:
    data = gh_pages(f"repos/{REPO}/issues/{pr}/comments?per_page=100")
    out = []
    for row in data:
        body = row.get("body") or ""
        if "ARKUS_LOCAL_AUTOPILOT" not in body:
            continue
        marker = fields(body)
        actor = (row.get("user") or {}).get("login")
        state = marker.get("state")
        if state in {"SECOND_FAIL_OFFERED", "OVERDEFENSE_APPEAL_STARTED", "PROTOCOL_FIX_STARTED", "FAIL_AUDIT_COMPLETE"} and actor != "Arkus0":
            continue
        if state in {"OWNER_CONTINUE", "CONTINUE_UNAVAILABLE", "CONTINUE_EXPIRED"} and actor != "github-actions[bot]":
            continue
        out.append(dict(marker, _created_at=row.get("created_at") or ""))
    return out


def latest_marker(rows: list[dict[str, str]], state: str, sha: str | None = None) -> dict[str, str] | None:
    for row in reversed(rows):
        if row.get("state") == state and (sha is None or row.get("target sha", "").lower() == sha):
            return row
    return None


def owner_authorized_continuation(rows: list[dict[str, str]], fail_count: int) -> bool:
    # A count-2 click authorizes bounded continuation through count 3. If an
    # independent same-SHA appeal itself creates count 3, no count-2 click
    # existed; a count-3 click must be accepted without permitting count 4.
    offers = {(row.get("target sha", "").lower(), row.get("fail count")) for row in rows
              if row.get("state") == "SECOND_FAIL_OFFERED" and row.get("fail count") in {"2", "3"}}
    if any(row.get("state") in {"CONTINUE_EXPIRED", "CONTINUE_UNAVAILABLE"} and
           (row.get("target sha", "").lower(), row.get("fail count")) in offers for row in rows):
        raise StopFlow("Owner-continue decision expired or became unavailable; PC required")
    return any(row.get("state") == "OWNER_CONTINUE" and
               row.get("fail count") in {"2", "3"} and
               int(row["fail count"]) <= fail_count < 4 and
               (row.get("target sha", "").lower(), row["fail count"]) in offers
               for row in rows)


def fresh_offer(created_at: str, now: float | None = None) -> bool:
    try:
        stamp = datetime.fromisoformat(created_at.replace("Z", "+00:00")).timestamp()
    except (ValueError, TypeError):
        return False
    age = (time.time() if now is None else now) - stamp
    return -120 <= age <= CONTINUE_OFFER_TTL


@lru_cache(maxsize=4)
def validation_context_module(root: Path) -> Any:
    source = root / "scripts" / "validation-context.py"
    spec = importlib.util.spec_from_file_location("arkus_validation_context", source)
    if spec is None or spec.loader is None:
        raise StopFlow("Canonical validation-context resolver unavailable")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def ready_context_matches(root: Path, pr: dict[str, Any], row: dict[str, str] | None) -> bool:
    if not row or pr.get("state") != "open" or pr.get("draft") is not False or pr.get("merged"):
        return False
    sha = (pr.get("head") or {}).get("sha", "").lower()
    try:
        context = validation_context_module(root).resolve_context(pr, sha)
    except (OSError, ValueError, AttributeError) as exc:
        raise StopFlow(f"PR #{pr.get('number')} validation context invalid: {exc}") from exc
    digest = context["context_digest"]
    return (row.get("state") == "REVIEW_READY" and
            row.get("target sha", "").lower() == sha and
            row.get("key") == f"review-ready:{pr['number']}:{sha}:{digest}" and
            row.get("validation context digest", "").lower() == digest and
            row.get("effective wp") == context["wp"] and
            row.get("process only", "").lower() == context["process_only"] and
            row.get("non foundational", "").lower() == context["non_foundational"])


def prs_for_wp(wp: str) -> list[dict[str, Any]]:
    # PR body ownership is canonical.  Titles/search results are never enough.
    all_prs = gh_pages(f"repos/{REPO}/pulls?state=all&per_page=100")
    return [pr for pr in all_prs if fields(pr.get("body") or "").get("wp", "").upper() in {wp, f"WP-{wp}"}]


def canonical_pr(wp: str) -> dict[str, Any] | None:
    prs = prs_for_wp(wp)
    open_prs = [pr for pr in prs if pr["state"] == "open"]
    if len(open_prs) > 1:
        raise StopFlow(f"Multiple open PRs claim {wp}: {[p['number'] for p in open_prs]}")
    if open_prs:
        return open_prs[0]
    merged = [pr for pr in prs if pr.get("merged_at")]
    return max(merged, key=lambda p: p["merged_at"]) if merged else None


def assert_pr_checkout(root: Path, pr: dict[str, Any]) -> None:
    head = pr.get("head") or {}
    if ((head.get("repo") or {}).get("full_name") != REPO or
        not SHA_RE.fullmatch((head.get("sha") or "").lower()) or
        not head.get("ref")):
        raise StopFlow(f"PR #{pr['number']} does not have a same-repository canonical head")
    local_sha = run("git", "rev-parse", "HEAD", cwd=root).lower()
    local_branch = run("git", "branch", "--show-current", cwd=root)
    if local_sha != head["sha"].lower() or local_branch != head["ref"]:
        raise StopFlow(f"PR #{pr['number']} local checkout is not its exact head branch/SHA; switch to the canonical branch before adoption")
    if run("git", "status", "--porcelain", cwd=root):
        raise StopFlow(f"PR #{pr['number']} local checkout has uncommitted bytes outside its exact head SHA")


def validate_docsync(root: Path, pr: dict[str, Any], row: dict[str, str]) -> None:
    number = pr["number"]
    merge_at = pr.get("merged_at") or ""
    expected_wp = fields(pr.get("body") or "").get("wp", "").upper().removeprefix("WP-")
    key = row.get("key", "")
    match = re.fullmatch(rf"docsync-complete:{number}:([0-9a-f]{{40}})", key)
    if (not merge_at or not expected_wp or not match or
        row.get("wp", "").upper().removeprefix("WP-") != expected_wp or
        row.get("_created_at", "") < merge_at or not row.get("next wp")):
        raise StopFlow(f"PR #{number} has malformed or premature DOCSYNC_COMPLETE")
    commit = match.group(1)
    merge_commit = (pr.get("merge_commit_sha") or "").lower()
    if not SHA_RE.fullmatch(merge_commit):
        raise StopFlow(f"PR #{number} lacks its accepted merge commit SHA")
    after_merge = subprocess.run(("git", "merge-base", "--is-ancestor", merge_commit, commit),
                                 cwd=root, env=clean_env(), capture_output=True, check=False)
    if after_merge.returncode:
        raise StopFlow(f"PR #{number} DocSync commit does not descend from its merge")
    ancestor = subprocess.run(("git", "merge-base", "--is-ancestor", commit, "origin/main"),
                              cwd=root, env=clean_env(), capture_output=True, check=False)
    if ancestor.returncode:
        raise StopFlow(f"PR #{number} DocSync commit is not on current main")
    if commit == merge_commit:
        if not row.get("detail", "").strip():
            raise StopFlow(f"PR #{number} zero-commit DocSync needs a concrete detail")
        return
    parents = run("git", "rev-list", "--parents", "-n", "1", commit, cwd=root).split()
    if len(parents) < 2:
        raise StopFlow(f"PR #{number} DocSync commit lacks a first parent")
    changed = set(run("git", "diff", "--name-only", parents[1], commit, cwd=root).splitlines())
    if not changed or any(not name.startswith("Docs/") for name in changed):
        raise StopFlow(f"PR #{number} DocSync commit is not docs-only")
    # Which Docs files changed accepted meaning is a DocSync judgment. The
    # controller can check byte scope, not infer a closed list of authorities.


def next_from_merged_pr(root: Path, pr: dict[str, Any]) -> str | None:
    if not pr.get("merged_at"):
        raise StopFlow("Cannot route a next WP before merge")
    row = latest_marker(markers(pr["number"]), "DOCSYNC_COMPLETE")
    if not row:
        raise StopFlow(f"PR #{pr['number']} merged but lacks DOCSYNC_COMPLETE")
    # The successful Reviewer can merge and DocSync while this process keeps
    # running. Refresh main before checking the key's local ancestry.
    run("git", "fetch", "origin", "main", cwd=root)
    validate_docsync(root, pr, row)
    raw = row.get("next wp", "")
    if raw.upper() == "NONE":
        return None
    return normalize_wp(raw)


def latest_next_wp(root: Path) -> str | None:
    # One recent-comments request, not an O(all-PRs) scan. Missing handoff fails closed.
    comments = gh_json("api", f"repos/{REPO}/issues/comments?sort=created&direction=desc&per_page=100")
    for comment in comments:
        body = comment.get("body") or ""
        if "ARKUS_AUTOMATION_V2" not in body:
            continue
        if (comment.get("user") or {}).get("login") != "Arkus0":
            continue
        row = fields(body)
        if row.get("state") != "DOCSYNC_COMPLETE":
            continue
        issue_url = comment.get("issue_url") or ""
        match = re.search(r"/issues/([1-9][0-9]*)$", issue_url)
        if not match:
            continue
        source = int(match.group(1))
        pr = gh_json("api", f"repos/{REPO}/pulls/{source}")
        if not pr.get("merged"):
            raise StopFlow(f"DOCSYNC_COMPLETE on unmerged PR #{source}")
        row["_created_at"] = comment.get("created_at") or ""
        validate_docsync(root, pr, row)
        raw = row.get("next wp", "")
        if raw.upper() == "NONE":
            return None
        wp = normalize_wp(raw)
        if canonical_pr(wp):
            raise StopFlow(f"Next WP {wp} already has a PR; inspect #{source} handoff and current ownership")
        return wp
    raise StopFlow("No recent merged DOCSYNC_COMPLETE handoff; supply --wp explicitly")


@dataclass(frozen=True)
class Window:
    remaining: float
    minutes: int
    resets_at: int


def quota_decision(payload: dict[str, Any], now: int, threshold: float = 3.0) -> tuple[str, int | None]:
    buckets = payload.get("rateLimitsByLimitId") or {"legacy": payload.get("rateLimits")}
    windows: list[Window] = []
    if not isinstance(buckets, dict) or not buckets:
        raise StopFlow("Quota unavailable")
    for bucket in buckets.values():
        if not isinstance(bucket, dict):
            raise StopFlow("Quota bucket unavailable")
        for name in ("primary", "secondary"):
            value = bucket.get(name)
            if value is None:
                continue
            if not all(isinstance(value.get(k), (int, float)) for k in ("usedPercent", "windowDurationMins", "resetsAt")):
                raise StopFlow("Incomplete quota window")
            windows.append(Window(100 - float(value["usedPercent"]), int(value["windowDurationMins"]), int(value["resetsAt"])))
        if bucket.get("rateLimitReachedType"):
            raise StopFlow(f"Rate limit reached: {bucket['rateLimitReachedType']}")
    if not windows:
        raise StopFlow("No readable quota windows")
    if not any(w.minutes > 360 for w in windows):
        raise StopFlow("General quota window unavailable; cannot protect 3% floor")
    low = [w for w in windows if w.remaining <= threshold]
    if not low:
        return "run", None
    # A long/general window wins over a simultaneous short-window reset.
    if any(w.minutes > 360 for w in low):
        return "stop_general", None
    reset = max(w.resets_at for w in low)
    if reset <= now:
        raise StopFlow("Quota reset time is stale; re-read limits")
    return "wait_short", reset


class AppServer:
    def __init__(self) -> None:
        self.proc: asyncio.subprocess.Process | None = None
        self.next_id = 0

    async def __aenter__(self) -> Self:
        self.proc = await asyncio.create_subprocess_exec("codex", "app-server", "--stdio",
            stdin=asyncio.subprocess.PIPE, stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.DEVNULL, env=clean_env())
        await self.call("initialize", {"clientInfo": {"name": "arkus_local_wp_autopilot", "title": "Arkus Local WP Autopilot", "version": "0.1.0"}})
        await self.send({"method": "initialized"})
        return self

    async def __aexit__(self, *_: object) -> None:
        if self.proc:
            self.proc.terminate()
            await self.proc.wait()

    async def send(self, value: dict[str, Any]) -> None:
        assert self.proc and self.proc.stdin
        self.proc.stdin.write((json.dumps(value) + "\n").encode())
        await self.proc.stdin.drain()

    async def call(self, method: str, params: dict[str, Any] | None = None) -> dict[str, Any]:
        self.next_id += 1
        ident = self.next_id
        msg: dict[str, Any] = {"id": ident, "method": method}
        if params is not None:
            msg["params"] = params
        await self.send(msg)
        assert self.proc and self.proc.stdout
        while True:
            line = await asyncio.wait_for(self.proc.stdout.readline(), timeout=30)
            if not line:
                raise StopFlow("App Server closed unexpectedly")
            response = json.loads(line)
            if response.get("id") != ident:
                continue
            if "error" in response:
                raise StopFlow(f"App Server {method}: {response['error']}")
            return response.get("result") or {}

    async def assert_chatgpt(self) -> None:
        account = (await self.call("account/read", {"refreshToken": False})).get("account") or {}
        if account.get("type") != "chatgpt":
            raise StopFlow("Codex must be logged in with ChatGPT subscription, not an API key")

    async def guard(self) -> tuple[str, int | None]:
        await self.assert_chatgpt()
        limits = await self.call("account/rateLimits/read")
        return quota_decision(limits, int(time.time()))

    async def assert_models(self) -> None:
        result = await self.call("model/list", {"limit": 100, "includeHidden": False})
        catalog = {row.get("model") or row.get("id"): row for row in result.get("data", [])}
        for model, efforts in {"gpt-6-sol": {"high", "xhigh"},
                               "gpt-6-luna": {"high", "xhigh"}}.items():
            row = catalog.get(model)
            available = {item.get("reasoningEffort") for item in (row or {}).get("supportedReasoningEfforts", [])}
            if not efforts.issubset(available):
                raise StopFlow(f"Model/effort unavailable: {model} {efforts - available}")


async def quota_before_reasoning(app: AppServer, wp: str, pr: int | None) -> None:
    while True:
        decision, reset = await app.guard()
        if decision == "stop_general":
            notify("HUMAN_ACTION_REQUIRED", "Cuota general <=3%; controlador detenido sin reanudación automática.", wp, pr)
            raise StopFlow("General quota <=3%; stopped", notified=True)
        if decision == "run":
            return
        assert decision == "wait_short" and reset is not None
        print(f"Short quota <=3%; pause until {reset} (Unix seconds)", flush=True)
        while int(time.time()) < int(reset) + 30:
            await asyncio.sleep(min(60, int(reset) + 30 - int(time.time())))


def notify(state: str, detail: str, wp: str = "", pr: int | None = None,
           target_sha: str = "", fail_count: int | None = None) -> None:
    # Reuse the repository's existing Telegram workflow; never handle bot secrets here.
    payload = {"state": state, "detail": detail[:420], "wp": wp,
               "key": f"autopilot:{state}:{wp}:{pr or 0}:{int(time.time())}",
               "target_sha": target_sha}
    if pr:
        payload["pr"] = pr
    if fail_count is not None:
        payload["fail_count"] = fail_count
    run("gh", "api", "--method", "POST", f"repos/{REPO}/dispatches", "--input", "-",
        input_data=json.dumps({"event_type": "arkus_notification", "client_payload": payload}))


def dispatch_continue_receiver(pr: int, sha: str, fail_count: int) -> None:
    payload = {"event_type": "arkus_continue_request",
               "client_payload": {"pr": pr, "target_sha": sha, "fail_count": fail_count}}
    run("gh", "api", "--method", "POST", f"repos/{REPO}/dispatches", "--input", "-",
        input_data=json.dumps(payload))


def offer_continue(pr: int, sha: str, fail_count: int, wp: str, detail: str) -> None:
    existing = local_markers(pr)
    if any(row.get("state") in {"CONTINUE_EXPIRED", "CONTINUE_UNAVAILABLE"} and
           row.get("target sha") == sha and row.get("fail count") == str(fail_count) for row in existing):
        raise StopFlow("Owner-continue decision expired or became unavailable; PC required")
    offers = [row for row in existing if row.get("state") == "SECOND_FAIL_OFFERED" and
              row.get("target sha") == sha and row.get("fail count") == str(fail_count)]
    if offers and not all(fresh_offer(row.get("_created_at", "")) for row in offers):
        raise StopFlow("Owner-continue offer is stale; PC required before any receiver restart")
    if not offers:
        body = ("ARKUS_LOCAL_AUTOPILOT\nState: SECOND_FAIL_OFFERED\n"
                f"Target SHA: {sha}\nFail count: {fail_count}\n"
                "Detail: Luna classified the latest FAIL as valid; owner may authorize bounded continuation. No PASS or proof claim.\n")
        run("gh", "api", "--method", "POST", f"repos/{REPO}/issues/{pr}/comments", "-f", f"body={body}")
    # Re-send on recovery. A duplicate visible button is safer than a durable
    # offer with no message after a transport failure; the callback is one-shot.
    notify("SECOND_FAIL_DECISION", detail, wp, pr, sha, fail_count)
    # Restarting the same wait replaces a stale poller without sending another button.
    dispatch_continue_receiver(pr, sha, fail_count)


async def wait_for_owner_continue(pr: int, sha: str, fail_count: int, timeout: int = 21000) -> None:
    deadline = time.monotonic() + timeout
    while time.monotonic() < deadline:
        current = gh_json("api", f"repos/{REPO}/pulls/{pr}")
        if current.get("state") != "open" or current["head"]["sha"].lower() != sha:
            raise StopFlow("PR changed while awaiting Telegram owner decision")
        rows = local_markers(pr)
        if any(row.get("state") in {"CONTINUE_UNAVAILABLE", "CONTINUE_EXPIRED"} and
               row.get("target sha") == sha and row.get("fail count") == str(fail_count) for row in rows):
            raise StopFlow("Telegram continue receiver unavailable or expired; PC intervention required")
        if any(row.get("state") == "OWNER_CONTINUE" and row.get("target sha") == sha and
               row.get("fail count") == str(fail_count) for row in rows):
            return
        await asyncio.sleep(30)
    raise StopFlow("Telegram continue button expired without owner decision")


@contextmanager
def one_controller(state: Path):
    state.mkdir(parents=True, exist_ok=True)
    with (state / "controller.lock").open("a+b") as handle:
        handle.seek(0)
        handle.write(b"0")
        handle.flush()
        try:
            if os.name == "nt":
                import msvcrt
                handle.seek(0)
                msvcrt.locking(handle.fileno(), msvcrt.LK_NBLCK, 1)
            else:
                import fcntl
                fcntl.flock(handle.fileno(), fcntl.LOCK_EX | fcntl.LOCK_NB)
        except OSError as exc:
            raise StopFlow("Another local WP controller is already running") from exc
        try:
            yield
        finally:
            if os.name == "nt":
                handle.seek(0)
                msvcrt.locking(handle.fileno(), msvcrt.LK_UNLCK, 1)
            else:
                fcntl.flock(handle.fileno(), fcntl.LOCK_UN)


def wp_path(root: Path, wp: str) -> Path:
    found = list((root / "Docs" / "workpacks").rglob(f"WP-{wp}.md"))
    if len(found) != 1:
        raise StopFlow(f"WP-{wp} does not resolve uniquely in Docs/workpacks")
    return found[0]


def dependency_wps(wp_text: str) -> list[str]:
    line = re.search(r"^Depends on:\s*(.+)$", wp_text, re.MULTILINE | re.IGNORECASE)
    if not line:
        raise StopFlow("WP lacks a machine-readable Depends on line")
    return list(dict.fromkeys(re.findall(r"WP-([A-Z][A-Z0-9]*-[0-9A-Z]+)", line.group(1).upper())))


def accepted_main_doc(root: Path, dependency: str) -> bool:
    for relative in (f"Docs/evidence/WP-{dependency}/DOCSYNC.md",
                     f"Docs/evidence/{dependency}/DOCSYNC.md"):
        result = subprocess.run(("git", "show", f"origin/main:{relative}"), cwd=root,
                                env=clean_env(), capture_output=True, text=True,
                                encoding="utf-8", errors="replace", check=False)
        if result.returncode == 0 and re.search(r"DOCSYNC_COMPLETE|DOCSYNC_PERSISTED", result.stdout):
            return True
    return False


def assert_dependencies(root: Path, wp_text: str) -> None:
    line = re.search(r"^Depends on:\s*(.+)$", wp_text, re.MULTILINE | re.IGNORECASE)
    assert line is not None
    remainder = re.sub(r"`?WP-[A-Z][A-Z0-9]*-[0-9A-Z]+`?", "", line.group(1).upper())
    remainder = re.sub(r"\b(PASS|MERGE|DOCSYNC|COMPLETE|NONE|AND)\b|[+;,`\s✅]", "", remainder)
    if remainder:
        raise StopFlow(f"Non-WP prerequisite needs explicit verification: {line.group(1).strip()}")
    for dependency in dependency_wps(wp_text):
        pr = canonical_pr(dependency)
        if not pr or not pr.get("merged_at"):
            raise StopFlow(f"Dependency {dependency} is not merged")
        # Older accepted WPs may have a separate PROCESS_ONLY DocSync PR rather
        # than an issue-comment marker. Require one accepted main-side signal.
        has_marker = bool(latest_marker(markers(pr["number"]), "DOCSYNC_COMPLETE"))
        has_document = accepted_main_doc(root, dependency)
        if not (has_marker or has_document):
            raise StopFlow(f"Dependency {dependency} has no accepted DocSync evidence")


def effort_for_worker(wp_text: str) -> str:
    # Class, not incidental prose in Forbidden scope, selects the quality-first default.
    klass = re.search(r"^Class:\s*(.+)$", wp_text, re.MULTILINE | re.IGNORECASE)
    if not klass:
        return "high"
    positive_class = re.sub(r"\bNON[-_ ]?FOUNDATIONAL\b", "", klass.group(1), flags=re.IGNORECASE)
    return "xhigh" if re.search(r"FOUNDATIONAL|ARCHITECTURE|COMPLETENESS", positive_class, re.IGNORECASE) else "high"


def prepare_new_wp(root: Path) -> None:
    if run("git", "status", "--porcelain", cwd=root):
        raise StopFlow("Worktree must be clean before starting next WP")
    run("git", "switch", "--detach", "origin/main", cwd=root)


async def codex_role(root: Path, state: Path, role: str, prompt: str, model: str, effort: str,
                     schema: Path | None = None) -> str:
    state.mkdir(parents=True, exist_ok=True)
    stamp = f"{int(time.time())}-{role}"
    output = state / f"{stamp}.txt"
    log = state / f"{stamp}.jsonl"
    cmd = ["codex", "exec", "--json", "--ignore-user-config", "--cd", str(root), "--model", model,
           "--config", f'model_reasoning_effort="{effort}"',
           "--config", 'approval_policy="never"',
           "--sandbox", "danger-full-access", "--output-last-message", str(output)]
    if schema:
        cmd += ["--output-schema", str(schema)]
    cmd.append(prompt)
    with log.open("w", encoding="utf-8") as fh:
        proc = await asyncio.create_subprocess_exec(*cmd, stdin=asyncio.subprocess.DEVNULL, stdout=fh,
            stderr=asyncio.subprocess.STDOUT, env=clean_env())
        code = await proc.wait()
    if code:
        raise StopFlow(f"{role} session failed ({code}); inspect {log}")
    if not output.exists():
        raise StopFlow(f"{role} produced no final message; inspect {log}")
    return output.read_text(encoding="utf-8")


def reviewed_verdicts(pr: int) -> list[dict[str, str]]:
    reviews = gh_pages(f"repos/{REPO}/pulls/{pr}/reviews?per_page=100")
    comments = gh_pages(f"repos/{REPO}/issues/{pr}/comments?per_page=100")
    by_id = {}
    for item in reviews + comments:
        if (item.get("user") or {}).get("login") != "Arkus0":
            continue
        record = fields(item.get("body") or "")
        verdict = record.get("reviewer verdict", "").upper()
        review_id = record.get("autopilot review id", "").lower()
        reviewed_sha = record.get("reviewed candidate sha", "").lower()
        if verdict in {"PASS", "FAIL", "PROTOCOL_FIX", "REVIEW_BLOCKED"} and SHA_RE.fullmatch(reviewed_sha):
            if not REVIEW_ID_RE.fullmatch(review_id):
                raise StopFlow(f"PR #{pr} has a manual/untagged exact-SHA verdict; PC reconciliation required before autopilot adoption")
            row = {"id": review_id, "sha": record["reviewed candidate sha"].lower(),
                   "verdict": verdict, "body": item.get("body") or "",
                   "at": item.get("submitted_at") or item.get("created_at") or ""}
            prior = by_id.get(review_id)
            if prior and (prior["sha"], prior["verdict"]) != (row["sha"], row["verdict"]):
                raise StopFlow(f"Contradictory duplicate review ID {review_id} on PR #{pr}")
            if not prior or row["at"] < prior["at"]:
                by_id[review_id] = row
    return sorted(by_id.values(), key=lambda row: row["at"])


def reviewed_fails(pr: int) -> list[dict[str, str]]:
    return [row for row in reviewed_verdicts(pr) if row["verdict"] == "FAIL"]


def assert_verdict_sequence(pr: int, sha: str, verdicts: list[dict[str, str]],
                            local_rows: list[dict[str, str]]) -> None:
    same = [row for row in verdicts if row["sha"] == sha]
    first_fail = next((index for index, row in enumerate(same) if row["verdict"] == "FAIL"), None)
    if first_fail is None:
        if any(row["verdict"] == "PASS" for row in same[:-1]):
            raise StopFlow(f"PR #{pr} has a verdict after fixed same-SHA PASS")
        return
    prior = same[:first_fail]
    if any(row["verdict"] == "PASS" for row in prior):
        raise StopFlow(f"PR #{pr} has FAIL after fixed same-SHA PASS")
    later = same[first_fail + 1:]
    if not later:
        return
    appeal = latest_marker(local_rows, "OVERDEFENSE_APPEAL_STARTED", sha)
    appeal_id = (appeal or {}).get("review id", "").lower()
    if (len(later) != 1 or not REVIEW_ID_RE.fullmatch(appeal_id) or
        later[0]["id"] != appeal_id or later[0]["verdict"] not in {"PASS", "FAIL"} or
        not (same[first_fail]["at"] <= appeal.get("_created_at", "") <= later[0]["at"])):
        raise StopFlow(f"PR #{pr} has a same-SHA verdict after material FAIL without the one authorized appeal")


def audit_policy(audit: dict[str, Any], fail_count: int) -> str:
    if (audit.get("classification") not in {"valid", "overdefense", "uncertain"} or
        any(type(audit.get(key)) is not bool for key in
            ("same_foundational_defect_class", "self_shrinking_completeness",
             "proof_machinery_expansion_without_progress")) or
        any(not isinstance(audit.get(key), str) or not audit[key].strip()
            for key in ("criterion", "evidence", "minimal_next_action"))):
        raise StopFlow("FAIL audit has invalid or missing decision fields")
    if (audit["self_shrinking_completeness"] or
        audit["proof_machinery_expansion_without_progress"] or
        (fail_count >= 2 and audit["same_foundational_defect_class"])):
        return "circuit_breaker"
    if audit["classification"] == "uncertain" or (fail_count != 2 and audit["classification"] == "overdefense"):
        return "needs_pc"
    if fail_count == 2 and audit["classification"] == "overdefense":
        return "appeal"
    return "valid"


async def wait_for_state(pr: int, predicate, timeout: int = 1800) -> tuple[dict[str, Any], list[dict[str, str]]]:
    deadline = time.monotonic() + timeout
    while time.monotonic() < deadline:
        current = gh_json("api", f"repos/{REPO}/pulls/{pr}")
        rows = markers(pr)
        if predicate(current, rows):
            return current, rows
        await asyncio.sleep(30)
    raise StopFlow(f"Timed out waiting for GitHub state on PR #{pr}")


async def main_async(args: argparse.Namespace) -> None:
    root = Path(args.root).resolve()
    state = Path(args.state).resolve()
    require_repo(root)
    run("git", "fetch", "origin", "main", cwd=root)
    if not args.dry_run and run("git", "status", "--porcelain", cwd=root):
        raise StopFlow("Working tree is dirty; refusing autonomous role launch")
    wp = normalize_wp(args.wp) if args.wp else latest_next_wp(root)
    if wp is None:
        print("No next WP (DOCSYNC_COMPLETE says NONE)")
        return
    async with AppServer() as app:
        await app.assert_models()
        count = 0
        while wp is not None and (not args.one_wp or count == 0):
            if not args.dry_run:
                run("git", "fetch", "origin", "main", cwd=root)
            pr = canonical_pr(wp)
            if pr and pr.get("merged_at"):
                raise StopFlow(f"{wp} already has merged PR #{pr['number']}; use its validated DocSync handoff, not a new cycle")
            if pr is None and not args.dry_run:
                prepare_new_wp(root)
            wp_file = wp_path(root, wp)
            wp_text = wp_file.read_text(encoding="utf-8")
            assert_dependencies(root, wp_text)
            if pr and pr["state"] == "open" and not args.adopt:
                raise StopFlow(f"{wp} already has open PR #{pr['number']}; use --adopt only after its current role has stopped")
            if pr and pr["state"] == "open":
                assert_pr_checkout(root, pr)
            if args.dry_run:
                decision, reset = await app.guard()
                print(f"Would route {wp}; PR={pr['number'] if pr else 'new'}; worker effort={effort_for_worker(wp_text)}; quota={decision}; reset={reset}")
                return
            appeal_sha = ""
            appeal_rejected_sha = ""
            second_fail_detail = "Luna confirmó un FAIL real; al cuarto FAIL se detiene obligatoriamente."
            while True:
                if pr is None:
                    await quota_before_reasoning(app, wp, None)
                    await codex_role(root, state, "worker", f"$implement-workpack Worker {wp}. Sigue PRODUCT_SHA_CLOSURE.md: usa Main Safety same-PR/same-SHA como preflight normal y evita reejecuciones redundantes. Este controlador iniciará un Reviewer fresco solo tras REVIEW_READY. No revises ni inicies otro WP.",
                                     "gpt-6-sol", effort_for_worker(wp_text))
                    pr = canonical_pr(wp)
                    if not pr or pr["state"] != "open":
                        raise StopFlow(f"Worker ended without a canonical open PR for {wp}")
                    await wait_for_state(pr["number"], lambda p, m: ready_context_matches(root, p, latest_marker(m, "REVIEW_READY", p["head"]["sha"].lower())) or bool(latest_marker(m, "BLOCKED")))
                    continue
                current = gh_json("api", f"repos/{REPO}/pulls/{pr['number']}")
                rows = markers(pr["number"])
                if not current.get("merged") and current.get("state") == "open":
                    assert_pr_checkout(root, current)
                if latest_marker(rows, "BLOCKED") or latest_marker(rows, "HUMAN_ACTION_REQUIRED"):
                    raise StopFlow(f"PR #{pr['number']} has a human-action/block marker")
                if current.get("merged"):
                    frozen_merged = fields(current.get("body") or "").get("frozen candidate sha", "").lower()
                    merged_verdicts = reviewed_verdicts(pr["number"])
                    assert_verdict_sequence(pr["number"], frozen_merged, merged_verdicts, local_markers(pr["number"]))
                    accepted = next((row for row in reversed(merged_verdicts)
                                     if row["sha"] == frozen_merged), None)
                    if (not SHA_RE.fullmatch(frozen_merged) or
                        frozen_merged != current["head"]["sha"].lower() or
                        not accepted or accepted["verdict"] != "PASS"):
                        raise StopFlow(f"Merged PR #{pr['number']} lacks an exact frozen-SHA independent PASS; DocSync blocked")
                    completed = latest_marker(rows, "DOCSYNC_COMPLETE")
                    if completed:
                        wp = next_from_merged_pr(root, current)
                        count += 1
                        break
                    await quota_before_reasoning(app, wp, pr["number"])
                    await codex_role(root, state, "docsync", f"Finaliza DocSync del PR #{pr['number']} de {wp}. Confirma PASS y merge exacto en GitHub. Sigue PRODUCT_SHA_CLOSURE.md y $update-handoff: cero commits por defecto si ninguna autoridad documental cambia; si cambia, una reconciliación acotada. Emite DOCSYNC_COMPLETE con Next WP válido. No cambies implementación.", "gpt-6-luna", "high")
                    await wait_for_state(pr["number"], lambda p, m: bool(latest_marker(m, "DOCSYNC_COMPLETE")))
                    continue
                if current.get("state") != "open":
                    raise StopFlow(f"PR #{pr['number']} is closed without merge; no role may continue")
                frozen = fields(current.get("body") or "").get("frozen candidate sha", "").lower()
                if not SHA_RE.fullmatch(frozen) or frozen != current["head"]["sha"].lower():
                    raise StopFlow(f"PR #{pr['number']} has no coherent frozen SHA; Worker must repair handoff")
                verdicts = reviewed_verdicts(pr["number"])
                assert_verdict_sequence(pr["number"], frozen, verdicts, local_markers(pr["number"]))
                fails = [row for row in verdicts if row["verdict"] == "FAIL"]
                repair = latest_marker(rows, "REPAIR_REQUIRED", frozen)
                ready_candidate = latest_marker(rows, "REVIEW_READY", frozen)
                ready = ready_candidate if ready_context_matches(root, current, ready_candidate) else None
                latest_current_verdict = next((row for row in reversed(verdicts) if row["sha"] == frozen), None)
                if latest_current_verdict and latest_current_verdict["verdict"] == "REVIEW_BLOCKED":
                    raise StopFlow(f"PR #{pr['number']} Reviewer reported REVIEW_BLOCKED; PC assessment required")
                if latest_current_verdict and latest_current_verdict["verdict"] == "PROTOCOL_FIX":
                    if ready and ready.get("_created_at", "") > latest_current_verdict["at"]:
                        # A new context-bound marker confirms same-SHA metadata closure.
                        pass
                    else:
                        protocol_statuses = [row for row in verdicts if row["sha"] == frozen and row["verdict"] == "PROTOCOL_FIX"]
                        if len(protocol_statuses) > 1:
                            raise StopFlow(f"PR #{pr['number']} has repeated protocol-only closure failures")
                        local_rows = local_markers(pr["number"])
                        started = any(row.get("state") == "PROTOCOL_FIX_STARTED" and
                                      row.get("target sha") == frozen and
                                      row.get("review id") == latest_current_verdict["id"] for row in local_rows)
                        if started:
                            raise StopFlow(f"PR #{pr['number']} protocol correction started without new REVIEW_READY; PC reconciliation required")
                        await quota_before_reasoning(app, wp, pr["number"])
                        body = ("ARKUS_LOCAL_AUTOPILOT\nState: PROTOCOL_FIX_STARTED\n"
                                f"Target SHA: {frozen}\nReview ID: {latest_current_verdict['id']}\n"
                                "Detail: metadata-only correction; no product commit, semantic FAIL or proof rerun.\n")
                        run("gh", "api", "--method", "POST", f"repos/{REPO}/issues/{pr['number']}/comments", "-f", f"body={body}")
                        await codex_role(root, state, "protocol-fix",
                            f"Corrige solo los defectos de metadata/lifecycle PROTOCOL_FIX del Reviewer ID {latest_current_verdict['id']} en PR #{pr['number']} / {wp}. Mantén PRODUCT_SHA {frozen}; no hagas git commit ni cambies implementación/evidencia. Sigue PRODUCT_SHA_CLOSURE.md: reusa producto GREEN y solicita como máximo una nueva evaluación Ready. Si el SHA material cambia o el bloqueo no es metadata pura, detente sin fingir corrección. No actúes como Reviewer.",
                            "gpt-6-sol", "high")
                        new_pr, _ = await wait_for_state(pr["number"], lambda p, m, frozen=frozen, after=latest_current_verdict["at"]:
                                                         p["head"]["sha"].lower() != frozen or
                                                         bool((row := latest_marker(m, "REVIEW_READY", frozen)) and
                                                              row.get("_created_at", "") > after and ready_context_matches(root, p, row)))
                        if new_pr["head"]["sha"].lower() != frozen:
                            raise StopFlow(f"PR #{pr['number']} protocol-only correction changed PRODUCT_SHA")
                        continue
                if latest_current_verdict and latest_current_verdict["verdict"] == "PASS":
                    await wait_for_state(pr["number"], lambda p, m: bool(p.get("merged")))
                    continue
                if latest_current_verdict and latest_current_verdict["verdict"] == "FAIL" and not repair:
                    await wait_for_state(pr["number"], lambda p, m, frozen=frozen: bool(latest_marker(m, "REPAIR_REQUIRED", frozen)))
                    continue
                if repair:
                    if not latest_current_verdict or latest_current_verdict["verdict"] != "FAIL":
                        raise StopFlow(f"PR #{pr['number']} has REPAIR_REQUIRED without a parseable independent FAIL on {frozen}")
                    if len(fails) >= 4:
                        notify("BLOCKED", "Cuarto FAIL: detención obligatoria. Requiere acudir al PC y reauditar.", wp, pr["number"])
                        raise StopFlow("Fourth FAIL; PC required", notified=True)
                    local_rows = local_markers(pr["number"])
                    owner_continued = owner_authorized_continuation(local_rows, len(fails))
                    appeal_started = latest_marker(local_rows, "OVERDEFENSE_APPEAL_STARTED", frozen)
                    if appeal_started:
                        baseline = appeal_started.get("fail count", "")
                        if not baseline.isdecimal() or len(fails) <= int(baseline):
                            raise StopFlow(f"PR #{pr['number']} appeal started but has no new verdict; PC reconciliation required")
                    audit_row = next((row for row in reversed(local_rows)
                                      if row.get("state") == "FAIL_AUDIT_COMPLETE" and
                                      row.get("target sha") == frozen and
                                      row.get("fail count") == str(len(fails))), None)
                    if audit_row:
                        try:
                            audit = json.loads(audit_row["audit json"])
                        except (KeyError, json.JSONDecodeError) as exc:
                            raise StopFlow(f"PR #{pr['number']} has malformed durable FAIL audit") from exc
                    else:
                        schema = root / "scripts" / "local_wp_fail_audit.schema.json"
                        await quota_before_reasoning(app, wp, pr["number"])
                        result = await codex_role(root, state, "fail-audit",
                            f"Audita el FAIL material #{len(fails)} del PR #{pr['number']} / {wp} contra GitHub y contratos exactos. Decide si el último es defecto real, sobredefensa/duplicación de garantía aceptada, o incierto. Incluso al primer FAIL, detecta inmediatamente una prueba de completitud autocircular/autorreductora. En cada FAIL detecta también clase fundacional repetida y expansión de maquinaria de prueba sin progreso. No edites ni emitas veredicto. Responde al esquema JSON.",
                            "gpt-6-luna", "xhigh", schema)
                        audit = json.loads(result)
                        if not all(isinstance(audit.get(key), str) and audit[key].strip()
                                   for key in ("criterion", "evidence", "minimal_next_action")):
                            raise StopFlow("FAIL audit lacks a concrete criterion, evidence or next action")
                        body = ("ARKUS_LOCAL_AUTOPILOT\nState: FAIL_AUDIT_COMPLETE\n"
                                f"Target SHA: {frozen}\nFail count: {len(fails)}\n"
                                f"Audit JSON: {json.dumps(audit, ensure_ascii=False, separators=(',', ':'))}\n")
                        run("gh", "api", "--method", "POST", f"repos/{REPO}/issues/{pr['number']}/comments", "-f", f"body={body}")
                    policy = audit_policy(audit, len(fails))
                    if policy == "circuit_breaker":
                        notify("BLOCKED", f"Circuit breaker de prueba/arquitectura: {audit['evidence']}", wp, pr["number"])
                        raise StopFlow("Foundational circuit breaker", notified=True)
                    if policy == "needs_pc":
                        notify("BLOCKED", f"FAIL #{len(fails)} requiere juicio humano: {audit['evidence']}", wp, pr["number"])
                        raise StopFlow("FAIL audit requires PC", notified=True)
                    if policy == "appeal" and not appeal_started and appeal_rejected_sha != frozen:
                        appeal_sha = frozen
                    if len(fails) == 2 and policy == "valid":
                        second_fail_detail = f"Luna confirmó FAIL real: {audit['evidence']}. Pulsa continuar para otra reparación; cuarto FAIL exige PC."
                    if appeal_sha == frozen:
                        before_appeal_fails = len(fails)
                        await quota_before_reasoning(app, wp, pr["number"])
                        review_id = uuid.uuid4().hex
                        body = ("ARKUS_LOCAL_AUTOPILOT\nState: OVERDEFENSE_APPEAL_STARTED\n"
                                f"Target SHA: {frozen}\nFail count: {before_appeal_fails}\n"
                                f"Review ID: {review_id}\n"
                                "Detail: one independent same-SHA appeal is starting; restart must not repeat it.\n")
                        run("gh", "api", "--method", "POST", f"repos/{REPO}/issues/{pr['number']}/comments", "-f", f"body={body}")
                        await codex_role(root, state, "appeal-reviewer",
                            f"$validate-workpack Reviewer independiente NUEVO del PR #{pr['number']}, SHA {frozen}. Hubo FAIL previo y auditoría de posible sobredefensa. Reconstruye el contrato sin confiar en la auditoría; si el FAIL es inválido, deja PASS exact-SHA razonado que lo supersede; si es válido, mantén FAIL. Publica un solo veredicto en GitHub con líneas literales 'Reviewer verdict: PASS' o 'Reviewer verdict: FAIL', 'Reviewed candidate SHA: {frozen}' y 'Autopilot review ID: {review_id}'. No edites ni repares.",
                            "gpt-6-sol", "xhigh")
                        appeal_sha = ""
                        await wait_for_state(pr["number"], lambda p, m, review_id=review_id, pr_number=pr["number"]:
                                             any(row["id"] == review_id for row in reviewed_verdicts(pr_number)))
                        appeal_verdict = next(row for row in reviewed_verdicts(pr["number"]) if row["id"] == review_id)
                        if appeal_verdict["verdict"] == "FAIL":
                            appeal_rejected_sha = frozen
                            second_fail_detail = "Un Reviewer Sol independiente sostuvo el FAIL que Luna consideró posible sobredefensa. Pulsa continuar para otra reparación; cuarto FAIL exige PC."
                        continue
                    if len(fails) >= 2 and not owner_continued:
                        offer_continue(pr["number"], frozen, len(fails), wp,
                            second_fail_detail)
                        await wait_for_owner_continue(pr["number"], frozen, len(fails))
                        continue
                    await quota_before_reasoning(app, wp, pr["number"])
                    await codex_role(root, state, "repair", f"$repair-workpack Corrige el FAIL material de {wp}. PR canónico #{pr['number']}; conserva historia, revalida, pre-review y congela SHA nuevo. Sigue PRODUCT_SHA_CLOSURE.md: no repitas ejecución same-SHA por metadata. No actúes como Reviewer.", "gpt-6-sol", effort_for_worker(wp_text))
                    await wait_for_state(pr["number"], lambda p, m, frozen=frozen:
                                         p["head"]["sha"].lower() != frozen and
                                         ready_context_matches(root, p, latest_marker(m, "REVIEW_READY", p["head"]["sha"].lower())))
                    continue
                if ready:
                    review_id = uuid.uuid4().hex
                    await quota_before_reasoning(app, wp, pr["number"])
                    await codex_role(root, state, "reviewer", f"$validate-workpack Reviewer independiente NUEVO del PR #{pr['number']} / {wp}, SHA {frozen}. No recibes contexto del Worker. Sigue PRODUCT_SHA_CLOSURE.md: usa CI exact-SHA para hechos mecánicos; usa PROTOCOL_FIX/REVIEW_BLOCKED, no FAIL, para metadata pura. Publica una sola revisión o comentario en GitHub con líneas literales 'Reviewer verdict: PASS|FAIL|PROTOCOL_FIX|REVIEW_BLOCKED' (elige un valor, sin barras), 'Reviewed candidate SHA: {frozen}' y 'Autopilot review ID: {review_id}'. Si PASS, finaliza merge y DocSync documental según protocolo. No repares implementación.", "gpt-6-sol", "xhigh")
                    await wait_for_state(pr["number"], lambda p, m, review_id=review_id, pr_number=pr["number"]:
                                         any(row["id"] == review_id for row in reviewed_verdicts(pr_number)))
                    continue
                raise StopFlow(f"PR #{pr['number']} is neither REVIEW_READY nor REPAIR_REQUIRED for frozen SHA")
            if wp and not args.one_wp:
                # A later WP starts only after the prior DOCSYNC_COMPLETE and a fresh quota check.
                pr = None
                args.adopt = False
        print(f"Stopped after {count} completed WP(s); next={wp or 'NONE'}")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", default=".")
    parser.add_argument("--state", default=str(Path(os.environ.get("LOCALAPPDATA", str(Path.home()))) / "Arkus" / "Juego2" / "autopilot"))
    source = parser.add_mutually_exclusive_group(required=True)
    source.add_argument("--wp")
    source.add_argument("--next", action="store_true")
    parser.add_argument("--one-wp", action="store_true", help="stop after one DocSync; default continues while quota and WPs allow")
    parser.add_argument("--adopt", action="store_true", help="resume an already-open PR only after its current role stopped")
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()
    try:
        if args.dry_run:
            asyncio.run(main_async(args))
        else:
            with one_controller(Path(args.state).resolve()):
                asyncio.run(main_async(args))
    except (StopFlow, OSError, json.JSONDecodeError) as exc:
        if not args.dry_run and not getattr(exc, "notified", False):
            try:
                notify("HUMAN_ACTION_REQUIRED", f"Controlador local detenido: {exc}", args.wp or "")
            except (StopFlow, OSError):
                pass
        print(f"AUTOPILOT_STOP: {exc}", file=sys.stderr)
        return 2
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
