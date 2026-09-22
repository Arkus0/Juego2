# WP-DW-00 — residual-risk audit

## In-claim status

No known material residual currently remains that can falsify the DW-00 central claim while all owned guards are green. Final readiness still depends on exact-candidate canonical observation and strict Worker pre-review.

## Bounded residuals / non-claims

| Residual | Why non-blocking for DW-00 | Owner / next boundary |
|---|---|---|
| `AnchoredTextAuthorityReader` receives explicit projection definitions rather than discovering the complete semantic universe of a document. | DW-00 completeness is proven against a separately supplied `IDesignAuthorityUniverse`; it does not claim a universal Markdown/domain extractor. Missing a required universe member remains causal RED. | Domain adapters / DW-01+ as their complete universes are defined. |
| Source provenance is conservatively document-digest-bound: an unrelated byte edit in the same authority file stales existing anchors until rebuild. | This can cause a safe rebuild requirement/false-positive stale condition, not a false valid fact. DW-00 prioritizes authority preservation over minimal invalidation. | Later performance/granularity refinement if measured useful. |
| Typed fact payload is normalized inside the DW-owned extension envelope rather than becoming a new H0 canonical type. | Intentional domain isolation. H0 remains generic and accepted; DW data is derived/disposable. | No action unless a later consumer supplies causal evidence that the public seam is insufficient. |
| Full CITY semantic correctness, access-role invariants and complete programme coverage are not proven. | Explicitly forbidden from DW-00; the representative CITY row is a shape probe only. | DW-01 / DW-02. |
| PA composition, negative findings/dispositions and PA completeness are not proven. | Outside DW-00. | DW-03. |
| Structured-context savings and paired agent-quality preservation are not measured. | Outside DW-00 and depends on accepted CTX-03 baseline. | DW-04. |
| Unity/design drift and engine-backed behavior are not proven. | DW-00 is `REMOTE_OK` and deliberately consumes H0 without Unity. | H1 / downstream post-DW work if authorized. |
| External-repository/public-package distribution is not proven. | Explicit track non-claim; DW is second-consumer evidence before H2 boundary acceptance. | H2 planning/gate. |

## Dependency / IP audit

DW-00 adds **no external runtime/library dependency**. `Arkus.DesignWorld` references only the repository-owned `Arkus.Game.World` project. Existing xUnit/Test SDK infrastructure is reused for proof.

## Predecessor reopen audit

No evidence currently shows an accepted H0 guarantee is false or inapplicable. The generic `WorldObject` + `WorldReference` + `WorldExtensionData` surface carries the bounded real Juego2 shape without changing H0 semantics. Therefore the exact `WP-HK-GATE` reopen condition is not triggered.

## Proof-budget audit

Product surface is intentionally small: one consumer assembly, one reference adapter, deterministic projector/validator/diff and focused tests. Proof machinery maps directly to the WP's named false-green classes (self-shrinking completeness, stale/missing/ambiguous provenance, hidden derived truth, explicit version staleness, forgotten-version effective-rule drift and domain leakage). The validator now rebuilds from the current reader/rules when version and provenance still appear current, so stale cached state cannot self-confirm merely because a manual rule-version string was left unchanged. No guard is added solely for hypothetical trusted-infrastructure pathology.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`
