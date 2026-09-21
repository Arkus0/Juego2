# WP-CITY-03 — Boundary + handoff audit

Baseline: `main@1b503cfabeca340c42055aaf31df90d72ca28e68`  
Audited semantic owner: `Docs/production/CITY_PRODUCT_SEED.md` v0.2  
Purpose: recompute the selected seed's mechanical handoff properties without redefining product semantics.

## 1. Exact boundary arithmetic

Selected hard polygon, in order:

```text
B01 (-20,-60)
B02 (70,-80)
B03 (205,-48)
B04 (245,22)
B05 (220,95)
B06 (150,145)
B07 (70,132)
B08 (-20,55)
```

Shoelace recomputation:

```text
area = 44,817.5 m²
     = 0.0448175 km²
```

Result: **PASS** against inherited CITY-00 retained-seed band `0.03–0.06 km²`.

The audit deliberately does not subtract an invented “net land” quantity. CITY-00 delegates one retained-seed band and CITY-03 defines one hard planning envelope. Water/no-build masks are semantic non-traversable regions inside that envelope, not a second area acceptance oracle.

## 2. Anchor containment

Point-in-polygon recomputation on the exact hard polygon:

| Anchor | Target | Inside? |
|---|---:|---:|
| `W.LANDING` | `(0,0)` | YES |
| `W.X1` | `(50,-32)` | YES |
| `O.X1` | `(42,-64)` | YES |
| `W.CASCO` | `(80,35)` | YES |
| `W.X5` | `(92,108)` | YES |
| `E.X5` | `(90,122)` | YES |
| `W.PLAZA` | `(150,58)` | YES |
| `W.SHOP` | `(195,70)` | YES |

Result: **8/8 selected anchors inside**.

Explicit outside anchors remain outside by semantic declaration and are not required to have CITY-03 target coordinates because they are not built by CITY-04: W.X2/E.X2/E.HOME, W.CM_NE/W.VEGA/W.BARRIO/W.X3/W.X4, W.RIBERA, O.PUERTO_UP/O.QUAY/O.ENTRADA.

## 3. Site-region containment + family-band check

All vertices of F01–F08 and S01–S03 were checked against the hard polygon: **11/11 regions fully contained**.

Bounded frontage-region dimensions also stay within accepted CITY-05 parcel planning bands:

| Slot | Approx. bounding size | Inherited family band | Result |
|---|---:|---|---|
| F01 bar | 8×15 m | `pc.historic_row` 5–9 × 9–18 | PASS |
| F02 ayuntamiento | 18×22 m | `pc.civic_frontage` 12–24 × 15–30 | PASS |
| F03 everyday shop | 10×18 m | `pc.commercial_row` 6–12 × 12–24 | PASS |
| F04 old-row home | 7×14 m | `pc.historic_row` | PASS |
| F05 old-row home | 7×12 m | `pc.historic_row` | PASS |
| F06 plaza-edge frontage | 12×16 m | `pc.commercial_row` | PASS |
| F07 mixed frontage | 8×16 m | `pc.commercial_row` | PASS |
| F08 old-row house | 7×13 m | `pc.historic_row` | PASS |

S01–S03 are bounded open/exterior site regions. Final clear-route offsets, threshold aprons, bank geometry and shell footprints remain CITY-04 physical validation inside these bounded regions; CITY-04 does not choose a different site.

## 4. Crossing / graph conformance

Playable inter-landmass crossings represented by CITY-03:

- Río: **X1 only**.
- Arroyo: **X5 only**, preserving low-water-only availability.

Explicit non-playable future crossing socket:

- `W.LANDING` may visually expose future X6/X7 relation but neither X6 nor X7 traversal geometry is built.

Negative graph checks:

- no dry Wedge→Puerto edge: PASS;
- no Ensanche→Orilla-sur edge: PASS;
- no eighth crossing: PASS;
- X5 not counted as permanent redundancy: PASS;
- shared court not counted as public graph edge: PASS;
- Casco micro.A/B remain within one W.CASCO node and are not used as CITY-01 proof: PASS.

## 5. Selected place/depth/interior coverage

Selected hard seed contains:

- I3: `loc.casco.bar` only;
- I2: `loc.plaza.ayuntamiento`;
- I1: `loc.calle.everyday_shop`;
- I0 second layers: `loc.plaza.market`, `loc.casco.bridgehead`, `loc.casco.shared_court`;
- ordinary C/S1 closed frontages through F04–F08.

CITY-06 selected-seed minimums:

| Requirement | Witness | Result |
|---|---|---|
| ≥1 I2/I3 deep interior | bar I3 + ayuntamiento I2 | PASS |
| ≥1 I1 shallow or exterior-led S2/S3 | shop I1 + market/shared court | PASS |
| ordinary/quiet place valuable without secret | W13/S03 ordinary quiet fabric | PASS |
| authored discovery + distinct future owner route | bar and/or ayuntamiento opportunity | PASS |
| no extra I3 hero | bar only | PASS |
| extraordinary strand may be zero | selected zero | PASS |

## 6. Access-role preservation

Selected functional bindings preserve the accepted CITY-05 role sets:

