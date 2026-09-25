# WP-CTX-DW-H1-02 — proof matrix

PROOF_MATRIX_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0 within the declared selective-adoption/routing boundary
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Proof boundary

This WP validates **routing/adoption**, not H1 product semantics. Its independent inputs are the accepted H1-01 projection lifecycle and the accepted H1-05/H1-06/H1-07 claim anchors. The projection capability surface is emitted by rebuilding the real accepted `H1DesignWorldProvider`.

`ADOPTION_CASES.json` now contains identification metadata only: claim ID, consumer, accepted anchor and representative query where applicable. The semantic contract that binds each canonical claim ID to material capabilities, mandatory reads, public-isolation and the accepted current disposition is frozen separately in `scripts/ctx-dw-h1-02-proof.py::CLAIM_CONTRACTS`. Fixture attempts to supply semantic override fields are invalid.

The accepted projection capabilities are bounded to catalogue/source navigation: catalogue entries, source records/slices, `declared-in`, `adopted-from-source`, and component-schema catalogue entries. Effective scene state, publication, prefab relationship equality, effective component adapters and field round-trip are deliberately absent.

| Claim | Positive/effective evidence | Causal negative control | Result / residual |
|---|---|---|---|
| Real H1-05/H1-06/H1-07 claims, no shadow reruns | verifier-owned claim contracts bind exact accepted anchors and semantic requirements | fixture metadata mismatch, missing anchor or semantic override field makes proof RED | READY; accepted product evidence is consumed, not recreated |
| Claim semantics are independent of evaluated fixture | `CLAIM_CONTRACTS` owns required capabilities, mandatory reads, public isolation and exact expected disposition | explicit H1-07 inventory↔unsupported semantic swap preserves valid JSON, eight cases, anchors and `3/2/3`, but fixture semantic authority is rejected | READY; Review #5313616912 falsifier closed |
| Exact per-claim disposition is published | proof computes the result from verifier-owned requirements + real projection capabilities and parses the claim-ID table in `ADOPTION_DISPOSITION.md` | any claim-ID disposition mismatch fails even if global class counts remain `3/2/3` | READY |
| `expected_classification` cannot authorize a route | evaluated fixture contains no expected result field | injecting/changing only `expected_classification` is rejected before routing | READY |
| `USE` only for capabilities actually represented by current H1-01 projection | focused .NET test rebuilds the real projection, enumerates fact/relation capability surface and source-open representative facts | adding an undeclared effective-product capability to the observation fails accepted-shape validation | READY; projection expansion requires its own owner/review |
| `USE` gated by lifecycle currentness | accepted projection validates under `ctx-dw-h1-01-v1` | same projection checked under a different projection version emits `h1.lifecycle_projection_schema_stale`; a baseline USE route becomes OPTIONAL/source-first | READY |
| Projected facts remain source-open | representative Quaternius prefab and component-schema facts carry exact catalogue provenance digests/anchors | missing/wrong provenance path makes observation inadmissible | READY |
| CTX/product mandatory reads cannot be hidden | actual route execution opens the accepted claim anchor and, for `USE`, source-opens projected provenance; only afterward is the resulting opened set compared with verifier-owned mandatory obligations | malicious control removes one actually opened required path and the independent audit detects it | READY |
| Classification is structural, not label/text driven | route compares verifier-owned material requirements with projection capabilities | adding H1/domain labels and suggestive free text leaves result unchanged; a simulated broad `WP-H1-* => USE` policy disagrees on product-only claims | READY |
| Partial navigation cannot close product truth | H1-06 relationship equality and H1-07 round-trip retain product/effective requirements absent from DW | partial cases remain OPTIONAL and keep H1-06/H1-07 authority reads open | READY |
| Correct abstention exists | H1-05 scene/publication and H1-07 unsupported/effective behavior have zero represented deciding capability | broad adoption would disagree with the canonical contract | READY |
| H1-GATE public-client trial remains uncontaminated | accepted adoption plan explicitly forbids Juego2-private CTX/DW pre-seeding | verifier-owned public-isolation contract forces `NOT_MATERIAL` even though catalogue capability exists | READY |
| Navigation effect is bounded and subordinate | exact-SHA verifier records serialized source-open query bytes against full accepted catalogue authority bytes for admitted USE routes | admitted USE fails if its selective payload is not smaller than the authority document | READY; no model-quality/performance claim |
| H1 product chain remains independent | `WP-H1-07` still depends directly on `WP-H1-06` | verifier fails if H1-07 gains a `WP-CTX-DW-H1-02` dependency | READY |

## Adversarial controls required GREEN

The deterministic proof requires all of these controls:

1. `stale_lifecycle_rejects_use`;
2. `capability_inflation_rejected`;
3. `labels_and_free_text_cannot_change_route`;
4. `broad_h1_default_would_be_detected`;
5. `hidden_mandatory_read_detected`;
6. `partial_projection_cannot_close_product_oracle`;
7. `component_schema_does_not_imply_effective_adapter`;
8. `public_client_isolation_forces_abstention`;
9. `semantic_claim_swapping_rejected`;
10. `expected_classification_cannot_authorize_route`.

## Result shape

The exact canonical map is mandatory:

- `h105-catalogue-source-navigation` → `USE`
- `h105-scene-publication` → `NOT_MATERIAL`
- `h106-source-prefab-navigation` → `USE`
- `h106-relationship-equality` → `OPTIONAL`
- `h107-component-schema-inventory` → `USE`
- `h107-component-roundtrip` → `OPTIONAL`
- `h107-unsupported-field` → `NOT_MATERIAL`
- `h1gate-public-client-isolation` → `NOT_MATERIAL`

The aggregate must therefore be exactly `3 USE / 2 OPTIONAL / 3 NOT_MATERIAL`, but that aggregate alone is never sufficient.

The exact-SHA verifier writes only ephemeral observations under `artifacts/observed/`; durable authority remains the accepted predecessor documents plus this reviewed disposition. No generated observation becomes a new H1 product oracle.

## Residual-risk boundary

Future H1-08+ claims are not preclassified by name. They apply the same capability/currentness/mandatory-read rule to their actual claim. H2 public knowledge portability, projection-schema expansion, runtime behavior, art production and public-client packaging remain outside this WP.
