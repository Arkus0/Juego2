# H2F ↔ ART-01 sequencing decision — 2026-09-27

Status: **OWNER-DIRECTED / PROPOSED FOR MERGE**

## Decision

Run the H2 foundation track to `WP-H2F-GATE` before ART-01 freezes its effective Unity candidate or claims PASS.

The explicit order is:

```text
ART-01 PREFOUNDATION_INPUT
-> H2F-00
-> H2F-01
-> H2F-02
-> H2F-03
-> H2F-GATE
-> ART-01 effective CANDIDATE / PASS
-> CITY-07 keeper realization
```

## Reason

The previous contracts formed a causal cycle: H2F-01/H2F-02 consumed ART-01 PASS, while ART-01's final visual benchmark needs the production render/toolchain baseline that H2F-02/GATE are responsible for selecting, adopting and freezing.

The cycle is resolved by splitting ART-01 execution into a non-PASS, renderer-independent/source-facing `PREFOUNDATION_INPUT` handoff and a later effective Unity candidate/PASS phase on the H2F-GATE baseline.

## Authority boundary

- ART-01 owns visual/source demand, assembly/dimensional grammar, keeper-readiness and the no-silent-proxy oracle.
- H2F owns foundation selection, exact dependency adoption, URP/toolchain migration, lifecycle compatibility and foundation freeze.
- H2F fixtures remain non-keeper and cannot satisfy ART-01 readiness.
- ART-01 must not independently migrate the canonical render/toolchain baseline to make its own benchmark pass.
