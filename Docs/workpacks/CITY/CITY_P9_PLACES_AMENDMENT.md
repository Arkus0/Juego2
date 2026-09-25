# CITY P9 — additional small enterable places in the retained seed (amendment proposal)

Status: **PROPOSED / NOT ACCEPTED**  
Class: DOCS_ONLY / PRODUCT-SCOPE AMENDMENT  
Causal owners amended: **CITY-02** (`CITY_LOCATION_PROGRAMME.md` programme + interior-priority ledger) and **CITY-06** (`CITY_INTERIORS_DISCOVERY.md` I1 allocation)  
Also required: **CITY-03** (new frontage regions F09–F19) and **CITY-05** (§9 mapping rows, existing families only)  
Depends on: `CITY_P8_LANES_AMENDMENT.md` for the seven places marked “P8”  
Origin: concept `Docs/production/concept/CITY_07_GAME_MAP_CONCEPT.md` §5b  
Base: `main` at `2aefda4`

Acceptance requires independent review, merge and DocSync. Until then, opening any of these façades in CITY-07 is CITY-06 §12 *door inflation*.

## 1. Problem

The seed opens exactly three enterable places (F01 bar I3, F02 ayuntamiento I2, F03 shop I1). In the concept map, only **27 %** of drawn public street lies within 25 m of an enterable place. The owner's reference for a good game district is Yakuza's Kamurocho, where nearly every frontage offers something to do. CITY-02 RD-1 (meaningful destinations per traversal) is the programme's own metric for this gap.

## 2. Programme additions (CITY-02 §4 rows)

All are ordinary market-town places consistent with SETTING / VISUAL_BIBLE (fictional Potes / Liébana). None is hero; `loc.casco.bar` stays the only I3.

| Place | Name | Node | Use | A–D | S | I | Access roles | Seed region | P8 |
|---|---|---|---|---|---|---|---|---|---|
| `loc.casco.taberna_puente` | Taberna del Puente | W.X1 / W12b | drink, food, social | B | S2 | I1 | public + service | F09 | — |
| `loc.casco.barberia` | Barbería | W12a | service, social, information | B | S2 | I1 | public + private | F10 | — |
| `loc.casco.orujeria` | Orujería del alambique | W06 | work, retail, seasonal | B | S2 | I1 | public + service | F11 | — |
| `loc.casco.queseria` | Quesería | W05b | retail, food | B | S2 | I1 | public + service | F13 | — |
| `loc.casco.ferreteria` | Ferretería | W05c | retail, work supply | B | S2 | I1 | public + service | F14 | — |
| `loc.casco.horno` | Horno de pan | W.HORNO / W21 | food, work | B | S2 | I1 | public + service | F12 | yes |
| `loc.casco.cafe_billar` | Café-billar | W24 (door faces the Pasadizo, not W13) | social, leisure, late | B | S3 | I1 | public + service | F15 | yes |
| `loc.casco.fonda_tintes` | Fonda de los Tintes | W20 | food, outsider social | B | S2 | I1 (ground-floor common room only) | public + service | F16 | yes |
| `loc.casco.herreria` | Taller del herrero | W19 | work, repair | B | S2 | I1 | public + service | F17 | yes |
| `loc.casco.estanco` | Estanco-quiosco | W22 | retail, information | C→B | S2 | I1 | public + private | F18 | yes |
| `loc.ensanche.ultramarinos` | Ultramarinos del Ensanche | E06 | everyday retail | B | S2 | I1 | public + service | F19 | yes |
| `loc.casco.tintoreria` | Tintorería | S04 Rincón del Tinte | domestic work, exterior | B | S1 | **I0** | public | S04 | yes |

Depth distribution stays varied (RD-5): S1/S2/S3/S4 and I0/I1/I2/I3 are all present, and A-places are unchanged. `loc.calle.bakery` (I2, Calle Mayor) and `loc.entrada.fonda` (I2, Entrada) remain the deep production and lodging places. The horno and fonda here are deliberately shallow and do not duplicate their depth.

## 3. Interior families (CITY-06)

No new family is needed (CITY-06 §4.1):

