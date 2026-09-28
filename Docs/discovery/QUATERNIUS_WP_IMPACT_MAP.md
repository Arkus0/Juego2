# Quaternius adaptation audit — workpack impact map

Status: **DISCOVERY / PROPOSED PROCESS INPUT**  
Date: 2026-09-28  
Companion research: `Docs/discovery/QUATERNIUS_ADAPTATION_ECOSYSTEM_AUDIT.md`  
Candidate process amendment: `Docs/product/QUATERNIUS_ADAPTATION_REUSE_PRECHECK_AMENDMENT.md`

## Purpose

Record exactly which existing Juego2 workpacks are materially affected by the Quaternius adaptation/tooling research, which ones are only downstream consumers, and which ones must **not** be reopened or expanded because of it.

The audit changes the default implementation approach for future ART industrialization:

`Quaternius/native capability -> audited existing tooling/technique -> bounded adaptation/spike -> custom tooling only for the remaining gap`

It does not make external tooling canonical by itself.

## Directly affected workpacks

| WP | Impact | Required interpretation if the companion amendment is accepted |
|---|---|---|
| `WP-ART-01` | DIRECT / MEDIUM | Recovery/adaptation work must consult the audit before declaring a tooling/source gap. ART-01 may use discovered techniques as diagnostic/reuse input, but must **not** become the final procedural factory. Its existing H2F sequencing, third-person quality oracle and `DIRECT/ADAPT/DONOR/CREATE_DERIVED` rules remain intact. |
| `WP-ART-CHAR-01` | DIRECT / VERY_HIGH | Before inventing a character/wardrobe pipeline, evaluate Quaternius-native modularity plus the audited character/garment pipelines. Record what is reused, adapted, reference-only or rejected. Exact dependency/IP review remains mandatory before code/tool adoption. |
| `WP-ART-ANIM-01` | DIRECT / HIGH | Evaluate UAL/shared-rig advantages and audited Quaternius import/packaging helpers before building new retarget/import automation. The WP still owns real coverage/retarget/import evidence on accepted characters. |
| `WP-ART-ENV-02` | DIRECT / VERY_HIGH | Primary owner of environment-factory consequences. Begin with semantic donor catalog + external-tooling precheck + bounded spikes before writing a new generator/export/material pipeline. Any selected helper must remain behind Juego2-owned grammar/provenance/quality contracts. |
| `WP-ART-URBAN-01` | DIRECT CONSUMER / MEDIUM | Must consume the factory selected/proved by `ART-ENV-02`; may extend it with justified port-specific families but must not create a second unrelated building/adaptation pipeline. |
| `WP-ART-03` | DIRECT GATE / MEDIUM | Final integrated lock must prove the chosen factories actually reduce repeat-work and still produce coherent keeper-quality output. Tool success alone is not acceptance. |

## Downstream affected, normally no immediate contract rewrite

| WP | Impact | Reason |
|---|---|---|
| `WP-ART-CHAR-02` | INDIRECT / HIGH | Executes the character factory defined by `ART-CHAR-01`; benefits from tooling choices but should not reopen source/pipeline selection casually. |
| `WP-ART-ANIM-02` | INDIRECT / HIGH | Consumes the animation baseline and mappings defined by `ART-ANIM-01`; should not independently invent another retarget lane. |
| `WP-CITY-URBAN-00` | INDIRECT / LOW | Supplies briefs/demand. It should not own Quaternius tooling selection. |
| `WP-CITY-URBAN-01` | INDIRECT / MEDIUM | Consumes keeper visual production; no direct dependency on third-party DCC tooling should leak into CITY semantics. |
| `WP-H2-01`, `WP-H2-02`, `WP-H2-03`, `WP-ART-02` | INDIRECT | Benefit from better production output, but their accepted system/product claims should not be expanded merely because tooling exists. |

## Explicitly **not** reopened by this audit

### `WP-H2F-02`

**No scope change / do not stop.**

`WP-H2F-02` owns exact Unity/URP/toolchain/dependency adoption and reproducibility. The Quaternius audit sits above that foundation. External Blender/Quaternius helpers may later need compatibility proof against the accepted H2F baseline, but they do not replace H2F lifecycle/materialize/reconcile/clean-rebuild evidence.

A future ART spike may discover one small import helper worth adopting; if so, the owning WP must follow normal dependency policy and the H2F integration boundary. That is not a reason to restart or delay the current H2F-02 candidate.

### `WP-H2F-03` / `WP-H2F-GATE`

No new product claim. They continue proving the foundation. Later ART-produced assets/tools must operate on that accepted baseline.

### CITY topology WPs / `CITY-07`

No direct tooling authority. The audit improves how assets are produced, not accepted CITY topology, access, place semantics or route geometry.

### GC2 workpacks

Separate concern. GC2 Hub reuse is being audited independently. Do not mix Game Creator extension selection with Quaternius visual-asset tooling except at normal prefab/content handoff boundaries.

## Recommended ownership split

```text
H2F foundation
      |
      +--> ART-01: prove source/adapt/donor viability + keeper quality
      |
      +--> ART-CHAR-01: select/define character + wardrobe production recipe
      |       `--> ART-CHAR-02: prove repeatable population output
      |
      +--> ART-ANIM-01: select/define animation intake + coverage baseline
      |       `--> ART-ANIM-02: prove reusable runtime vocabulary
      |
      `--> ART-ENV-02: select/define environment factory
              `--> ART-URBAN-01: extend same factory to first port-town block
                        `--> ART-03: integrated repeatability + look lock
```

## Minimum evidence expected from directly affected WPs

Where a tool/technique from the audit is materially relevant, the owning WP should record:

1. native/current path considered;
2. audited external candidate(s) considered;
3. `USE / ADAPT / REFERENCE / SPIKE / WATCH / REJECT` decision;
4. exact causal reason;
5. dependency/license boundary if adoption is proposed;
6. bounded benchmark against the custom/manual alternative;
7. replacement/fallback path;
8. confirmation that external tooling does not become visual/semantic authority.

This is a **reuse precheck**, not a requirement to install more software.
