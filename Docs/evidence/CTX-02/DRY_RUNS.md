# CTX-02 Representative Dry Runs

WP: `WP-CTX-02`  
Baseline: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Circuit-breaker lineage: `fail_cycle: 5`  
Final audit: `Docs/evidence/CTX-02/FINAL_CIRCUIT_BREAKER_AUDIT.md`

Purpose: demonstrate representative H1/CITY/PA predecessor reconstruction from compact capsules, then defect-inject the trust boundary so configuration, omission or semantic substitution cannot silently narrow what CTX-02 checks.

## 1. Representative consumer runs

### H1 — Worker H1-02

Start surface: exact H1-02 contract + validated `WP-HK-GATE` capsule + independently confirmed accepted identity + normal Worker protocol.

Recovered compactly: H0 authoring-readiness passed; H0 semantics remain engine-neutral; Unity bridge work is authorized but direct gameplay implementation is not; H1-02 owns the Unity editor/package/project/toolchain boundary.

Escalation remains mandatory for exact H0 proof/residual questions. The independent representative oracle pins the HK workpack, verdict, proof matrix and residual-risk source inventory, so deleting one of those navigation authorities cannot silently keep the representative surface GREEN.

Verdict: `CONTEXT_CLOSED` for initial ownership; exact proof questions `ESCALATE_HUMAN`.

### CITY — Worker CITY-04

The capsule remains boundary-only. It recovers that CITY-03 owns one accepted keeper seed and CITY-04 executes/measures rather than redesigning it.

`Docs/production/CITY_PRODUCT_SEED.md` remains exact and non-compressible. The checker derives `track=CITY` and `content_mode=boundary_summary` from `WP-CITY-03`; changing the capsule to `track=H1` while deleting the mandatory seed read is now a real `--audit-index` RED rather than a way to disable the CITY oracle.

Verdict: compact ownership navigation; geometry/construction stays on the exact product source.

### PA — Worker PA-04

The starting accepted-result family is PA-01 + PA-02 + PA-03. Structured dispositions preserve exact source statuses; PA-03 directionality remains asymmetric; PA-02 bounded discovery and PA-03 rejection of default global/N-hop discovery survive compression.

PA completeness is now joined through a fully checker-owned mechanical chain:

```text
COMPLETE canonical WP-PA-NN
  -> checker-derived canonical PA-NN.md
  -> canonical WP-PA-NN capsule path
  -> checker-owned disposition table selector
  -> exact source key/status equality
```

The index may document the glob/template but cannot choose them.

Verdict: materially smaller cumulative predecessor start, with any semantic uncertainty escalating to original accepted results.

## 2. Full validation surface

The canonical command surface is:

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-omission-controls.py
python3 scripts/context-capsule-pa-semantic-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

The protocol, DocSync skill and CI workflow must all contain the same complete set. `context-capsule-controls.py` verifies that wiring and checks any other skill that explicitly claims the “full CTX-02 validation surface”.

## 3. Historical defect controls retained

The current surface still requires RED for:

- accepted-PA whole `disposition_source` + `dispositions` omission;
- malformed/blank reopen or escalation elements;
- review id with independent verdict changed PASS→FAIL;
- same-ID/pointer/fingerprint guarantee and exclusion inversion;
- representative reopen/escalation inversion;
- one-of-many material guarantee/exclusion omission via the real independent semantic oracle;
- COMPLETE future PA workpack with result+capsule+index jointly absent;
- omitted/reclassified PA disposition;
- A→B/B→A collapse and changed-but-still-distinct representative direction;
- source bytes/fingerprints mismatch;
- predecessor live state `REOPENED` or exact-SHA mismatch;
- CITY seed substitution.

## 4. Cycle-5 selector/configuration defect injection

The independent control harness builds a temporary repository and invokes the production subprocess path:

```text
python3 scripts/context-capsule-check.py --audit-index --repo-root <temp-repo> --index Docs/engineering/context-capsules/index.json
```

