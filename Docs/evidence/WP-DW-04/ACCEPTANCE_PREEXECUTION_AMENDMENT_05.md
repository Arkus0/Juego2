# DW-04 acceptance pre-execution amendment 05

Status: `PRE_CALL / NO_PROVIDER_CALLS / NO_CAMPAIGN_CLAIM`.

Arming attempt run `35853042419` on candidate `780d2f3ce8666726b3f337bd14f95dabc6fb4a5b` failed in `Validate frozen acceptance request` with shell exit `141` before provider credential validation, before the durable `DW-04 acceptance campaign start` claim, and before any model/provider request.

Causal defect: the acceptance workflow resolved the immutable assembly commit with `git log ... | head -1` while `set -o pipefail` was active. `head` closed the pipe after the first line and `git log` exited on SIGPIPE, producing status 141. This is workflow plumbing only; no semantic execution slot was consumed and acceptance provider/model usage remains `0/36`.

Repair boundary:

- replace only the fragile assembly-commit lookup with `git log -1 --format=%H --diff-filter=A -- Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json`;
- repin the acceptance workflow blob in `ACCEPTANCE_PROTOCOL.json`;
- issue a replacement pre-call freeze and rematerialize the deterministic context assembly against that freeze;
- preserve the six selected tasks, source oracles, expected answers, CTX/DW context semantics, Luna/OpenAI effective configuration, canonical response schema, R1/R2/R3 order, 18 matched pairs / 36 slots, all-36 correctness, >=30% saving threshold, and terminal/no-replacement invalid policy;
- do not count run `35853042419` as an acceptance campaign or semantic rerun because it never passed the pre-call validation barrier and created no campaign claim or provider request.

No Reviewer, DocSync, merge, or DW-05 action is authorized by this amendment.
