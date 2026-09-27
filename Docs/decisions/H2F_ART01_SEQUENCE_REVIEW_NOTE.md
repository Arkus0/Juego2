# Review note — H2F before ART-01 final candidate

Review only the causal sequencing amendment introduced by this branch.

The intended invariant is:

`ART-01 PREFOUNDATION_INPUT -> H2F-00 -> H2F-01 -> H2F-02 -> H2F-03 -> H2F-GATE -> ART-01 effective CANDIDATE/PASS -> CITY-07`.

No H1 guarantee, CITY semantic, ART visual oracle or H2F lifecycle/performance oracle is weakened. The change only removes the circular ART-01 PASS dependency from H2F and makes H2F-GATE the foundation freeze that ART-01 must consume for its final Unity benchmark.
