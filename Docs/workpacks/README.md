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

`CITY/` exists so spatial/product preproduction can be reasoned about before local production begins. `WP-CITY-00` through `WP-CITY-03` are documentation/research/planning only and may run remotely before `WP-HK-GATE`; they do not authorize Unity scene construction, asset import or serious game production. `WP-CITY-04` is the first local blockout/engine-validation workpack and MUST NOT begin before `WP-HK-GATE` has passed and the relevant Unity bridge prerequisites are accepted.
