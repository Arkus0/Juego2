# CTX-03 — Final Circuit-Breaker Audit

Status: **CANDIDATE EVIDENCE / NOT FINAL CLEAN RECORD**  
Trigger: independent FAIL review `#5276314835` on candidate `9e71a220b79de79e3fbfe7bedd505256932a26f0`  
Scope: B1/B2 defect classes plus requested future-extension, omission, lifecycle and semantic-authority challenge.

This record is repository evidence produced **before** the final exact-SHA Worker pre-review. It is intentionally not `WORKER_PRE_REVIEW: CLEAN`; that terminal record must be external GitHub evidence created only after every repository/evidence byte is final.

## Defect classes closed

### B1 — derived current state cannot define its own oracle

The failed checker hardcoded the pre-adoption snapshot (`CTX-01/02 accepted`, `CTX-03 next`). The repair now derives CTX authority independently from numeric `Docs/workpacks/CTX/WP-CTX-*.md` contracts plus required PASS/PR-bearing DocSync closures. Only after deriving that authority does it compare `ACCEPTED_STATE_INDEX.json` and current-state prose.

Properties challenged:

- `ACCEPTED_STATE_INDEX` cannot select the accepted universe being checked;
- history prose cannot redefine expected current state;
- accepted CTX contracts must form a prefix;
- every COMPLETE CTX contract requires a matching persisted/complete DocSync closure with independent PASS and implementation-PR provenance;
- next CTX contract is derived from the first numeric non-COMPLETE contract, or `null` when none remains;
- numeric ordering is explicit, so a future `WP-CTX-10` cannot sort before `WP-CTX-04` merely by filename ordering.

### B2 — every repository-backed dynamic mandatory source is bounded

The failed design bounded base/fixed-conditional/representative-route context but exempted route-dependent repository inputs. The repair separates external payloads from repository-backed dynamic context and adds:

- checker-owned classification of every dynamic placeholder in canonical role profiles;
- fail-closed behavior for a new unreviewed placeholder class;
- per-resolved-repository-source ceiling plus aggregate dynamic-route ceiling;
- concrete resolver for exact WP/contract, repository-vs-external Worker evidence, dependency evidence and manifest-named repository files;
- repository-wide CI discovery of all present/future `Docs/workpacks/**/WP-*.md` contracts without using `CANONICAL_ROUTE_CONFIGS`;
- dependency/required-input discovery where the workpack contract makes those sources mandatory;
- anchored local manifest discovery with named repository files included automatically;
- explicit ceiling-evolution contract requiring policy revision + justification.

The global discovery oracle does **not** treat every path mentioned anywhere in prose as mandatory. That first attempt produced causal false-reds on future output paths, donor-only references and historical evidence mentions. The final design distinguishes mandatory routing authority from incidental/output mentions while retaining fail-closed concrete-route resolution.

## Additional variants found by the Worker before CLEAN

The requested circuit-breaker found and repaired variants beyond the two Reviewer examples:

1. **B1 future numeric ordering** — discovery originally accumulated lexicographically sorted filenames. A two-digit CTX extension could eventually make ordering depend on naming rather than numeric sequence. Contracts are now explicitly sorted by numeric CTX id.
2. **B2 future-role coupling** — exact-contract re-discovery initially selected the contract slot from a closed mapping of current role names. A new role using an already-reviewed `<EXACT_WP>` slot could count the WP itself but miss contract-named mandatory repository sources. Resolution is now slot-semantic and role-name agnostic.
3. **B2 global-discovery false-red** — the first repository-wide audit treated every path mention in every WP as mandatory. That incorrectly classified future output paths, donor-repository inputs and historical references as current repository context requirements. The CI oracle was split so exact WPs always count, dependencies/required-source surfaces count by contract, manifest authority remains strict, and incidental prose does not acquire semantic authority.
4. **B2 duplicate-oracle drift** — an intermediate `ctx03-dynamic-universe-check.py` remained beside the production dynamic checker with slightly different discovery semantics. Even though it was not the CI gate, retaining two mutable completeness implementations would create a future self-confirmation/drift surface. The duplicate was removed; `ctx03-dynamic-context-check.py` is the single dynamic-universe/budget oracle exercised by CI and the final circuit-breaker.

No variant above was deferred to the Reviewer.

## B1 causal controls

`scripts/ctx03-final-circuit-breaker.py` drives the production current-state oracle against synthetic repository states:

1. pre-CTX-03 accepted state -> GREEN;
2. CTX-03 changed to COMPLETE + valid PASS/PR-bearing CTX-03 DocSync + derived index updated -> GREEN **without changing checker/oracle code**;
3. authority says CTX-03 accepted while index remains pre-CTX-03 -> RED;
4. wrong `next_contract_hint` -> RED;
5. remove the entire accepted CTX-03 DocSync closure -> RED;
6. add fictitious accepted WP only to derived index -> RED;
7. modify history prose only -> derived current state unchanged;
8. remove the whole CTX projection surface / replace accepted list by `[]` -> RED while authoritative discovery remains intact;
9. introduce future CTX-04 and CTX-10 -> next state remains numerically derived.

The defect cannot self-heal by editing the same index that is under audit.

## B2 causal controls

The dynamic controls exercise a synthetic future track not belonging to H1/CITY/PA:

1. future exact WP automatically enters the budget;
2. growing that WP past its ceiling -> RED;
3. add a new repository evidence source made mandatory by the exact route contract -> automatically counted;
4. grow the new source past the limit -> RED;
5. omit that source from caller-supplied route bindings while route authority still requires it -> still counted;
6. remove the entire dynamic binding surface -> RED;
7. grow a wholly non-mandatory repository file -> unchanged/GREEN;
8. `repair_worker`: repository-backed original Worker evidence is counted while a live Reviewer FAIL may remain explicitly external;
9. `h1_local_executor`: anchored repository manifest and manifest-named repository file are counted without a caller-maintained named-file list;
10. future role profile using an existing reviewed exact-WP slot inherits the resolver;
11. new dynamic slot class -> RED pending explicit oracle review.

The six H1/CITY/PA routes remain calibration/quality cases only; they no longer define the future dynamic universe.

## Whole structured-surface omission

The final circuit-breaker additionally drives production oracles with:

- `CONTEXT_ESCALATIONS.evaluations` field removed;
- `evaluations = []`;
- `evaluations = {}`;
- representative `routes = []`;
- representative `routes` field removed;
- entire conditional-profile budget collection emptied;
- entire CTX current-state projection removed/emptied.

Each case must RED because the real required universe is independently reconstructed, not because the fixture asserts that it deleted a field.

The accepted CTX-02 Context Capsule Validation remains binding and is rerun on the exact candidate. Its independent omission/semantic controls continue covering whole-surface/capsule/PA disposition and authoritative-source reconstruction classes; CTX-03 does not replace those accepted oracles.

## Semantic substitution challenge

No new compact surface gains semantic authority:

- current-state index remains `DERIVED_NAVIGATION_ONLY`;
- history is reconstruction/provenance only;
- context envelope/budgets are process gates only;
- capsules remain navigation only under accepted CTX-02 rules;
- mechanical PASS cannot substitute for exact authoritative sources, predecessor reopen or fresh independent Reviewer reasoning;
- quality replay proves authoritative H1/CITY/PA sources remain reachable when escalation is material.

## Future-extension challenge

Synthetic additions cover:

- a new CTX accepted transition;
- a new role profile;
- the existing process-control case for a new fixed conditional source;
- a new dynamic mandatory repository slot;
- a future non-H1/CITY/PA workpack and repository evidence source.

Each extension either joins an independently discovered universe automatically or fails closed pending explicit reviewed oracle/budget evolution.

## Lifecycle/retry boundary retained

The repair does not alter the already-repaired exact-SHA lifecycle. The canonical exact candidate validation must continue to prove:

- external CLEAN pointer belongs to exact PR/exact SHA;
- post-CLEAN repository mutation invalidates readiness;
- wrong PR / wrong SHA CLEAN pointers are RED;
- missing Ready marker blocks;
- existing marker + red gate blocks;
- same-SHA gate repair can close by reusing the durable marker;
- HEAD movement after marker blocks.

Final terminal sequencing remains: finish repository bytes -> exact HEAD -> full validation + baseline diff + Worker challenge -> external durable CLEAN -> metadata only -> Ready/freeze -> exact-SHA gates/terminal closure -> STOP for fresh Reviewer.

## Adoption simulation requirement

The final candidate is not ready unless the post-adoption synthetic state described above is GREEN with unchanged checker/oracle code. That simulation is part of the mandatory CTX Process Envelope CI via `scripts/ctx03-final-circuit-breaker.py`.

## Current conclusion

The class-level repair is implemented, but this document itself is still a repository-byte mutation. Therefore all prior CLEAN/freeze evidence is invalid and no Reviewer should start from this intermediate state. A new exact-SHA complete Worker pre-review is required after the remaining evidence/protocol bytes are finalized.