| Place | Required roles | CITY-03 representation | Result |
|---|---|---|---|
| bar | `{public, service, semi-private}` | three distinct role-bearing relations; hero overlay does not replace role | PASS |
| ayuntamiento | `{public, service, private}` | public civic + staff/service + private records/work | PASS |
| everyday shop | `{public, service}` | one I1 public room + explicit service threshold/pocket | PASS |
| market | `{public}` | public open-site; apron cannot erase route | PASS |
| bridgehead | `{public}` | public open-site | PASS |
| shared court | public passage + semi-private court | roles remain distinct; no public through-cut | PASS |

No form qualifier (`rear`, `court`, `vertical`, `staff`, etc.) substitutes for a required access role.

## 7. Classification / reuse posture

Hard playable seed covers:

- systemic importance: A, B and C;
- spatial depth: S1, S2, S3, S4;
- interior depth: I0, I1, I2, I3.

D/S0 remains soft scenic envelope as intended rather than being promoted into hard playable content.

Selected reusable production families include:

- `bf.house` old-row variant;
- `bf.bar_social`;
- `bf.civic`;
- `bf.shop_service` / compatible mixed-use shell;
- ordinary mixed frontage;
- `if.social_house` + one inherited `hero.bar_layered` overlay;
- `if.civic_office`;
- `if.retail_shallow`.

The selected seed is therefore not dominated by one-off deep interiors.

## 8. Route/time arithmetic

Only fully represented accepted CITY-01 edges are listed for CITY-04 measurement.

Recomputed planning sums:

```text
O.X1 → X1 → W.X1 → W.CASCO → W.PLAZA
= X1 1.0 + W12 1.0 + W05 1.75
= 3.75 min

W.SHOP → W.PLAZA → W.CASCO → W.LANDING
= W04 0.5 + W05 1.75 + W06 2.25
= 4.50 min

W.CASCO → W.X5 → E.X5
= W13 1.0 + X5 0.4
= 1.40 min when X5 is available
```

Arithmetic: **PASS**.

The handoff explicitly forbids extrapolating absent full-city routes into “measured” facts.

## 9. Scenario coverage audit

WP-CITY-03 minimum future scenario coverage → selected CITY-04 pack:

| Contract coverage | Scenario witness |
|---|---|
| quiet ordinary morning | SCN-01 |
| market/commercial flow | SCN-02 |
| NPC home/work/social trip | SCN-03 spatial proxy |
| second materially different ordinary trip | SCN-04 |
| follow/search route choice | SCN-05 |
| bar/social activity spatial support | SCN-06 |
| river/bridge traversal | SCN-07 |
| material delivery/work consequence | SCN-08 proxy + visible material change |
| municipal access/routing/service change | SCN-09 access/service consequence proxy |
| low-stakes player perturbation | SCN-10 |
| leave/return to changed local state | SCN-11 proxy |
| discovery with >1 truthful route when later systems exist | SCN-12 |
| changing/blocked route + alternate | SCN-13 |

Every future runtime-dependent scenario is explicitly marked proxy/readiness, not runtime proof.

## 10. Expansion-seam audit

Named seams:

1. `seam.commercial_ne` — Calle Mayor / X2 / Vega direction;
2. `seam.ensanche_x5` — Ensanche bank direction;
3. `seam.port_landing` — future direct X6/X7 Puerto relation;
4. `seam.orilla_x1` — camino sur → Puerto/Entrada;
5. `seam.upper_future` — later upper/Barrio continuity through accepted graph.

Result: **5 named directions**, minimum required = 4.

None requires moving retained W.PLAZA, W.CASCO, X1, W04/W05/W06/W12/W13 or F01–F03.

## 11. CITY-04 execution sufficiency

A LOCAL Worker receives without selecting another city slice:

- exact hard polygon;
- selected anchor targets;
- exact accepted edges/crossings represented;
- bounded F01–F08/S01–S03 site regions;
- selected A/B/C places and I0–I3 obligations;
- preserved access-role sets;
- retained-vs-temporary declaration;
- five expansion seams;
- soft-envelope rules;
- thirteen spatial scenarios;
- segment/time hypotheses and physical/access/sightline measurements;
- causal owner routing if physical evidence fails.

Result: **handoff is construction/measurement, not renewed macro-layout selection**.

## 12. Negative controls

1. Remove X1 → inherited seed/bridge scenario fails.
2. Make X5 permanent → inherited crossing semantics fail.
3. Connect W.LANDING dry to Puerto → CITY-00 geography fails.
4. Route through S03 semi-private court → access laundering / false graph edge.
5. Remove F03 service role → CITY-05 role binding fails.
6. Open F04/F05/F06/F07/F08 interiors → I0/open-door inflation.
7. Count PA route as implemented → CITY-06 causal boundary fails.
8. Move F01/F02/F03 to another street during greybox → CITY-04 becomes city redesign.
9. Promote another hero interior → CITY-02/06 fails.
10. Use a scenic/soft-envelope path as playable seam → hard/soft boundary fails.

All controls target a causal contract boundary rather than visual preference.

## 13. Audit verdict

**BOUNDARY_AND_HANDOFF_AUDIT: CLEAN**

No predecessor reopen signal is present in the selected planning candidate. Metric plausibility remains deliberately falsifiable by CITY-04 after its own prerequisites are satisfied.