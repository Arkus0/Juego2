# WP-H1-06 content-shape probe

Status: **PENDING PHYSICAL-LOCAL EXECUTION**

H1-06 must perform the mandatory bounded content-shape probe against the already approved representative Juego2 target. This file deliberately does not invent observations from the owner-local Quaternius archive.

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
