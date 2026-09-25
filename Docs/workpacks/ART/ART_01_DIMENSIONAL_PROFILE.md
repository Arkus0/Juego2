# ART-01 first-slice dimensional profile

Status: **PROPOSED / ART-01 COMPANION**  
Class: DOCS_ONLY / PRODUCT-COMPOSITION METRIC CONTRACT  
Owner: `WP-ART-01` for kit compatibility; CITY constraints still own street/route/site bands  
Consumers: `ART_ENVIRONMENT_ASSEMBLY_GRAMMAR.md`, `WP-CITY-07`, future reviewed H2 world authoring

## Purpose

Give the first retained environment kit a shared metric language so independently authored walls, openings, roofs, thresholds, props and human-scale checks fit together instead of being individually plausible but mutually inconsistent.

These are **stylized game-production defaults**, not Spanish building-code claims, structural-engineering rules or historical-survey truth. A reviewed family or one-off may override a default when the exception is recorded and remains compatible with accepted CITY constraints and human-scale inspection.

## Coordinate / snap policy

- Unity world unit: **1 unit = 1 metre**.
- CITY planning grid remains **1.0 m** where its accepted documents use that grid.
- Environment assembly default snap: **0.25 m** for modular massing/facade/roof placement.
- Fine snap: **0.05 m** for thresholds, opening recesses, trims and small connection corrections.
- Terrain, retaining response and intentionally irregular historic edges are **not forced** onto the modular snap when that would create visible or traversal defects.
- Rotation defaults: **5°** for authored irregularity; orthogonal/roof/module sockets may impose stricter reviewed values.

The snap system is an authoring aid, not semantic authority. Accepted CITY geometry/bands win over convenience snapping.

## Human reference

Use at least one inspection proxy with:

- standing height: **1.70–1.85 m**;
- eye/camera reference for ordinary third-person scale checks: **1.55–1.70 m** above the support surface;
- shoulder/body width proxy: **0.45–0.60 m**.

The proxy is a scale instrument, not a final character specification.

## Building vertical bands

Unless a reviewed building family or one-off says otherwise:

| Measure | First-slice default band |
|---|---:|
| Floor-to-floor height | **2.8–3.4 m** |
| Clear visible room height | **2.4–3.0 m** |
| Ground-floor commercial/social floor-to-floor | **3.0–3.6 m** |
| Visible wall/plinth zone above local ground | **0.15–0.60 m** |
| Eave projection from finished wall face | **0.25–0.65 m** |
| Visible stylized wall depth at openings/returns | **0.25–0.50 m** |
| Porch/arcade clear head height | **2.1–2.8 m** |

These bands do not override an accepted CITY-05 family, parcel envelope or one-off composition.

## Door / threshold bands

| Measure | First-slice default band |
|---|---:|
| Ordinary public door clear width | **0.85–1.10 m** |
| Service/private door clear width | **0.75–1.00 m** |
| Door clear height | **1.95–2.20 m** |
| Exterior threshold recess / reveal depth | **0.10–0.40 m** |
| Single visible entrance step rise | **0.12–0.18 m** |
| Small threshold landing depth | **0.90–1.50 m** where space/grammar permits |

A player-facing public threshold must still be checked against accepted CITY access/clearance obligations; these numbers do not grant a new step, ramp or access role.

## Window / facade bands

| Measure | First-slice default band |
|---|---:|
| Typical window opening width | **0.65–1.40 m** |
| Typical window opening height | **0.80–1.55 m** |
| Typical sill height above visible interior floor | **0.70–1.10 m** |
| Minimum visible jamb/lintel return used to read an opening | **0.10 m** unless an integrated module supplies an equivalent cue |
| Minimum separation between unrelated facade openings | **0.25 m** unless a reviewed paired module intentionally joins them |

The purpose is to prevent sticker-like facade inserts and incompatible module proportions, not to force identical houses.

## Steps, kerbs and vertical transitions

| Measure | First-slice default band |
|---|---:|
| Stair/step riser | **0.14–0.19 m** |
| Stair/step tread | **0.26–0.36 m** |
| Ordinary kerb/edge rise | **0.08–0.16 m** |
| Ordinary kerb/edge visible width | **0.12–0.30 m** |
| Low retaining/garden wall height | **0.45–1.20 m** before it reads as a major wall condition |

Any route grade, stair route or level relation still requires the appropriate CITY authority. This sheet only constrains the physical realization of already-authorized transitions.

## Roof compatibility bands

- Roof module span and pitch must be declared in its ART-01 metadata.
- Eave height must derive from the supporting wall/mass assembly, not from an independent world-space Y guess.
- Ridge/hip pieces must declare which roof family/pitch they terminate.
- A roof may overhang the supported mass within the family band above; larger canopies/porches are separate reviewed secondary assemblies.
- Visible gaps or penetrations greater than **0.05 m** at intended closed roof/wall joins require an explicit authored reason or are defects.

The `0.05 m` tolerance is a visible-join production tolerance for the first slice, not a physics or architectural standard.

## Ground / street join tolerances

- Intended coplanar retained surfaces should not create duplicate traversable ownership.
- Visible seams intended to read as continuous should resolve within **0.02 m** vertical separation unless a deliberate joint/kerb/channel is authored.
- A deliberate drainage groove, kerb, threshold or retaining step is modeled as a named transition, not hidden as arbitrary surface overlap.
- Collision ownership must remain singular on the player path even if rendering uses layered material/mesh detail.

## Kit compatibility record

Every keeper-capable modular piece used in the representative benchmark records, where applicable:

- nominal width / height / depth;
- support/contact plane;
- insertion or opening size;
- pivot and facing convention;
- compatible snap / socket interval;
- roof pitch/span family if relevant;
- threshold/step/kerb compatibility if relevant;
- human-scale exceptions or one-off status.

A source asset outside these defaults is not automatically rejected. It is classified and, if retained, its exception is explicit so a fresh author does not combine incompatible scales accidentally.

## Acceptance use

ART-01 cannot claim its representative kit is composition-ready unless:

1. every structural/hosted piece used by the benchmark has sufficient dimensional metadata to assemble it without visual guesswork;
2. the representative building and street plan identify any intentional exceptions to this sheet;
3. one human-scale inspection confirms the numbers produce plausible third-person proportions;
4. the fresh-author/Astra smoke test can consume the dimensional profile rather than infer all scale from screenshots or asset names.

## Negative gates

FAIL the dimensional-readiness claim if:

- door/window/roof modules only fit because they are non-uniformly stretched ad hoc with no retained rule;
- each building invents its own storey/threshold scale independently of the shared profile;
- a street/threshold/step is tuned visually but violates accepted CITY clearance/grade/access truth;
- duplicated/coplanar surface layers are tolerated merely because their separation is visually small;
- or a fresh author cannot determine the intended size/pivot/support convention of a required structural kit piece.
