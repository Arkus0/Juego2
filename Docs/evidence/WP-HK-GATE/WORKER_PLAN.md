# WP-HK-GATE Worker plan

WP: `WP-HK-GATE — AI authoring readiness gate`  
Baseline SHA: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`  
Active Worker: ChatGPT GPT-5.6 Sol  
Worker state: ACTIVE  
Branch: `wp/hk-gate-ai-authoring-readiness`

## PREDECESSOR_CONTRACT_CHECK

Direct accepted predecessor: `WP-HK-10 — Strict quality closure`.

- Reviewed candidate SHA: `813ccf08e33fdd77a34c59da5ed766882840cbee`.
- Independent PASS: PR #60 review `#5261301248`.
- Exact-SHA validation: Actions run `35527218067` GREEN.
- Implementation merge SHA: `f893ad51d756090eeecac41b8fc7cb14f8bd359a`.
- Post-acceptance DocSync/current main baseline: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`.
- HK10 accepted closure records `UNRESOLVED_PROOF_OBLIGATIONS: 0`, `KNOWN_UNDETECTED_DEFECT_CLASSES: 0`, `FOUNDATIONAL_PROOF_VERDICT: READY` and `PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

### Inherited guarantees consumed by GATE

GATE consumes rather than redesigns or defensively re-proves accepted causal guarantees from HK01→HK10, including:

1. canonical discovery/schema and structured dispatch/error semantics, including the accepted HK01 thrown-handler amendment;
2. deterministic canonical world identity/hash and inspection/query semantics;
3. atomic whole-world-CAS mutation, validation, idempotency and provenance/journal/replay guarantees;
4. snapshot import/export and semantic-diff contracts;
5. engine-neutral composition, reference JSONL transport and mechanically projected MCP surface;
6. HK08A coherent batched authoring/pagination primitives and the accepted 96-operation atomic transaction shape;
7. HK08B bounded machine-readable same-lineage stale-plan recovery/repair and interaction budget, without automatic-merge claims;
8. HK09A below-transport host-capability containment and HK09B transport-neutral resource/persistence envelope;
9. HK10 deterministic quality closure, twelve causal negative-conformance controls, Protocol v1 compatibility and bounded 512-transaction endurance/restart evidence.

These guarantees remain owned by their accepted predecessor workpacks. GATE may cite and exercise them in composition, but does not create replacement semantics merely to make the gate green.

### Guarantees newly owned by GATE

GATE owns the integration/readiness claim that the accepted H0 guarantees compose through public client-facing paths into a usable AI-authoring foundation:

- one deterministic clean-checkout reference-client scenario covering the complete 14-step gate flow;
- an independently defined canonical capability universe and exact reference↔MCP semantic-equivalence evidence for the relevant gate surface, including HK08A/HK08B interaction semantics;
- scenario evidence for representative multi-resource authoring, invalid-request repair, stale recovery, batching, export/restart/replay, semantic diff and provenance;
- public-path evidence that HK09A authority containment and HK09B limits/persistence remain transport-neutral;
- reuse/execution of the accepted HK10 bounded endurance and full headless validation surfaces on the exact candidate;
- exhaustive consumption of the accepted HK10 residual ledger without shrinking, reclassifying or hiding OUT-BOUNDARY classes;
- causal proof that a mandatory GATE execution stage cannot disappear while its declarative stage label remains present;
- material H0 dependency/IP inventory and engine-bridge-boundary audit;
- a fresh independent AI-agent trial using the public discovered contract and an accepted client-facing adapter path for at least the representative authoring flow.

The AI-agent trial is independent review evidence and cannot be fabricated or replaced by this Worker context. This Worker may prepare deterministic inputs/instructions and validate the public path, but the final evidence must come from a fresh independent agent that did not rely on implementation source.

### Concrete predecessor reopen condition

An inherited causal boundary is reopened only on concrete gate evidence that its accepted guarantee is false or inapplicable on the effective public path: for example a discoverable capability missing from public discovery, reference/MCP semantic disagreement, invalid or partial commit, silent stale overwrite, unusable/misanchored HK08B recovery context, accepted coherent intent exceeding the inherited HK08A/HK09B shape, hidden privileged authority, replay/hash divergence, or another hard blocker explicitly named by `WP-HK-GATE`.

