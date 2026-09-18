# WP-HK-00 — Foundational proof verdict

```text
FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
```

This is the **Worker** verdict. It is a claim that the candidate is ready to be
reviewed, not a claim that it has been reviewed. `WP-HK-01` may not begin until a
fresh independent Reviewer PASS names the exact candidate SHA, as
`Docs/workpacks/README.md` requires.

## What the counts mean here

**Unresolved proof obligations: 0.** Every acceptance criterion in the workpack
has a row in `PROOF_MATRIX.md` with a completeness argument, positive evidence
and a causal negative control that was demonstrated red and then green. No row is
UNKNOWN or PARTIAL.

**Known undetected defect classes: 0.** Every material class identified while
building this — including the two that evaluation-time checks are structurally
blind to — has a guard, and each guard has a recorded negative control. The
limitations in `RESIDUAL_RISK.md` are bounded scope (no `.resx` or source
generators exist; no reflection exists; Unity import is forbidden scope), not
classes that could leave this claim false while CI is green.

## Freeze state

| Item | State |
|---|---|
| Candidate SHA | recorded below once the freeze commit exists |
| Branch | `claude/wp-hk-00-worker-550se1` |
| Exact-SHA CI | recorded below |
| Proof pipeline locally | GREEN (`scripts/proof.sh`, SDK 8.0.131, configuration Release) |
| Self-attack suite locally | 19 of 19 passed, each red then green |
| Tests | 69 passed, 0 failed |
| Production sources | 22, unclassified 0 |
| Projects | 10 classified: 6 portable-kernel, 1 host, 2 tooling, 1 tests |
| Findings in the final run | 0 |

### Exact-SHA CI

<!-- Filled by the Worker after the freeze commit; a verdict without this is not frozen. -->

- Candidate SHA: _pending freeze commit_
- CI run: _pending_
- Result: _pending_

## What a Reviewer should attack first

Stated deliberately, because the standard asks the Reviewer to find an omission
class the Worker did not highlight. The weakest joints, in the Worker's own
judgement:

1. **Ownership is directory-shaped.** A project nested inside another project's
   directory would make "deepest directory wins" carry more weight than it
   should. No such layout exists today and duplicate compilation would trip A1,
   but the rule itself has not been attacked.
2. **`AllowExternalCompiledSources` is a per-class escape hatch.** It is `true`
   only for the test class, because `Microsoft.NET.Test.Sdk` injects an entry
   point. If a production class ever gained that flag, the strongest source
   oracle would go quiet for it; there is a manifest test forbidding exactly that,
   and it deserves a second look.
3. **The generated-source allow-list is three patterns.** It is checked, and
   attacked, but widening it is an easy way to re-open the hiding place that
   `hidden-generated-source` closes.
4. **The tests assert the module direction that the manifest declares.** Both
   would have to be edited together to move the boundary — that is the intent,
   but it means a single reviewer reading only the manifest diff would not see
   the whole change.
