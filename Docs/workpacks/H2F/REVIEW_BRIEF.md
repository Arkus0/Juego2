# H2F planning review brief

Review target: the proposed `H2F — Pre-H2 product/toolchain foundation freeze` phase and its binding insertion before retained keeper production.

## Causal problem being solved

The accepted H2/CITY/ART direction now requires real keeper worldbuilding, URP-quality visual presentation, third-person traversal and representative characters. Those WPs would otherwise be allowed to discover or replace rendering, road/spline, terrain/vegetation, camera/input/navigation, animation/retarget and other material production tooling while keeper content is already being authored. That creates avoidable rework and dependency drift.

H2F solves only that boundary. It does not reopen H0/H1, does not redesign CITY, and does not replace ART authority.

## Proposed order

```text
H1-GATE + ART-01 KEEPER_READY
        ↓
H2F-00 survey
        ↓
H2F-01 decision spikes
        ↓
H2F-02 exact adoption + URP/toolchain bootstrap
        ↓
H2F-03 integrated non-keeper compatibility fixture
        ↓
H2F-GATE
        ↓
CITY-07 keeper realization
        ↓
H2 implementation / ART-02 / H2-GATE
```

H2F-00 may overlap late ART-01 as research-only work; no adoption or keeper claim may bypass the declared predecessor.

## Review questions

1. Does the capability survey close the relevant decision space without requiring an impossible enumeration of the entire Asset Store?
2. Do H2F-01 spikes resolve only expensive-to-reverse uncertainty instead of installing everything?
3. Does H2F-02 preserve exact-version/IP/provenance/reproducibility and H0/H1 authority boundaries?
4. Does H2F-03 prove the selected stack together on a deliberately non-keeper fixture rather than smuggling CITY-07 completion into the phase?
5. Does H2F-GATE freeze a useful production baseline without banning later evidence-driven amendments?
6. Are the downstream prerequisites reachable from canonical `WP-CITY-07`, `WP-H2-01`, `WP-H2-02` and `WP-H2-GATE`?
7. Is any H2/CITY/ART authority accidentally transferred to a Unity/plugin dependency?

## Expected review scope

A blocker should identify a concrete false PASS, contradiction of accepted predecessor authority, missing contractual reachability, or an oracle unable to prove its stated claim. Broader tool recommendations belong in H2F-00 discovery unless their omission makes the plan structurally incapable of closing a required capability class.
