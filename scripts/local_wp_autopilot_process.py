#!/usr/bin/env python3
"""PROCESS_ONLY state-aware remote lifecycle wrapper.

/run and /work both adopt canonical GitHub state. /run advances the complete
existing lifecycle. /work permits only Worker/Repair Codex roles and stops
before any Reviewer or other non-Worker/Repair reasoning turn.
"""

from __future__ import annotations

import argparse
import asyncio
import importlib.util
import json
import re
import sys
import time
from pathlib import Path
from typing import Any

HERE = Path(__file__).parent


def _load(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise SystemExit(f"Cannot load {path.name}")
    module = importlib.util.module_from_spec(spec)
    sys.modules[spec.name] = module
    spec.loader.exec_module(module)
    return module


remote = _load("arkus_local_wp_autopilot_remote_process", HERE / "local_wp_autopilot_remote.py")
adoption = _load("arkus_reviewer_verdict_adoption_process", HERE / "reviewer_verdict_adoption.py")
autopilot = remote.autopilot

WORK_ONLY = False
ADOPTION_TIMEOUT = 900


class WorkBoundary(Exception):
    def __init__(self, kind: str, wp: str, sha: str, state: str = "") -> None:
        super().__init__(kind)
        self.kind = kind
        self.wp = wp
        self.sha = sha
        self.state = state


def _raw_review_items(pr: int) -> tuple[list[dict[str, Any]], list[dict[str, Any]]]:
    return (
        autopilot.gh_pages(f"repos/{autopilot.REPO}/pulls/{pr}/reviews?per_page=100"),
        autopilot.gh_pages(f"repos/{autopilot.REPO}/issues/{pr}/comments?per_page=100"),
    )


def _authoritative_verdicts(pr: int) -> list[dict[str, str]]:
    reviews, comments = _raw_review_items(pr)
    try:
        return adoption.authoritative_verdicts(reviews, comments)
    except adoption.AdoptionError as exc:
        raise autopilot.StopFlow(f"PR #{pr} Reviewer verdict invalid: {exc}") from exc


def _bot_adoption(pr: int, review_id: str) -> dict[str, str] | None:
    comments = autopilot.gh_pages(f"repos/{autopilot.REPO}/issues/{pr}/comments?per_page=100")
    try:
        return adoption.adoption_marker(comments, review_id)
    except adoption.AdoptionError as exc:
        raise autopilot.StopFlow(f"PR #{pr} adoption ledger invalid: {exc}") from exc


def _adoption_complete(pr: int, verdict: dict[str, str]) -> bool:
    marker = _bot_adoption(pr, verdict["id"])
    if marker is None:
        return False
    if ((marker.get("target sha") or "").lower() != verdict["sha"] or
            (marker.get("verdict") or "").upper() != verdict["verdict"]):
        raise autopilot.StopFlow(f"PR #{pr} adoption ledger contradicts Reviewer ID {verdict['id']}")
    if verdict["verdict"] != "FAIL":
        return True
    cycle = marker.get("fail cycle", "")
    if not cycle.isdecimal() or int(cycle) < 1:
        raise autopilot.StopFlow(f"PR #{pr} FAIL adoption ledger has invalid fail_cycle")
    comments = autopilot.gh_pages(f"repos/{autopilot.REPO}/issues/{pr}/comments?per_page=100")
    if not adoption.repair_marker_exists(comments, verdict["sha"]):
        return False
    current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr}")
    try:
        return adoption.fail_cycle(current.get("body") or "") == int(cycle)
    except adoption.AdoptionError as exc:
        raise autopilot.StopFlow(f"PR #{pr} fail_cycle invalid: {exc}") from exc


