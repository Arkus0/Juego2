# CTX-01 — Dry runs (repair cycle 1)

Status: **PASS**

The repair reruns the representative role-routing cases and adds the causal freshness cases required by FAIL `#5271197524`.

## Worker PA-04

Initial pack remains exact WP + Worker protocol/profile. Unrelated foundational proof is not loaded by default. If PA-04 later binds a foundational claim, the profile escalates to `FOUNDATIONAL_PROOF_STANDARD.md`.

Result: `CONTEXT_CLOSED` for the initial non-foundational route; conditional deepening preserved.

## Worker H1-02

The exact H1 range triggers `H1_REMOTE_LOCAL_EXECUTION.md` and any claim-bound foundational proof surface. The local executor does not inherit semantic/design authority.

Result: correct H1 overlay; no authority widening.

## Reviewer exact WP

The index may navigate, but authoritative predecessor evidence plus complete frozen candidate material must be opened independently. Worker prose/index is not proof.

Result: Reviewer independence preserved.

## Blocked CITY-04

The exact contract exposes the unresolved H1-08 cross-track dependency. The profile deepens before deciding eligibility.

Result: `ESCALATE:CROSS_TRACK_DEPENDENCY` then `BLOCKED` when the dependency remains unaccepted.

## Freshness controls

A `CANDIDATE` projection anchored to the reconstruction source returns `STALE`, `mutable_hints_usable=false`, reason `projection_phase_not_persisted`.

A synthetic persisted projection with `SOURCE_MAIN_SHA=aaaa...`, `DOCSYNC_MAIN_SHA=bbbb...`, `projection_phase=DOCSYNC_PERSISTED`, `generated_from_main_sha=SOURCE_MAIN_SHA`, and `first_parent(DOCSYNC_MAIN_SHA)=SOURCE_MAIN_SHA` returns `FRESH`. The persisted file never needs to contain `DOCSYNC_MAIN_SHA`.

After a later `main=cccc...` whose first parent is `bbbb...`, the unchanged index anchored to `aaaa...` returns `STALE`.

Result: deterministic stale detection is preserved and the original self-reference is removed.
