# WP-H1-02 — remote closeout

REMOTE_CLOSEOUT: VERIFIED
LOCAL_ROUND_ACCEPTED: 4

## Exact chain

- EXECUTION_BASE_SHA: `e9e022f871ba6b7da95f663956b07d0519b99b2d`
- MANIFEST_COMMIT_SHA: `27602941ebc98dd2befa4dea7b2354242459d049`
- PRODUCT_RESULT_SHA: `ed7f0d00f4a6d38daee51c6661af50ff2a423cd4`
- EVIDENCE_COMMIT_SHA: `91e1e42bc02e95952a00af6db549b78a9e94975e`
- Durable handoff anchor: PR #152 comment `#5796605073`
- Durable result anchor: PR #152 comment `#5796728071`

Git history verifies each arrow directly:

```text
e9e022f871ba6b7da95f663956b07d0519b99b2d
  -> 27602941ebc98dd2befa4dea7b2354242459d049
  -> ed7f0d00f4a6d38daee51c6661af50ff2a423cd4
  -> 91e1e42bc02e95952a00af6db549b78a9e94975e
```

The manifest commit changes only `LOCAL_EXECUTION.md`. The product-result
commit contains exactly the 24 allowlisted Unity-generated/project/evidence
paths enumerated by `LOCAL_EXECUTION_RESULT.md` and excludes that summary. The
evidence commit is the direct child of the product result and changes only
`LOCAL_EXECUTION_RESULT.md`. Both external anchors were published only after
their referenced commit was verified as the canonical remote branch HEAD.

## Effective result reconciliation

- exact Unity Editor: `6000.3.24f1 (4e7b9b5b6244)`;
- Force Text and Visible Meta Files effective;
- built-in render pipeline;
- four resolved packages and five effective assemblies;
- Test Framework exactly `1.6.0` with exact local legal-file observation;
- EditMode: 5 total, 5 passed, 0 failed;
- clean second import equal on all six normalized owned fields;
- fixed Arkus batch entry equal on the same six fields;
- final repository checker GREEN and all six defect-injection controls causal
  RED;
- local mutation allowlist satisfied.

Round 1 and Round 2 wrapper failures and the Round 3 EditMode false green were
preserved in durable stop comments. None produced a candidate commit. Round 4
causally exercises the repaired exact-process wait and required-output
postcondition, so the earlier failed evidence is superseded rather than hidden.

No effective result contradicts the accepted H0 engine-neutral predecessor
boundary. The final candidate must descend from EVIDENCE_COMMIT_SHA and may not
change any proof-relevant Unity input without a new local round.