def _dispatch_adoption(pr: int, verdict: dict[str, str]) -> None:
    payload = {
        "event_type": "arkus_adopt_reviewer_verdict",
        "client_payload": {
            "pr": pr,
            "target_sha": verdict["sha"],
            "review_id": verdict["id"],
            "verdict": verdict["verdict"],
        },
    }
    autopilot.run("gh", "api", "--method", "POST", f"repos/{autopilot.REPO}/dispatches",
                  "--input", "-", input_data=json.dumps(payload))


def _ensure_adopted(pr: int, verdict: dict[str, str]) -> None:
    if verdict["verdict"] not in {"PASS", "FAIL"} or _adoption_complete(pr, verdict):
        return
    _dispatch_adoption(pr, verdict)
    deadline = time.monotonic() + ADOPTION_TIMEOUT
    while time.monotonic() < deadline:
        if _adoption_complete(pr, verdict):
            return
        time.sleep(5)
    raise autopilot.StopFlow(
        f"PR #{pr} timed out waiting for durable adoption of Reviewer ID {verdict['id']}")


def strict_reviewed_verdicts(pr: int) -> list[dict[str, str]]:
    """Read GitHub verdicts strictly and durably adopt the current exact-SHA one."""
    verdicts = _authoritative_verdicts(pr)
    current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr}")
    if not isinstance(current, dict):
        raise autopilot.StopFlow(f"PR #{pr} lookup returned malformed data")
    if current.get("state") != "open" or current.get("merged"):
        return verdicts

    body = current.get("body") or ""
    frozen = autopilot.fields(body).get("frozen candidate sha", "").lower()
    head = ((current.get("head") or {}).get("sha") or "").lower()
    if not autopilot.SHA_RE.fullmatch(frozen) or frozen != head:
        return verdicts

    rows = autopilot.markers(pr)
    ready_candidate = autopilot.latest_marker(rows, "REVIEW_READY", frozen)
    ready = ready_candidate if autopilot.ready_context_matches(HERE.parent, current, ready_candidate) else None
    current_verdicts = [row for row in verdicts if row["sha"] == frozen]
    if current_verdicts and ready is None:
        raise autopilot.StopFlow(
            f"PR #{pr} has an exact-current-SHA verdict without current context-bound REVIEW_READY")
    if ready is not None:
        ready_at = ready.get("_created_at", "")
        for row in verdicts:
            if row["at"] >= ready_at and row["sha"] != frozen:
                raise autopilot.StopFlow(
                    f"PR #{pr} has wrong-SHA Reviewer verdict {row['sha']} in current REVIEW_READY cycle")
    autopilot.assert_verdict_sequence(pr, frozen, verdicts, autopilot.local_markers(pr))
    latest = next((row for row in reversed(verdicts) if row["sha"] == frozen), None)
    if latest and latest["verdict"] in {"PASS", "FAIL"}:
        _ensure_adopted(pr, latest)
    return verdicts


def _current_identity_from_prompt(prompt: str) -> tuple[str, int | None, str]:
    wp, pr = remote._role_identity(prompt)
    sha = ""
    if pr is not None:
        current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr}")
        sha = ((current.get("head") or {}).get("sha") or "").lower()
    if not autopilot.SHA_RE.fullmatch(sha):
        match = re.search(r"\bSHA\s+([0-9a-f]{40})\b", prompt, re.IGNORECASE)
        sha = match.group(1).lower() if match else ""
    return wp, pr, sha


def _role_boundary_kind(role: str) -> tuple[str, str]:
    if role == "reviewer":
        return "REVIEW_READY", ""
    return "REVIEWER_REQUIRED", role.upper().replace("-", "_")


async def bounded_codex_role(root: Path, state: Path, role: str, prompt: str, model: str,
                             effort: str, schema: Path | None = None,
                             assets_root: Path | None = None) -> str:
    if WORK_ONLY and role not in {"worker", "repair"}:
        wp, _pr, sha = _current_identity_from_prompt(prompt)
        if not wp or not autopilot.SHA_RE.fullmatch(sha):
            raise autopilot.StopFlow(
                f"/work reached forbidden role {role!r} without exact WP/SHA identity")
        kind, state_name = _role_boundary_kind(role)
        raise WorkBoundary(kind, wp, sha, state_name)
    return await remote.remote_codex_role(root, state, role, prompt, model, effort,
                                          schema, assets_root)


