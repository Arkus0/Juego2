# DW-04 strict Worker pre-review — final repaired candidate

WORKER_PRE_REVIEW: CLEAN
FOUNDATIONAL_PROOF_VERDICT: PASS_CANDIDATE
CANDIDATE_HEAD_SHA: bf1614dd674259d82e96c1360220cce93b89320a
CANDIDATE_CAMPAIGN_SHA: 25272a8f9ce1631dbe3b9586c3af7dc12f02b3b1
REPAIRED_FREEZE: 1032343542f09d969a3eabe81734fe53b5cc3ec2
CANONICAL_CAMPAIGN_RUN: 35861322561
CANONICAL_CAMPAIGN_ARTIFACT: 10750049310
CANONICAL_CAMPAIGN_ARTIFACT_DIGEST: sha256:60878580a4d77fb7781ae29bb9a3eda96a5cfe3a63a6f3f2958f572e70594ad1

## Campaign history and non-adaptation

Campaign 01 remains immutable terminal FAIL evidence (`196012fb809bca68448ef0ad6d468115acb36ac5`, run `35853858468`, artifact `10746802865`). None of its 36 answers was rescored into a PASS, carried forward, selectively rerun or counted in the repaired generation.

The repaired generation executed all 36 acceptance slots afresh under the same six task identities, accepted source truth/oracles, semantic questions, CTX baseline semantics, Luna/OpenAI effective provider/model configuration, R1/R2/R3 ordering, 100% correctness rule, >=30% median source-byte reduction threshold, no-replacement policy and fail-closed CTX-pass/DW-fail decision rule.

## Causal repair classes

- PA disposition retrieval separates the canonical uppercase `Disposition` token from explanatory `Qualification`, closing the repeated Campaign-01 A-PA-03 `REJECT as requirement/authority` failure class without changing accepted PA truth.
- Previously unconstrained result fields use structural domains (`access_code`, `pa_id`, `disposition`) so canonical identifiers cannot be replaced by prose while expected task values remain outside the adapter.
- Required frozen evidence remains mandatory. Additional evidence can no longer fail solely by cardinality when it comes from the already-frozen authoritative vocabulary. Facts, causal blockers and verdict remain exact.
- Calibration requests are validated against the frozen calibration run policy; acceptance requests remain validated against their frozen no-replacement run policy. Effective model/provider dimensions remain equal across the comparison.

## Final campaign evidence

- Fresh provider/model executions: 36/36.
- Unique provider request IDs: 36/36.
- Resolved provider: OpenAI for every execution.
- Replacement calls: 0.
- Deterministic final audit: `PASS`.
- Matched pairs: 18/18 `CTX-pass + DW-pass`; therefore all 36 individual executions satisfy the frozen oracle.
- Structural completeness: GREEN.
- Median injected source bytes: CTX `2258.5`, DW `1380.5`.
- Contractual median source-byte reduction: `38.875%` (threshold `>=30%`).
- Provider diagnostics: median prompt tokens CTX `1500`, DW `1425` (~5.0% reduction); total prompt tokens CTX `29019`, DW `26541` (~8.54% reduction).
- Observed campaign cost is effectively flat/slightly higher for DW, therefore no cost-saving claim is made.

Canonical campaign outputs are persisted at:

- `Docs/evidence/WP-DW-04/ACCEPTANCE_TRANSCRIPT.jsonl`
- `Docs/evidence/WP-DW-04/ACCEPTANCE_PROVIDER_RAW.jsonl`
- `Docs/evidence/WP-DW-04/CAMPAIGN_START_RECEIPT.json`
- `Docs/evidence/WP-DW-04/CAMPAIGN_RECEIPT.json`
- `Docs/evidence/WP-DW-04/TRIAL_RESULT.json`

## Post-review evidence-integrity repair

The Reviewer identified one remaining false-green class: the frozen verifier replayed `ACCEPTANCE_TRANSCRIPT.jsonl` but did not prove that the scored transcript was the transcript named by `CAMPAIGN_RECEIPT.json` or that each scored response was the compact projection of the corresponding immutable provider record.

This is closed without changing or rerunning the acceptance campaign:

- `scripts/dw04-evidence-integrity.py` hashes the exact transcript bytes and requires equality with `CAMPAIGN_RECEIPT.json.transcript_sha256`.
- It requires exactly 36 scored records and 36 raw provider records, identical slot ordering, 36 unique provider request IDs and exact equality with the receipt's ordered request-ID inventory.
- For every slot it reconstructs the executor's durable compact response projection from `ACCEPTANCE_PROVIDER_RAW.jsonl` and requires exact equality with the response scored in `ACCEPTANCE_TRANSCRIPT.jsonl`.
- A causal negative control mutates only one scored verdict while leaving provider raw evidence and receipt untouched and requires that comparison to go RED.
- `scripts/arkus-verify-exact-sha.sh` executes this integrity checker before the frozen DW-04 verifier, so normal exact-SHA validation cannot declare DW-04 GREEN without proving the provider-evidence binding.

No campaign answer, oracle, scorer, route, receipt, raw provider record or trial result was modified by this repair.

## Worker conclusion

The bounded central claim of WP-DW-04 is established on the frozen six-task universe: the repaired DW route preserves agent correctness relative to CTX for every designated paired execution while exceeding the contractual source-context reduction threshold. The post-review integrity repair additionally binds that scored PASS causally to the immutable provider evidence. This does not claim universal model superiority or universal cost reduction.

Candidate is CLEAN and ready for independent Reviewer. Do not merge or unlock DW-05 without Reviewer PASS.
