# H1 risk, residual and H0S relationship

Status: ACCEPTED H1 planning contract / WP-H1-00_AND_WP-H1-01 COMPLETE / WP-H1-02 NEXT
Date: 2026-09-21
Status reconciled: 2026-09-22
Planning acceptance: PR `#71`; reviewed candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`; PASS review `#5263596722`; merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`

## H0 residuals H1 deliberately owns

| H0 residual | H1 owner | Planned closure boundary |
|---|---|---|
| `R-00A-05` concrete engine abstractions/capabilities absent | `WP-H1-00` + `WP-H1-01`, final confirmation at GATE | executable engine-neutral projection contract plus composed Unity capability definitions |
| `R-01-09` Unity scoped-provider instantiation absent | `WP-H1-01` | composed Unity producer, schemas and bindings with no parallel registry |
| `R-05-05` Unity/editor validation absent | `WP-H1-08` | stable preflight/post-materialization Unity diagnostics |

H1 narrows but does not generically close:

- `R-02-05`: H1 supports provider-owned logical Unity catalogue references, not a universal cross-world/external-resource model.
- `R-02A-01`: the schema-aware Unity producer derives known references; arbitrary raw opaque payloads can still hide undeclared meaning.
- `R-04-01`: H1 provides project-checkpoint/rebuild for its representative scope, not WAL/fsync/power-loss durability for all canonical state.
- `R-06B-04`: H1 promotes authored Unity binding intent only; live transforms, clocks, physics, animation state, AI and simulation remain outside canonical authored replay.

Accepted H0 residual classifications are not changed by this planning PR. DocSync updates the ledger only after each owner receives independent PASS.

## H0S relationship

H0S remains parallel and non-blocking by default. H1 records, but does not pre-solve:

- canonical object count and snapshot size of the representative bridge slice;
- materialization plan size and duration;
- catalogue size and scan duration;
- drift/observation cost;
- canonical commit latency/stale rate when H1 authoring is exercised.

Only measured evidence that the H1 representative scenario cannot operate inside accepted H0 limits may promote an H0S item into the H1 critical path. The fix remains transport-neutral and owner-reviewed; H1 may not introduce per-resource CAS, Merkle identity, automatic merge or scheduling merely because Unity has granular objects.

## Primary H1 risks

| Risk | Earliest owner | Required disposition |
|---|---|---|
| Unity IDs become canonical IDs | H1-00/H1-04 | logical/native identity split; causal relocation/recreation tests |
| catalogue or component registry proves its own completeness | H1-04/H1-07 | independent AssetDatabase/effective-adapter universe |
| bridge creates a second mutation authority | H1-01/H1-09 | producer emits mutation proposals; only H0 commits |
| H0 host policy is skipped or weakened for editor effects | H1-03 | separate Unity policy at the below-transport seam |
| public H0/MCP host cannot truthfully reach Unity or grows a private/parallel route | H1-03A | fixed external-host → batch-worker topology; same composed handler for reference/MCP; structured lifecycle proof |
| disposable substitute art hides real source/import/catalogue behavior | H1-04 | adopt exact Quaternius Source at first game-art use; positive game-shaped catalogue proof uses accepted source content |
| materialization partially replaces the active projection | H1-05 | generation staging, publication manifest and interruption controls |
| purchased source prefab/asset is overwritten | H1-06 | adopted upstream source is read-only to bridge materialization; only bridge-managed projection derivatives writable |
| generic reflection silently broadens component mutation | H1-07 | explicit schema adapters and effective allowlist coverage |
| engine validation suppresses unrelated diagnostics | H1-08 | dependency-local ambiguity, preserving HK05 lesson |
| manual Unity edits silently become authored truth | H1-09 | drift plus explicit import proposal; never auto-pull |
| checkpoint/restoration redefines snapshot/replay lineage or silently substitutes missing source art | H1-10 | consume HK06B/HK06C; Unity rebuild remains downstream and binds accepted source fingerprints |
| bounded early source slice hides broader real hierarchy/material/rig/animation failures | H1-11/GATE | broaden the already-adopted Quaternius Source baseline into an exact representative selected-item conformance slice |
| H1 absorbs finished-art/Cantabrian adaptation ownership and blocks the first playable | H1-04..GATE | source-first bridge proof only; later H2/ART/CITY creates Juego2-derived content demand-first |
| H1 conformance fixture silently takes CITY geography/seed ownership | H1-11/GATE | consume CITY-00 shape only; exact seed and keeper realization stay CITY-03/07-owned |
| gate invents semantics to close an omission | H1-GATE | closure-only rule and predecessor reopen routing |

## Residuals expected after H1

Unless separately promoted by evidence, H1 does not claim:

- arbitrary Unity asset/component support;
- byte-identical Unity serialization across versions/platforms;
- background live synchronization or multi-user editor coordination;
- low-latency/parallel Editor dispatch, a long-lived Editor daemon or a startup-time SLO beyond the accepted batch-per-operation H1 profile;
- merge of independent canonical/Unity histories;
- power-loss durability for editor publication;
- shipping-scale catalogue/world throughput or SLOs;
- addressables/asset-bundle delivery, remote build farm or cloud tenancy;
- runtime/save-state parity, navigation, physics, lighting quality or gameplay correctness;
- finished Cantabrian art conversion, complete clothing/wardrobe, all missing props/meshes/variants or all gameplay/demo animations;
- portability of Unity-specific binding intent to another engine without a reviewed translator.

These residuals are visible H2+/ART/CITY/H0S/product decisions, not hidden green claims.