def _print_boundary(boundary: WorkBoundary) -> None:
    if boundary.kind == "CLOSED":
        print(f"WORK_BOUNDARY CLOSED WP={boundary.wp} SHA={boundary.sha or 'NONE'} STATE={boundary.state or 'CLOSED'}")
    elif boundary.state:
        print(f"WORK_BOUNDARY {boundary.kind} WP={boundary.wp} SHA={boundary.sha} STATE={boundary.state}")
    else:
        print(f"WORK_BOUNDARY {boundary.kind} WP={boundary.wp} SHA={boundary.sha}")


def _work_preflight(root: Path, wp: str) -> WorkBoundary | None:
    pr = autopilot.canonical_pr(wp)
    if pr is None:
        return None
    current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr['number']}")
    if current.get("merged") or current.get("state") == "closed":
        sha = ((current.get("head") or {}).get("sha") or "").lower()
        return WorkBoundary("CLOSED", wp, sha if autopilot.SHA_RE.fullmatch(sha) else "", "MERGED" if current.get("merged") else "CLOSED")
    if current.get("state") != "open":
        raise autopilot.StopFlow(f"PR #{pr['number']} has unsupported state")

    body = current.get("body") or ""
    frozen = autopilot.fields(body).get("frozen candidate sha", "").lower()
    head = ((current.get("head") or {}).get("sha") or "").lower()
    if not autopilot.SHA_RE.fullmatch(frozen) or frozen != head:
        return None  # ACTIVE/IN_PROGRESS or Worker-side handoff still in progress.

    verdicts = strict_reviewed_verdicts(pr["number"])
    rows = autopilot.markers(pr["number"])
    latest = next((row for row in reversed(verdicts) if row["sha"] == frozen), None)
    if latest and latest["verdict"] == "PASS":
        return WorkBoundary("CLOSED", wp, frozen, "PASS")
    if latest and latest["verdict"] in {"REVIEW_BLOCKED", "PROTOCOL_FIX"}:
        return None
    if latest and latest["verdict"] == "FAIL":
        # strict_reviewed_verdicts synchronously waits for durable adoption; the
        # canonical lifecycle may now run Repair only if its non-Worker prerequisites
        # are already durable. bounded_codex_role stops before any missing audit role.
        return None

    ready_candidate = autopilot.latest_marker(rows, "REVIEW_READY", frozen)
    ready = ready_candidate if autopilot.ready_context_matches(root, current, ready_candidate) else None
    if ready:
        return WorkBoundary("REVIEW_READY", wp, frozen)
    return None


