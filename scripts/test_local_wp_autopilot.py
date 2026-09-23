#!/usr/bin/env python3
"""Pure tests for local routing; no Codex turns, GitHub writes, or Telegram."""

import importlib.util
import unittest
from pathlib import Path
from unittest.mock import patch

SCRIPT = Path(__file__).with_name("local_wp_autopilot.py")
spec = importlib.util.spec_from_file_location("local_wp_autopilot", SCRIPT)
module = importlib.util.module_from_spec(spec)
import sys

sys.modules[spec.name] = module
spec.loader.exec_module(module)


class RoutingTests(unittest.TestCase):
    def test_wp_id_is_strict(self):
        self.assertEqual(module.normalize_wp("WP-H1-03"), "H1-03")
        self.assertEqual(module.normalize_wp("dw-05"), "DW-05")
        for bad in ("", "../H1-03", "H1-03;echo", "H1_03"):
            with self.assertRaises(module.StopFlow):
                module.normalize_wp(bad)

    def test_quota_general_wins_and_unknown_fails_closed(self):
        limits = {"rateLimitsByLimitId": {"codex": {
            "primary": {"usedPercent": 97, "windowDurationMins": 300, "resetsAt": 2000},
            "secondary": {"usedPercent": 98, "windowDurationMins": 10080, "resetsAt": 3000},
        }}}
        self.assertEqual(module.quota_decision(limits, 1000), ("stop_general", None))
        limits["rateLimitsByLimitId"]["codex"]["secondary"]["usedPercent"] = 20
        self.assertEqual(module.quota_decision(limits, 1000), ("wait_short", 2000))
        limits["rateLimitsByLimitId"]["codex"]["primary"]["usedPercent"] = 96
        self.assertEqual(module.quota_decision(limits, 1000), ("run", None))
        with self.assertRaises(module.StopFlow):
            module.quota_decision({}, 1000)

    def test_marker_requires_matching_sha(self):
        sha1, sha2 = "a" * 40, "b" * 40
        rows = [{"state": "REVIEW_READY", "target sha": sha1},
                {"state": "REPAIR_REQUIRED", "target sha": sha2}]
        self.assertIsNone(module.latest_marker(rows, "REPAIR_REQUIRED", sha1))
        self.assertEqual(module.latest_marker(rows, "REVIEW_READY", sha1), rows[0])

    def test_review_ready_must_match_current_validation_context(self):
        root = Path(__file__).resolve().parents[1]
        sha = "a" * 40
        pr = {"number": 123, "head": {"sha": sha}, "body": "WP: WP-H1-03\nClass: FOUNDATIONAL\n",
              "state": "open", "draft": False, "merged": False}
        context = module.validation_context_module(root).resolve_context(pr, sha)
        digest = context["context_digest"]
        row = {"state": "REVIEW_READY", "target sha": sha,
               "key": f"review-ready:123:{sha}:{digest}",
               "validation context digest": digest, "effective wp": context["wp"],
               "process only": context["process_only"],
               "non foundational": context["non_foundational"]}
        self.assertTrue(module.ready_context_matches(root, pr, row))
        changed = dict(pr, body="WP: WP-H1-03\nClass: NON-FOUNDATIONAL\n")
        self.assertFalse(module.ready_context_matches(root, changed, row))
        self.assertFalse(module.ready_context_matches(root, pr, dict(row, key="stale")))
        self.assertFalse(module.ready_context_matches(root, dict(pr, draft=True), row))
        self.assertFalse(module.ready_context_matches(root, dict(pr, state="closed"), row))

    def test_markdown_verdict_fields(self):
        body = "## **Reviewer verdict:** `FAIL`\n**Reviewed candidate SHA:** `" + "a" * 40 + "`"
        parsed = module.fields(body)
        self.assertEqual(parsed["reviewer verdict"], "FAIL")
        self.assertEqual(parsed["reviewed candidate sha"], "a" * 40)

    def test_dependency_precheck_is_bounded(self):
        self.assertEqual(module.dependency_wps("Depends on: `WP-H1-01` PASS, `WP-H1-02` PASS"),
                         ["H1-01", "H1-02"])
        with patch.object(module, "canonical_pr", side_effect=[{"merged_at": "date", "number": 1}, {"merged_at": None}]), \
             patch.object(module, "accepted_main_doc", return_value=True), \
             patch.object(module, "markers", return_value=[]), \
             self.assertRaisesRegex(module.StopFlow, "H1-02 is not merged"):
            module.assert_dependencies(Path("."), "Depends on: WP-H1-01, WP-H1-02")

    def test_recent_docsync_route_refuses_existing_owner(self):
        comment = {"body": "ARKUS_AUTOMATION_V2\nState: DOCSYNC_COMPLETE\nNext WP: H1-03",
                   "issue_url": "https://api.github.com/repos/Arkus0/Juego2/issues/123",
                   "user": {"login": "Arkus0"}}
        with patch.object(module, "gh_json", side_effect=[[comment], {"merged": True}]), \
             patch.object(module, "validate_docsync"), \
             patch.object(module, "canonical_pr", return_value={"number": 124}), \
             self.assertRaisesRegex(module.StopFlow, "already has a PR"):
            module.latest_next_wp(Path("."))

    def test_adopt_requires_exact_canonical_branch_and_sha(self):
        sha = "a" * 40
        pr = {"number": 123, "head": {"sha": sha, "ref": "codex/h1-03",
                                     "repo": {"full_name": module.REPO}}}
        with patch.object(module, "run", side_effect=[sha, "codex/h1-03"]):
            module.assert_pr_checkout(Path("."), pr)
        with patch.object(module, "run", side_effect=[sha, "main"]), \
             self.assertRaisesRegex(module.StopFlow, "exact head branch"):
            module.assert_pr_checkout(Path("."), pr)
        with patch.object(module, "run", side_effect=["b" * 40, "codex/h1-03"]), \
             self.assertRaisesRegex(module.StopFlow, "exact head branch"):
            module.assert_pr_checkout(Path("."), pr)

    def test_docsync_marker_requires_canonical_key_and_wp(self):
        pr = {"number": 123, "merged_at": "2026-01-01T00:00:00Z", "body": "WP: WP-H1-02"}
        row = {"key": "docsync-complete:123:" + "a" * 40,
               "wp": "WP-H1-03", "next wp": "WP-H1-04",
               "_created_at": "2026-01-02T00:00:00Z"}
        with self.assertRaisesRegex(module.StopFlow, "malformed"):
            module.validate_docsync(Path("."), pr, row)

    def test_docsync_rejects_older_valid_commit_from_another_pr(self):
        pr = {"number": 123, "merged_at": "2026-01-01T00:00:00Z", "body": "WP: WP-H1-02",
              "merge_commit_sha": "b" * 40}
        row = {"key": "docsync-complete:123:" + "a" * 40,
               "wp": "WP-H1-02", "next wp": "WP-H1-03",
               "_created_at": "2026-01-02T00:00:00Z"}
        with patch.object(module.subprocess, "run", return_value=type("Result", (), {"returncode": 1})()), \
             self.assertRaisesRegex(module.StopFlow, "does not descend from its merge"):
            module.validate_docsync(Path("."), pr, row)

    def test_zero_commit_docsync_accepts_exact_merge_key(self):
        sha = "a" * 40
        pr = {"number": 123, "merged_at": "2026-01-01T00:00:00Z", "body": "WP: WP-H1-02",
              "merge_commit_sha": sha}
        row = {"key": f"docsync-complete:123:{sha}", "wp": "WP-H1-02",
               "next wp": "WP-H1-03", "detail": "No authoritative document meaning changed.",
               "_created_at": "2026-01-02T00:00:00Z"}
        with patch.object(module.subprocess, "run", return_value=type("Result", (), {"returncode": 0})()):
            module.validate_docsync(Path("."), pr, row)

    def test_docsync_can_reconcile_an_engineering_authority(self):
        merge_sha, docs_sha = "a" * 40, "b" * 40
        pr = {"number": 123, "merged_at": "2026-01-01T00:00:00Z", "body": "WP: WP-H1-02",
              "merge_commit_sha": merge_sha}
        row = {"key": f"docsync-complete:123:{docs_sha}", "wp": "WP-H1-02",
               "next wp": "WP-H1-03", "_created_at": "2026-01-02T00:00:00Z"}
        def fake_git(*args, **_kwargs):
            if args[1] == "rev-list":
                return f"{docs_sha} {merge_sha}"
            if args[1] == "diff":
                return "Docs/engineering/WORKER_REVIEW_PROTOCOL.md"
            raise AssertionError(args)
        with patch.object(module.subprocess, "run", return_value=type("Result", (), {"returncode": 0})()), \
             patch.object(module, "run", side_effect=fake_git):
            module.validate_docsync(Path("."), pr, row)

    def test_next_wp_refreshes_main_after_merge(self):
        pr = {"number": 123, "merged_at": "2026-01-01T00:00:00Z"}
        row = {"state": "DOCSYNC_COMPLETE", "next wp": "H1-03"}
        with patch.object(module, "markers", return_value=[row]), \
             patch.object(module, "validate_docsync") as validate, \
             patch.object(module, "run", return_value="") as run:
            self.assertEqual(module.next_from_merged_pr(Path("."), pr), "H1-03")
            run.assert_called_once_with("git", "fetch", "origin", "main", cwd=Path("."))
            validate.assert_called_once_with(Path("."), pr, row)

    def test_public_comment_cannot_authorize_continuation(self):
        sha = "a" * 40
        body = f"ARKUS_LOCAL_AUTOPILOT\nState: OWNER_CONTINUE\nTarget SHA: {sha}\nFail count: 2\n"
        rows = [{"body": body, "user": {"login": "public-commenter"}},
                {"body": body, "user": {"login": "github-actions[bot]"}}]
        with patch.object(module, "gh_pages", return_value=rows):
            markers = module.local_markers(123)
        self.assertEqual(len(markers), 1)
        self.assertEqual(markers[0]["state"], "OWNER_CONTINUE")

    def test_owner_continuation_requires_matching_second_fail_offer(self):
        sha = "a" * 40
        rows = [{"state": "OWNER_CONTINUE", "target sha": sha, "fail count": "2"}]
        self.assertFalse(module.owner_authorized_continuation(rows, 2))
        rows.append({"state": "SECOND_FAIL_OFFERED", "target sha": "b" * 40, "fail count": "2"})
        self.assertFalse(module.owner_authorized_continuation(rows, 2))
        rows.append({"state": "SECOND_FAIL_OFFERED", "target sha": sha, "fail count": "2"})
        self.assertTrue(module.owner_authorized_continuation(rows, 2))
        self.assertTrue(module.owner_authorized_continuation(rows, 3))
        self.assertFalse(module.owner_authorized_continuation(rows, 4))
        count_three = [{"state": "SECOND_FAIL_OFFERED", "target sha": sha, "fail count": "3"},
                       {"state": "OWNER_CONTINUE", "target sha": sha, "fail count": "3"}]
        self.assertFalse(module.owner_authorized_continuation(count_three, 2))
        self.assertTrue(module.owner_authorized_continuation(count_three, 3))
        rows.append({"state": "CONTINUE_EXPIRED", "target sha": sha, "fail count": "2"})
        with self.assertRaisesRegex(module.StopFlow, "expired or became unavailable"):
            module.owner_authorized_continuation(rows, 3)

    def test_expired_offer_cannot_be_redispatched(self):
        sha = "a" * 40
        rows = [{"state": "SECOND_FAIL_OFFERED", "target sha": sha, "fail count": "2"},
                {"state": "CONTINUE_EXPIRED", "target sha": sha, "fail count": "2"}]
        with patch.object(module, "local_markers", return_value=rows), \
             self.assertRaisesRegex(module.StopFlow, "expired or became unavailable"):
            module.offer_continue(123, sha, 2, "H1-03", "Decision")
        stale = [{"state": "SECOND_FAIL_OFFERED", "target sha": sha, "fail count": "2",
                  "_created_at": "2020-01-01T00:00:00Z"}]
        with patch.object(module, "local_markers", return_value=stale), \
             self.assertRaisesRegex(module.StopFlow, "offer is stale"):
            module.offer_continue(123, sha, 2, "H1-03", "Decision")

    def test_marker_provenance_matches_state(self):
        sha = "a" * 40
        ready = f"ARKUS_AUTOMATION_V2\nState: REVIEW_READY\nTarget SHA: {sha}\n"
        docsync = "ARKUS_AUTOMATION_V2\nState: DOCSYNC_COMPLETE\nNext WP: NONE\n"
        rows = [{"body": ready, "user": {"login": "Arkus0"}},
                {"body": ready, "user": {"login": "github-actions[bot]"}},
                {"body": docsync, "user": {"login": "github-actions[bot]"}},
                {"body": docsync, "user": {"login": "Arkus0"}}]
        with patch.object(module, "gh_pages", return_value=rows):
            accepted = module.markers(123)
        self.assertEqual([r["state"] for r in accepted], ["REVIEW_READY", "DOCSYNC_COMPLETE"])

    def test_duplicate_review_and_comment_count_once(self):
        sha = "a" * 40
        body = f"Reviewer verdict: FAIL\nReviewed candidate SHA: {sha}\nAutopilot review ID: {'1' * 32}\n"
        owner = {"body": body, "user": {"login": "Arkus0"}, "created_at": "2026-01-01T00:00:00Z"}
        public = {"body": body, "user": {"login": "public-commenter"}, "created_at": "2026-01-01T00:01:00Z"}
        with patch.object(module, "gh_pages", side_effect=[[owner], [owner, public]]):
            self.assertEqual(len(module.reviewed_fails(123)), 1)

    def test_same_words_different_review_ids_count_separately(self):
        sha = "a" * 40
        rows = [{"body": f"Reviewer verdict: FAIL\nReviewed candidate SHA: {sha}\nAutopilot review ID: {i * 32}\n",
                 "user": {"login": "Arkus0"}, "created_at": f"2026-01-01T00:00:0{i}Z"} for i in ("1", "2")]
        with patch.object(module, "gh_pages", side_effect=[rows, []]):
            self.assertEqual(len(module.reviewed_fails(123)), 2)

    def test_protocol_fix_is_not_a_material_fail(self):
        sha = "a" * 40
        row = {"body": f"Reviewer verdict: PROTOCOL_FIX\nReviewed candidate SHA: {sha}\nAutopilot review ID: {'1' * 32}\n",
               "user": {"login": "Arkus0"}, "created_at": "2026-01-01T00:00:00Z"}
        with patch.object(module, "gh_pages", side_effect=[[row], []]):
            self.assertEqual(module.reviewed_verdicts(123)[0]["verdict"], "PROTOCOL_FIX")

    def test_manual_untagged_verdict_stops_adoption(self):
        sha = "a" * 40
        row = {"body": f"Reviewer verdict: FAIL\nReviewed candidate SHA: {sha}\n",
               "user": {"login": "Arkus0"}, "created_at": "2026-01-01T00:00:00Z"}
        with patch.object(module, "gh_pages", side_effect=[[row], []]), \
             self.assertRaisesRegex(module.StopFlow, "manual/untagged"):
            module.reviewed_verdicts(123)

    def test_material_fail_only_same_sha_appeal_can_supersede(self):
        sha = "a" * 40
        failed = {"id": "1" * 32, "sha": sha, "verdict": "FAIL", "at": "2026-01-01T00:00:00Z"}
        appeal = {"state": "OVERDEFENSE_APPEAL_STARTED", "target sha": sha,
                  "review id": "2" * 32, "_created_at": "2026-01-01T00:01:00Z"}
        passed = {"id": "2" * 32, "sha": sha, "verdict": "PASS", "at": "2026-01-01T00:02:00Z"}
        module.assert_verdict_sequence(123, sha, [failed, passed], [appeal])
        with self.assertRaisesRegex(module.StopFlow, "one authorized appeal"):
            module.assert_verdict_sequence(123, sha, [failed, passed], [])
        protocol = dict(passed, verdict="PROTOCOL_FIX")
        with self.assertRaisesRegex(module.StopFlow, "one authorized appeal"):
            module.assert_verdict_sequence(123, sha, [failed, protocol], [appeal])
        with self.assertRaisesRegex(module.StopFlow, "fixed same-SHA PASS"):
            module.assert_verdict_sequence(123, sha, [passed, failed], [appeal])

    def test_environment_excludes_model_api_keys(self):
        old = module.os.environ.get("OPENAI_API_KEY")
        module.os.environ["OPENAI_API_KEY"] = "test-only"
        try:
            self.assertNotIn("OPENAI_API_KEY", module.clean_env())
        finally:
            if old is None:
                del module.os.environ["OPENAI_API_KEY"]
            else:
                module.os.environ["OPENAI_API_KEY"] = old

    def test_worker_effort(self):
        self.assertEqual(module.effort_for_worker("Class: FOUNDATIONAL AUTHORITY"), "xhigh")
        self.assertEqual(module.effort_for_worker("Class: PROCESS_ONLY"), "high")
        self.assertEqual(module.effort_for_worker("Class: PROCESS_ONLY\nForbidden: architecture work"), "high")

    def test_circuit_breaker_is_checked_on_first_and_third_fail(self):
        audit = {"classification": "valid", "same_foundational_defect_class": False,
                 "self_shrinking_completeness": True,
                 "proof_machinery_expansion_without_progress": False,
                 "criterion": "Bounded completeness claim", "evidence": "Fixture drops discovered targets",
                 "minimal_next_action": "Stop and reopen proof boundary"}
        self.assertEqual(module.audit_policy(audit, 1), "circuit_breaker")
        self.assertEqual(module.audit_policy(audit, 3), "circuit_breaker")
        audit["self_shrinking_completeness"] = False
        self.assertEqual(module.audit_policy(audit, 3), "valid")
        audit["classification"] = "overdefense"
        self.assertEqual(module.audit_policy(audit, 2), "appeal")
        self.assertEqual(module.audit_policy(audit, 3), "needs_pc")
        with self.assertRaisesRegex(module.StopFlow, "invalid or missing"):
            module.audit_policy(dict(audit, evidence=""), 2)


if __name__ == "__main__":
    unittest.main()
