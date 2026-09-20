# H1 architecture pre-review — PROCESS_ONLY

Status: COMPLETE for the planning candidate; not an implementation PASS
Initial reconstruction baseline: `main` at `c87c4c195d63cc9255745014f2b9757d9cd34050`
Reconciled integration base: `main` at `7fe44840076eba05f1b67a7633cd33fc67b9023d` after CITY programme v2 PASS, merge and DocSync
Date: 2026-09-20

## Scope

This is the architect's adversarial review of the complete H1 plan before any workpack is activated. It validates causal ownership, sequencing and proof boundaries. It does not independently review future implementation and cannot grant PASS to any `WP-H1-*`.

## Per-workpack split review

| WP | Dual-claim challenge | Predecessor / inherited-proof challenge | Universe + causal-control challenge | Immediate-next / temporary-contract challenge | Unity leakage + size challenge | Result |
|---|---|---|---|---|---|---|
| H1-00 | contract plus reference materializer are inseparable neutrality claim; Unity excluded | consumes H0; changes no H0 meaning | normalized effective resources independently compared; interruption changes active generation only if defect exists | H1-01 can use final versioned artifacts | no Unity type; larger split would make contract speculative | KEEP |
| H1-01 | public binding + derivation are one producer-truth claim | HK02A reopens only if canonical reference cannot be expressed | structured tree is independently walked; omitted reference control must RED | vocabulary covers all already-planned H1 units without claiming assets exist | Unity-named portable schema, no Unity CLR model | KEEP |
| H1-02 | editor, packages and project layering form one reproducibility claim | does not re-prove H0 builds | effective resolved packages/assemblies checked, not manifest alone | H1-03 consumes exact project/profile with no placeholder | Unity-specific by purpose but isolated below canonical graph | KEEP |
| H1-03 | authority admission is distinct from features | HK09A unchanged; new legitimate effects get a new policy | valid generic composed route attempts the actual bypass | all later editor effects cross this final seam | no feature behavior; sufficient independent value | KEEP |
| H1-04 | inventory and native/logical mapping share one catalogue truth | no canonical identity rewrite | AssetDatabase/type universe versus mapping; removed manifest row cannot shrink proof | H1-05 receives stable final logical IDs/fingerprint | catalogue remains project state; all asset classes share owner | KEEP |
| H1-05 | scene graph plus staging/publication are one truthful materialization claim | H0 canonical transaction remains separate | effective managed markers/scene observation; forced pre-publication failure | prefab/component semantics deliberately remain later and require no scene-contract break | only derived GameObjects; correctly sized first write boundary | KEEP |
| H1-06 | asset and prefab links share Unity asset authority | consumes catalogue/generation without replaying them | effective prefab/source/nested links and protected source hashes | H1-07 can add fields without changing prefab authority | does not promote prefab contents into canonical state | KEEP |
| H1-07 | finite component set is one adapter/schema completeness claim | producer reopens only on real encoding impossibility | effective adapter implementations versus composed schemas; dual enumeration prevents shrink | validation consumes final adapter inventory | avoids generic Unity reflection; per-component WPs would be micro-WPs | KEEP |
| H1-08 | invariant aggregation is one diagnostic-authority claim | HK05 unchanged; Unity IDs are namespaced | independent invariant IDs plus effective invalid fixtures; skipped route must RED | H1-09 can rely on detection; CITY-04 may consume it without becoming H1 evidence | Unity checks stay bridge-owned; one WP avoids validator fragments | KEEP |
| H1-09 | observation, drift classification and proposal compile are one synchronization-authority claim | H0 alone commits; HK08B used only on actual stale proposal | injected effective drift compared with accepted managed unit universe | H1-10 consumes stable sync semantics; no planned auto-pull later | no Unity object becomes canonical truth | KEEP |
| H1-10 | checkpoint plus clean rebuild are one continuity claim | imports/replay keep accepted lineage meaning | generated outputs deleted; hidden dependency and partial-publication controls are causal | H1-11 can substitute real inputs without new persistence contract | checkpoint references Unity metadata but canonical artifacts remain recovery truth | KEEP |
| H1-11 | exact content adoption plus conformance is one real-shape claim | generic owners reopen only on concrete counter-evidence | exact import/adoption manifest compared with effective selected assets | Gate consumes this non-keeper slice; CITY-03/07 retain seed/keeper ownership | assets are proof inputs, not schemas/gameplay/CITY; per-asset WPs would be absurd | KEEP |
| H1-GATE | closure/composed public readiness only | consumes every predecessor; cannot repair one | real executable stages + complete residual/dependency universe; omission controls attack execution | no downstream H2 starts before PASS/merge/DocSync | no new Unity/canonical semantics; gate size is one end-to-end claim | KEEP |