async def _resume_merged(args: argparse.Namespace, wp: str, pr: dict[str, Any]) -> None:
    root = Path(args.root).resolve()
    state = Path(args.state).resolve()
    current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr['number']}")
    frozen = autopilot.fields(current.get("body") or "").get("frozen candidate sha", "").lower()
    if not autopilot.SHA_RE.fullmatch(frozen):
        raise autopilot.StopFlow(f"Merged PR #{pr['number']} lacks frozen candidate SHA")
    verdicts = _authoritative_verdicts(pr["number"])
    autopilot.assert_verdict_sequence(pr["number"], frozen, verdicts, autopilot.local_markers(pr["number"]))
    accepted = next((row for row in reversed(verdicts) if row["sha"] == frozen), None)
    if not accepted or accepted["verdict"] != "PASS":
        raise autopilot.StopFlow(f"Merged PR #{pr['number']} lacks exact frozen-SHA PASS")
    autopilot.assert_exact_sha_merge(pr["number"], current, frozen)
    rows = autopilot.markers(pr["number"])
    completed = autopilot.latest_marker(rows, "DOCSYNC_COMPLETE")
    if not completed:
        assets_root = autopilot.resolve_assets_root(root, args.assets_root)
        async with autopilot.AppServer() as app:
            await app.assert_models()
            await autopilot.quota_before_reasoning(app, wp, pr["number"])
            await autopilot.codex_role(
                root, state, "docsync",
                f"Finaliza DocSync del PR #{pr['number']} de {wp}. Confirma PASS y merge exacto en GitHub. "
                "Sigue PRODUCT_SHA_CLOSURE.md y $update-handoff: cero commits por defecto si ninguna autoridad "
                "documental cambia; si cambia, una reconciliación acotada. Emite DOCSYNC_COMPLETE con Next WP válido. "
                "No cambies implementación.",
                "gpt-6-luna", "high", assets_root=assets_root)
        await autopilot.wait_for_state(pr["number"], lambda _p, markers: bool(autopilot.latest_marker(markers, "DOCSYNC_COMPLETE")))
        current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr['number']}")
    next_wp = autopilot.next_from_merged_pr(root, current)
    print(f"Stopped after 1 completed WP(s); next={next_wp or 'NONE'}")


async def process_main_async(args: argparse.Namespace) -> None:
    root = Path(args.root).resolve()
    wp = autopilot.normalize_wp(args.wp) if args.wp else autopilot.latest_next_wp(root)
    if wp is None:
        print("No next WP (DOCSYNC_COMPLETE says NONE)")
        return
    args.wp = wp
    args.next = False

    if args.work_only:
        boundary = _work_preflight(root, wp)
        if boundary is not None:
            raise boundary

    pr = autopilot.canonical_pr(wp)
    if pr and pr.get("merged_at"):
        if args.work_only:
            sha = ((pr.get("head") or {}).get("sha") or "").lower()
            raise WorkBoundary("CLOSED", wp, sha if autopilot.SHA_RE.fullmatch(sha) else "", "MERGED")
        await _resume_merged(args, wp, pr)
        return

    await remote.remote_main_async(args)


def _configure_remote() -> None:
    remote._control_dir()
    remote._campaign()
    remote._supervisor_url()
    autopilot.fresh_offer = lambda created_at, now=None: True
    autopilot.dispatch_continue_receiver = remote.no_hosted_receiver
    autopilot.wait_for_owner_continue = remote.wait_for_owner_continue_forever
    autopilot.codex_role = bounded_codex_role
    autopilot.reviewed_verdicts = strict_reviewed_verdicts


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", default=".")
    parser.add_argument("--assets-root")
    parser.add_argument("--state", default=str(Path(autopilot.os.environ.get("LOCALAPPDATA", str(Path.home()))) / "Arkus" / "Juego2" / "autopilot"))
    source = parser.add_mutually_exclusive_group(required=True)
    source.add_argument("--wp")
    source.add_argument("--next", action="store_true")
    parser.add_argument("--one-wp", action="store_true")
    parser.add_argument("--adopt", action="store_true")
    parser.add_argument("--work-only", action="store_true")
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()

    global WORK_ONLY
    WORK_ONLY = bool(args.work_only)
    try:
        _configure_remote()
        if args.dry_run:
            asyncio.run(process_main_async(args))
        else:
            with autopilot.one_controller(Path(args.state).resolve()):
                asyncio.run(process_main_async(args))
    except WorkBoundary as boundary:
        _print_boundary(boundary)
        return 0
    except (autopilot.StopFlow, OSError, json.JSONDecodeError, adoption.AdoptionError) as exc:
        if not args.dry_run and not getattr(exc, "notified", False):
            try:
                autopilot.notify("HUMAN_ACTION_REQUIRED", f"Controlador local detenido: {exc}", args.wp or "")
            except (autopilot.StopFlow, OSError):
                pass
        print(f"REMOTE_AUTOPILOT_STOP: {exc}", file=sys.stderr)
        return 2
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
