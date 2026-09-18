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
| Candidate SHA | `7aa70698bd48f9d2f8f42a1bfbee263b19cf1dab` |
| Branch | `claude/wp-hk-00-worker-550se1` |
| Exact-SHA CI | GREEN, both jobs (see below) |
| Proof pipeline locally | GREEN (`scripts/proof.sh`, SDK 8.0.131, configuration Release) |
| Self-attack suite locally | 19 of 19 passed, each red then green |
| Tests | 69 passed, 0 failed |
| Production sources | 22, unclassified 0 |
| Projects | 10 classified: 6 portable-kernel, 1 host, 2 tooling, 1 tests |
| Findings in the final run | 0 |

### Exact-SHA CI

- Candidate SHA: `7aa70698bd48f9d2f8f42a1bfbee263b19cf1dab`
- Run: <https://github.com/Arkus0/Juego2/actions/runs/35364202327>
- `Kernel boundary proof`: **success** — preflight green, proof green across
  `preflight+static+effective+compiler` with 0 findings, headless host smoke test
  green, 69 tests passed, generated inventories identical to the committed ones.
- `Causal self-attacks`: **success** — all 19 attacks injected, detected by their
  named check, reverted byte-exactly and re-proved green, on a clean checkout.

The run also produced an unplanned piece of positive evidence for the toolchain
pin. The GitHub runner had twelve SDKs available —

```text
8.0.100  8.0.130  8.0.206  8.0.319  8.0.424
9.0.120  9.0.205  9.0.317
10.0.111 10.0.204 10.0.303 10.0.400
```

— and `dotnet --version` under this repository resolved to **8.0.130**: inside
the pinned `8.0.1xx` feature band, and not any of the higher bands or major
versions sitting next to it. The inventories that SDK generated were
byte-identical to the ones generated locally on 8.0.131, which is what the
`latestPatch` pin is supposed to guarantee.

This evidence commit adds only this record; it changes no code, no manifest and
no inventory.

### Freeze against current main

The candidate was afterwards merged with the then-current `main`
(`89104b0`), which had added the Worker/Reviewer protocol, the PR template and
the automation workflows while this workpack was being implemented. The merge
touched no file this workpack owns, and the merged tree was re-proved from a
clean `artifacts/`: proof green across all four phases with 0 findings, 69 tests
passed, inventories byte-identical to the committed ones.

Per `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`, GitHub is the handoff surface:
the **Frozen candidate SHA**, its CI run and the Worker state live in the pull
request handoff block. The implementation itself has not changed since
`7aa70698bd48f9d2f8f42a1bfbee263b19cf1dab`, whose exact-SHA CI evidence is
recorded above; everything after it is the merge plus this record.

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
