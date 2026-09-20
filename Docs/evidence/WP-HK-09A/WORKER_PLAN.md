# WP-HK-09A Worker plan and implementation record

Status: PRE_REVIEW
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
- protocol payloads cannot manufacture host powers absent from reviewed canonical policy;
- production H0 exposes no caller-selected filesystem authority, so traversal/symlink path attempts fail before an external filesystem effect;
- capability policy is enforced below transport adapters, so JSONL/MCP/future projections cannot mint or bypass authority;
- accepted canonical metadata remains truthful about read-only/mutating/privileged/provenance semantics;
- protocol-controlled type names/selectors cannot trigger arbitrary runtime-type activation;
- ordinary accepted H0 read/author/replay/snapshot flows remain functional without shell, network or unrestricted filesystem authority.

HK09A does **not** own final numeric request/page/size/time/depth quotas or crash-consistent persistence mechanics; those remain `WP-HK-09B`.

### Concrete predecessor reopen conditions

A predecessor is reopened only if HK09A evidence demonstrates one of these concrete contradictions:

- an accepted canonical H0 handler already obtains undeclared host authority through a path required by HK08B or earlier guarantees;
- a transport accepted by HK07A/HK07B can manufacture canonical authority that is not present in the composed inventory;
- enforcing HK09A below transports requires a second semantic registry or mutation path because the accepted canonical composition boundary is insufficient.

No predecessor reopen condition was observed.

## Initial implementation observations

1. `ComposedContract.Dispatch` is the canonical dispatch boundary consumed by the neutral projection and therefore by JSONL/MCP.
2. Canonical definitions already carry `PolicySemantics`; a transport-local permission model would be a second source of truth and was not introduced.
3. Production H0 canonical capabilities use read-only/canonical-mutation/rebase/replay side-effect classes; generic host process/network capabilities were absent from the accepted inventory.
4. The reference JSONL executable nevertheless exposed `--file PATH`, opening a caller-supplied arbitrary path outside canonical dispatch. This was the concrete HK09A containment gap.
5. MCP derives tools from `NeutralProjectionService.Capabilities` and dispatches through the neutral projection; no independent MCP host-power route was found.

## Implemented design

1. `H0HostCapabilityPolicy` is enforced by `CanonicalWorldContract.Compose` before a composed contract becomes the production H0 surface consumed by projections. It rejects external side-effect classes, elevated/unknown privilege, invalid side-effect/transaction pairings, and canonical state-change metadata that is not `Authoring` + provenance `Required`.
2. The policy is admission-time rather than redundantly re-run on every dispatch because a `ComposedContract` has an immutable definition/route inventory after successful composition. Every JSONL/MCP production invocation therefore dispatches inside an already-admitted contract.
3. The initial idea of retaining `--file` under a path sandbox was intentionally abandoned. H0 needs no filesystem path authority for accepted flows, so the production executable now rejects `--file` before delegating to the legacy framing host. This removes traversal/symlink/TOCTOU complexity instead of partially sandboxing it.
4. Seven required HK09A negative classes are executable: parent traversal, real symlink escape, process/shell-shaped capability, network-shaped payload observed against a loopback listener, runtime-type selector, adapter-only privileged capability and transport bypass/effective-surface inventory.
5. The effective-source oracle is independent of canonical capability metadata so a hidden shell/network/type-activation route cannot become green merely by omitting it from the policy registry.
6. A Juego2 content-shape probe sourced from `Docs/art/VISUAL_BIBLE.md` exercises a Potes plaza/market/bar/workshop slice through policy-bound inspect → author → snapshot/import → replay and requires identical final canonical state.
7. `hk09a-observe-exact-sha.sh` and `hk09a-verify-exact-sha.sh` are registered in the canonical workflow routers. Freeze verification requires code, regression, negative/content evidence, proof markers and CLEAN Worker pre-review on the same candidate SHA.

## Worker pre-review correction already applied

The initial policy checked side-effect/transaction consistency but did not reject a canonical state-changing capability falsely published as `PublicRead` or without provenance `Required`. Worker pre-review identified that as a foundational false-green class. The policy and dedicated negative tests now reject both cases.

## Proof-boundary guardrails

- Repository-local software capability containment only; no external-system testing.
- No auth/cloud-security/anti-cheat/OS sandbox/container/pentesting scope.
- No resource-limit tuning owned by HK09B.
- No proof machinery for arbitrary out-of-contract OS/runtime corruption.
- Causal policy/path-boundary oracles preferred over string-name blocklists; source inventory is supplementary independent evidence, not the semantic policy itself.

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: `Docs/evidence/WP-HK-09A/RESIDUAL_RISK.md`
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
