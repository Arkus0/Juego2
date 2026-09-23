# WP-DW-03 — Reviewed source manifest v1

Manifest ID: `dw03-pa-corpus-manifest-v1`
Authority role: reviewed adapter contract; accepted PA sources remain semantic authority.

## Adopted source family

| PA | Semantic authority | Accepted Git blob | Adopted structured surfaces |
|---|---|---|---|
| PA-01 | `Docs/research/living-world/results/PA-01.md` | `40e854e4bc2e49630c1f49a6001eb5e60896f3ae` | §1 provenance table; §3 disposition table; §5 numbered invariants; §6 every P/NC fixture |
| PA-02 | `Docs/research/living-world/results/PA-02.md` | `ce05c64f8bb0cf1f2a0090d35fa1f32757fdd33d` | §1 provenance table; §3 disposition table; §4 numbered bounded-discovery requirements; §9 every P/NC fixture |
| PA-03 | `Docs/research/living-world/results/PA-03.md` | `f2c553e01fb00a4ef2da88e9bc76f644436f0dfe` | §1 provenance table; §4 relationship vocabulary/status table; §8 every CF/NC fixture; §13 complete failure-mode table |
| PA-04 | `Docs/research/living-world/results/PA-04.md` | `d065241d5dd4ed663dd686fa288e6d8a43e1dc46` | §1 provenance table; §11 disposition table; §12 every CF fixture; §13 every NC fixture; §17 complete failure-mode table |
| PA-05 | `Docs/research/living-world/results/PA-05.md` | `d38e8b6261876b28d297893f96150f9f527d7270` | §1 donor provenance bullets; §4 PA-05-H01..H10 finding subsections + exact Decision; §7 all hard negative gates |
| PA-05 fixtures | `Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md` | `3cfb1b431acd9ffb8e1e1098e195367abd96d553` | complete fixture sections CF-01, NC-01, CF-02, NC-02, CF-03, CF-04, CF-05 delegated by accepted PA-05 §8 |

Production validates these accepted blobs from normalized UTF-8 source text before parsing. Any byte change fails closed as an unreviewed authority revision. Therefore deleting a source row cannot also shrink the expected universe and remain GREEN.

## Exact consumed schemas

Table parsers validate the complete ordered header before consuming cells:

- PA-01 §1: `Input | Exact provenance | Juego2 use`; §3: `ID | Finding/mechanism | Disposition | Juego2 meaning`.
- PA-02 §1: `Input | Exact provenance | Juego2 use`; §3: `ID | Finding / mechanism | Disposition | Juego2 meaning`.
- PA-03 §1: `Input | Exact provenance | Juego2 use`; §4: `Relationship/mechanism | Status | Juego2 recommendation`; §13: `Failure mode | Why it fails PA-03`.
- PA-04 §1: `Input | Exact provenance | Juego2 use`; §11: `Mechanism | Status | Juego2 recommendation`; §17: `Failure mode | PA-04 guardrail`.
- PA-05 H01..H10 consume exact H3 subsection identity plus exact `**Decision:**` line; §1 evidence consumes the complete labeled donor-provenance bullet block; §7 consumes the complete numbered hard-negative list.
- Fixture parsers consume complete H3/H2 fixture sections under the exact reviewed fixture parent sections; each fact stores the full accepted fixture section as material content/provenance anchor, not only its heading.

Because the accepted blob is pinned, a heading/header rename, reorder or structural alteration cannot be silently reinterpreted even if column count stays constant.

## Adopted entity types

- `pa-corpus-root`
- `pa-finding`
- `pa-disposition`
- `pa-evidence`
- `pa-fixture`
- `pa-invariant`
- `pa-failure-mode`

Stable identities use explicit accepted IDs where the source supplies them (`DL-*`, `AG-*`, `PA-05-H*`, `P*`, `CF-*`, `NC-*`). Structured rows without an accepted explicit ID use a deterministic SHA-256 identity of the exact semantic source key, never table ordinal or enumeration order.

## Exact projected fields

Every material record consumes and retains:

- `pa-id`: accepted PA owner;
- `record-kind`: typed record family;
- `domain`: `living-world`;
- `failure-family`: PA-owned query grouping (`daily-life`, `npc-agency`, `social-graph`, `knowledge-belief`, `rumour-flow`);
- `source-key`: exact source-owned row/fixture key or heading identity;
- `material-text`: the complete adopted row/list item/fixture/finding subsection needed to preserve accepted meaning.

Disposition records additionally retain exact `disposition-text` and mechanical flags `contains-adopt`, `contains-adapt`, `contains-later`, `contains-reject`, `contains-baseline`. The exact text remains authoritative; flags are query indexes and never collapse mixed forms.

Every fact also retains generic DW provenance: authority id, exact source path, exact source anchor, source digest and anchor digest.

## Exact projected relations

- corpus root `contains-finding|contains-disposition|contains-evidence|contains-fixture|contains-invariant|contains-failure-mode` → every typed child in that PA;
- every child `declared-in` → its PA corpus root;
- every finding `has-disposition` → exactly one disposition record;
- every finding `same-authority-evidence` → each accepted evidence record under the same PA authority;
- every finding `same-authority-fixture` → each accepted fixture record under the same PA authority.

`same-authority-*` is intentionally a structural/navigation relation only: it means the records are accepted under the same PA result boundary. It does not invent a claim that every fixture individually proves every finding. The exact source remains available when a consumer needs finer causal interpretation.

## Why these fields/relations are necessary

They support the frozen DW-03 suite: query dispositions without dropping rejection/deferment; traverse finding→evidence/fixture; recover complete material row/fixture text compactly; group across failure families; detect missing/renamed/wrong-target/duplicate relations; and open exact accepted provenance without rereading all PA-01..05.

## Deliberately outside the typed model

Narrative exposition between adopted structured surfaces, future consumer implementation notes, deferred empirical/runtime proof, review/handoff/DocSync metadata, donor runtime architecture, tuning constants and duplicated summary restatements are not separate typed records. They are not claimed to be replaced. They remain reachable from the accepted source path/anchor context and stay authoritative in PA.

PA-05's §6 summary mechanism-disposition table is deliberately not duplicated as a second finding universe because the primary H01..H10 finding subsections + decisions are adopted in full; duplicate projection would create two identities for one accepted finding family.

## Forward extension rule

A later accepted PA revision or PA-06+ addition must update this manifest in a reviewed DW work item: new accepted blob pin, exact consumed schema/surfaces, stable identity rule, semantic oracle expectations and frozen query/proof coverage. Unknown/unreviewed source bytes fail closed; they are not auto-adopted.