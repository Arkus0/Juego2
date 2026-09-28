# WP-H2F-01A — Game Creator 2 Core capability extraction + anti-duplication freeze

Status: **COMPLETE / ACCEPTED**  
Accepted: PR `#256`; frozen candidate `0eabed44bd73a396c0292892405e349900da0e32`; independent PASS review `#5333319449`; merge `2276dc1c2b429f273b7715fda023539feefdb086`  
Class: `PRODUCT_CHECKPOINT / CAPABILITY_EXTRACTION`  
Mode: **HYBRID / LOCAL_UNITY_REQUIRED for licensed GC2 Core bytes; remote work may prepare docs, matrices and verifier logic**  
Depends on: `WP-H2F-01` PASS + `WP-H1-GATE` PASS + ART-01 `PREFOUNDATION_INPUT` already consumed by H2F-01  
Blocks: `WP-H2F-02`

## Purpose

H2F-01 selected the immediate foundation stack with bounded spikes. Its S06 result was intentionally narrow: controller/camera/input on the representative route. This workpack answers a different question:

> Given that the owner already owns Game Creator 2 Core, which generic capabilities are present, compatible and extensible enough that H2–H6 should reuse them instead of rebuilding equivalent plumbing?

This WP does not implement H3–H6 systems and does not evaluate separately licensed GC2 modules.

## H2F-01 verdict handling

- If H2F-01 causally rejected **GC2 Core itself** for a structural reason, use the `NOT_MATERIAL` fast path and perform no new Core probes.
- If GC2 merely lost one role, including S06 controller/camera, H2F-01A still runs.
- The accepted H2F-01 player/camera/input choice is the default and is **not silently re-litigated**.
- However, H2F-01 explicitly retained the possibility that materially stronger 01A evidence could justify a different controller/camera/input realization. Therefore, if C01–C07 produce new evidence showing a clear material advantage with acceptable authority, lifecycle and replacement cost, H2F-01A may propose **one explicit baseline amendment** for that role. The amendment must name the displaced H2F-01 choice, the measured benefit, the new authority/lifecycle burden and the exact H2F-02 handoff. Without that explicit amendment, the H2F-01 winner remains binding.

## Positive claim

A later Worker can determine, without rediscovering Core:

1. what the exact owner-supplied Core version actually provides;
2. which capabilities are public/supported enough to rely on;
3. which responsibilities remain Arkus-owned;
4. which generic plumbing should not be rebuilt bespoke without new evidence;
5. which capabilities are deferred, redundant or rejected;
6. what Core state H2F-02 must adopt and lifecycle-classify.

## Mandatory evidence

1. Exact installed Core identity, provisioning route, assemblies/public surfaces, settings/assets and dependencies.
2. Capability matrix covering every materially relevant Core family discovered by inventory.
3. Real bounded Unity evidence for every high-leverage reuse claim below.
4. Arkus ↔ GC2 authority map proving Core execution/presentation state does not become canonical world truth.
5. H2–H6 savings map with explicit `DO_NOT_DUPLICATE` guidance where evidence justifies it.
6. H2F-02 adoption/lifecycle handoff for every retained Core state family.
7. Licensed/vendor bytes remain outside the repository; only Juego2-owned adapters, recipes and evidence may be committed.

## Negative gates

FAIL if:

- H2F-01 is silently reopened or an S06 replacement occurs without the explicit baseline-amendment rule above;
- separately licensed Inventory, Dialogue, Quests, Behavior, Perception, Melee, Shooter or other modules are treated as Core;
- `DO_NOT_DUPLICATE` is asserted from documentation alone where representative evidence is required;
- GC2 Variables, SaveLoad IDs, component IDs or plugin-private identities become canonical Arkus/CITY/story identity;
- a Juego2 semantic contract depends on private/internal GC2 implementation where a small public adapter can keep it replaceable;
- H3/H4/H5/H6 product systems are implemented here;
- extra middleware is installed merely to complete a probe;
- licensed GC2 bytes are committed to the repository;
- capabilities from a newer vendor version are claimed when absent from the exact installed version.

## Capability inventory

At minimum classify the installed Core surface around:

- Character kernel for Player/NPC;
- input, locomotion/rotation and navigation hooks;
- camera/shot facilities;
- interaction focus/interact/Hotspots;
- Instructions/Actions, Conditions and Triggers/Events;
- supported custom Instructions, Conditions, Events and Properties;
- Variables and safe non-canonical uses;
- character model/avatar/animation execution;
- Gestures and States;
- IK/look-at/feet/contact helpers where present;
- footsteps/surface hooks;
- ragdoll/recovery where present;
- save/load/storage extensibility;
- any other materially relevant Core runtime/editor authoring or helper surface discovered in the exact version.

For every family record: `PRESENT / ABSENT`, evidence, public route, retained/generated state, likely horizon, authority risk, replacement risk and one disposition.

## Capability dispositions

Use exactly one:

- `USE_FOUNDATION_NOW`
- `USE_LATER_DO_NOT_DUPLICATE`
- `LOCAL_EXECUTION_ONLY`
- `DEFER_EVALUATION`
- `REDUNDANT_WITH_SELECTED_STACK`
- `REJECT_CAPABILITY`

`USE_LATER_DO_NOT_DUPLICATE` is a planning guard, not future-system adoption.

## Required bounded probes

### C01 — Exact Core surface + state inventory

Inventory the actual installed version, public/runtime/editor surfaces, settings/assets, dependencies and supported extension points. Reflection may assist discovery but cannot become the production seam.

### C02 — Character composition + S06 amendment check

Using the representative ART/Quaternius humanoid:

