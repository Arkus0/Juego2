# Arkus ↔ GC2 Core authority boundary

```text
Arkus / Juego2 semantics
  = canonical identity, facts, causal history, city/living-world/story authority
      ▲  reads facts / submits bounded transition requests (Arkus decides)
      │
Juego2-owned adapter (Juego2.H2F01A.Gc2Adapter: public GC2 bases only)
      │  Condition · Instruction · Event · Property · IGameSave · TDataStorage
      ▼
GC2 Core
  = admitted local execution: character bodies, interaction focus/interact, presentation helpers,
    visual-scripting execution, local variables
      │
Unity
  = scene, render, physics and serialized realization (H1 bridge owns materialize/reconcile)

Astra = authors through the approved Juego2/GC2 public surfaces (PUBLIC_AUTHORING_SURFACE.md)
```

## What the probes proved

| Claim | Proof | Evidence |
|---|---|---|
| GC2 types cannot reach Arkus state except through Juego2 code | The Arkus stand-in assembly references **nothing**. Its sources contain no `GameCreator` token. Only the adapter assembly references both GC2 Core and Arkus. The verifier checks all of this structurally. | `probe_project/Assets/H2F01A/Arkus/*`, `Gc2Adapter/*.asmdef` |
| Juego2 identity is never a GC2 identity | Every character, prop, rule and lamp carries a Juego2 key through `ArkusEntityBinding` (a bridge locator). The identity audit scanned every GC2 component in the scene for serialized ID/GUID/save-id fields: **none** in C02 or C07. The keys are unique. | `results/c02_result.json`, `results/c07_result.json` |
| GC2 executes, Arkus decides | Door, chair and mug interactions go through GC2 focus/interact and Trigger lists, but the semantic change is an `InstructionArkusRequestTransition` that Arkus accepts or rejects. A second "open" is **rejected** (`PRECONDITION:open`) and the stock GC2 presentation instruction after it does not run (1 presentation line, not 2). An NPC trying to take a mug held by the player is rejected. | `results/c03_result.json` |
| A GC2 object without a Juego2 binding cannot act semantically | The unbound barrel has the same GC2 Trigger and adapter instruction. Arkus rejects it with `UNBOUND_ENTITY`: a GameObject or GC2 component is not an identity. | `results/c03_result.json` |
| GC2 visual scripting reacts to Arkus and requests through Arkus | An Arkus fact change raises the custom GC2 Event. A custom GC2 Condition reads the Arkus fact. A custom Instruction requests `lamp.on` from Arkus. Only then does the stock GC2 Light instruction present it, targeting the lamp resolved through a Juego2-key Property. When the door is closed again the Trigger runs but stops at the Condition: no second request. A rejected transition raises no event (0 runs). | `results/c05_result.json` |
| GC2 save transports an opaque Juego2 payload; Arkus stays the judge | The versioned payload round-trips with an exact digest match. A foreign version (99) in GC2 storage is **refused** by Arkus (`VERSION:99`). No PlayerPrefs residue remains when the Juego2 storage backend is used. | `results/c06_result.json` |

## Binding rules (handed to H2F-02 as lints/presets)

1. **No GC2 type in Arkus or CITY/story contracts.** Arkus assemblies do not reference `GameCreator.*`. GC2 is reached only through the Juego2 adapter assembly.
2. **Juego2 identity = Juego2 keys.** Keys are carried by `ArkusEntityBinding` (or its H2F-02 successor). No GC2 Variable name, save key, marker ID, `Remember` unique ID, component instance ID or asset GUID may be used as an Arkus/CITY/story identity.
3. **Semantic transitions only through Arkus requests.** Stock GC2 instructions may present or execute locally *after* an accepted Arkus transition (the pattern: request, stop if rejected, stock effect). They must never directly set a keeper object's canonical properties. This covers transform, active state, instantiate/destroy, and variables standing in for facts.
4. **Variables are presentation/session only** (`LOCAL_EXECUTION_ONLY`). GC2 writes its global-variable keys into every save (C06), so a Variable used as a fact would silently fork persistence.
5. **`Remember` is not admitted on keeper objects** (`REJECT_CAPABILITY`).
6. **GC2 must not load/reload keeper scenes** (`scenes.instructions` = `REJECT_CAPABILITY`). C06 shows that `SaveLoadManager.Load` always reloads the saved scenes by name (`gc2_load_reloaded_scene=true`). If that path were used for keeper scenes it would bypass H1 materialize/reconcile.
7. **The save host is not an Arkus authority.** GC2's greedy reset calls `OnLoad` with the snapshot taken at `Subscribe` time before it loads the slot. In C06 a refused foreign payload therefore left Arkus at the subscribe-time baseline (`tampered_digest_equals_reset_baseline=true`), not at the pre-load state. A production host must make that reset explicit, for example by ignoring the reset instance or subscribing only at a defined Arkus baseline. H6 owns that decision (`save.host_storage` = `DEFER_EVALUATION`). This WP does not replace H0/H1 snapshot/replay authority.
8. **Input ownership stays Juego2's.** GC2 reads the Juego2 action map through its public InputAction value:
   - the GC2 player reads `J2_Input/Player/Move`;
   - the amended GC2 camera reads `Look` and `Zoom`.

   Juego2 creates, binds, enables and disables the map. NPC presets set their dormant player-unit input to none. With these rules the enabled actions are exactly the Juego2 ones, plus Unity's URP debug-menu actions (C02/C07). Workspace-A development runs without them showed GC2 enabling loose `Primary Motion`, `Secondary Motion` and `Zoom` device actions.
9. **Adapter types are frozen seam names.** GC2 stores adapter entries as `[SerializeReference]` with class/namespace/assembly strings (state S6). Renames need `[MovedFrom]`.

## What stays Juego2/Arkus-owned

- the canonical identity/fact model and the transition rules;
- the input action map;
- the camera preset values (the camera itself is GC2 under the S06 amendment);
- the UAL animation content and the explicit avatar mapping;
- ART materials and footstep sound mapping;
- AI Navigation NavMesh generation;
- the decision of where/why characters move or interact;
- save/replay authority.