## Cross-cutting pre-mortem questions

1. **Two causal claims in one WP?** No unresolved case. H1-00 deliberately couples the neutral contract to its only non-speculative reference implementation; H1-11 couples adoption to the sole value of the selected content: conformance. All other independently rejectable authorities are split.
2. **Silent H0 redefinition?** No. H0 remains canonical for identity, mutation, validation, journal, snapshot/replay, transport and containment. Any contradiction routes through each WP's exact reopen condition.
3. **Inherited guarantees re-proved?** No full H0 suite is demanded per WP. Each contract lists excluded re-proof and requires only delta evidence at its changed seam.
4. **Self-shrinking proof universe?** Catalogue, component, validator, managed resource, gate-stage and residual universes all require an independent/effective enumeration. Declared registries do not prove themselves.
5. **Non-causal negative controls?** Controls change the effective thing claimed: reference omission, policy bypass, resolved-package drift, active-generation publication, source overwrite, adapter execution, drift field, hidden rebuild dependency or executed gate stage.
6. **Predictable immediate reopening?** The authoring vocabulary, identity layers, generation receipt, catalogue fingerprint, adapter contract, diagnostics, proposal and checkpoint are versioned before consumers. No known temporary contract is scheduled for replacement two WPs later.
7. **Product semantics hidden in closure/gate?** No. Gate-owned work is orchestration, reconciliation and fresh-client readiness. Any missing semantic returns to H1-00..11.
8. **AI trial where deterministic proof suffices?** Only H1-GATE uses one fresh trial because fresh public Unity-client operability is new there. Deterministic engine evidence stays authoritative.
9. **Unity structure promoted to canonical semantics?** No. GameObjects/components/native IDs are derived projection/locators. Only portable Unity binding intent is canonical opaque extension data, and it remains provider-owned.
10. **Workpack size wrong?** No function/test/package/asset gets its own WP. Conversely, catalogue versus write, scene versus prefab, prefab versus components, validation versus reconciliation and continuity versus real content remain split because each can pass while its neighbor fails.
11. **CITY v2 ownership displaced?** No. The plan was rebased after CITY v2 acceptance. CITY-00 geography, CITY-03 seed, CITY-04 greybox, CITY-07 keeper realization and CITY-08 reuse-cost proof remain their owners. H1-08 and H1-GATE provide explicit prerequisites only.

## Reviewer-failure forecast

| Risk | WP(s) | Why review is most likely to find a real defect |
|---|---|---|
| native/logical/canonical identity aliasing or incomplete inventory | H1-04 | Unity lifecycle and sub-asset identity make false completeness easy |
| partial publication or receipt ahead of effective state | H1-05 | Unity file/scene effects cannot share H0's in-memory atomic transaction |
| adapter/schema universe self-shrink or volatile fields in parity | H1-07 | reflection and registry convenience can create hidden coverage gaps |
| sync direction or stale anchor error | H1-09 | reverse compilation can accidentally become a second writer authority |
| hidden generated-state dependency during restart | H1-10 | same-session success can mask false reconstruction |
| real package import/licensing/rig assumptions | H1-11 | external content exercises shapes synthetic fixtures omit |
| stage/residual omission or fresh-client documentation gap | H1-GATE | closure can be green while a real execution or input universe is absent |

The plan therefore expects Reviewer FAIL to be informative at these owners rather than catastrophic to an omnibus H1 implementation.

## Freeze decision

The planning candidate is internally coherent and may be submitted for human review. No implementation workpack is active. If the planning PR merges unchanged, `WP-H1-00` is the first default Worker; `WP-H1-02` is the only independent parallel branch and still requires explicit authorization.

## Mechanical planning validation

- all 13 H1 contract files contain the required boundary, inheritance, proof, negative-conformance, residual, reopen and execution sections;
- statuses are uniformly `PLANNED / NOT_STARTED`; no H1 Worker is active/frozen;
- execution classification is 2 `REMOTE_OK`, 7 `LOCAL_UNITY_REQUIRED` and 4 `HYBRID`;
- referenced local planning documents resolve, the staged change is documentation/process-only and `git diff --check` is clean;
- the legacy `scripts/hk00a-architecture-check.sh` reports the same pre-existing failure on baseline and candidate: its frozen text oracle still requires a phrase in superseded `WP-HK-07.md` that is absent on accepted `main`. This plan does not rewrite historical H0 evidence to make that stale checker green;
- the planning environment has no `dotnet` executable. No source/project/runtime byte is changed, so product test execution is not presented as planning evidence.
