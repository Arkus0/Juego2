# WP-HK-09A — Foundational proof matrix

HK09A is foundational because it defines the host-capability containment boundary inherited by later local adapters. This matrix separates inherited canonical guarantees from new HK09A obligations and uses causal negative controls for the new boundary.

## Inherited contracts consumed, not redefined

- HK01/HK00A: one canonical capability/schema authority and truthful machine-readable metadata.
- HK04/HK06A–C: canonical mutation/rebase/replay atomicity, provenance, snapshot and replay semantics.
- HK07A/HK07B: neutral projection plus JSONL/MCP as projections of the composed canonical inventory, not independent registries.
- HK08A/HK08B: bounded ordinary authoring/recovery interactions and accepted structured stale recovery.

HK09A re-runs the full test suite to detect contradictions but does not reopen these accepted guarantees without concrete evidence.

## New HK09A obligations

| Contract obligation | Implementation boundary | Independent / executable proof | Result |
|---|---|---|---|
| No generic shell/process authority | `H0HostCapabilityPolicy` rejects external semantics; production source has no process-launch primitive | process-capability negative + effective-source inventory | PASS |
| No ambient/protocol-triggered network authority | No H0 network capability and no production network primitive | loopback-listener negative + effective-source inventory | PASS |
| Filesystem limited to reviewed authority | H0 production executable exposes no caller-selected file mode | traversal + real symlink negatives; pre-delegation `--file` rejection | PASS |
| Traversal/symlink fail before external effect | production CLI rejects the entire file-authority route | traversal/symlink executable negatives, stdout remains empty | PASS |
| Truthful canonical policy metadata | `H0HostCapabilityPolicy.Validate` checks admitted side-effect/transaction pairing and rejects elevated/external semantics | invalid-definition negative controls + production discovery inspection | PASS |
| No arbitrary runtime-type activation | protocol data remains schema data; no production dynamic activation primitive | `$type` negative + effective-source inventory | PASS |
| Policy below transports | `CanonicalWorldContract.Compose` pre-admits the production contract and `NeutralProjectionService` independently enforces H0 policy on every composed contract crossing into projection | causal direct `ContractComposer` → projection negative plus production JSONL/MCP regression | PASS |
| JSONL/MCP cannot mint authority | both projections consume the same policy-enforcing neutral projection and unknown invented powers fail canonically | adapter-only capability negative + source oracle | PASS |
| Accepted H0 workflows remain useful | policy admits read/canonical mutation/rebase/replay and no shell/network/filesystem power is required | representative Juego2 content-shape probe: inspect → author → snapshot/import → replay | PASS |

## Negative-conformance completeness

`NEGATIVE_CONFORMANCE_MATRIX.md` maps every required HK09A negative class to an executable causal oracle. The transport-boundary fixture specifically starts from a generic `ContractComposer` result so the test cannot become green merely because production composition already called the policy. The source-surface oracle is deliberately independent of the capability registry under test, preventing a hidden host path from becoming green merely because it was omitted from canonical metadata.

## Representative product proof

`CONTENT_SHAPE_PROBE.md` binds the public-semantic change to the approved `Docs/art/VISUAL_BIBLE.md` product source and exercises a Potes-market micro-slice through the policy-admitted canonical path. The probe checks representability, stable identity/granularity, containment and the inspect/author/snapshot/replay boundary.

## Exact-SHA observation and freeze gate

Canonical observation: `scripts/hk09a-observe-exact-sha.sh <candidate-sha>`.

Canonical freeze verification: `scripts/hk09a-verify-exact-sha.sh <candidate-sha>`, routed by `scripts/arkus-verify-exact-sha.sh` for `WP-HK-09A`. It requires locked restore, Release build, all HK09A tests, full regression, foundational markers, negative/content evidence and a CLEAN Worker pre-review on the same frozen candidate.

## Proof budget

The proof budget is bounded to the repository-local H0 host and seven required negative classes plus one representative content slice. It intentionally does not expand into OS sandboxing, cloud/auth, external penetration testing, H1 Unity/editor semantics, or HK09B resource/crash-consistency work.

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
