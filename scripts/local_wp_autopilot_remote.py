#!/usr/bin/env python3
"""Hotfix shim for recovered-Worker handoff liveness.

The accepted remote controller implementation is retained byte-for-byte in
local_wp_autopilot_remote_base.py. This shim adds one fail-closed wait before
canonical adoption so a recovered Worker cannot race the asynchronous
REVIEW_READY marker emitted after Candidate Validation completes.
"""

from pathlib import Path

_BASE_PATH = Path(__file__).with_name("local_wp_autopilot_remote_base.py")
exec(compile(_BASE_PATH.read_text(encoding="utf-8"), str(_BASE_PATH), "exec"), globals(), globals())

_ORIGINAL_RUN_CANONICAL_ADOPTED = _run_canonical_adopted


async def _run_canonical_adopted(args: argparse.Namespace) -> None:
    """Wait for the durable post-freeze transition before canonical adoption."""
    root = Path(args.root).resolve()
    wp = autopilot.normalize_wp(args.wp) if args.wp else None
    if wp:
        pr = autopilot.canonical_pr(wp)
        if pr and pr.get("state") == "open" and not pr.get("merged_at"):
            current = autopilot.gh_json("api", f"repos/{autopilot.REPO}/pulls/{pr['number']}")
            head = ((current.get("head") or {}).get("sha") or "").lower()
            frozen = autopilot.fields(current.get("body") or "").get("frozen candidate sha", "").lower()
            if autopilot.SHA_RE.fullmatch(frozen) and frozen == head:
                def transitioned(candidate: dict, rows: list[dict[str, str]]) -> bool:
                    if (autopilot.latest_marker(rows, "BLOCKED") or
                            autopilot.latest_marker(rows, "HUMAN_ACTION_REQUIRED") or
                            autopilot.latest_marker(rows, "REPAIR_REQUIRED", frozen)):
                        return True
                    ready = autopilot.latest_marker(rows, "REVIEW_READY", frozen)
                    return bool(ready and autopilot.ready_context_matches(root, candidate, ready))

                rows = autopilot.markers(pr["number"])
                if not transitioned(current, rows):
                    current, rows = await autopilot.wait_for_state(
                        pr["number"], transitioned)
                if (autopilot.latest_marker(rows, "BLOCKED") or
                        autopilot.latest_marker(rows, "HUMAN_ACTION_REQUIRED")):
                    raise autopilot.StopFlow(
                        f"PR #{pr['number']} became blocked while awaiting post-Worker handoff")

    await _ORIGINAL_RUN_CANONICAL_ADOPTED(args)
