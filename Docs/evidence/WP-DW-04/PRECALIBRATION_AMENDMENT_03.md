# DW-04 pre-calibration amendment 03 — provider route and non-leaking response vocabulary

Status: **PRE-EXECUTION / RESULT-INDEPENDENT**

Effective pre-calibration universe remains: `9cbed950a3469897cf286c9a90624c240701528f`.

No DW-04 calibration provider call and no acceptance model call occurred before this amendment. GitHub Actions run `35837616972` stopped at a missing-credential precondition before campaign claim and before provider execution; it consumed zero calibration slots.

## Provider/model choice

The initial continuation implementation prepared a direct OpenAI route using `gpt-5.6-luna`. The repository does not have an `OPENAI_API_KEY`; the user has an OpenRouter credential instead. Provider/model are explicitly calibratable dimensions under `PRECALIBRATION_FREEZE`, and no model outcome exists, so the route is changed before calibration to:

- gateway: OpenRouter;
- credential: repository Actions secret `OPENROUTER_API_KEY`;
- exact model slug: `deepseek/deepseek-v4.1-flash` (no moving `latest` alias);
- serving provider: DeepSeek only;
- provider fallback: disabled;
- required-parameter routing: enabled;
- temperature: `0`;
- tools: none;
- extra reasoning control: none;
- structured output: JSON schema;
- calibration task identities/order/count, source truth, semantic oracles, CTX route, 30% context-reduction threshold and 100% correctness rule: unchanged.

The superseded OpenAI adapter is removed from the active candidate. Its earlier Git history remains auditable.

## Response-contract leakage correction

The first structured-output protocol used task-specific allowed blocker/verdict/evidence vocabularies. Even though labelled formatting-only, a singleton allowed blocker or evidence ID could reveal part of the expected answer to the model.

Before any provider call, calibration response contracts are therefore changed so every calibration task shares the same union vocabulary for:

- `allowed_blockers`;
- `allowed_verdicts`;
- `evidence_ids`.

Only the fact-key names remain task-specific because they define the requested output fields, not their values. The model must decide from the supplied authority which blocker/verdict/evidence entries actually apply. The deterministic oracle remains external and unchanged.

## Unchanged proof boundary

This amendment does not change:

- eligible task identities or calibration/acceptance partition;
- deterministic acceptance selection rule;
- any accepted CITY/PA source bytes or authority anchors;
- any calibration or acceptance expected fact value, blocker, verdict or evidence oracle;
- CTX or DW retrieval semantics;
- calibration proof budget;
- acceptance pair count/order policy;
- objective invalid-run policy;
- 30% deterministic median source-byte threshold;
- 100% required correctness;
- `INCONCLUSIVE`/restart policy.

The next provider call, if any, must occur only after an exact candidate containing this amendment, the OpenRouter adapter, protocol and one-shot GitHub Actions campaign executor is GREEN under canonical observation.