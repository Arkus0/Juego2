# CTX-03 — Final Circuit-Breaker Audit

Status: **CANDIDATE EVIDENCE / NOT FINAL CLEAN RECORD**  
Trigger: independent FAIL review `#5276314835` on candidate `9e71a220b79de79e3fbfe7bedd505256932a26f0`  
Scope: B1/B2 defect classes plus future-extension, omission, lifecycle and semantic-authority challenge.

This repository record is produced before the terminal exact-SHA Worker pre-review. It is intentionally not `WORKER_PRE_REVIEW: CLEAN`; final CLEAN must be durable external GitHub evidence created after every repository/evidence byte is final.

## B1 — current state cannot define its own oracle

The failed checker hardcoded the pre-adoption snapshot. The repair derives CTX authority independently from numeric `Docs/workpacks/CTX/WP-CTX-*.md` contracts plus required PASS/PR-bearing DocSync closures, then audits `ACCEPTED_STATE_INDEX.json` and current-state prose only as projections.

Properties:

- the derived index cannot select the accepted universe;
- history prose cannot redefine expected state;
- accepted CTX contracts form a numeric COMPLETE prefix;
- every COMPLETE contract requires a matching persisted/complete DocSync closure with independent PASS + implementation-PR provenance;
- next is the first numeric non-COMPLETE contract, or `null` when none remains;
- numeric ordering handles future two-digit CTX ids.

### B1 causal controls

The production oracle is driven against synthetic states:

1. current pre-CTX-03 state -> GREEN;
2. CTX-03 COMPLETE + valid CTX-03 DocSync + updated projection -> GREEN **without checker/oracle edits**;
3. authority accepted while projection stays pre-CTX-03 -> RED;
4. wrong next hint -> RED;
5. remove entire accepted closure -> RED;
6. fictitious accepted WP only in projection -> RED;
7. history-only mutation -> expected state unchanged;
8. remove/empty entire CTX projection -> RED while authoritative accepted discovery remains intact;
9. add CTX-04 and CTX-10 -> next remains numerically correct.

## B2 — dynamic repository context cannot escape ceilings

The failed design bounded base/fixed-conditional/representative routes but exempted route-dependent repository inputs. The final design adds a fourth dynamic repository layer:

- checker-owned classification of reviewed dynamic slot classes;
- fail-closed behavior for an unknown placeholder class;
- repository/external separation;
- per-source + aggregate dynamic-route ceilings;
- exact contract, direct dependency, contract-mandatory repository input and concrete repository evidence reconstruction;
- anchored manifest + manifest-named repository input reconstruction;
- repository-wide discovery of present/future `Docs/workpacks/**/WP-*.md`, independent of `CANONICAL_ROUTE_CONFIGS`;
- explicit policy-revision + justification requirement for ceiling increases.

The global resolver intentionally does not turn every path mentioned in prose into context authority. Output paths, donor-only references and historical mentions remain non-mandatory unless an independent routing/contract rule makes them required.

### B2 causal controls

A synthetic FUTURE track outside H1/CITY/PA proves:

1. exact WP automatically enters a budget;
2. exact WP growth past ceiling -> RED;
3. newly contract-required repository evidence enters automatically;
4. growth of that evidence past ceiling -> RED;
5. caller omission cannot hide a source still required by exact contract authority;
6. removing the whole dynamic binding surface -> RED;
7. wholly non-mandatory repository growth -> GREEN;
8. `repair_worker` repository evidence is counted while a genuinely external GitHub FAIL remains explicitly external;
9. `h1_local_executor` counts anchored manifest + manifest-named repository file without caller-maintained file enumeration;
10. a future role using an existing reviewed exact-contract slot inherits the resolver;
11. a new dynamic slot class fails closed pending review.

`scripts/ctx03-dynamic-slot-controls.py` independently scans both `initial_reads` and `conditional_reads`, closing the future variant where an unknown dynamic placeholder appears only in a conditional surface.

## Additional variants found by the Worker before CLEAN

The circuit-breaker found and repaired variants beyond the two Reviewer examples:

1. **B1 numeric-order drift** — lexicographic CTX discovery could mishandle CTX-10 vs CTX-04; ordering is numeric.
2. **B2 future-role coupling** — exact-contract rediscovery depended on current role names; it is now slot-semantic.
3. **B2 path-overreach false-red** — an early repository-wide audit promoted every path mention to mandatory context; final grammar follows mandatory routing/contract semantics instead.
4. **B2 duplicate-oracle drift** — an intermediate second dynamic-universe checker had different discovery semantics; it was removed so the dynamic context checker is the sole budget/completeness authority.
5. **Dangling duplicate-oracle CI reference** — after removing that checker, the full baseline diff exposed stale workflow calls to the deleted script. CI now calls only `ctx03-dynamic-context-check.py --audit-policy` and its independent controls.
6. **Conditional dynamic-slot blind spot** — the production slot discovery was centered on current `initial_reads`; a future placeholder could have been introduced only in `conditional_reads`. An independent all-read-surface control is now mandatory in CI and fails closed on any unreviewed placeholder there.

No known variant above is deferred to the Reviewer.

## Whole structured-surface omission

The final circuit-breaker exercises real oracles with:

- `CONTEXT_ESCALATIONS.evaluations` removed;
- `evaluations = []`;
- `evaluations = {}`;
- representative route collection removed/empty;
- conditional budget collection emptied;
- CTX current-state projection removed/emptied.

RED must arise because the independently reconstructed required condition is absent, not merely because the fixture announces that it deleted a field.

Accepted CTX-02 whole-surface/capsule/PA semantic controls remain separately binding and are rerun on the exact candidate.

## Semantic substitution challenge

No compact/mechanical surface gains semantic authority:

- accepted-state index is navigation projection only;
- history is provenance/reconstruction only;
- envelope/budget/config surfaces are process gates only;
- capsules remain navigation only under accepted CTX-02 rules;
- mechanical PASS cannot replace exact authoritative sources, predecessor reopen or independent Reviewer reasoning;
- quality replay proves source reachability, not model semantic competence.

## Future extension challenge

Synthetic extension covers a new CTX transition, new role profile, new fixed conditional repository path, new dynamic placeholder class, conditional-only dynamic placeholder and future non-H1/CITY/PA workpack/source. Each is either incorporated automatically into the correct independently discovered universe or turns RED pending explicit reviewed oracle/budget evolution.

## Lifecycle/retry boundary retained

The earlier repairs remain closed:

- external CLEAN pointer must be exact PR/exact SHA;
- repository mutation after CLEAN invalidates readiness;
- wrong PR/wrong SHA CLEAN pointer -> RED;
- missing Ready marker -> blocked;
- existing marker + red gate -> blocked;
- same-SHA gate repair can reuse the durable marker and close;
- HEAD movement after marker -> blocked.

Terminal sequence remains: finish bytes -> exact HEAD -> full validation + complete baseline diff + Worker challenge -> durable external CLEAN -> metadata only -> Ready/freeze -> exact-SHA gates + terminal closure -> STOP for fresh Reviewer.

## Conclusion before terminal pre-review

The class-level repair and the additional circuit-breaker variants are represented in repository evidence. Because this file is itself a repository byte, every earlier CLEAN/freeze remains invalid. The next phase is the final exact-HEAD validation/pre-review with writers stopped. Any new blocker found there must be repaired before CLEAN.
