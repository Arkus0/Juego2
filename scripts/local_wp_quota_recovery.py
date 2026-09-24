#!/usr/bin/env python3
"""Pure quota classification for interrupted local Codex roles.

This module has no GitHub, Telegram, subprocess, or sleep side effects.  It only
classifies a fresh app-server rate-limit payload after a role process exits
abnormally, so the remote controller can distinguish a recoverable short-window
exhaustion from the protected general/weekly floor or a non-quota failure.
"""

from __future__ import annotations

from dataclasses import dataclass
from typing import Any


class QuotaRecoveryError(RuntimeError):
    pass


@dataclass(frozen=True)
class QuotaWindow:
    bucket: str
    name: str
    remaining: float
    minutes: int
    resets_at: int
    explicitly_reached: bool


def classify_after_session_failure(payload: dict[str, Any], now: int,
                                   threshold: float = 3.0) -> tuple[str, int | None]:
    """Return stop_general, wait_short, not_quota, or unknown_reached.

    Long/general exhaustion always wins over a simultaneous short-window reset.
    A short window is auto-retryable only when its reset is still in the future.
    Unknown reached-type shapes fail closed rather than inventing a reset.
    """
    buckets = payload.get("rateLimitsByLimitId") or {"legacy": payload.get("rateLimits")}
    if not isinstance(buckets, dict) or not buckets:
        raise QuotaRecoveryError("Quota unavailable after failed role")

    windows: list[QuotaWindow] = []
    unknown_reached = False
    for bucket_name, bucket in buckets.items():
        if not isinstance(bucket, dict):
            raise QuotaRecoveryError("Quota bucket unavailable after failed role")
        reached = bucket.get("rateLimitReachedType")
        if reached is not None and not isinstance(reached, str):
            unknown_reached = True
        reached_name = reached.lower() if isinstance(reached, str) else ""
        matched_reached = False
        for name in ("primary", "secondary"):
            value = bucket.get(name)
            if value is None:
                continue
            if not isinstance(value, dict) or not all(
                    isinstance(value.get(key), (int, float))
                    for key in ("usedPercent", "windowDurationMins", "resetsAt")):
                raise QuotaRecoveryError("Incomplete quota window after failed role")
            explicit = reached_name == name
            matched_reached = matched_reached or explicit
            windows.append(QuotaWindow(
                str(bucket_name), name, 100.0 - float(value["usedPercent"]),
                int(value["windowDurationMins"]), int(value["resetsAt"]), explicit))
        if reached_name and reached_name not in {"primary", "secondary"}:
            unknown_reached = True
        elif reached_name and not matched_reached:
            unknown_reached = True

    if not windows:
        raise QuotaRecoveryError("No readable quota windows after failed role")
    if not any(window.minutes > 360 for window in windows):
        raise QuotaRecoveryError("General quota window unavailable after failed role")

    general = [window for window in windows
               if window.minutes > 360 and
               (window.remaining <= threshold or window.explicitly_reached)]
    if general:
        return "stop_general", None

    short = [window for window in windows
             if window.minutes <= 360 and
             (window.remaining <= threshold or window.explicitly_reached)]
    if short:
        reset = max(window.resets_at for window in short)
        if reset <= now:
            raise QuotaRecoveryError("Short quota reset is stale; re-read limits")
        return "wait_short", reset

    if unknown_reached:
        return "unknown_reached", None
    return "not_quota", None
