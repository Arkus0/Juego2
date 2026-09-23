# DW-04 acceptance pre-execution amendment 02 — close CITY §4 selector class

Status: mechanical pre-execution correction; acceptance provider/model usage remains 0/36.

Deterministic assembly had already proved two members of the same selector defect class before any model call: the original freeze failed on `loc.puerto.landing`, and replacement freeze `960ef57ce2ae99c6499fbc6cd2e9391181c20d86` failed in run `35849331838` / job `107143000824` on `loc.plaza.ayuntamiento`. In both cases a short `line_contains` needle matched the intended §4 programme row and a later table repeating the same programme identifier.

This amendment closes that entire known CITY §4 class by making the two remaining short §4 row needles (`loc.casco.shared_court`, `loc.plaza.ayuntamiento`) uniquely identify their already intended §4 ledger rows. The already-corrected Puerto needle is preserved. No task identity, semantic question, authority file, expected fact/blocker/verdict/evidence, CTX or DW meaning, model/provider/configuration, answer schema, scorer, 36-slot order, correctness rule, byte threshold or retry policy changes.

Both failed assemblies stopped before context measurement completion and before provider execution; neither produced an acceptance model response or provider request ID. The replacement freeze therefore remains pre-result. This is owner/Worker-directed mechanical pre-execution repair, not independent Reviewer approval.
