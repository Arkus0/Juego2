# WP-HK-09B — Resource limits + persistence integrity

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-09A`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Completion metadata

- implementation PR: `#56`;
- baseline SHA: `ada532f99282c96db15813eb17963bc9cb6d08fb`;
- reviewed frozen candidate: `8ed02586da9a5b6e159e1cdc76a47ae7ca89c763`;
- independent Reviewer verdict: `PASS` (review `#5261068513`);
- exact-SHA freeze validation: GREEN, Actions `35522581043` (Candidate Validation run #470);
- implementation merge SHA: `2e7a258fdcec2e26492d308c3cb199ab62201dcd`.

Accepted semantics: `arkus.h0-resource-envelope@1`, discoverable through `system.resource-envelope.describe@1.0`, is the machine-readable H0 envelope. Enforcement is two-layer and transport-neutral: `H0ResourcePolicy` admits portable request bytes (896 KiB canonical arguments), nesting depth (32), mutation operations (96), decoded mutation payload (512 KiB), relations per resource (256), extension payload (256 KiB), query page size (100) and snapshot size before canonical dispatch; `WorldResourceLimits` independently rechecks the materialized canonical state (640 KiB canonical world bytes, 10,000 resources) before publication. Bounded session growth is enforced at the canonical write authorities (10,000 mutation transactions, 1,024 snapshot import receipts). A 5,000 ms execution budget is checked cooperatively and again before authoritative publication. Mutation, snapshot import and replay each stage a complete aggregate — state plus receipt, lineage, rebase evidence and journal — and publish atomically, so an interrupted, expired or rejected writer cannot leave a partial canonical world, advance revision/hash, or acquire false success evidence. Persistence is explicitly declared: `durabilityLevel=process-local-checkpoint`, `publicationBoundary=validate-stage-aggregate-publish`, `powerLossDurabilityClaimed=false`. The accepted HK08A 96-operation coherent edit remains one atomic transaction, and successfully framed JSONL and MCP requests reach identical neutral resource semantics.

The inherited `arkus.reference.jsonl@1` transport contract is unchanged: 1,048,576 bytes per frame with `transport.frame_too_large` for larger input. The HK09B envelope is derived to fit beneath it, leaving 128 KiB of framing headroom and keeping the advertised snapshot envelope traversable through the frozen reference transport.

One earlier frozen candidate `dcea0901a4fe78549d81271a7f192bbae77e58b7` failed review `#5261028914` because it changed the frozen `arkus.reference.jsonl@1` frame bound and oversized-frame code in place and moved the inherited HK07A regression oracle with the implementation, producing a false green against the predecessor compatibility guarantee. The accepted repair restores the HK07A implementation and oracle exactly to baseline and fits the neutral envelope below the inherited frame instead of widening it.

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

## Resource-envelope ordering

The resource envelope must be derived from the accepted representative HK08A/HK08B shapes, not used to redefine them after the fact. In particular, a legacy operation-count, payload or execution cap is not authoritative merely because it already exists: if it rejects the accepted coherent multi-resource edit, the cap must be justified and revised rather than forcing the edit into multiple persisted transactions.

Limit tuning may tighten cost, but it may not weaken accepted atomicity, validation, provenance or recovery semantics.

## Persistence-interruption proof boundary

Interruption tests are scoped to the persistence mechanism and durability claim actually accepted by H0. HK09B must attack the real publication boundary strongly enough to prove that interrupted/rejected work cannot become a partial canonical state or acquire false success evidence.

This requirement does **not**, by itself, mandate a WAL, distributed transaction protocol, crash-consistency subsystem, OS-level durability model, `fsync` protocol or new storage architecture. If the accepted H0 mechanism is checkpoint/snapshot-oriented, the proof should exercise that mechanism's validate/stage/publish boundary and demonstrate fail-closed atomic publication at the durability level it actually claims.

A process-kill/power-loss style defect is required only when the accepted persistence mechanism claims correctness across that failure mode. Otherwise, injecting broader failures must not be used to expand H0 scope through overdefense.

## Explicit boundary

HK09B owns **how much accepted work may consume and how persistence/import fail under limits/interruption**. It consumes the host-authority boundary accepted by HK09A rather than re-proving shell/network/filesystem capability containment.

Large-world asymptotic optimization, streaming architecture for arbitrary production scale, distributed storage and multi-process writer coordination remain outside H0 unless measured evidence explicitly promotes them.

## Forbidden scope

New host capabilities, authentication/multi-user cloud security, cloud/distributed persistence, OS container orchestration, large-scale load testing, multi-agent/distributed writer coordination, penetration testing or external-system testing.

## DoD

Accepted H0 requests have explicit tested resource limits, the representative HK08A/HK08B workflow fits those limits without sacrificing required atomicity, oversize/over-budget work fails closed, and import/persistence interruption cannot publish a partial or falsely evidenced canonical state; independent PASS.
