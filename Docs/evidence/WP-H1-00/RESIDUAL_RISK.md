# WP-H1-00 residual-risk boundary

RESIDUAL_RECONCILIATION: `COMPLETE`
UNCLASSIFIED_RESIDUALS: `0`
PREDECESSOR_REOPEN_TRIGGERED: `NO`

H1-00 proves only the portable engine-neutral projection contract and the deterministic fileless reference materializer. It deliberately does not promote later Unity/editor concerns into false green claims.

| Residual / limitation | Classification | Owner / reopen condition |
|---|---|---|
| Real catalogue inventory completeness, logical catalogue ID mapping and type/version compatibility | downstream | `WP-H1-01` and later catalogue/provider work |
| Unity serialization, `AssetDatabase`, native GUID/local-file IDs, `GlobalObjectId`, scenes/prefabs/components and Editor-effective observation | downstream | Unity bridge/materialization WPs, especially `WP-H1-05` |
| Exact Unity editor/package/platform behavior and byte-level YAML identity | out of H1-00 claim | later pinned Unity WPs; normalized parity, not cross-version byte identity, remains the architecture rule |
| Project checkpoint/restart/rebuild against real Unity files | downstream | `WP-H1-10`; H1-00 rebuild proof is deliberately fileless/reference-only |
| WAL/fsync/power-loss/crash atomicity across canonical state and engine files | out of H1 architecture claim | not introduced by H1-00; only explicit later durability requirements could reopen it |
| Public host-to-Editor batch lifecycle, cancellation/crash recovery and Editor main-thread execution | downstream | `WP-H1-03A` / feature owners under ADR-H1-004 |
| Unity-to-canonical supported-edit proposal compilation | downstream | later synchronization owner; H1-00 creates no canonical mutation route |
| Gameplay/runtime state, navigation, physics, animation behavior, AI/save state and CITY keeper realization | out of boundary | H2+/CITY owners |

## H0 predecessor reconciliation

No executable H1-00 evidence contradicts the accepted H0 guarantees consumed from `WP-HK-GATE`. The reference materializer consumes an immutable supplied snapshot/anchor, does not reinterpret H0 mutation/CAS/journal semantics, and has no reference to `Arkus.Harness.Runtime` or transport adapters.

H0 would be reopened only by concrete evidence that the accepted canonical snapshot/anchor cannot be consumed without changing canonical semantics or that a required neutral projection cannot be represented through the accepted boundary. No such evidence was found.

## Dependency / IP result

`Arkus.EngineBridge` introduces no package or project dependency. The current WP therefore adds no external runtime/IP adoption record and no vendor semantic authority.
