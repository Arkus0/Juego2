# Workpacks

Each workpack is one independently reviewable contract. Work only inside its allowed scope and stop when its Definition of Done is met.

For all `HK-*` workpacks through `WP-HK-GATE`:

- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` is binding;
- exact-SHA evidence is required;
- self-attacks must be causal and reverted before freeze;
- completeness universes must be independently/effectively justified and must not self-shrink;
- a fresh independent Reviewer must PASS before the dependent WP begins.

The H0 sequence is intentionally serial:

```text
HK-00 -> HK-00A -> HK-01 -> HK-02 -> HK-03 -> HK-04 -> HK-02A -> HK-05
       -> HK-06A -> HK-06B -> HK-06C -> HK-07A -> HK-07B
       -> HK-08 -> HK-09 -> HK-10 -> HK-GATE
```

`HK-00A` freezes the commercial product/adoption boundary before contract implementation. It prevents later WPs from accidentally making MCP, Unity or an external harness the semantic source of truth.

The serial gate prevents parallel implementation from baking unreviewed assumptions into later layers.

## Non-foundational tracks

`OPS/`, `ART/` and `CITY/` are parallel non-foundational tracks. They are not bound by `FOUNDATIONAL_PROOF_STANDARD.md`, require no exact-SHA evidence or self-attack matrix, do not appear in the H0 DAG, and can neither block nor unblock any `HK-*` workpack.

A non-foundational workpack is never a valid reason to delay, weaken or reinterpret an H0 acceptance criterion.

`CITY/` is the single operational product-space track. Because accepted `WP-CITY-00` already references `CITY-01..04`, those IDs retain their original owners and the accepted post-CITY-00 spine is intentionally non-numeric:

```text
CITY-00 -> CITY-01 -> CITY-02 -> CITY-05 -> CITY-06 -> CITY-03 -> CITY-04 -> CITY-07 -> CITY-08
```

Responsibilities:

- `CITY-01`: mobility / route graph / walk-time hypotheses;
- `CITY-02`: systemic locations + A–D importance + S0–S4 spatial depth + interior programme;
- `CITY-05`: streets/parcels/reusable building families;
- `CITY-06`: detailed interiors + layered discovery;
- `CITY-03`: exact retained seed + scenario specification;
- `CITY-04`: LOCAL Unity greybox/traversal validation;
- `CITY-07`: LOCAL keeper realization;
- `CITY-08`: LOCAL Arkus authoring proof + reuse closure.

`CITY-01`, `CITY-02`, `CITY-05`, `CITY-06` and `CITY-03` are documentation/research/planning only. `CITY-04`, `CITY-07` and `CITY-08` are LOCAL and MUST NOT begin before their stated Unity/bridge/catalogue prerequisites have passed.

`WP-CITY-00` is COMPLETE. CITY Programme v2 is ACCEPTED on candidate `87a902584f2c46b2d256f6fef26829e9182e7605` (review `#5261734418`, PR `#70`, merge `058e2f4f7c5b018d60cce84e9c89bd07249dd36a`). The next CITY workpack is `WP-CITY-01 — Mobility, district graph + walk-time topology`.
