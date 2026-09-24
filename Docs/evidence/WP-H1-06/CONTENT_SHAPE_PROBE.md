# WP-H1-06 content-shape probe

Status: **OBSERVED ON IMPLEMENTATION SHA `b1af5c4cf16cf42273095529102f8ca7a1e635fb`; FINAL SHA RECHECK PENDING**

The physical-local command inspected the owner-local archive at its accepted SHA-256. It found at least 12 discoverable prefab/FBX candidates in each required category: wall, roof, door, window and prop. The bounded report includes the first 12 sorted candidates per category and 40 representative `.prefab` dependency rows. Each sampled prefab had one to three Unity GUID references; two sampled wall corner prefabs had three. These are archive-shape observations, not proof that every listed source has been adopted or effectively imported.

The accepted wall/window FBX used by the public H1-06 conformance run retained its mesh and material references through a managed prefab variant. The archive also contains distinct roof, door and prop prefabs with dependency-shaped references. This is an **H1-06 omission check**: the implementation must preserve observed source/variant and dependency relationships, including nested prefab relationships where present. A separate harness-only nested fixture now proves the save/reload and flattening control on that specific mechanism. The current accepted H1-04 project-local slice is narrower than the archive. Broader prefab import, component/property fidelity and production art adaptation are **named future/residual decisions**, not H1-06 source adoption. No concrete H1-04/H1-05 predecessor contradiction was observed; no CITY/H2 production is authorized.

## Exact source boundary

The probe is constrained to the H1-04 accepted Medieval Village archive:

- relative owner Source path: `Medieval Village/Engine Projects/Medieval Village MegaKit[Unity URP].zip`
- accepted SHA-256: `b9d757dd2608a5cee4d9ee1e8183f6cb4cad9d27480841a905180def9c7d8b10`
- classification: accepted-source **inspection only**; no new import/adoption/mapping is authorized by this probe.

## Probe

`scripts/h1-06-content-shape-probe.py` inspects the exact archive and requires discoverable prefab/FBX candidates covering the street-corner-shaped categories required by the WP:

- wall;
- roof/thatch;
- door/gate;
- window;
- prop/utilities such as barrel, crate, cart, bench, table, lamp, sign, well or fence.

For representative `.prefab` entries, it also reports dependency-shaped Unity GUID references. The output is an omission detector only: it does not claim catalogue completeness and it does not authorize adding those archive entries to H1-04's accepted project-local slice.

The H1-06 positive realization proof remains bound to the already accepted logical Quaternius wall/window prefab and its effective mesh/material relationships. Broader archive observations are used only to catch an implementation shape that would work for a lone primitive but obviously miss the approved representative content.

## Classification rule

After exact local execution, findings are classified as:

- **H1-06 blocker** if the implemented prefab relationship model cannot represent the observed source/prefab/nested/dependency shape needed by the accepted representative slice;
- **predecessor reopen condition** only if concrete evidence proves an accepted H1-04/H1-05 guarantee false or inapplicable;
- **future/residual** for component/property breadth explicitly deferred beyond H1-06;
- **out of boundary** for broad Cantabrian remodeling, gameplay, production art or new source adoption.

The canonical execution command is the same H1-06 receipt command recorded in `PROOF_MATRIX.md`; its scratch output is `Unity/ArkusUnity/Library/Arkus/H1PrefabRealization/content-shape.json`.