A theoretical desire for extra hardening or duplicate proof is not a reopen condition. If concrete product-semantic evidence appears, repair belongs at the causal owner rather than being silently invented by GATE.

## Claim and trust boundary

Claim: from an exact clean checkout, a client restricted to the documented launch/bootstrap/public contract can discover and execute the representative H0 authoring workflow through reference and MCP projections with equivalent canonical semantics, bounded recovery, deterministic replay/provenance and accepted authority/resource limits, without implementation-source knowledge or engine/vendor dependencies.

Trusted base: exact Git checkout/object semantics; pinned .NET/MSBuild/NuGet documented behavior; normal documented GitHub-hosted runner process/filesystem/OS behavior; BCL/JSON/SHA-256 primitives; accepted predecessor guarantees unless contradicted by concrete effective gate evidence.

Outside the claim: Unity/gameplay behavior, automatic merge/per-resource locking, multi-process or multi-agent throughput, cloud/auth/tenancy, power-loss WAL/fsync durability, model-vendor quality benchmarking and arbitrary out-of-contract toolchain/OS behavior.

## Planned implementation/evidence

1. Add a deterministic gate reference client/scenario using only public bootstrap/discovery and accepted client-facing transport semantics; emit machine-readable transcript/evidence rather than hand-authored success claims.
2. Add exact gate observation/verification entrypoints that run the scenario, transport conformance, HK10 endurance/closure regression and full headless validation on the exact SHA.
3. Build the gate capability universe from canonical/effective composition independently of either adapter inventory, then compare both reference and MCP projections against it.
4. Persist gate evidence for capability inventory, scenario anchors/hashes/diffs/provenance, stale recovery, batching/interaction metrics, HK09A/HK09B checks, dependency/IP boundary and negative-conformance coverage.
5. Prepare a concise public-only AI trial brief and evidence template. Do not mark the trial complete in Worker evidence unless a genuinely fresh independent agent supplies the transcript.
6. Perform strict Worker pre-review over the complete baseline→candidate diff, hard blockers, proof universe and evidence before freeze.
7. After deterministic evidence and Worker pre-review are clean, freeze the exact candidate with the single explicitly external AI-agent trial still recorded as `NOT_READY/1`. The external trial must then bind to that frozen SHA through PR evidence without changing Git history; final exact-SHA verification is what closes the external obligation to effective `READY/0`. Any additional tracked-file change after freeze invalidates the trial binding and requires a new freeze/trial cycle.

## Reviewer repair cycle 1

Independent review of frozen candidate `2c0df70c1ec8245999e4d144e710816d2f7eb335` returned FAIL with two GATE-owned proof blockers and explicitly accepted the product/public-path semantics and prior AI trial itself.

1. **Residual reconciliation:** replace the seven-row summary with explicit consumption of all 53 IDs from accepted `WP-HK-10/RESIDUAL_RISK.md`, preserving every trusted-base, `CLOSED-BY`, `HK10-COVERED`, `DEFERRED` and `OUT-BOUNDARY` classification. Strengthen exact-SHA verification so `COMPLETE / 0` requires the fixed 53-ID universe rather than only two headers.
2. **G1 causality:** seed omission of the real unfiltered stage-14 `dotnet test` command in `hkgate-observe-exact-sha.sh` while leaving `GateStepUniverse` untouched. The normal-path `hkgate-proof-infrastructure-check.sh` oracle must reject the mutant specifically because executable stage 14 disappeared.

No runtime/product semantic file is repaired in this cycle. The previous independent AI-agent PASS remains evidence for its original SHA, but the tracked proof/evidence repair necessarily creates a new candidate SHA. Under the existing exact-SHA rule, freeze occurs only after deterministic observation is green, and one fresh AI trial is then required against that final SHA.

## Proof-budget guard

GATE is an integration/readiness proof, not HK11. New runtime/product semantics, public capability families, concurrency models, persistence subsystems or engine abstractions are forbidden unless concrete gate evidence reopens the causal owning workpack. Prefer one deterministic scenario runner plus existing accepted proof surfaces over duplicating predecessor test machinery.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
