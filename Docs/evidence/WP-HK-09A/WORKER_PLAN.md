# WP-HK-09A Worker plan

Status: IMPLEMENTING
Branch: `wp/hk-09a-capability-containment`

## PREDECESSOR_CONTRACT_CHECK

### Accepted dependency

- Direct predecessor: `WP-HK-08B`.
- Accepted reviewed candidate SHA: `31370f91b48408b90a587d7ad5178ba1be8d6bfe`.
- Independent Reviewer verdict: PASS, PR #52, review #5260337340.
- Implementation merge SHA: `bdf4675c17d842d73ff59637fa36314d14c2707a`.
- HK08B exact-SHA validation was GREEN and its foundational proof verdict was READY with zero unresolved obligations.

### Inherited guarantees consumed by HK09A

HK09A consumes rather than re-proves, unless its own effective path contradicts them:

1. one canonical composed capability inventory and dispatcher below transports;
2. one accepted canonical mutation/rebase/replay authority with validation, atomicity, optimistic CAS, idempotency and provenance semantics;
3. reference JSONL and MCP as projections of the same neutral/canonical capability surface, with no adapter-owned semantic registry;
4. HK08A/HK08B bounded ordinary authoring interactions, including snapshot/replay and structured stale recovery;
5. HK08B recovery does not create a second mutation authority and retry remains ordinary canonical plan/dry-run/apply.

### Guarantees owned by HK09A

HK09A owns only the host-capability containment boundary required by its contract:

- the production local H0 host has no generic shell/process execution authority and no ambient network authority;
- protocol payloads cannot manufacture host powers that are absent from the reviewed canonical policy;
- any filesystem read/write authority used by the accepted host is limited to explicit Arkus-owned locations and rejects parent/symlink escape before the out-of-boundary effect;
- capability policy is enforced below transport adapters, so JSONL/MCP/future projections cannot mint or bypass authority;
- accepted canonical metadata remains truthful about read-only/mutating/privileged operations;
- protocol-controlled type names/selectors cannot trigger arbitrary runtime-type activation;
- ordinary accepted H0 read/author/replay/snapshot flows remain functional without shell, network or unrestricted filesystem authority.

HK09A does **not** own final numeric request/page/size/time/depth quotas or crash-consistent persistence mechanics; those remain `WP-HK-09B`.

### Concrete predecessor reopen conditions

A predecessor is reopened only if HK09A evidence demonstrates one of these concrete contradictions:

- an accepted canonical H0 handler already obtains undeclared host authority through a path required by HK08B or earlier guarantees;
- a transport accepted by HK07A/HK07B can manufacture canonical authority that is not present in the composed inventory;
- enforcing HK09A below transports requires a second semantic registry or mutation path because the accepted canonical composition boundary is insufficient.

Absent such evidence, HK09A must repair its own host-policy/adapter seam rather than reopening accepted predecessors.

## Initial implementation observations

1. `ComposedContract.Dispatch` is the common canonical dispatch boundary consumed by the neutral projection and therefore by JSONL/MCP.
2. Canonical definitions already require `PolicySemantics` including privilege, transaction and provenance metadata; there is no need to create a transport-local policy model.
3. Production H0 canonical capabilities use canonical/read-only/rebase/replay side-effect classes; generic host process/network capabilities are not part of the accepted inventory.
4. The reference JSONL host nevertheless exposes `--file PATH` and directly opens the caller-supplied arbitrary path. This is real host filesystem authority outside canonical dispatch and is the first concrete HK09A containment gap.
5. MCP derives every tool from `NeutralProjectionService.Capabilities` and dispatches through `NeutralProjectionService.InvokeAsync`; no independent MCP host-power route has been identified.

## Planned implementation

1. Add one H0 host-capability admission policy in Runtime, enforced at the composed contract boundary before a capability becomes an effective production host surface and again at dispatch as a fail-closed invariant. The H0 policy permits the accepted canonical/read-only/rebase/replay classes and rejects external host-side-effect classes unless a later reviewed policy explicitly owns them.
2. Preserve the existing canonical contract model and privilege metadata as semantic authority; do not add adapter-owned capability metadata.
3. Replace unrestricted reference-host file opening with an Arkus-owned request-file boundary. Keep the accepted one-shot file convenience, but only for files rooted under a fixed harness-owned location; reject traversal and symlink/reparse escapes before opening.
4. Add repository-local negative-conformance tests for every HK09A required negative class: traversal, symlink escape, process/shell-shaped unsupported capability, network-shaped protocol authority attempt, runtime-type selector, adapter-only privileged capability and transport bypass attempt.
5. Add an independent effective-surface inventory/oracle covering product assemblies/source surfaces relevant to shell/network/filesystem/type activation so proof cannot silently shrink with the policy registry under test.
6. Run a bounded Juego2 content-shape probe through ordinary inspect/mutate/snapshot/replay flows under the policy to prove containment does not narrow the product-shaped authoring surface.
7. Produce `NEGATIVE_CONFORMANCE_MATRIX.md`, `PROOF_MATRIX.md`, `RESIDUAL_RISK.md`, `CONTENT_SHAPE_PROBE.md`, strict `WORKER_PRE_REVIEW.md`, and exact-SHA verification scripts/gates before freeze.

## Proof-boundary guardrails

- This is repository-local software capability containment, not external-system testing.
- No auth/cloud-security/anti-cheat/OS sandbox/container/pentesting scope.
- Do not tune resource limits owned by HK09B.
- Do not add proof machinery for arbitrary out-of-contract OS/runtime corruption.
- Prefer causal policy/path-boundary oracles over lists of dangerous strings.

FOUNDATIONAL_PROOF_VERDICT: NOT_READY
UNRESOLVED_PROOF_OBLIGATIONS: 7
KNOWN_UNDETECTED_DEFECT_CLASSES: 7
TRUST_BOUNDARY: pending `Docs/evidence/WP-HK-09A/RESIDUAL_RISK.md`
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
