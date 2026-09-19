# Foundational Proof Standard

Version: 1.2 — 2026-09-18

This standard binds every `HK-*` workpack through `WP-HK-GATE`.

Green CI is necessary but insufficient. A foundational candidate must show why its central architectural claim is complete enough to trust downstream work **inside an explicit, finite trust boundary**. Foundational proof is not a mandate to prove arbitrary pathological behavior of the language toolchain, operating system, package manager, CI runner or other declared trusted infrastructure.

## Project-domain and terminology note

Juego2 / Arkus Harness is a game-development and software-verification project, not a cybersecurity project. The proof mechanisms in this standard operate only on repository-owned code, fixtures, tests and CI.

Historical evidence may use terms such as `self-attack`, `attack fixture`, `bypass` or `adversarial review`. Their intended meaning here is ordinary software-quality testing: `negative conformance test`, `defect-injection test`, `undeclared/alternate code path` and `strict independent review`. New work should use the neutral terms. This terminology clarification does not weaken or expand any proof obligation.

## Adoption boundary

Version 1.2 applies to every foundational candidate frozen after the commit containing this version reaches `main`.

A candidate already validly frozen before adoption remains reviewable under the proof standard that governed its freeze. An ACTIVE/Draft candidate that has not frozen yet must reconcile its proof claim with this version before freeze. This process change does not itself create an implementation defect; it narrows what is required to demonstrate the declared workpack claim.

## Required sequence

```text
implementation
→ explicit claim + trust boundary
→ independently defined/evaluated universe where completeness is claimed
→ mechanically enumerable inventory where useful
→ proof-obligation matrix
→ causal negative-conformance / defect-injection controls proportional to the claim
→ independent or evaluated oracle where self-confirmation is possible
→ residual-risk audit
→ proof-budget check
→ exact-SHA CI
→ freeze
→ fresh independent review
```

Each WP evidence must contain:

```text
FOUNDATIONAL_PROOF_VERDICT: READY | NOT_READY
UNRESOLVED_PROOF_OBLIGATIONS: <integer>
KNOWN_UNDETECTED_DEFECT_CLASSES: <integer>
TRUST_BOUNDARY: <short explicit summary or evidence path>
PROOF_BUDGET_VERDICT: WITHIN_BUDGET | REAUDIT_REQUIRED
```