- instantiate one Player and one NPC-compatible Character path;
- prove Core can coexist with the accepted H2F-01 route without duplicate canonical identity;
- exercise model/avatar assignment and one locomotion/navigation command;
- compare only the controller/camera/input facts needed to decide whether the explicit baseline-amendment threshold is met.

If no material new advantage is demonstrated, record `REDUNDANT_WITH_SELECTED_STACK` for that role and keep H2F-01 unchanged.

### C03 — Interaction / affordance execution

Use three tiny non-keeper props such as door, chair and bottle. Prove focus/interact/Hotspot routing with Juego2-owned semantic IDs/tags outside GC2. At least one interaction must be Player-invokable and, where supported, one must be callable by an NPC/Character or scripted Character path.

This proves execution only, not Inventory or persistent world-object semantics.

### C04 — Character presentation helpers

Exercise, where present: one Gesture, one looping State, look-at/IK equivalent, footsteps on two surfaces, and one ragdoll→recovery transition that requires no extra licensed module.

### C05 — Arkus ↔ GC2 public scripting seam

Prove through supported/public surfaces:

- one custom GC2 Condition reads an Arkus-owned test fact;
- one custom GC2 Instruction requests/records a bounded Arkus-owned semantic transition;
- one public Property/equivalent resolves a Juego2-owned entity/fact key;
- one Trigger/Event path executes the composition.

GC2 may execute/present the effect; Arkus remains semantic authority.

### C06 — Save-host feasibility

Roundtrip a deterministic versioned Juego2-owned payload through the supported Core save extensibility path. The restored digest/version must match. Core Variables remain local/presentation/session conveniences unless separately admitted later.

This does not replace H0/H1 snapshot/replay authority.

### C07 — Selected-stack coexistence + authoring handoff

Run the accepted H2F-01 route plus retained C02–C06 Core capabilities in one disposable composition and prove:

- clean compile/play;
- no unexplained duplicate player/camera/input ownership;
- no plugin-private canonical IDs;
- a documented public authoring recipe for H2F-02/H2F-03/Astra.

## Required outputs

Under `Docs/evidence/WP-H2F-01A/` publish at minimum:

- `PREDECESSOR_CONTRACT_CHECK.md`
- `CORE_VERSION_AND_PROVISIONING.md`
- `CORE_SURFACE_INVENTORY.md`
- `CORE_CAPABILITY_MATRIX.csv`
- `CORE_STATE_INVENTORY.md`
- `PROBE_RESULTS.md`
- `ARKUS_GC2_AUTHORITY_BOUNDARY.md`
- `HORIZON_SAVINGS_MAP.md`
- `H2F02_CORE_HANDOFF.md`
- `PUBLIC_AUTHORING_SURFACE.md`
- `RESIDUE_AND_LICENSE_LEDGER.md`
- owner-visible captures where feel materially affects a disposition
- exact-SHA verifier proving no mandatory capability/probe row is unresolved
- `S06_BASELINE_AMENDMENT.md` only if C02 actually changes the accepted controller/camera/input realization; otherwise record `NO_AMENDMENT` in `PROBE_RESULTS.md`.

## H2–H6 savings map

Evaluate without implementing the later phase:

- **H2:** Character/camera/input/navigation/presentation pieces actually admitted.
- **H3:** interaction/Hotspot/visual-scripting/property primitives usable beneath future world-object/Inventory semantics.
- **H4:** Character executor, navigation hooks, States/Gestures, IK/look-at, footsteps/ragdoll/presentation helpers.
- **H5:** only Core primitives available without Melee; no claim of a combat system.
- **H6:** generic visual-scripting, variable/presentation and save-host plumbing; no claim that Core replaces Quests/Dialogue.

Every saving must name the bespoke work reduced and the responsibility that remains Juego2/Arkus-owned.

## Authority boundary

```text
Arkus / Juego2 semantics
  = canonical identity, facts, causal history, city/living-world/story authority

GC2 Core
  = admitted local execution, interaction, character/presentation and authoring primitives

Unity
  = scene, render, physics and serialized realization

Astra
  = author using approved Juego2/GC2 public surfaces
```

## Handoff to H2F-02

If Core remains admitted, H2F-02 must freeze exact version/license/provisioning, classify retained Core settings/assets/state under the H1 lifecycle matrix, preserve the public replacement boundary, distinguish local GC2 Variables/save state from Arkus-owned payload/state, and avoid separately licensed modules.

If an S06 baseline amendment is accepted, H2F-02 must adopt that amended controller/camera/input realization instead of the displaced H2F-01 realization and retain the causal amendment evidence.

## PASS only if all are true

- H2F-01 PASS is consumed without silent reopening;
- NOT_MATERIAL is used only for a structural Core-wide reject;
- otherwise the exact Core surface is inventoried and every material family has a disposition;
- C02–C07 produce real evidence or explicit causal absence/reject/defer results;
- representative Juego2/Quaternius content is used rather than vendor demos alone;
- the public Arkus↔GC2 seam is proven or its limitation recorded;
- save-host feasibility, if claimed, roundtrips a Juego2-owned payload without transferring semantic authority;
- H2–H6 receive explicit anti-duplication guidance without early implementation;
- no separately licensed module or restricted vendor byte is smuggled into scope;
- H2F-02 receives a complete Core lifecycle/adoption handoff;
- any player/camera/input change exists only as an explicit evidence-backed baseline amendment;
- the exact candidate has no unresolved mandatory capability/probe row.

## Explicit non-claims

H2F-01A does not prove or implement Inventory, Living World decision semantics, combat quality, Quests/Dialogue, keeper CITY geometry, ART-01 `KEEPER_READY`, or replacement of Arkus canonical state by GC2 Variables/SaveLoad.
