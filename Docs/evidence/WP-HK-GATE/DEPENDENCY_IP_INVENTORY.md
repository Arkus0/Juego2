# WP-HK-GATE dependency / IP inventory and Engine Bridge boundary

DEPENDENCY_IP_INVENTORY: COMPLETE
ENGINE_BRIDGE_BOUNDARY: CLEAN
UNCLASSIFIED_MATERIAL_DEPENDENCIES: 0

## Material shipped/runtime dependency inventory

### ModelContextProtocol.Core 2.2.0

- Exact package: `ModelContextProtocol.Core` `2.2.0`.
- Upstream: official `modelcontextprotocol/csharp-sdk`; accepted HK07B adoption evidence records release/upstream commit `6fa3825`.
- License posture: HK07B recorded the applicable Apache-2.0/MIT code-license transition and that CC-BY-4.0 documentation was not copied into Arkus product material.
- Arkus guarantee implemented: standards-compliant MCP stdio framing/server integration at the outer adapter.
- Guarantees it does **not** own: canonical capability identity/schema, world identity/hash, transaction/validation semantics, provenance/journal/replay, snapshot/diff, host-capability policy, resource envelope or neutral projection semantics.
- Replacement/conformance boundary: `Arkus.Harness.Mcp` is an adapter over the neutral projection. Accepted MCP/reference conformance tests compare effective behavior to the independently composed canonical universe; replacement of the SDK must preserve those tests rather than redefine Arkus semantics.
- Network/vendor requirement: none for normal H0 operation; the accepted MCP host is local stdio.

This dependency was adopted and reviewed in `Docs/evidence/WP-HK-07B/DEPENDENCY_ADOPTION.md`; GATE consumes that accepted record and re-executes its conformance boundary.

## Build/test-only dependencies

Pinned test/build packages are not shipped semantic authorities:

- `Microsoft.NET.Test.Sdk` 17.14.1;
- `xunit.core` 2.9.3;
- `xunit.assert` 2.9.3;
- `xunit.runner.visualstudio` 2.8.2.

They support verification only. Removing or replacing them may change test execution mechanics but cannot legitimately change an Arkus public contract.

## Trusted toolchain

`global.json` pins .NET SDK `8.0.425` with roll-forward disabled and prerelease disabled. Normal documented .NET/MSBuild/NuGet behavior is inside the Foundational Proof Standard trusted base; exact-SHA CI confirms the intended pinned path is selected and locked restore is used.

## Repository IP posture

Root `LICENSE` is `ARKUS SOURCE-VISIBLE PROPRIETARY LICENSE` v1.0. It explicitly keeps third-party components under their own licenses. GATE found no new third-party source/assets introduced by this candidate.

## H0 → Engine Bridge audit

The H0 source/project surface remains engine-neutral:

- canonical protocol/runtime/projection contracts have no Unity dependency;
- repository search for `UnityEngine` yields no H0 source reference;
- the roadmap boundary remains canonical contract → authoring/validation → core/world → Engine Bridge abstractions → Unity implementation;
- the GATE scenario uses generic canonical IDs/types/references/extensions rather than Unity objects, transforms, assets or editor APIs;
- neither reference JSONL nor MCP requires an engine runtime to discover, author, validate, diff, journal or replay the representative world.

Therefore H0 is suitable to hand off to an Engine Bridge workpack without embedding Unity or any other engine as canonical authority.

## Release follow-up

Before any commercial distribution, retain the accepted HK07B third-party notice/license obligations for the MCP SDK. This is a packaging/release obligation, not an H0 semantic blocker.
