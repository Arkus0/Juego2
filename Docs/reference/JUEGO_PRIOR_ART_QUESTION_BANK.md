# Juego Prior-Art & Spike Question Bank

Version: 1.0 — 2026-09-20  
Status: **NON-BINDING REFERENCE / PROCESS_ONLY HARVEST.**

Companion to `JUEGO_KNOWLEDGE_LEDGER.md`. This file preserves **questions, behavioral patterns, falsifiable spikes and kill conditions** from `Arkus0/Juego` without carrying forward its dependency decisions, architecture, licenses, milestone graph or implementation authority.

No external package/tool/mod is approved by this document. Any actual adoption is governed by Juego2's binding `Docs/engineering/DEPENDENCY_IP_POLICY.md` and must re-check the exact version, license, boundary and replacement strategy at the time of adoption.

## 1. Why preserve the question bank

The donor repo paid for useful research before implementation. The most reusable structure was not its final answer; it was often:

```text
product question
 -> candidate prior art
 -> what mechanism might transfer?
 -> smallest falsifiable spike
 -> measurable acceptance
 -> kill condition
 -> adopt/adapt/reject decision
```

That discipline should survive. It prevents two opposite failures:

- reinventing mature patterns because the old repo was abandoned;
- inheriting a foreign architecture because a candidate looked impressive on paper.

A future Hx owner should steal the **question and experiment shape**, then re-derive the answer under current Juego2 contracts.

---

# 2. High-value prior-art questions recovered

## 2.1 Schedules, activities and interruption

Donor sources: `SYSTEMS_PRIOR_ART_DOSSIER.md`, `GAMEPLAY_SPIKE_PLAN.md`, Gothic/Exult observations.

Useful questions:

- Is a routine best expressed as ordered time blocks pointing at semantic activities rather than authored movement scripts?
- Does an activity need explicit start/loop/end or acquire/use/release phases to guarantee cleanup on interruption?
- What is the minimal override/resume model that handles danger, story, weather and temporary obligations without rewriting the base schedule?
- When an actor becomes visible after off-screen time, can the runtime project the actor coherently into its expected activity without visibly teleporting through a fake path history?
- Which schedule mistakes can be rejected at authoring time: overlap ambiguity, missing fallback, impossible POI, midnight wrap, capacity contention?

Candidate spike shape for H3:

```text
1 actor
4-5 daily blocks
3 semantic activities
3 reservable POI slots
1 contention case
1 temporary override
1 unavailable destination
1 snapshot/reload during an activity
```

Kill conditions worth preserving:

- normal routine needs bespoke code per actor;
- interruption leaves reservations/activities orphaned;
- authoring one ordinary new block is disproportionately expensive;
- the actor flickers/thrashes between competing activities;
- navigation failure can strand semantic state without a visible diagnostic/fallback.

## 2.2 Smart objects / semantic affordances / reservations

Donor research used Unreal Smart Objects as a **pattern**, not a dependency.

Useful reduced problem:

```text
POI / affordance
  -> slots or capacity
  -> semantic activity tags / requirements
  -> preconditions
  -> claim/reservation
  -> invalidation / release reason
```

Questions for H3/H4:

- Do we actually need a generic smart-object layer, or are typed POIs + small reservation semantics sufficient?
- Can one reduced model cover seats, counters, work spots, beds and interaction anchors without per-category exceptions?
- What happens when a slot becomes invalid while in use?
- Is reservation state runtime-only, reconstructible, or save-worthy for continuity?
- Can an agent inspect **why no eligible slot exists** without reading Unity state manually?

Kill rule: if the abstraction starts turning every prop into an ontology entry, shrink it to the affordances required by current gameplay.

## 2.3 Knowledge, belief, provenance and lies

Donor research compared Talk of the Town, Versu/Ensemble and Morrowind-like dialogue patterns.

Questions for H4:

- Does separating objective/runtime truth from actor belief create player-visible value beyond a boolean flag?
- Is belief provenance actually consumed by dialogue, investigation or behavior, or is it decorative metadata?
- How should confidence/uncertainty be represented, if at all, before it earns complexity?
- Can an actor hold and relay a false claim while truth remains unchanged?
- Is receiving information enough to change behavior, or must the receiver independently decide whether/how to act or relay it?
- What minimum trace lets tooling answer “who told A this, and why does A believe it?”

Falsifiable minimal scenario:

```text
truth F
A witnesses F
A tells B
B may relay/withhold to C
variant: A tells a false value
compare later dialogue/behavior
```

Kill conditions:

- player cannot perceive any difference from a simpler known/not-known model;
- confidence never affects an observable decision;
- provenance cannot be surfaced in investigation/debugging;
- rumor propagation becomes automatic global truth synchronization.

## 2.4 Dialogue filtering, fallback, saliency and chatter

Donor research compared Morrowind/OpenMW patterns and Yarn/Ink capabilities. **No old package decision travels.**

Reusable behavioral requirements/questions:

- Can dialogue options/lines be selected by structured conditions without text becoming state authority?
- Can tooling explain why a candidate line/choice lost?
- Should a failed social/relationship gate produce an explicit fallback response rather than a silent missing interaction?
- Are player topics/subjects their own duplicated state, or a view derived from player knowledge?
- Does the game need content saliency (“which eligible line is most appropriate / least recently used”) rather than only first-match branching?
- Does ambient/background chatter need interruption and resumption without taking over the main dialogue authority?
- Can multiple ambient conversations coexist without corrupting shared runtime state?
- Can localization replace text while stable semantic/line identity remains intact?

If a future H4/H5 runtime dependency is considered, benchmark the **same small conversation** through candidates and measure authoring/debugging/integration rather than comparing feature lists.

Kill condition: if the adapter becomes a second condition/state engine, stop and redesign the boundary.

## 2.5 Deterministic event interruption

Donor Spike D is worth keeping almost intact as a concept:

```text
normal routine
 -> event condition becomes true
 -> event acquires/overrides participating actors
 -> event action/outcome
 -> completion OR abort
 -> cleanup
 -> actors resume/re-evaluate
```

Questions:

- Are trigger conditions explainable?
- Are priorities deterministic when two events want one actor?
- Do complete and abort execute equivalent ownership/reservation cleanup?
- Can one-shot/cooldown semantics prevent duplicate firing?
- Can tooling answer why the event fired and why the actor was interrupted?

Absolute kill condition: any test leaves an ownerless override, stranded actor or stale reservation.

## 2.6 Off-screen simulation

Donor research looked at A-Life/Gothic-style ideas but explicitly rejected full A-Life complexity.

Keep only the bounded question:

> What is the cheapest representation that lets an off-screen persistent actor remain causally coherent when it becomes relevant again?

Likely information to measure rather than assume:

- expected semantic location/activity;
- pending/committed meaningful action;
- time advance;
- relevant events/knowledge changes;
- POI capacity if it materially affects outcomes;
- deterministic ordering/seed where choices occur.

Do not simulate pathfinding/Animator frame-by-frame off-screen merely for fidelity. Do not decide Near/Mid/Far tiers before H3/H7 measurements show what work must be skipped.

## 2.7 Third-person integration traps

Recovered from the donor Eye-of-the-Beholder benchmark/checklist. Keep as tests, never as a dependency claim:

- camera distance must not alter attack or interaction distance unintentionally;
- camera position must not become projectile/melee semantic origin unless designed;
- switching exploration/combat/cinematic modes must not reveal hidden first-person component ownership;
- target selection should not be an accidental function of presentation-only offsets;
- camera collision/reframing cannot change canonical combat facts;
- interaction focus/range should remain understandable in third person.

These are especially valuable because they are easy to miss with a first-person-derived prototype and cheap to test once H2/H6 exists.

## 2.8 World/content production throughput

Donor research around World-of-Daggerfall-style tooling and later M11/M13 plans contains a useful benchmark question independent of DFU:

> Can a representative content batch be authored, validated, reviewed and recovered through our own structured surface without requiring an external editor as hidden semantic authority?

Future post-VS benchmark should consider:

- first-pass valid rate;
- invalid mutations blocked before state change;
- retries/replans;
- human intervention;
- grouped diff reviewability;
- partial failure and resume/retry;
- idempotency/recovery boundary;
- tool calls/tokens/elapsed time when observable;
- manual authoring vs structured commands only when comparison is meaningful;
- provenance/asset-lineage inspection;
- Unity-backed and headless operations preserving the same semantic contract.

No marketing score. No invented percentage. Benchmark a fixed corpus and retain failure cases.

---

# 3. Kill-condition discipline worth restoring

The donor `SYSTEMS_PRIOR_ART_DOSSIER.md` and `GAMEPLAY_SPIKE_PLAN.md` repeatedly used **kill conditions**: an idea is not allowed to grow merely because it is elegant.

Juego2 should reuse that planning habit selectively for risky post-GATE systems.

Examples translated to current phases:

| Candidate idea | Example kill condition |
|---|---|
| richer belief confidence | remove it if player-visible behavior is indistinguishable from simple known/unknown state |
| generic smart-object framework | shrink it if ordinary POIs require exception-heavy schemas or every prop becomes modeled |
| dialogue middleware adapter | reject/redesign if it becomes a second semantic condition/state engine |
| off-screen simulation richness | cut detail if it cannot remain deterministic/bounded or if the player cannot observe the extra fidelity |
| general autonomous agency | stop expansion if initiatives loop/thrash, sabotage authored constraints or cannot explain their reasons |
| asset transformation automation | do not mass-scale if a 10–20 item pilot has unacceptable rework/failure rate |
| production batch tooling | do not promise universal atomicity; state exactly which partial failures can recover/retry |
| external authoring GUI/tool | reject as required production path if essential semantics cannot be represented/validated through Arkus-owned data/contracts |

This is not a new process gate. It is a compact way for future WP authors to write **what evidence would make us abandon an attractive idea** before sunk cost makes that hard.

---

# 4. What the old dossier does NOT prove

The donor prior-art dossier contained source/license/activity observations dated around 2026-09-15. They are useful breadcrumbs, but this harvest intentionally downgrades them to **historical research leads**.

Do not infer from this file that:

- Yarn, Ink, Talk of the Town, Ensemble, Quaternius, Kenney, ALEX, Unity packages or any mod is currently approved;
- an observed historical license is the exact license of the version we might adopt;
- a feature comparison from Juego establishes current product fit;
- a GPL/restrictive project can be copied because we only like one mechanism;
- an external tool is allowed to become the sole source of canonical state/semantics;
- the donor's proposed choice must be repeated.

At adoption time, start again from the current product question and Juego2 policy. The archive saves **discovery and question-framing time**, not review responsibility.

---

# 5. Phase-oriented question shortlist

| Phase | Questions to reopen from this bank only when relevant |
|---|---|
| H1 | Which real-asset failure classes does the bridge need to expose? Which external asset/tool candidates deserve exact-version audit? |
| H2 | Which composition/placement affordances materially reduce manual Unity work without becoming a second world model? |
| H3 | Are schedule blocks + activities + POI reservation enough? What recovery/fallback does a real NPC day expose? |
| H4 | Does belief provenance change observable play? What condition/filter/debug surface does dialogue actually require? |
| H5 | What authority/skip/cancel/timing behavior must be proven before cinematic polish? Does ambient chatter need a distinct presentation path? |
| H6 | Which combat-feel claims require instrumented play? Which third-person origin/range/camera regressions are present in our implementation? |
| H7 | What minimum off-screen state preserves causality? Which autonomy dimensions earn their CPU/state/debug cost? |
| post-VS | Can batch authoring recover from partial failure? What throughput metrics reveal actual agent advantage? Which external tools remain optional rather than semantic authorities? |

Use this as a **question index**, not a backlog. If a question has no current consumer, it waits.
