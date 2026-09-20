# WP-HK-09A — Residual risk and trust boundary

## Trust boundary

HK09A governs the repository-local **production H0 software host**: canonical composition, neutral projection, reference JSONL executable and MCP projection. It controls which host powers this software surface possesses or can acquire from protocol-controlled data.

It does not claim to sandbox the operating system, defend against an actor who already controls the local process account, secure an external network service, provide authentication/authorization, harden Unity/editor plugins, implement anti-cheat, or police arbitrary native/runtime corruption. Those are outside the H0 contract.

TRUST_BOUNDARY_VERDICT: EXPLICIT

## Accepted residuals

### Resource and persistence hardening belongs to HK09B

HK09A does not choose final numeric limits for request/page/batch/depth/time/size budgets and does not add crash-consistent persistence mechanics. HK09B owns those explicit limits and interruption/import integrity. Existing inherited bounds remain regression-covered but are not promoted into HK09A proof claims.

### No production caller-selected filesystem authority

The H0 executable rejects `--file` before delegating to the legacy JSONL framing host, so traversal and symlink path strings cannot produce a filesystem effect. The internal HK07A parser code still contains the retired file-framing implementation as implementation residue; it is not an accepted production entrypoint or capability. The effective-surface oracle pins that residue and verifies the executable rejection precedes delegation. A future production entrypoint that chose to expose file authority would require a reviewed policy/location boundary and new evidence rather than silently inheriting HK07A's old convenience.

### Policy admission is intentionally H0-specific

`H0HostCapabilityPolicy` admits the current H0 canonical read/mutation/rebase/replay classes and rejects external side effects/elevated privilege. It is attached at `CanonicalWorldContract.Compose`, below transports but above adapters. Generic `ContractComposer` remains engine-neutral and extensible; H1 may introduce separately reviewed host authority without weakening the H0 policy.

### Source-surface oracle is a repository conformance proof, not an OS sandbox

The independent source inventory exists to detect effective shell/network/runtime-activation paths omitted from canonical metadata. It does not claim that textual source analysis can protect a compromised SDK/runtime or malicious dependency. Dependency/runtime trust remains part of the pinned build substrate inherited from the repository process.

## Reopen triggers

HK09A must be reopened if any accepted production H0 entrypoint can:

- launch a generic process/shell or initiate network access from protocol-controlled input;
- open caller-selected filesystem paths;
- activate arbitrary runtime types from selectors;
- expose external/elevated capability semantics without H0 policy rejection;
- mint an adapter-only capability that bypasses canonical composition; or
- make the approved product-shaped inspect/author/snapshot/replay path depend on one of those forbidden powers.

No such in-scope residual is accepted by this candidate.

UNRESOLVED_IN_SCOPE_RESIDUALS: 0
