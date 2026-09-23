# DW-04 pre-call ready receipt

Status: `PRE_CALL / ACCEPTANCE_NOT_ARMED / 0_OF_36`.

Effective acceptance freeze: `70904fd670680fe71f2a534a5029cd095799edd0`.
Immutable context assembly commit: `8780bf945ad110deebe316467cfbacc4db3512a2`.

The final deterministic assembly is bound to that freeze and passed the pre-call structural gate:

- six selected tasks: A-CITY-01, A-CITY-02, A-CITY-03, A-PA-01, A-PA-02, A-PA-03;
- 18 matched pairs / 36 frozen execution slots;
- `structural = true`;
- median CTX injected source bytes = `2258.5`;
- median DW injected source bytes = `1380.5`;
- median saving = `0.3887535975204782` (38.87535975204782%), above the unchanged 30% threshold;
- all required source-oracle context is present on both routes;
- typed DW queries were replayed deterministically during materialization;
- A-PA-03 uses `pa-disposition` for DL-11..DL-14 and exact accepted PA-01 source rows carrying REJECT / REJECT / LATER / REJECT;
- accepted source documents remain semantic authority.

The frozen acceptance protocol pins scorer/audit, assembler, canonical executor, OpenRouter Luna adapter, typed retrieval adapter, assembly workflow, acceptance workflow, final verifier and observer. The acceptance invalid policy is terminal/no-replacement and semantic misses are never rerun.

Model remains `openai/gpt-5.6-luna-20260709`, with OpenRouter serving provider constrained to `OpenAI` and provider fallback disabled.

No acceptance trigger is present in the PR body. No acceptance campaign-start claim exists for the assembly candidate. No acceptance provider request has been made. This receipt is non-executable evidence only; it does not modify or supersede the frozen experiment substrate or assembly.
