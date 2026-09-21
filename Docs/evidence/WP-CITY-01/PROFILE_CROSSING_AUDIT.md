# WP-CITY-01 — Profile / water-crossing repair audit

Workpack: `WP-CITY-01`  
Repair cycle: **1**  
Trigger: independent review `#5263730661` on candidate `2fac578973cf3686d264b57c8ba7519b8ad507b2`  
Semantic owner under audit: `Docs/production/CITY_MOBILITY_TOPOLOGY.md` v1.3

This is derived audit evidence only. It does not own graph edges or create new movement semantics.

## 1. Reviewer blocker restated

The failed candidate defined `EW` but did not assign it to X1–X7 and did not close `EW` behaviour for slower pedestrian or bicycle. In particular, X2/X4 bicycle traversal and X6 slower-pedestrian wait composition were not derivable from the authoritative rules.

The repair is intentionally local. CITY-00 geography, X1–X7 topology, State-1/State-2 availability and ordinary-pedestrian base costs are unchanged.

## 2. Crossing classification audit

Every inter-landmass edge now has an explicit elevation and access pair:

| Crossing | Elev. | Access | Availability | Ordinary movement cost |
|---|---|---|---|---:|
| X1 | EW | AH | State 1+2 permanent | 1.0 |
| X2 | EW | AR | State 1+2 permanent | 0.7 |
| X3 | EW | AF | State 1+2 permanent | 0.5 |
| X4 | EW | AR | seasonal / flood-closable | 0.9 |
| X5 | EW | AF | low water only | 0.4 |
| X6 | EW | AX6 | State 1 only; hours/fare; high water suspends | 1.0 + `W_ferry` |
| X7 | EW | AR | State 2 only; permanent | 1.0 |

Audit result: **PASS.** No eighth crossing, availability change or CITY-00 reinterpretation was introduced.

## 3. Slower-pedestrian crossing cost audit

Rule: `EW` locomotion uses ×1.35. X6 wait is temporal context and is added after the locomotion multiplier; it is never multiplied.

| Crossing | Recomputed slower-ped cost |
|---|---:|
| X1 | 1.350 |
| X2 | 0.945 |
| X3 | 0.675 |
| X4 | 1.215 |
| X5 | 0.540 |
| X6 | 1.350 + `W_ferry` |
| X7 | 1.350 |

At `W_ferry = 0`, X6 slower-ped cost is 1.35 min. At `W_ferry = 4`, it is 5.35 min, **not** 6.75 min. That proves the wait term is not accidentally multiplied.

Audit result: **PASS — all EW costs are deterministic.**

## 4. Bicycle crossing audit

The bicycle rule is now derived from `EW + Access`:

| Crossing | Bicycle result | Reason |
|---|---|---|
| X1 | push/dismount only | explicit AH exception |
| X2 | rideable | EW + AR |
| X3 | unavailable | EW + AF |
| X4 | rideable while available | EW + AR + seasonal availability |
| X5 | unavailable | EW + AF |
| X6 | unavailable / carriage not assumed | EW + AX6 |
| X7 | rideable in State 2 | EW + AR |

The rule therefore does not infer bicycle access merely from “bridge” prose.

### X2 preference recomputation

For CITY-01 comparative route weights, rideable bicycle segments use the same ledger base movement weight; no bicycle speed-up is asserted.

Ensanche home → Calle Mayor shop via permanent X2:

`E01 + X2 + W03 = 1.1 + 0.7 + 1.7 = 3.5 min planning weight`.

Road-capable X4 alternative while available:

`E01 + E05 + X4 + W16 + W02 + W03 = 1.1 + 0.8 + 0.9 + 1.0 + 1.8 + 1.7 = 7.3 min planning weight`.

X3/X5 are AF and therefore unavailable to the bicycle profile; X4 is seasonal. Thus the statement that a cyclist prefers road-capable X2 is now a direct consequence of the authoritative profile/access/cost rules.

Audit result: **PASS.**

## 5. Affected scenario re-audit

- **X2 closure:** pedestrians retain X4/X3 alternatives exactly as before. Bicycle can use X4 only while X4 is available; no new bicycle permission is invented for X3.
- **State-1 ferry:** ordinary pedestrian uses `1.0 + W_ferry`; slower pedestrian uses `1.35 + W_ferry`; bicycle carriage remains unclaimed.
- **State-2 landing crossing:** X7 remains AR, so bicycle and full cart may cross; ordinary route costs remain unchanged.
- **Port / break-bulk:** X6 remains porter/carryable-load scale only; the repair does not broaden freight capacity.
- **CITY-04 handoff:** represented EW examples may now validate ride/dismount behaviour without implying full-city measurement outside the CITY-03 seed.

Audit result: **PASS.**

## 6. Regression boundary

The repair intentionally does **not** change:

- X1–X7 endpoints;
- State-1/State-2 topology;
- Plaza-removal proof;
- ordinary-pedestrian route-cost matrix;
- X2 closure selection;
- CITY-03 retained-seed ownership;
- CITY-04 bounded measurement authority;
- runtime/Unity/H0/H1/ART semantics.

Overall affected audit: **CLEAN.**
