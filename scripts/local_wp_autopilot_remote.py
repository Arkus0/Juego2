#!/usr/bin/env python3
"""Hotfix shim for recovered-Worker handoff and short-quota liveness.

The accepted remote controller implementation is retained byte-for-byte in
local_wp_autopilot_remote_base.py. This shim adds two bounded PROCESS_ONLY fixes:
- wait for the durable post-freeze transition before canonical adoption, so a
  recovered Worker cannot race the asynchronous REVIEW_READY marker;
- classify an explicitly reached five-hour quota window as wait_short instead of
  aborting before quota_before_reasoning can keep the campaign alive.
"""

from pathlib import Path

_BASE_PATH = Path(__file__).with_name("local_wp_autopilot_remote_base.py")
exec(compile(_BASE_PATH.read_text(encoding="utf-8"), str(_BASE_PATH), "exec"), globals(), globals())


def _quota_decision_with_reached(payload: dict, now: int,
                                 threshold: float = 3.0) -> tuple[str, int | None]:
    """Pre-role quota guard with explicit reached-window support.

    A long/general window always wins over a short window. A known short window
    at/below the protected floor, or explicitly reported as reached, waits for its
    reset. Unknown reached-type shapes fail closed rather than being guessed.
    """
    buckets = payload.get("rateLimitsByLimitId") or {"legacy": payload.get("rateLimits")}
    windows: list[tuple[float, int, int, bool]] = []
    if not isinstance(buckets, dict) or not buckets:
        raise autopilot.StopFlow("Quota unavailable")

    for bucket in buckets.values():
        if not isinstance(bucket, dict):
            raise autopilot.StopFlow("Quota bucket unavailable")
        reached = bucket.get("rateLimitReachedType")
        if reached is not None and not isinstance(reached, str):
            raise autopilot.StopFlow("Rate limit reached type malformed")
        reached_name = reached.lower() if isinstance(reached, str) else ""
        present: set[str] = set()
        for name in ("primary", "secondary"):
            value = bucket.get(name)
            if value is None:
                continue
            if (not isinstance(value, dict) or
                    not all(isinstance(value.get(key), (int, float))
                            for key in ("usedPercent", "windowDurationMins", "resetsAt"))):
                raise autopilot.StopFlow("Incomplete quota window")
            present.add(name)
            windows.append((
                100.0 - float(value["usedPercent"]),
                int(value["windowDurationMins"]),
                int(value["resetsAt"]),
                reached_name == name,
            ))
        if reached_name and reached_name not in present:
            raise autopilot.StopFlow(f"Unknown rate limit reached type: {reached}")

    if not windows:
        raise autopilot.StopFlow("No readable quota windows")
    if not any(minutes > 360 for _, minutes, _, _ in windows):
        raise autopilot.StopFlow("General quota window unavailable; cannot protect 3% floor")

    low = [window for window in windows if window[0] <= threshold or window[3]]
    if any(minutes > 360 for _, minutes, _, _ in low):
        return "stop_general", None

    short = [window for window in low if window[1] <= 360]
    if not short:
        return "run", None
    reset = max(window[2] for window in short)
    if reset <= now:
        raise autopilot.StopFlow("Quota reset time is stale; re-read limits")
    return "wait_short", reset


# AppServer.guard() resolves quota_decision from the canonical autopilot module at
# call time, so replacing this symbol affects both initial /run admission and the
# post-reset re-check without changing the accepted lifecycle implementation.
autopilot.quota_decision = _quota_decision_with_reached

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
