#!/usr/bin/env python3
"""Supervisor-only proof helpers for local Telegram owner authority."""

from __future__ import annotations

import hashlib
import hmac
import json
import re

REPO = "Arkus0/Juego2"
SHA_RE = re.compile(r"^[0-9a-f]{40}$")
CAMPAIGN_RE = re.compile(r"^[0-9a-f]{32}$")
PROOF_RE = re.compile(r"^[0-9a-f]{64}$")


class OwnerProofError(ValueError):
    pass


def _canonical_continue(pr: int, sha: str, fail_count: int, campaign_id: str,
                        telegram_update_id: str) -> bytes:
    sha = sha.strip().lower()
    campaign_id = campaign_id.strip().lower()
    telegram_update_id = telegram_update_id.strip()
    if pr < 1 or not SHA_RE.fullmatch(sha) or fail_count not in (2, 3):
        raise OwnerProofError("invalid PR/SHA/fail-count")
    if not CAMPAIGN_RE.fullmatch(campaign_id):
        raise OwnerProofError("invalid campaign id")
    if not telegram_update_id or len(telegram_update_id) > 120:
        raise OwnerProofError("invalid Telegram callback identity")
    payload = {
        "campaign_id": campaign_id,
        "fail_count": fail_count,
        "kind": "OWNER_CONTINUE",
        "pr": pr,
        "repo": REPO,
        "sha": sha,
        "telegram_update_id": telegram_update_id,
        "version": 1,
    }
    return json.dumps(payload, sort_keys=True, separators=(",", ":")).encode("utf-8")


def sign_owner_continue(secret: str, pr: int, sha: str, fail_count: int,
                        campaign_id: str, telegram_update_id: str) -> str:
    if not secret:
        raise OwnerProofError("owner proof secret missing")
    message = _canonical_continue(pr, sha, fail_count, campaign_id, telegram_update_id)
    return hmac.new(secret.encode("utf-8"), message, hashlib.sha256).hexdigest()


def verify_owner_continue(secret: str, proof: str, pr: int, sha: str, fail_count: int,
                          campaign_id: str, telegram_update_id: str) -> bool:
    if not secret or not PROOF_RE.fullmatch((proof or "").strip().lower()):
        return False
    try:
        expected = sign_owner_continue(secret, pr, sha, fail_count, campaign_id, telegram_update_id)
    except OwnerProofError:
        return False
    return hmac.compare_digest(expected, proof.strip().lower())
