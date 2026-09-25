#!/usr/bin/env python3
"""PROCESS_ONLY liveness policy for the local WP controller and owner console.

GitHub verdicts, exact SHAs and authority remain authoritative. This module
only classifies operational failures and derived local campaign state.
"""

from __future__ import annotations

from enum import Enum
import hashlib
import hmac
import json
from typing import Any


class Disposition(str, Enum):
    RECOVER = "RECOVERABLE"
    RETRY = "RETRYABLE"
    PAUSE = "PAUSED_RECOVERABLE"
    HARD = "HARD_BLOCKER"


class RetryableInfrastructure(Exception):
    """A bounded, idempotent retry may succeed without changing authority."""


class HardBlocker(Exception):
    """A material contradiction or required human judgment stops this WP."""


def campaign_proof(secret: str, payload: dict[str, Any]) -> str:
    """Authenticate only the local routing hint, never a GitHub verdict."""
    unsigned = {key: value for key, value in payload.items() if key != "proof"}
    data = json.dumps(unsigned, sort_keys=True, ensure_ascii=False,
                      separators=(",", ":")).encode("utf-8")
    return hmac.new(secret.encode("utf-8"), b"arkus-campaign-v1\0" + data,
                    hashlib.sha256).hexdigest()


def valid_campaign_proof(secret: str, payload: dict[str, Any]) -> bool:
    proof = payload.get("proof")
    return isinstance(proof, str) and hmac.compare_digest(
        proof, campaign_proof(secret, payload))


_TRANSIENT = (
    "timed out", "timeout", "temporarily unavailable", "connection reset",
    "connection refused", "could not resolve host", "network is unreachable",
    "could not connect", "error connecting", "service unavailable", "bad gateway",
    "gateway timeout", "tls handshake", "app server closed unexpectedly",
    "session failed", "quota reset time is stale",
)


def classify_failure(error: BaseException) -> Disposition:
    """Unclassified failures fail closed for the WP, never for the supervisor."""
    if isinstance(error, RetryableInfrastructure | TimeoutError | ConnectionError):
        return Disposition.RETRY
    if isinstance(error, HardBlocker):
        return Disposition.HARD
    message = str(error).lower()
    if any(fragment in message for fragment in _TRANSIENT):
        return Disposition.RETRY
    if message.startswith("gh failed") and any(fragment in message for fragment in
                                               ("http 5", "http 429", "connection", "timeout")):
        return Disposition.RETRY
    return Disposition.HARD


def child_disposition(log_text: str, exit_code: int) -> Disposition:
    """Use the child's explicit record; an unrecorded crash is retryable."""
    if exit_code == 0:
        return Disposition.RECOVER
    for line in reversed(log_text.splitlines()):
        if line.startswith("WORK_PLANE_RESULT: "):
            value = line.partition(": ")[2].split(" ", 1)[0]
            try:
                return Disposition(value)
            except ValueError:
                break
    for line in reversed(log_text.splitlines()):
        if line.startswith("REMOTE_AUTOPILOT_STOP: "):
            return classify_failure(Exception(line.partition(": ")[2]))
    return Disposition.RETRY


def active_blocker(rows: list[dict[str, Any]], sha: str) -> dict[str, Any] | None:
    """A blocker bound to a superseded SHA cannot stop the current candidate."""
    for row in reversed(rows):
        if row.get("state") not in {"BLOCKED", "HUMAN_ACTION_REQUIRED"}:
            continue
        target = str(row.get("target sha") or "").lower()
        if not target or target == sha:
            return row
    return None
