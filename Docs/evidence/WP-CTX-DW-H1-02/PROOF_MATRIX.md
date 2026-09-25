# WP-CTX-DW-H1-02 — proof matrix

PROOF_MATRIX_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0 within the declared selective-adoption/routing boundary
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Proof boundary

This WP validates **routing/adoption**, not H1 product semantics. Its independent inputs are the accepted H1-01 projection lifecycle and the accepted H1-05/H1-06 evidence anchors. The projection capability surface is emitted by rebuilding the real accepted `H1DesignWorldProvider`; the adoption case fixtures declare only material capability requirements, mandatory reads and accepted source anchors. A case's expected classification is checked after the route is computed and cannot define projection capabilities or lifecycle truth.

The accepted projection capabilities are bounded to catalogue/source navigation: catalogue entries, source records/slices, `declared-in`, `adopted-from-source`, and component-schema catalogue entries. Effective scene state, publication, prefab relationship equality, effective component adapters and field round-trip are deliberately absent.

| Claim | Positive/effective evidence | Causal negative control | Result / residual |
|---|---|---|---|
| Real H1-05 and H1-06 observations, no shadow reruns | case anchors source exact accepted `PROOF_MATRIX.md` evidence from both WPs; no Unity invocation exists in this WP | missing/changed anchor text or predecessor acceptance marker makes proof RED | READY; accepted product evidence is consumed, not recreated |
| `USE` only for capabilities actually represented by current H1-01 projection | focused .NET test rebuilds the real projection, enumerates fact/relation capability surface and source-open representative facts | adding an undeclared effective-product capability to the observation fails accepted-shape validation | READY; projection expansion requires its own owner/review |
| `USE` gated by lifecycle currentness | accepted projection validates under `ctx-dw-h1-01-v1` | same projection checked under a different projection version emits `h1.lifecycle_projection_schema_stale`; a baseline USE route becomes OPTIONAL/source-first | READY |
| Projected facts remain source-open | representative Quaternius prefab and component-schema facts carry exact catalogue provenance digests/anchors | missing/wrong provenance path makes observation inadmissible | READY |
| CTX/product mandatory reads cannot be hidden | every case independently declares accepted evidence/source reads and route audit requires the complete set after DW advice | malicious control drops a mandatory read and audit must detect it | READY |
| Classification is structural, not label/text driven | route compares material requirements with projection capabilities | adding H1/domain labels and suggestive free text leaves result unchanged; a simulated broad `WP-H1-* => USE` policy disagrees on product-only claims | READY |
| Partial navigation cannot close product truth | H1-06 relationship equality and H1-07 round-trip retain product/effective requirements absent from DW | partial cases must remain OPTIONAL and keep H1-06/H1-07 authority reads open | READY |
| Correct abstention exists | H1-05 scene/publication and H1-07 unsupported/effective behavior have zero represented deciding capability | if broad adoption classifies them USE, falsifier turns RED | READY |
| H1-07 guidance is selective | three real H1-07 claim shapes produce one `USE`, one `OPTIONAL`, one `NOT_MATERIAL` | proof fails if H1-07 collapses to a single blanket class | READY |
| H1-GATE public-client trial remains uncontaminated | accepted adoption plan explicitly forbids Juego2-private CTX/DW pre-seeding | public-isolation case forces `NOT_MATERIAL` even though catalogue capability exists | READY |
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
8. `public_client_isolation_forces_abstention`.

## Result shape

The real case universe must contain all three routing classes. H1-05 and H1-06 must each contain at least one source-open admitted `USE` observation. H1-07 must contain exactly the selective class set `{USE, OPTIONAL, NOT_MATERIAL}` across its three declared claim shapes. H1-GATE must remain `NOT_MATERIAL` for private CTX/DW context.

The exact-SHA verifier writes only ephemeral observations under `artifacts/observed/`; durable authority remains the accepted predecessor documents plus this reviewed disposition. No generated observation becomes a new H1 product oracle.

## Residual-risk boundary

Future H1-08+ claims are not preclassified by name. They apply the same capability/currentness/mandatory-read rule to their actual claim. H2 public knowledge portability, projection-schema expansion, runtime behavior, art production and public-client packaging remain outside this WP.