- `if.retail_shallow`: barbería, quesería, ferretería, horno (sales room only), estanco, ultramarinos, taberna and café-billar (counter room + service pocket), fonda (common room + service; upper rooms are **not** promised).
- `if.work_support_shallow`: orujería, herrería (work bay + customer threshold).
- Tintorería is exterior-only (`I0`).

No new discovery opportunity is required. CITY-06 §12 anti-inflation still applies: no hidden rooms, no loot, and no service door becomes public.

## 4. Exterior mapping (CITY-05 §9 rows, existing families)

| Place | Family / parcel |
|---|---|
| taberna, café-billar | `bf.bar_social` on `pc.historic_row` |
| barbería, orujería, quesería, ferretería, horno, herrería, estanco | `bf.mixed_use_house` on `pc.historic_row` (street-level public/service bay) |
| fonda | `bf.mixed_use_house` (fonda shell) on `pc.historic_row` |
| ultramarinos | `bf.shop_service` on `pc.ensanche_garden` |
| tintorería | `pc.open_site` S04 |

## 5. Frontage regions (CITY-03 §4.2 rows)

Planning polygons in the seed frame (U,V). Each was validated with shapely against: the hard polygon; water masks and effective bank shoulders; F01–F08, S01, S03, the plaza, the Casco square and the X1 bridgehead; every accepted and P8 route buffered by its half-width + 0.3 m; and each other. Each has a 1.9–2.7 m threshold setback from its lane. Sizes sit inside the `pc.historic_row` bands (frontage 5–9 m, depth 9–18 m); F19 sits inside `pc.ensanche_garden` (frontage 8–16 m, depth 16–30 m).

| ID | Polygon | Size (front × depth) |
|---|---|---|
| F09 | (68,-22.5),(71.5,-15.5),(62.5,-11),(59,-18) | 8 × 10 |
| F10 | (67,12),(69.5,17.5),(61,21),(58.5,15.5) | 6 × 9 |
| F11 | (49.5,25),(57.5,27),(55.5,36.5),(47.5,35) | 8 × 10 |
| F12 | (102,16),(96,13.5),(99.5,5),(106,8) | 7 × 9 |
| F13 | (110.5,56.5),(104.5,53.5),(108,45.5),(114.5,48) | 7 × 9 |
| F14 | (125,63.5),(118.5,60),(123,52),(129,55.5) | 7 × 9 |
| F15 | (89,70),(88.5,62),(98.5,61),(99,69) | 8 × 10 (public face north to W24) |
| F16 | (158,42.5),(158,33.5),(170,33.5),(170,42.5) | 9 × 12 |
| F17 | (144.5,5),(135.5,4.5),(136.5,-6.5),(145.5,-6) | 9 × 11 |
| F18 | (153.5,96.5),(156,91),(165,95),(162,100.5) | 6 × 9.5 |
| F19 | (54.5,81.5),(61,87.5),(50,99),(43.5,93) | 9 × 16 |

## 6. Effect

- Enterable places in the seed: 3 → 15 (14 interiors + the exterior tintorería).
- Drawn public street within 25 m of an enterable place: **27 % → 77 %** (concept-map measurement over accepted + P8 routes, 2 m samples). This is a planning proxy for RD-1, not a playtest result.
- Quiet balance (RD-4) is kept: W13, W23, W25, W26, S06 and S07 carry no public thresholds, and the café faces the Pasadizo precisely to keep W13 quiet.
- The CITY-08 reserved slice (`W04 + F03 + F07 + S01`) is untouched.

## 7. Required downstream evidence

If accepted, the CITY-07 asset-rich demo adds:

- RD-1 destinations per 5 minutes on the representative traversals;
- RD-4 quiet minutes;
- threshold readability for each new place (public vs service or private);
- the owner's “Kamurocho test”: from representative street positions, is at least one thing to do or discover in view?

Runtime content (keepers, hours, stock, conversations) remains with PA owners.

## 8. Negative gates

FAIL if:

- any other I0 façade opens by implication;
- a place gains I2+ depth, upper floors or a hidden room;
- a service door becomes public;
- the café or another threshold is moved onto W13 or S03;
- a P8-dependent place is realized without the corresponding accepted P8 edge;
- or programme wording implies gameplay systems exist.