`READY` requires unresolved obligations and known undetected defect classes to be zero **inside the declared claim and trust boundary**, plus `PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

`KNOWN_UNDETECTED_DEFECT_CLASSES` never means that unknown unknowns have been proven not to exist. It means that no currently known material defect class inside the declared claim remains able to escape the accepted evidence.

## Default trusted base

Unless a workpack explicitly narrows it further or has a product requirement that justifies expanding the proof boundary, foundational work may trust:

- Git object/checkout semantics for the exact candidate SHA;
- the pinned .NET SDK/runtime and normal documented behaviour of the selected compiler and MSBuild toolchain;
- normal documented NuGet restore/lock/content-hash behaviour under the repository's canonical build path;
- the operating system, filesystem and CI/runner as infrastructure;
- cryptographic/hash primitives and upstream package/runtime integrity mechanisms used according to their documented contract.

The repository must pin/version/configure these layers where reproducibility materially depends on doing so. Proof may verify that the intended pinned/configured toolchain is actually selected and that the repository uses the canonical path, but it does **not** need to prove that trusted infrastructure cannot exhibit arbitrary out-of-contract behavior.

A workpack may intentionally move part of this trusted base inside its claim only when its acceptance criteria genuinely require that guarantee, for example isolation/containment work. The WP must then state that expansion explicitly and justify its cost.

## Proof matrix

For every acceptance criterion record:

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive evidence | Negative control / defect injection | Result | Residual risk |
|---|---|---|---|---|---|---|

Representative happy-path tests are not a completeness argument, but neither is proof required to defend against mechanisms explicitly outside the trust boundary.

## Independent-universe rule

A completeness proof is invalid if the same registry, manifest, root list, dispatcher list, validator list or configuration under test can silently shrink the universe that the proof then declares complete.

When the claim is “all X are classified/discovered/validated/compiled/exposed”, the candidate must identify how the universe of X is obtained independently of the classification/discovery/validation declaration being checked, or demonstrate an effective/evaluated oracle that makes omission observable.

Examples:

- repository/project/source completeness begins from an independent repository/effective-build universe, not only a manifest-declared scan root;
- command discovery completeness cannot use discovery itself as the only command inventory;
- validator completeness cannot use only the validator registry to decide which invariants exist;
- transport/adaptor completeness cannot use only one adapter's exported surface as the canonical capability inventory.

If deleting, relocating, unregistering or hiding a material object **inside the declared universe** can make both the object and the proof obligation disappear while CI stays green, the proof boundary is self-shrinking and the central claim is false.

This rule does not require recursively proving the trusted base used to evaluate that universe.

## Negative-conformance rule

For each material defect class inside the claim that could realistically leave CI green while the acceptance claim is false:

1. inject a controlled repository-local defect;
2. demonstrate the intended guard/oracle turns red for the intended reason;
3. revert the defect;
4. demonstrate green again;
5. preserve exact commands and evidence.

A compile failure for an unrelated reason is not valid evidence.

Negative-conformance tests should target causal omission classes, not syntax variants. If multiple alternate paths share one causal class, fix/prove the class rather than accumulating case-specific checks.

Do not add a new defect-injection test merely because a hypothetical pathological repository configuration could rely on undocumented or intentionally out-of-bound behaviour of the trusted base. Record such cases as non-blocking residual risk unless the WP explicitly owns that guarantee.

## Effective-behaviour rule

When a claim concerns what a compiler, serializer, dispatcher, transport, validator or runtime actually consumes, prefer an oracle over the **evaluated/effective result** rather than a growing list of forbidden syntax. Static guards remain defence in depth.

Independent discovery and effective evaluation are complementary: an evaluated oracle proves what a known participant actually did, while an independent-universe oracle proves that no material participant inside the claim disappeared before evaluation.

Effective-behaviour proof must remain at the semantic boundary needed by the WP. It is not a requirement to exhaustively enumerate every internal switch or extension point of trusted infrastructure unless the WP explicitly claims control over that surface.

## External-component rule

Adopting a library/framework transfers implementation work, not Arkus product obligations. For every external component on a foundational path, record:

- exact version/upstream identity and license where materially relevant;
- which Arkus guarantee it helps implement;
- which Arkus guarantees remain outside its scope;
- a conformance/replacement boundary so the dependency cannot silently become the product's semantic authority.

Do not claim an upstream project's tests or popularity as evidence of Arkus completeness. Conversely, do not recursively re-prove an upstream implementation when the dependency is explicitly inside the trusted base and the Arkus claim only requires conformance at its boundary.

## Proof budget

Proof machinery is support code, not the product. It must earn its cost by protecting a concrete acceptance claim.

Every material guard, oracle or causal negative-conformance control added after the initial implementation must map to at least one of:

- an explicit WP acceptance criterion;
- a realistic false-green defect class that would materially invalidate that criterion;
- a regression class already observed in Worker/Reviewer history.

Do not add proof infrastructure solely to reduce philosophical residual risk or to demonstrate arbitrary out-of-contract behavior of trusted infrastructure.

There is no fixed proof-to-product line ratio. Some foundational WPs legitimately require more proof than runtime code. The required control is **convergence**:

- if two consecutive repair/pre-review cycles materially increase proof/support machinery without changing product behaviour, closing a stated acceptance gap or eliminating a realistic in-boundary false-green class, STOP and re-audit the claim/trust boundary before adding more proof;
- if the proof system becomes harder to reason about than the behaviour it protects, simplify the proof, narrow the claim or move the disputed layer into the trusted base;
- a newly imagined out-of-boundary scenario is not by itself justification for another repair cycle.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET` means the Worker can explain why the remaining proof is proportionate to the WP claim. `REAUDIT_REQUIRED` blocks freeze until the architecture/claim is right-sized.

## Residual risk

Residual risk is blocking only when it can materially falsify an acceptance claim **inside the declared trust boundary**.

Risks that require corruption or arbitrary out-of-contract behavior of the declared trusted base, use a non-canonical unsupported build path, or concern guarantees the WP does not claim must be documented when useful but are non-blocking by default.

A Reviewer may recommend a future hardening WP for such risks without failing the current candidate.

## Freeze blockers

Do not freeze while any of these is true:

- an acceptance claim is UNKNOWN/PARTIAL;
- an enumerable in-claim inventory has unclassified entries;
- the universe behind an in-claim completeness assertion can self-shrink without detection;
- a material in-boundary negative-conformance test remains unexercised;
- a required negative control fails to turn red for the intended reason;
- a known defect class inside the claim would escape detection;
- exact evidence and candidate SHA do not match;
- residual risk can still falsify the central in-boundary claim;
- proof budget requires re-audit.

Do not block freeze solely because an out-of-boundary trusted component could theoretically behave outside its documented contract.

## Circuit breaker

Two independent Reviewer FAILs exposing the same foundational defect class require architecture re-audit before another repair cycle.

A Reviewer finding that demonstrates a self-shrinking proof universe, circular completeness oracle or equivalent false proof boundary **inside the declared claim** triggers architecture re-audit immediately; do not spend another cycle adding a case-specific exception first.

Independently, two consecutive cycles of expanding proof without corresponding product/acceptance progress trigger the proof-budget re-audit even without a Reviewer FAIL.

## Reviewer duty

The independent Reviewer must reconstruct scope from the repository, challenge the Worker's completeness argument and inspect causal negative controls. The Reviewer should actively try to find omission/false-green classes **inside the WP claim and declared trust boundary**, including classes not highlighted by the Worker.

The Reviewer is not required to invent a new defect class in order to PASS, and must not fail a candidate merely by demonstrating arbitrary out-of-contract behavior of infrastructure explicitly declared trusted unless the WP acceptance criteria put that infrastructure inside the claim.

Reviewer PASS names the exact candidate SHA and means the declared claim is adequately supported within its stated trust boundary; it does not certify the universe outside that boundary.
