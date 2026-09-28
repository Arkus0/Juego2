# Urban expansion decision — Juego2

Status: **ACCEPTED product amendment**
Date: 2026-09-27
Accepted: PR `#257`, candidate `fdfc9be334a750217f2152c76d17bfcd20ba5ded`, Reviewer PASS `#5333313032`, merge `6bf2d6e74215be51e73d85f659a2a734752ea34c`.
Transition: after `WP-H2F-01A`; its active candidate and acceptance contract stay intact.
Current scale terminology: amended by [`PORT_TOWN_SCALE_AMENDMENT.md`](PORT_TOWN_SCALE_AMENDMENT.md). The accepted 2026-09-27 record below remains historical evidence; current product language is **large port town / villa portuaria**, while `district` remains a production/neighbourhood term.

## Decision

Juego2 remains a systemic living-world game built on Arkus. Its final product target expands from a small inland town/island to a **fictional port city in northern Spain around the late 1990s or early 2000s**, developed as compact, distinctive districts. The fantasy is **daily life → investigation → adventure**. The player investigates a murder, meets people with routines and histories, visits businesses and interiors, works and trains, follows suspects, plays activities and minigames, and eventually fights and pursues people. Inspiration from Shenmue II concerns the shape of the experience, not its protected content.

**GC2 ejecuta. Arkus recuerda y conecta.** Game Creator 2 accelerates local gameplay and presentation. Arkus remains the active persistent/causal authority for the facts and relationships that make the city remember and react. The split is specified in [`GC2_ARKUS_RUNTIME_SPLIT.md`](../architecture/GC2_ARKUS_RUNTIME_SPLIT.md).

Aim for four or five compact districts as an **orientation**, never a gate before the first slice. A working hypothesis is port/lonja, old town, market/commercial, residential and nightlife/industrial; boundaries and sites require their own CITY decision. Density, reasons to revisit and meaningful connections precede map area. Approximately 80–120 visible/interchangeable people, 10–15 principal people and 20–40 interactive secondary people are **planning ranges**, not commitments or current performance promises. [`NPC_DEPTH_TIERS.md`](../design/NPC_DEPTH_TIERS.md) allocates depth by role. The later port-town amendment additionally separates routine coverage from narrative depth and allows lightweight schedules for many Tier-C people without making them deeply systemic.

The first 20–30 minute slice must mix investigation, daily life, an activity, reactivity, a chase, a fight and a changed return visit. Neither all districts nor a citywide simulation is a prerequisite. Pursuit, combat and minigames enter the roadmap as bounded gameplay, with persistent consequences when authored and useful.

## What remains binding

| Classification | Meaning here |
|---|---|
| ACCEPTED_HISTORY | Accepted H0/H1/H1-GATE, CITY, ART, H2/H2F planning, CTX, DW and PA decisions, proof and reviews remain truthful records. No retrospective editing to imply the urban target was previously accepted. |
| ACTIVE_FOUNDATION | Arkus canonical world state and public authoring boundary, Unity/H1 bridge, H2F selected production foundation, ART composition/provenance and CITY physical constraints still apply where their accepted scopes apply. Arkus is not legacy, optional or dormant. |
| ACTIVE_PRODUCT_DIRECTION | Following this amendment's acceptance, the port-city direction, selective NPC depth and GC2 local execution guide pending product work; current diegetic scale terminology is amended to large port town / villa portuaria by `PORT_TOWN_SCALE_AMENDMENT.md`. |
| SUPERSEDED_SCOPE_ASSUMPTION | The Potes/Liébana small town or island as the **final** geographical and population ceiling, and a universal requirement to make every visible NPC deep, no longer define the final game. Historical setting and pilot evidence remain in place. |

The accepted inland CITY seed, Puente Viejo → Casco → Bar chain, Quaternius-based visual work and current ART-01 input remain a **retained pilot and reusable construction evidence**. They do not magically become a port district or authorize a new coastline, topology, site or access role. `CITY-URBAN-00` owns the explicit geographic/programme transition; `ART-URBAN-01` owns the port visual kit; `CITY-URBAN-01` realizes the first keeper urban block. Existing CITY-07/H2 contracts can finish the representative pilot on their own accepted geometry; later urban blocks consume reusable results. ART's new port-town identity is validated on the actual first urban block before claiming it as the full game look.

## Decision controls

- H2F-01A finishes on its current contract. H2F-02 consumes its actual accepted handoff, including any reviewed S06 amendment; planning does not predetermine a Core result that 01A has not proven.
- Stable Arkus infrastructure remains in use. Extend it only for an identified persistent or cross-system game need; buy/adopt separately licensed GC2 modules through the dependency policy when their phase demonstrates value. Ownership of Core is not ownership of those modules.
- A minigame or immediate reaction can remain entirely local. A meaningful later consequence crosses the reviewed GC2→Arkus seam; do not make GC2 identifiers canonical.
- Product PASS primarily shows gameplay visible to a player. Preserve strong integrity checks for canonical state, loss, saves, determinism where required, authority and clean rebuild. Reviews cannot add speculative universal requirements after a WP's acceptance contract is frozen.
- This amendment authorizes planning and WP changes. It does not claim licensed packages are installed, port geography is accepted, an urban district exists, or gameplay is implemented.

Executable succession and WP ownership: [`POST_H2F01A_ROADMAP.md`](../roadmap/POST_H2F01A_ROADMAP.md) and [`POST_H2F01A_WP_AUDIT.md`](../roadmap/POST_H2F01A_WP_AUDIT.md).
