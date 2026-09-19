# WP-HK-02A representative content-shape probe

Date: 2026-09-19
Product sources: `Docs/art/VISUAL_BIBLE.md` §4–5/§8 and `Docs/art/SETTING.md`
Status: complete against observed candidate `f3c9a8f918a4389298cbb4a9515e4cd701f990ad`; final evidence-bearing SHA revalidation pending

## Bounded scenario

Model the H2 hero slice as authored content: a fictional Potes/Liébana town root, plaza, one or two streets, bar/shop interiors, market furniture and six NPC objects. Exercise only the generic state shape that HK02A owns:

| Example | Canonical HK02A expression | Boundary exercised |
|---|---|---|
| Ana's profile/schedule document | extension `arkus.npc-profile@1`, subject `npc.ana` | per-object resource identity |
| Ana works at the shop and visits the plaza | typed extension dependencies `works_at -> building.tienda`, `visits -> plaza.mayor` | inspectable referential integrity |
| One bench's authored placement document | extension `arkus.spatial@1`, subject `bench.01` | independent object anchoring |
| Weather/lighting authoring policy | global extension with no subject | retained world-global semantics |

Payload bytes remain opaque. `arkus.npc-profile` and `arkus.spatial` are illustrative owner namespaces, not schemas introduced by this workpack.

## Results against the declared contract

| Question | Expected HK02A answer | Classification |
|---|---|---|
| Can two NPCs use the same extension owner/version without sharing one blob identity? | Yes; `SubjectId` distinguishes their canonical resources. | current-WP acceptance |
| Can validation see an object identity used by opaque extension semantics? | Yes, when the producer declares it as a typed dependency; undeclared payload meaning is not inferred. | current-WP acceptance and explicit trust boundary |
| Does deleting the shop leave Ana's declared `works_at` target valid? | No; candidate validation must reject the dangling dependency before commit. | current-WP negative conformance |
| Can reads and mutations address Ana's data rather than the global owner blob? | Yes; the composite extension identity propagates through HK03/HK04. | current-WP acceptance |
| Does moving one bench avoid whole-world optimistic concurrency? | No; HK04 CAS remains whole-world. The change/resource identity is finer, but automatic merge is not claimed. | non-blocking residual/product decision |
| Can Arkus diff individual schedule or transform fields inside opaque bytes? | No; payload remains one field. Typed dependencies are separate semantic edges. | explicit out-of-scope limitation |

## Adjacent findings

- **Authored versus live time:** the canonical state here describes authored schedule data, not a ticking simulation. `WP-HK-06` owns the explicit authored-state snapshot/journal boundary; live clock, transient NPC position and deterministic simulation remain outside HK02A.
- **Behavior/dialogue/quest logic:** data-versus-code authoring remains a later reviewed architecture decision. HK02A supplies only a generic opaque document plus declared dependencies.
- **Asset binding:** engine/asset adapters remain H1 scope; no prefab or Unity type is introduced into the canonical kernel.
- **World partition:** the current single-world aggregate remains sufficient for the bounded H2 hero slice. Valley-scale partitioning is a future product decision, not evidence against this additive anchor.

None of these adjacent findings falsifies an accepted predecessor guarantee. They do not justify reopening HK01–HK04 without concrete evidence on their effective paths.

## Candidate execution

The completed candidate exercises the bounded shape through the canonical surfaces:

- two NPC subjects carry the same owner/schema version as distinct resources;
- query filtering returns Ana's object-scoped descriptor and dependency count;
- bounded read returns subject plus sorted typed dependencies and reconstructs the exact canonical world hash;
- transactional put addresses two subjects independently and reports dependency field/reference effects;
- removing either Ana (the subject) or the shop (a dependency target) fails candidate validation before commit;
- global extensions used by the accepted regression fixtures retain their prior call shape and semantics.

Observed evidence: Actions run `35454985416`, focused HK02A 8/8 GREEN and full regression 95/95 GREEN on `f3c9a8f918a4389298cbb4a9515e4cd701f990ad`.

## Probe verdict

The optional subject plus typed dependency surface is sufficient for the bounded H2 content shape and is the smallest generic change that closes the observed per-object anchoring and referential-visibility gaps. The candidate introduces no content-specific validator, payload interpreter, simulation system or gameplay schema. No in-scope blocker or predecessor reopen condition remains.
