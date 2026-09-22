# Workpacks

Each workpack is one independently reviewable contract. Work only inside its allowed scope and stop when its Definition of Done is met.

For all `HK-*` workpacks through `WP-HK-GATE`, all H1 workpacks marked `FOUNDATIONAL`, and all DW workpacks:

- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` is binding;
- exact-SHA evidence is required;
- negative-conformance controls must be causal and reverted before freeze;
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

`OPS/`, `ART/`, `CITY/` and `PA/` are parallel non-foundational tracks. They are not bound by `FOUNDATIONAL_PROOF_STANDARD.md`, require no exact-SHA evidence or negative-conformance matrix, do not appear in the H0 DAG, and can neither block nor unblock any `HK-*` workpack.

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

`WP-CITY-00`, `WP-CITY-01`, `WP-CITY-02`, `WP-CITY-05`, `WP-CITY-06` and `WP-CITY-03` are COMPLETE. CITY Programme v2 is ACCEPTED on candidate `87a902584f2c46b2d256f6fef26829e9182e7605` (review `#5261734418`, PR `#70`, merge `058e2f4f7c5b018d60cce84e9c89bd07249dd36a`). `WP-CITY-01` passed on candidate `3444555a983415645dcc2897b748d6c4f6294f19` (review `#5263809993`, PR `#73`, merge `a9ff655e5bf2319690d919b88bd57389a32483f3`). `WP-CITY-02` passed on candidate `ddcb22d9a2dfcf68054db2b762fc7ff7aed68ae8` (review `#5266113192`, PR `#81`, merge `1bdb7b6e914692493b17a9d2215d881dfe326cb0`). `WP-CITY-05` passed on candidate `10d1528b0354b16a614fb10a3933a25b32f15f28` (review `#5267776704`, PR `#83`, merge `47909a72eb6d38332f62e9c01426c8cd40e1863b`). `WP-CITY-06` passed on candidate `c73cbb19f8b152bd4b97ce3154eb18a78deaec3c` (review `#5268249668`, PR `#90`, merge `acb84ec9ba7aa94e962bf7b35d7e53c0549ba758`). `WP-CITY-03` passed on candidate `420999e9a60e7240bcfb514b3239b652f0e09a5f` (review `#5268853516`, PR `#94`, merge `a3abeaf82752bfe9dac2f594c06eb8def3b108e2`). The next CITY workpack in the spine is `WP-CITY-04 — LOCAL retained-seed greybox + traversal validation`, but it remains blocked until `WP-H1-08` PASS.

`PA/` is the non-foundational Living World research/adoption track. Canonical research meanings remain in `Docs/research/living-world/PA_ROADMAP.md`; the executable Worker → Reviewer contracts live in `Docs/workpacks/PA/`.

The accepted PA spine is:

```text
PA-01 -> PA-02 -> PA-03 -> PA-04 -> PA-05
     -> PA-06 -> PA-07 -> PA-08 -> PA-09
     -> PA-10 -> PA-11 -> PA-12 -> PA-13 -> PA-14
```

The PA programme plan is **ACCEPTED** on candidate `95fc0f5935561fd61a20221f993e2532ce513795` (independent review `#5267699844`, PR `#84`, merge `4c1672db639f9d56e7bd6f837ff5e1881d789b16`). `WP-PA-01` is **COMPLETE** on candidate `87c4cbe81f8195770eb23ba7f2a5d5ca2a237715` (review `#5268013445`, PR `#89`, merge `3688b7b9a27355b0fda160c20e57a385e40c6814`). `WP-PA-02` is **COMPLETE** on candidate `015bb28ddc9facc46459c2d1dd87a89740b6c9ef` (review `#5270038879`, PR `#97`, merge `85d23489a6478da6bc9f33c3f017640e46ab05e9`). `WP-PA-03` is **COMPLETE** on candidate `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927` (review `#5270421825`, PR `#100`, merge `2f3862b601b0521a6a3d5a57afe54f182d037e97`). `WP-PA-04` is **COMPLETE** on candidate `5d38ea38b983cd5227f57afa1d880d24746f9249` (review `#5280879115`, PR `#129`, merge `d6041b719292f24c4481dea28727e2cfd5f7ed5b`).

`WP-PA-01..05` are `REMOTE_HARVEST`: they are cheap Juego→Juego2 adoption/revalidation gates for already independently reviewed donor research, **not a restart of PA-01..05 research**. `WP-PA-06..13` are `REMOTE_RESEARCH`. `WP-PA-14` is deliberately deferred until PA-01..13 have passed **and the future H2 playable-shell phase has reached its accepted closure/gate**; the future H2 plan owns that gate's exact ID/name.

