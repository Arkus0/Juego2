# WP-H1-01 residual-risk boundary

RESIDUAL_RECONCILIATION: `COMPLETE`
UNCLASSIFIED_RESIDUALS: `0`
PREDECESSOR_REOPEN_TRIGGERED: `NO`

H1-01 proves only the portable/versioned Unity authoring binding producer, its mechanical dependency derivation and its admission into the accepted canonical composition/projection path. It deliberately does not convert later Editor/resource concerns into false green claims.

| Residual / limitation | Classification | Owner / reopen condition |
|---|---|---|
| Logical catalogue resource existence, exact Unity type compatibility and catalogue fingerprint acquisition | downstream | later Unity catalogue/materialization work; does not reopen H1-01 unless the portable typed reference vocabulary itself is insufficient |
| Logical catalogue ID ↔ Unity GUID/path/local-file-ID resolution | downstream/private Unity implementation | later Unity-facing provider/materializer; native locators remain non-authoritative |
| Effective scene/prefab/component serialization and object lifecycle in Unity Editor | downstream | H1-05 and parity/materialization owners |
| Public host-to-Editor process lifecycle, main-thread dispatch, cancellation/crash/restart behavior | downstream | H1-03A/process boundary under the accepted H1 architecture |
| Editor-side drift/reconciliation of generated resources | downstream | H1-05/H1-10 parity/rebuild owners |
| Generic external-resource references outside the H1-01 catalogue kinds | intentionally unmodeled | future provider/version if a concrete admitted use case requires them |
| Additional Unity component-document kinds beyond canonical-link/renderer/animator | versioned extension | future compatible/minor or breaking/major evolution under HK01 rules, as semantics require |
| Gameplay/runtime state, physics/navigation/AI/save behavior and CITY keeper realization | out of boundary | H2+/CITY owners |

## Predecessor reconciliation

`WP-H1-00` remains intact: H1-01 uses portable data only and does not add `UnityEngine`/`UnityEditor` or native asset authority to the neutral bridge contract. H1-00 would reopen only if the accepted neutral boundary could not represent the producer without violating its neutrality/versioning guarantee; no such evidence was found.

HK02A remains intact: the structured canonical-link in the Potes probe is expressible exactly as the accepted typed canonical dependency (`kind`, `targetId`), and the generated extension mutation uses that ordinary dependency metadata. The exact predecessor reopen condition is therefore not triggered. Catalogue references are deliberately outside the canonical world-object universe and cannot by themselves reopen HK02A.

HK04 remains canonical write authority: H1-01 produces a `put-extension` mutation fragment but never commits it directly. The focused integration test sends that fragment through accepted `authoring.change.plan`, `dry-run` and `apply`.

HK07 remains transport authority: JSONL/MCP adapters receive the same composed inventory. H1-01 changes shared production composition, not adapter-owned route registries; delta process-level conformance verifies the three scoped routes and representative compile result.

## Dependency / IP result

The portable producer introduces no Unity package dependency and no Unity runtime/editor assembly reference. It depends only on in-repository Arkus protocol/runtime surfaces required to create a reviewed scoped canonical contribution. No vendor package, asset or native Unity serialization format becomes semantic authority in this WP.