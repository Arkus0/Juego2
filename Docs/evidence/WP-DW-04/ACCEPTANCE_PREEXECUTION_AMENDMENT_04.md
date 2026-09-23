# DW-04 acceptance pre-execution amendment 04 — typed PA dispositions + full executable pinning

Status: `PRE_EXECUTION / 0_OF_36`; owner-directed mechanical closure, not independent Reviewer approval.

Historical freeze `0c57a766f8b25e0c1e22d746c05a5678ae275841` remains superseded pre-call; assembly run `35848012735` failed before any model request on an ambiguous CITY selector. Later freezes `960ef57ce2ae99c6499fbc6cd2e9391181c20d86` and `1847c45f953bfea0aabebeba0a1eb76e9d4a9a6d` also failed closed before acceptance calls. The latter materialized all routes in run `35849847978` / job `107144665438` but measured only 14.7% median saving. Amendment 03 then introduced owner-authorized compact PA navigation/source-open while retaining the 30% threshold.

Amendment 03 still represented A-PA-03 through a `pa-finding` navigation record. The controlling pre-call requirement is stricter: DL-11, DL-12, DL-13 and DL-14 must each be retrieved through the typed `pa-disposition` surface, with the route literally carrying REJECT, REJECT, LATER and REJECT respectively, while accepted PA-01 source rows remain authority. This amendment changes those four DW queries to `pa-disposition`, keeps their four exact source-row opens, and keeps PA navigation compact enough for the unchanged structural saving test.

The same refreeze also removes the competing executor and content-addresses scorer/audit, assembler, canonical executor, OpenRouter Luna adapter, retrieval adapter, assembly workflow, acceptance workflow and final verifier. The canonical executor writes a compact audit transcript plus separate raw provider evidence, requires exact Luna/OpenAI identity, preserves request hashes/freeze/assembly/source-byte accounting, and permits no replacement acceptance call after an objective invalid.

No task, semantic question, source oracle, expected fact/blocker/verdict/evidence, CTX meaning, authority, response schema, R1/R2/R3 ordering, 18-pair/36-slot budget, all-36 correctness rule, 30% threshold, model or provider changes. No acceptance campaign has been armed or executed.
