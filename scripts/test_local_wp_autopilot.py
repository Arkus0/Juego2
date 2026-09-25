#!/usr/bin/env python3
"""Routing regressions with safe-output Reviewer fixtures.

The accepted broad routing suite is loaded unchanged, then only the four
Reviewer-fixture tests whose old premise was 'legacy prose is authority' are
replaced. This keeps all unrelated lifecycle coverage intact while asserting
the new commit-bound ARKUS_INTENT_V1 boundary.
"""

from __future__ import annotations

import importlib.util
from pathlib import Path
import sys
import unittest
from unittest.mock import patch

HERE = Path(__file__).resolve().parent


def _load(name: str, path: Path):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(path)
    loaded = importlib.util.module_from_spec(spec)
    sys.modules[name] = loaded
    spec.loader.exec_module(loaded)
    return loaded


legacy = _load("arkus_local_wp_autopilot_routing_regressions", HERE / "_test_local_wp_autopilot_core.py")
module = legacy.module
RoutingTests = legacy.RoutingTests


def _pr() -> dict:
    return {"number": 123, "body": "WP: WP-H1-03\n"}


def _review(verdict: str, sha: str, review_id: str, *, actor: str = "Arkus0",
            at: str = "2026-01-01T00:00:00Z") -> dict:
    intent = module.safe.render_review(wp="WP-H1-03", pr=123, candidate_sha=sha,
                                       verdict=verdict, review_id=review_id)
    body = (f"Reviewer verdict: {verdict}\n"
            f"Reviewed candidate SHA: {sha}\n"
            f"Autopilot review ID: {review_id}\n"
            f"{intent}\n")
    return {"body": body, "user": {"login": actor}, "submitted_at": at,
            "commit_id": sha}


def test_duplicate_review_and_comment_count_once(self):
    sha = "a" * 40
    row = _review("FAIL", sha, "1" * 32)
    with patch.object(module, "gh_json", return_value=_pr()), \
         patch.object(module, "gh_pages", return_value=[row, dict(row)]):
        self.assertEqual(len(module.reviewed_fails(123)), 1)


def test_same_words_different_review_ids_count_separately(self):
    sha = "a" * 40
    rows = [_review("FAIL", sha, i * 32, at=f"2026-01-01T00:00:0{i}Z") for i in ("1", "2")]
    with patch.object(module, "gh_json", return_value=_pr()), \
         patch.object(module, "gh_pages", return_value=rows):
        self.assertEqual(len(module.reviewed_fails(123)), 2)


def test_protocol_fix_is_not_a_material_fail(self):
    sha = "a" * 40
    row = _review("PROTOCOL_FIX", sha, "1" * 32)
    with patch.object(module, "gh_json", return_value=_pr()), \
         patch.object(module, "gh_pages", return_value=[row]):
        self.assertEqual(module.reviewed_verdicts(123)[0]["verdict"], "PROTOCOL_FIX")
        self.assertEqual(module.reviewed_fails(123), [])


def test_manual_untagged_verdict_stops_adoption(self):
    sha = "a" * 40
    row = {"body": f"Reviewer verdict: FAIL\nReviewed candidate SHA: {sha}\n",
           "user": {"login": "Arkus0"}, "submitted_at": "2026-01-01T00:00:00Z",
           "commit_id": sha}
    with patch.object(module, "gh_json", return_value=_pr()), \
         patch.object(module, "gh_pages", return_value=[row]), \
         self.assertRaisesRegex(module.StopFlow, "manual/untagged"):
        module.reviewed_verdicts(123)


RoutingTests.test_duplicate_review_and_comment_count_once = test_duplicate_review_and_comment_count_once
RoutingTests.test_same_words_different_review_ids_count_separately = test_same_words_different_review_ids_count_separately
RoutingTests.test_protocol_fix_is_not_a_material_fail = test_protocol_fix_is_not_a_material_fail
RoutingTests.test_manual_untagged_verdict_stops_adoption = test_manual_untagged_verdict_stops_adoption


if __name__ == "__main__":
    unittest.main(module=legacy)
