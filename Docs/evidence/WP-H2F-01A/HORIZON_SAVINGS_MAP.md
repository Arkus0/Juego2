# H2–H6 savings map (anti-duplication guidance, no early implementation)

Each saving names the bespoke work it removes, the evidence that justifies the `DO_NOT_DUPLICATE` guard, and what stays Juego2/Arkus-owned. `USE_LATER_DO_NOT_DUPLICATE` is a planning guard: a later WP that needs the capability reuses GC2 Core through the adapter instead of building an equivalent. It is not adoption of a later system here.

No H3–H6 system was implemented by this WP. The probes are disposable, non-keeper fixtures.

## H2 — character, camera, input, navigation, presentation actually admitted

| Admitted now | Bespoke work avoided | Evidence | Still Juego2-owned |
|---|---|---|---|
| GC2 Character as the player body (S06 amendment) and the NPC body kernel | a project-owned locomotion controller for the player, plus a separate NPC body stack | C02/C02g (route parity), C07 (composition) | input action map, UAL clips, avatar mapping, character preset values |
| GC2 Main Camera + Third Person shot as the player camera (S06 amendment, owner choice) | a camera-root rig + Cinemachine tuning for the enterable-threshold framing | C02g vs C02 (equal route metrics, rendered body visibility), C07 | camera preset values; orbit/zoom inputs are Juego2 actions |
| GC2 player input unit reading the Juego2 Input System map | a bespoke input-to-motion layer | C02, C07 | action asset and its lifecycle |
| ChangeModel + Kinematic animim driving the Juego2 UAL controller | a speed/blend parameter driver | C02 (`J2_UAL_Locomotion`, foot 0.026 m) | UAL content, controller asset |
| Juego2 adapter surface (Condition/Instruction/Event/Property) | — (this is the seam itself) | C05 | the transition table and all facts |

Not admitted for H2:

- Cinemachine for the player camera (displaced by the amendment; H2F-02 decides whether it stays for other roles);
- GC2 default device inputs (redundant);
- GC2 locomotion clips/state assets (redundant with UAL).

## H3 — world objects, affordances and interaction

| Reuse (DO_NOT_DUPLICATE) | Bespoke work avoided | Evidence | Still Juego2-owned |
|---|---|---|---|
| Interaction focus + interact (`Character.Interaction`, `InteractionTracker`, Trigger **On Interact**) | proximity/focus selection, interact dispatch for player **and** NPC | C03: player focus → J2 Interact → door/mug; NPC focus → public `Interact()` → chair; rejections honoured | world-object identity, affordance semantics, allowed transitions, Inventory semantics (not Core) |
| Hotspot (radius activation) + Spots | affordance highlight/prompt presentation | C03: inactive → active near door | which objects are interactable and why |
| Stock visual-scripting library, gated by Arkus | per-object glue scripts for local effects (lights, logs, sounds, transforms of presentation objects) | C05 (stock Light/Debug/CheckConditions driven by Juego2 conditions/properties) | canonical state changes (only through Arkus requests) |

Not claimed: Inventory, held-item semantics or persistent world-object state. `props.attach` is deferred and was not probed.

## H4 — living-world character execution and presentation

| Reuse (DO_NOT_DUPLICATE) | Bespoke work avoided | Evidence | Still Juego2-owned |
|---|---|---|---|
| NavMesh driver + `Motion.MoveToLocation/Follow` | a NavMeshAgent executor wrapper | C02 (NPC 7.5 s arrival), C07 (NPC to seat) | NPC decisions (where/why), schedules, living-world semantics; the AI Navigation NavMesh |
| Gestures + States (UAL clips through GC2 playables) | an animation-layer system for one-shot gestures and looping poses (talk, sit, drink) | C04: gesture excursion 0.607 m then stops; sitting state 0.822 → 0.47 m, held a full loop, restored | clip selection and meaning |
| Look-at IK (`RigLookTo`) | head/spine look-at on the Quaternius skeleton | C04: 45.2° → 3.9° | attention targets |
| Footsteps + material sounds on ART textures | surface detection + footstep audio routing | C04: cobble/wood → distinct clips 18/19; C07: 62 player step events | the surface ↔ sound mapping asset; Fulcrum tuning for UAL (residual) |
| Ragdoll + recovery | knock-down/fall ragdoll and get-up | C04: ragdoll hips 0.153 m → recovered 0.898 m, walks 1.31 m | when a fall happens (no combat claim) |

Deferred: other IK rigs (feet plant, align ground, lean, breathing, twitching, aim), markers as identity.

## H5 — combat (only Core primitives, no Melee)

Core contains combat scaffolding without the separately licensed Melee module: Combat, Weapons, Shields, Poise, Invincibility, Targets, Reactions and Munition. It was **not exercised** and is `DEFER_EVALUATION`. The ragdoll/recovery reuse above is the only H5-relevant saving with evidence. No combat system is claimed.

## H6 — visual-scripting, variables/presentation and save-host plumbing

| Item | Guidance | Evidence |
|---|---|---|
| Custom Instruction/Condition/Event/Property adapters | Reuse and extend the frozen seam; do not build a parallel scripting bridge. | C05 |
| Variables | Presentation/session only. Never facts. | C06 (GC2 persists them in every save) |
| Save host (`IGameSave` + `TDataStorage`) | `DEFER_EVALUATION`. It can transport an opaque Arkus payload (roundtrip digest match, foreign version refused, no registry residue). However, GC2 load reloads scenes and its greedy reset writes the subscribe-time snapshot. H6 must decide explicitly before relying on it. This WP does not replace H0/H1 snapshot/replay authority. | C06 |
| Quests/Dialogue | Not Core. No claim that Core replaces them. | — |
