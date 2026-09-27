# H2F before ART-01 — amendment summary

Owner-directed causal fix: remove the circular dependency where H2F-01/H2F-02 expected ART-01 PASS even though ART-01's final visual candidate needs the URP/toolchain baseline frozen by H2F.

New order:

`ART-01 PREFOUNDATION_INPUT -> H2F-00 -> H2F-01 -> H2F-02 -> H2F-03 -> H2F-GATE -> ART-01 effective CANDIDATE/PASS -> CITY-07`.

Files changed: ART-01, H2F-01, H2F-02, H2F-03, H2F-GATE, plus the owner decision/review note. No product/runtime code changes.
