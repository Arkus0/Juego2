# WP-HK-07B Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 1
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-07B/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-07B`.
- Baseline: `6173471245da081b21cf66fd164cbb3a30dcba1d`.
- Branch: `wp/hk-07b-mcp-projection`.
- Direct accepted dependency: `WP-HK-07A`.
- Implementation/test SHA observed GREEN: `abf22ba58a7645f8a74e25fb814d5613cb014e95`.
- Actions run: `35494482169`; artifact: `10599982945`.

This is Worker quality-gate evidence only. It is not an independent Reviewer verdict.

## Predecessor split rechecked

`Docs/evidence/WP-HK-07B/WORKER_PLAN.md` still matches the accepted HK07A evidence and current baseline. HK07B consumes the frozen `arkus.neutral-projection@1`, composed canonical inventory/dispatch authority, JSONL reference transport and HK01-HK06C semantics. It owns only MCP projection/conformance/dependency isolation. No concrete evidence invalidates the predecessor guarantee.

## Complete diff challenge

The baseline→candidate diff was re-read against the exact WP, foundational proof standard, worker/reviewer protocol and dependency policy. Product changes are limited to one `Arkus.Harness.Mcp` executable/adapter plus package/solution wiring. No source under Protocol, Runtime, Projection, Game Core/World/Validation/Authoring or the accepted JSONL CLI implementation was modified to make MCP fit.

The candidate was challenged for:

- a hand-maintained MCP command/schema registry that could self-shrink;
- MCP discovery omitting scoped providers or inventing adapter-only tools;
- direct Authoring/World/mutation dependencies bypassing neutral dispatch;
- success/error schema drift hidden behind input-only discovery;
- transport names or `_meta` leaking into the accepted neutral request;
- SDK types/dependencies creeping into Protocol/Runtime/Projection;
- cancellation becoming an MCP framing exception before neutral admission;
- timeout/canonical errors changing semantic class across transports;
- an MCP naming limitation being solved by changing canonical identity;
- representative mutation succeeding while provenance/diff/replay diverged;
- proof circularity where MCP discovery was also the only expected inventory;
- forbidden HTTP/cloud/Unity/vendor/HK08 scope.

## Finding fixed during Worker pre-review

**Finding 1 — MCP call cancellation could terminate in framing before the accepted neutral admission semantics.**

The first `CallToolHandler` called `cancellationToken.ThrowIfCancellationRequested()` before invoking the neutral projection. A pre-cancelled MCP request could therefore escape as an SDK/framing cancellation instead of the accepted `projection.cancelled` neutral outcome, even though HK07B explicitly owns clean cancellation mapping.

The repair routes the call through a single `InvokeProjectedAsync` seam that resolves the canonical projected definition and delegates the token/deadline to `NeutralProjectionService.InvokeAsync`. A causal synthetic-provider control pre-cancels the token and requires `projection.cancelled` while `FixtureEngineHandler.InvocationCount` remains exactly zero.

## Regression integration reconciliation

Adding the MCP production assembly exposed an inherited test-harness load-closure assumption in HK01's independent `src/*` reflection scanner: it correctly discovered the new assembly, but the test process initially lacked `ModelContextProtocol.Core` while calling `Assembly.GetTypes()`. The fix did **not** exclude MCP or weaken `RouteUniverse`; the test project receives the pinned SDK as `PrivateAssets=all` solely so the existing independent production universe remains loadable. Full regression is green afterward.

This was implementation integration work, not evidence that HK01's canonical route guarantee is false: MCP contains no canonical capability handler and the scanner still enumerates every production assembly.

## Causal/independent proof challenged

- Expected production capabilities/schemas come from an independently composed canonical contract, while actual MCP tools come from a real MCP stdio process.
- Missing, extra and duplicate identities are each defect-injected and detected.
- Full canonical definition metadata is compared, and deliberate success-schema drift turns the oracle red.
- A synthetic scoped provider unknown to MCP appears automatically without adapter registry edits.
- Pre-cancelled MCP admission leaves the synthetic canonical handler at zero invocations.
- MCP's `@` naming pressure is absorbed as `_40`; deliberately adding `toolName` to the neutral field universe turns the framing-leak oracle red.
- Protocol/Runtime/Projection have no MCP dependency; deliberately injecting the SDK into Projection turns the replacement-boundary oracle red.
- Real JSONL and MCP processes execute summary, validation, mutation, provenance, snapshot, diff, import and replay, then compare normalized neutral outcomes.

## Green observation

Exact implementation/test SHA `abf22ba58a7645f8a74e25fb814d5613cb014e95` passed:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk07B*`: 8/8 GREEN;
- full regression: 159/159 GREEN;
- clean before/after exact-SHA observation: YES;
- Actions run `35494482169`: SUCCESS;
- artifact `10599982945`.

## Scope/proof-budget conclusion

No alternate semantic registry, canonical mutation authority, neutral-contract amendment, SDK leak or forbidden future scope remains in the diff. Remaining risks are bounded in `RESIDUAL_RISK.md`. The final causal controls correspond directly to explicit HK07B acceptance/negative-conformance clauses; another proof-expansion cycle would not be justified.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

No known in-boundary blocker remains. The resulting documentation/verification reconciliation SHA must pass `scripts/hk07b-verify-exact-sha.sh` unchanged before freeze and handoff to an independent Reviewer.