PA research can inform later H2+ design but cannot claim runtime/Unity proof or silently alter H0/H1/CITY contracts. The next executable PA workpack is `WP-PA-05 — Adopt/revalidate Rumours findings`; after PA-05 is accepted in Juego2, the first genuinely new research workpack is `WP-PA-06 — Memory & Consequences research`.

## Process-efficiency track

`CTX/` is a **PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL** context-efficiency track. Its accepted programme plan changes no product/runtime semantics and exists to improve context precision first, then token/local-agent cost, while preserving all accepted proof, predecessor, exact-SHA and independent-review obligations.

The CTX programme plan is **ACCEPTED** on frozen candidate `c9ff3605048e05fded4d58a1de27450661d9fe0e` (independent review `#5270686380`, PR `#102`, merge `f3c8362b3d76fd4f78107d8142e07e476985f973`). Its reviewed sequence is:

```text
CTX-01 -> CTX-02 -> CTX-03
```

`WP-CTX-01 — Role-specific bootstrap + accepted-state navigation` is **COMPLETE** on repaired frozen candidate `ea92e4eab36566ab3d0367fef64fefc0b2b0ff39` (independent PASS review `#5273801466`, PR `#110`, implementation merge `fbd3e5526e760efc89f54e7c12a274af10d4765f`). `WP-CTX-02 — Accepted-contract capsules + predecessor inheritance compression` is **COMPLETE** on final circuit-breaker candidate `e5053b778e050cff83e2443fef888c64883c88ca` (independent PASS review `#5274937744`, PR `#113`, implementation merge `af63528b63ba9b3ddf2e612c0ad8dff96a57a6c2`). The accepted capsule layer remains non-authoritative navigation and fails closed toward authoritative reconstruction when its bounded guarantees cannot be trusted.

`WP-CTX-03 — Structured evidence, quality-preserving context envelope, DocSync and history separation` is **COMPLETE** on final circuit-breaker candidate `8851f3a295c848be5724d5d9b796e00d2037d6e8` (independent PASS review `#5277502546`, PR `#121`, implementation merge `d2cf7145b4ec83c3935286f2d9faeabbcd032148`). The reviewed CTX-01 → CTX-02 → CTX-03 programme is complete; there is no next CTX workpack. CTX remains the owner of process-context/bootstrap/capsule policy and does not semantically block H1/CITY/PA. After the CTX-03 DocSync, the CTX-03 side prerequisite referenced by `WP-DW-04` is accepted, while DW retains its own serial and PA prerequisites.

## Foundational second-consumer track

`DW/` is an **ACCEPTED FOUNDATIONAL VALIDATION** track. Its architecture is `Docs/engineering/DW_DESIGN_WORLD_ARCHITECTURE.md`; all seven DW implementation/gate workpacks are governed by `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` v1.5 or later accepted successor.

The DW programme plan is **ACCEPTED** on frozen candidate `cbb0114bb9ee1248739c093109a9b71e26751444` (independent PASS review `#5274794434`, PR `#117`, merge `b831050e9df8b61b76744e0c5f544bd7ec2d79b5`). Its reviewed sequence is:

```text
DW-00 -> DW-01 -> DW-02 -> DW-03 -> DW-04 -> DW-05 -> DW-GATE
                               ^        ^
                               |        |
                         PA-01..05   CTX-03
                          accepted   accepted
```

DW does **not** invalidate CTX. CTX remains the owner of process-context/bootstrap/capsule policy and its full accepted sequence remains useful independently. DW adds a typed, provenance-preserving Design World projection over accepted CITY/PA facts. The measured structured-context comparison waits for accepted `CTX-03` so DW must beat the best accepted baseline rather than an obsolete one.

`WP-DW-00 — Authority-preserving Design World projection contract` is **COMPLETE** on frozen candidate `333013b80ba1422b5dfd0b4b5c590cd9b314e007` (independent PASS review `#5280464264`, PR `#126`, implementation merge `2da4b6fd4a6eb7bac166c77ba8fe05796874cbbf`, final frozen exact-SHA validation `35749413198` GREEN). `WP-DW-01 — first CITY semantic/invariant consumer with causal omission detection` is now the next default dependency-valid DW workpack and remains `NOT_STARTED` until a human explicitly starts its Worker. Later DW workpacks require their stated serial/cross-track predecessors. H1 continues independently in parallel. Final H2 external/public-boundary acceptance must explicitly consume accepted `DW-GATE` evidence or review/disposition the accepted DW interlock, while H1-GATE remains the independent Unity/gameplay-readiness owner.

The track is intentionally limited to two real consumers (CITY and PA), one structured-context trial and one generic-boundary stress/closure step. Design↔Unity drift, generated art briefs, source-catalogue coverage, replay QA, narrative knowledge validation and non-game product experiments remain downstream candidates, not current DW implementation scope.