Baseline temp repository: **GREEN**.

Then each material mutation is injected independently:

| Mutation | Actual outcome |
|---|---|
| keep COMPLETE `WP-PA-04`, omit result/capsule/index and narrow glob to exclude 04 | `RED_AUTOMATIC` — non-canonical selector |
| same paired omission with canonical glob restored | `RED_AUTOMATIC` — missing canonical PA-04 result |
| PA selector matches nothing | `RED_AUTOMATIC` |
| PA selector broadened to `*.md` | `RED_AUTOMATIC` |
| PA result template redirected to alternate tree | `RED_AUTOMATIC` |
| index protocol redirected | `RED_AUTOMATIC` |
| index entry points to alternate valid-looking capsule file | `RED_AUTOMATIC` |
| CLI points to alternate index file | `RED_AUTOMATIC` |
| identity source points to same-name workpack under alternate root | `RED_AUTOMATIC` |
| CITY capsule relabelled `H1` + mandatory seed deleted | `RED_AUTOMATIC` |
| CITY content mode changed | `RED_AUTOMATIC` |
| PA disposition selector points to second valid-looking table in same canonical result | `RED_AUTOMATIC` |
| PA disposition status column changed | `RED_AUTOMATIC` |

After restoring every mutation, baseline `--audit-index`: **GREEN**.

These are integration controls, not direct helper assertions: the real CLI decides the RED.

## 5. Semantic substitution / omission controls

Some materially wrong prose is intentionally structurally valid. It is therefore tested against independent bounded semantic oracles rather than pretending the production checker can understand arbitrary natural language.

- actual HK guarantee: same ID/pointer/fingerprints, inverted statement -> production shape/source GREEN, representative oracle `RED_SEMANTIC_ORACLE`;
- actual CITY exclusion: same ID/pointer/fingerprints, inverted statement -> `RED_SEMANTIC_ORACLE`;
- actual HK reopen condition changed to opposite non-empty rule -> `RED_SEMANTIC_ORACLE`;
- actual CITY escalation changed to opposite non-empty rule -> `RED_SEMANTIC_ORACLE`;
- actual PA-03 direction changed but still syntactically distinct -> `RED_SEMANTIC_ORACLE`;
- one HK material export removed while others remain -> production GREEN, omission oracle RED;
- one HK material exclusion removed while another remains -> production GREEN, omission oracle RED;
- HK proof-matrix source removed while remaining sources/fingerprints stay valid -> production GREEN, representative source-inventory oracle RED.

Future arbitrary prose equivalence or a coordinated capsule+test-fixture semantic rewrite is explicitly `ESCALATE_HUMAN / RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`; it is not represented as automatic GREEN.

## 6. Future PA fail-closed run

A future `WP-PA-NN` may be COMPLETE and authoritative even before CTX-02 knows which result table should be used as a deterministic disposition oracle. In that state:

```text
canonical result remains authoritative
capsule navigation coverage = NOT COMPLETE
result = RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES
```

A DocSync repair must add a reviewed checker-owned section/key/status selector before claiming capsule-chain completeness. The capsule cannot nominate its own selector.

## 7. Reviewer reopening remains independent

A capsule's historical accepted identity cannot prevent reopening. External accepted state `REOPENED` or exact-SHA mismatch fails closed when supplied; concrete contradictory evidence always routes the Reviewer to original accepted sources and normal predecessor-reopen rules.

A representative semantic GREEN proves only the bounded test fixture survived the tested defect classes. It never converts the capsule or CTX-02 test oracle into semantic authority.

## Conclusion

The class-level dry runs cover the complete current false-green family: self-defined universes, self-selected oracles, coordinated omission, alternate-source/path rebinding, shape-valid semantic substitution, PASS spoofing, track/mode-assisted hiding and workflow drift. The durable final matrix records `UNSAFE: 0`; unresolved natural-language proof remains explicit human escalation rather than silent success.
