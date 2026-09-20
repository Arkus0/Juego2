# WP-HK-09B — Resource limits + persistence integrity

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-09A`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Bound the resource cost of accepted Arkus operations and make persistence/import boundaries fail closed under oversize input or interruption, without weakening the representative interaction shapes accepted by HK08A/HK08B.

## Acceptance

- Input size, batch size, query page size, recursion/depth and execution-time/resource limits are explicit, machine-readable where useful and tested.
- Limits are justified against accepted representative HK08A/HK08B flows rather than chosen as arbitrary constants that force valid coherent authoring intents into smaller persisted fragments.
- The accepted representative multi-resource edit from HK08A remains expressible as one atomic request inside the enforced batch/resource envelope.
- Oversized/deep/over-budget requests fail closed with stable machine-readable diagnostics and no partial canonical effect.
- Snapshot/import paths validate format/version and relevant size/resource constraints before replacing canonical state.
- A rejected oversized/invalid import leaves canonical state and accepted local history/evidence unchanged.
- Persistence interruption cannot leave a half-committed canonical world or machine-readable evidence that claims a commit which did not become authoritative.
- Time/cancellation/resource enforcement does not create a second mutation authority or bypass accepted HK04/HK05/HK06 semantics.
- Resource enforcement lives below transport-specific adapters so reference transport, MCP and future projections observe equivalent limit semantics.
- The bounded long-session case later exercised by HK10 has an explicit H0 resource envelope against which growth can be judged.

## Required negative-conformance tests

RED→GREEN for:

- oversized request body;
- excessive batch operation count or payload bytes;
- excessive query page size;
- excessive nesting/recursion depth;
- execution-time/resource limit breach;
- representative HK08A coherent batch being rejected solely because an unjustified legacy cap was retained;
- oversized/unsupported snapshot or import replacing any part of canonical state;
- interruption during persistence producing a partial canonical world or false success evidence; and
- reference-transport versus MCP drift in resource-limit/error semantics.

## Explicit boundary

HK09B owns **how much accepted work may consume and how persistence/import fail under limits/interruption**. It consumes the host-authority boundary accepted by HK09A rather than re-proving shell/network/filesystem capability containment.

Large-world asymptotic optimization, streaming architecture for arbitrary production scale, distributed storage and multi-process writer coordination remain outside H0 unless measured evidence explicitly promotes them.

## Forbidden scope

New host capabilities, authentication/multi-user cloud security, cloud/distributed persistence, OS container orchestration, large-scale load testing, multi-agent/distributed writer coordination, penetration testing or external-system testing.

## DoD

Accepted H0 requests have explicit tested resource limits, the representative HK08A/HK08B workflow fits those limits without sacrificing required atomicity, oversize/over-budget work fails closed, and import/persistence interruption cannot publish a partial or falsely evidenced canonical state; independent PASS.
