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
DECISION_RE = re.compile(r"^[0-9a-f]{32}$")
DIGEST_RE = re.compile(r"^[0-9a-f]{64}$")
PROOF_RE = DIGEST_RE
WP_RE = re.compile(r"^(?:WP-)?([A-Z][A-Z0-9]*(?:-[A-Z0-9]+)+)$")


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


def decision_selected_digest(selected: str) -> str:
    if not isinstance(selected, str) or not selected or len(selected) > 240:
        raise OwnerProofError("invalid selected decision value")
    return hashlib.sha256(selected.encode("utf-8")).hexdigest()


def decision_request_digest(campaign_id: str, decision_id: str, wp: str, pr: int, sha: str,
                            question: str, detail: str, options: list[str]) -> str:
    campaign_id = campaign_id.strip().lower()
    decision_id = decision_id.strip().lower()
    sha = sha.strip().lower()
    match = WP_RE.fullmatch(wp.strip().upper())
    if not CAMPAIGN_RE.fullmatch(campaign_id) or not DECISION_RE.fullmatch(decision_id):
        raise OwnerProofError("invalid decision identity")
    if pr < 1 or not SHA_RE.fullmatch(sha):
        raise OwnerProofError("invalid decision PR/SHA")
    if not match:
        raise OwnerProofError("invalid decision WP")
    if not isinstance(question, str) or not question.strip() or len(question.strip()) > 800:
        raise OwnerProofError("invalid decision question")
    if not isinstance(detail, str) or len(detail.strip()) > 1200:
        raise OwnerProofError("invalid decision detail")
    if (not isinstance(options, list) or not 2 <= len(options) <= 3 or
            any(not isinstance(item, str) or not item.strip() or len(item.strip()) > 240 for item in options)):
        raise OwnerProofError("invalid decision options")
    normalized_options = [item.strip() for item in options]
    if len(set(normalized_options)) != len(normalized_options):
        raise OwnerProofError("decision options must be distinct")
    payload = {
        "campaign_id": campaign_id,
        "decision_id": decision_id,
        "detail": detail.strip(),
        "kind": "OWNER_DECISION_REQUEST",
        "options": normalized_options,
        "pr": pr,
        "question": question.strip(),
        "repo": REPO,
        "sha": sha,
        "version": 1,
        "wp": match.group(1),
    }
    canonical = json.dumps(payload, sort_keys=True, separators=(",", ":")).encode("utf-8")
    return hashlib.sha256(canonical).hexdigest()


def _canonical_decision(pr: int, sha: str, campaign_id: str, decision_id: str,
                        request_digest: str, choice: int, selected_digest: str,
                        telegram_update_id: str) -> bytes:
    sha = sha.strip().lower()
    campaign_id = campaign_id.strip().lower()
    decision_id = decision_id.strip().lower()
    request_digest = request_digest.strip().lower()
    selected_digest = selected_digest.strip().lower()
    telegram_update_id = telegram_update_id.strip()
    if pr < 1 or not SHA_RE.fullmatch(sha):
        raise OwnerProofError("invalid decision PR/SHA")
    if not CAMPAIGN_RE.fullmatch(campaign_id) or not DECISION_RE.fullmatch(decision_id):
        raise OwnerProofError("invalid decision identity")
    if not DIGEST_RE.fullmatch(request_digest) or not DIGEST_RE.fullmatch(selected_digest):
        raise OwnerProofError("invalid decision digest")
    if type(choice) is not int or not 0 <= choice <= 2:
        raise OwnerProofError("invalid decision choice")
    if not telegram_update_id or len(telegram_update_id) > 120:
        raise OwnerProofError("invalid Telegram callback identity")
    payload = {
        "campaign_id": campaign_id,
        "choice": choice,
        "decision_id": decision_id,
        "kind": "OWNER_DECISION",
        "pr": pr,
        "repo": REPO,
        "request_digest": request_digest,
        "selected_digest": selected_digest,
        "sha": sha,
        "telegram_update_id": telegram_update_id,
        "version": 1,
    }
    return json.dumps(payload, sort_keys=True, separators=(",", ":")).encode("utf-8")


def sign_owner_decision(secret: str, pr: int, sha: str, campaign_id: str, decision_id: str,
                        request_digest: str, choice: int, selected_digest: str,
                        telegram_update_id: str) -> str:
    if not secret:
        raise OwnerProofError("owner proof secret missing")
    message = _canonical_decision(pr, sha, campaign_id, decision_id, request_digest,
                                  choice, selected_digest, telegram_update_id)
    return hmac.new(secret.encode("utf-8"), message, hashlib.sha256).hexdigest()


def verify_owner_decision(secret: str, proof: str, pr: int, sha: str, campaign_id: str,
                          decision_id: str, request_digest: str, choice: int,
                          selected_digest: str, telegram_update_id: str) -> bool:
    if not secret or not PROOF_RE.fullmatch((proof or "").strip().lower()):
        return False
    try:
        expected = sign_owner_decision(secret, pr, sha, campaign_id, decision_id,
                                       request_digest, choice, selected_digest, telegram_update_id)
    except OwnerProofError:
        return False
    return hmac.compare_digest(expected, proof.strip().lower())
