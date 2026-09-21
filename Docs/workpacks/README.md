# Workpacks

Each workpack is one independently reviewable contract. Work only inside its allowed scope and stop when its Definition of Done is met.

For all `HK-*` workpacks through `WP-HK-GATE`, and all H1 workpacks marked `FOUNDATIONAL`:

- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` is binding;
- exact-SHA evidence is required;
- self-attacks must be causal and reverted before freeze;
- completeness universes must be independently/effectively justified and must not self-shrink;
- a fresh independent Reviewer must PASS before the dependent WP begins.

The accepted H0 sequence was intentionally serial after its reviewed splits:

```text
HK-00 -> HK-00A -> HK-01 -> HK-02 -> HK-03 -> HK-04 -> HK-02A -> HK-05
       -> HK-06A -> HK-06B -> HK-06C -> HK-07A -> HK-07B
       -> HK-08A -> HK-08B -> HK-09A -> HK-09B -> HK-10 -> HK-GATE
```

`HK-00A` freezes the commercial product/adoption boundary before contract implementation. It prevents later WPs from accidentally making MCP, Unity or an external harness the semantic source of truth.

H0 is complete. The H1 plan, DAG and boundary rationale in `Docs/workpacks/H1/README.md` are ACCEPTED after PR `#71` PASS on `58b0d78a57b8c617d167e6bf286a6cbd29b0612b` and merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`.

`WP-H1-00` and `WP-H1-01` are **COMPLETE**. `WP-H1-00` passed on frozen candidate `3dc513dd963b77116fd45b5af8d800ae8993dd34` (review `#5264661860`, PR `#75`, merge `c02cf54c89c13db43edda4a602b0c1620baa3fa2`, frozen exact-SHA validation `35579126516` GREEN). `WP-H1-01` passed on frozen candidate `385ce2466190003d18c849c8d944b881d184e1c7` (review `#5265525323`, PR `#79`, merge `0b8f23fbce227bbbc710c8410dff575fcb9fcf12`, frozen exact-SHA validation `35587471700` GREEN). `WP-H1-02 — pinned reproducible Unity project/toolchain/package baseline` is now the next default dependency-valid H1 workpack and remains `NOT_STARTED` until a human starts its Worker. Later H1 WPs still require their own stated predecessors' PASS + merge + DocSync; in particular `WP-H1-03` remains blocked until `WP-H1-02` is accepted.

## Non-foundational tracks

`OPS/`, `ART/`, `CITY/` and `PA/` are parallel non-foundational tracks. They are not bound by `FOUNDATIONAL_PROOF_STANDARD.md`, require no exact-SHA evidence or self-attack matrix, do not appear in the H0 DAG, and can neither block nor unblock any `HK-*` workpack.

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

`CITY-01`, `CITY-02`, `CITY-05`, `CITY-06` and `CITY-03` are documentation/research/planning only. `CITY-04`, `CITY-07` and `CITY-08` are LOCAL and MUST NOT begin before their stated Unity/bridge/catalogue prerequisites have passed. The H1 plan makes that interlock explicit: `H1-08 -> CITY-04` is a non-blocking greybox side edge, while `H1-GATE -> CITY-07` protects keeper realization; CITY-08 remains its own later authoring-efficiency owner.

`WP-CITY-00`, `WP-CITY-01` and `WP-CITY-02` are COMPLETE. CITY Programme v2 is ACCEPTED on candidate `87a902584f2c46b2d256f6fef26829e9182e7605` (review `#5261734418`, PR `#70`, merge `058e2f4f7c5b018d60cce84e9c89bd07249dd36a`). `WP-CITY-01` passed on candidate `3444555a983415645dcc2897b748d6c4f6294f19` (review `#5263809993`, PR `#73`, merge `a9ff655e5bf2319690d919b88bd57389a32483f3`). `WP-CITY-02` passed on candidate `ddcb22d9a2dfcf68054db2b762fc7ff7aed68ae8` (review `#5266113192`, PR `#81`, merge `1bdb7b6e914692493b17a9d2215d881dfe326cb0`). The next CITY workpack is `WP-CITY-05 — Streets, parcels + reusable building families`.

`PA/` is the non-foundational Living World research/adoption track. Canonical research meanings remain in `Docs/research/living-world/PA_ROADMAP.md`; the executable Worker → Reviewer contracts live in `Docs/workpacks/PA/`.

The planned PA spine is:

```text
PA-01 -> PA-02 -> PA-03 -> PA-04 -> PA-05
     -> PA-06 -> PA-07 -> PA-08 -> PA-09
     -> PA-10 -> PA-11 -> PA-12 -> PA-13 -> PA-14
```

`WP-PA-01..05` are `REMOTE_HARVEST`: they adopt/revalidate independently reviewed `Arkus0/Juego` research without importing donor runtime authority. `WP-PA-06..13` are `REMOTE_RESEARCH`. `WP-PA-14` is deliberately deferred until PA-01..13 have passed **and the future H2 playable-shell phase has reached its accepted closure/gate**; the future H2 plan owns that gate's exact ID/name.

PA research can inform later H2+ design but cannot claim runtime/Unity proof or silently alter H0/H1/CITY contracts. After the PA programme plan itself receives independent PASS + merge + DocSync, its next executable workpack is `WP-PA-01 — Adopt/revalidate NPC Daily Life findings`.
