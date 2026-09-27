# WP-HK-05 — Reopen 1: recoverable operation-grammar diagnostics

Status: **CORRECTION / OWNER-WAIVED INDEPENDENT REVIEW (owner instruction)**

## Trigger

This reopen was triggered by WP-H1-GATE fresh independent public-client AI-agent trial 2. The trial ran on PR `#238`, candidate `facfe0ebd3678f18ce35205c24d28714b6ae15a8`, run `36242209280`, and FAILED.

- It ran after WP-H1-04 reopen 1 (`#239`), and the catalogue recovery now worked.
- The agent then authored a `put-object` that also carried extension fields: `owner`, `schemaVersion`, `subjectId`, `dependencies` and `payloadBase64`.
- It received `world.change.invalid_request` "put-object contains fields outside its declared grammar." six times, with an empty `context`.
- The hint was "Use only the typed mutation envelope declared by system.describe.", but `system.describe` and MCP `tools/list` publish the operation item as one flattened union of the four kinds, with only `kind` required.
- The agent stopped with no canonical mutation.

Owner authority (2026-09-26):

- The owner instructed the WP-H1-GATE Worker to "do whatever is needed until the Gate is finished". The owner also waived independent review for the predecessor correction ("La corrección no necesita reviewer").
- This reopen applies that instruction to the second causal owner exposed by the same trial. It is recorded as such, and the owner may require a retroactive independent review.
- The WP-H1-GATE candidate itself still requires its own fresh independent Reviewer.

## PREDECESSOR_CONTRACT_CHECK

This reopen corrects WP-HK-05 itself. The direct accepted dependency is `WP-HK-02A`, which is COMPLETE.

- The correction consumes only these accepted guarantees:
  - the HK-02A/HK-04 canonical world state and the transactional mutation envelope;
  - the H0 `StructuredError` contract (`machineCode`, `message`, `path`, open `context`, `retryable`, `repairHint`);
  - `PortableData` for transport-neutral context values.
- None of them changes. The per-kind field sets stay exactly the accepted HK-04 grammar.
- Reopen condition checked: concrete effective evidence (trial 2) shows the HK-05 "repairable diagnostics" guarantee does not reach a public client for grammar violations. No HK-02A/HK-04 guarantee is contradicted, so none of them reopens.

## Accepted guarantee proved inapplicable

HK-05 guarantees "validation + repairable diagnostics". The four per-kind grammar rejections in `WorldMutationService.ParseOperation` were not repairable by a client that has only the published flattened schema. They did not name the kind's allowed fields, its required fields or the offending fields.

## Correction

A grammar violation for `put-object`, `remove-object`, `put-extension` or `remove-extension` now returns the following. The machine code, message, path and `retryable=false` are all unchanged.

- `context.operationKind`
- `context.allowedFields`, in declared order
- `context.requiredFields`
- `context.unexpectedFields`, in ordinal order
- A hint: "Remove unexpectedFields; each operation kind accepts only its own allowedFields. Object data (id, typeId, containerId, references) and extension data (owner, schemaVersion, subjectId, dependencies, payloadBase64) are separate put-object and put-extension operations in the same transaction."

## Not changed

- The grammar is unchanged: the same field sets are accepted and rejected.
- Machine codes, paths, capability schemas and the published operation schema are unchanged. `SchemaNode` still cannot express per-kind `oneOf`, and that remains a named future H0 schema-expressiveness decision.
- Canonical state, hashing, journal and validation semantics are unchanged.
- An unknown `kind` is still rejected earlier by the published schema enum (`contract.invalid_request`), so it is not affected.

## Exact-SHA verifier maintenance (pre-existing breakage)

`scripts/hk05-observe-exact-sha.sh` and `scripts/hk05-verify-exact-sha.sh` require a completely clean tree. Arkus Candidate Validation now writes `VALIDATION_CONTEXT.json` and `validation.log` before calling the verifier, so the HK-05 route could not pass in CI whatever the candidate contained.

Both scripts now tolerate exactly those receipt files, with the same pattern as the newer exact-SHA verifiers, for example `h1-04-verify-exact-sha.sh`. Any other untracked or modified file still fails.

## Proof

- `Hk05ValidationDiagnosticsTests.OperationGrammarViolationsNameTheAllowedAndUnexpectedFieldsForRecovery` covers the trial's exact mixed `put-object`:
  - on `authoring.change.plan` and `authoring.change.apply`, the rejection carries the exact code, path, allowed, required and unexpected fields, and the hint;
  - the error is `PortableData`-valid and valid against the canonical `StructuredError` schema;
  - the same intent, split into a declared `put-object`, is accepted;
  - the canonical state is unchanged.
- Local `Arkus.Harness.Tests` passes 451/451. Exact-SHA hosted evidence is recorded on the PR.
- The effective public proof is the WP-H1-GATE rerun and a new fresh-agent trial on the Gate's final frozen SHA.
