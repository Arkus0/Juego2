# GC2 ↔ Arkus runtime split

Status: **PROPOSED; binding for post-H2F-01A WPs after this amendment is accepted**. H2F-01A's narrower accepted handoff and H0/H1 authority contracts take precedence for their own tested boundaries.

**GC2 ejecuta. Arkus recuerda y conecta.** Unity/GC2 owns immediate character control, camera, interaction, local presentation and authored sequences. Arkus owns canonical persistent facts, important actor identities, knowledge, relationships, relevant schedules and causal outcomes that matter after the local sequence ends. The bridge passes reviewed Juego2 semantic IDs and small typed intents/results, not GC2 component IDs, GUIDs, Variable names or save slot IDs as canonical identity.

| Event | Local GC2/Unity execution | Arkus durable meaning | Return to presentation |
|---|---|---|---|
| Player asks a witness about a photograph | Focus, dialogue UI, animation, conditions | Witness's relevant knowledge; question asked and learned clue if future responses depend on them | Condition reads an Arkus-owned result for later dialogue |
| Suspect sees player and flees | Perception/trigger, path and chase camera | Store *seen* and chase outcome only when it affects later behaviour | Later encounter consults canonical outcome |
| Street fight | Melee, hits, camera and immediate crowd responses | Recorded witness, injury/reputation/access consequence where authored | NPC's later response reflects it |
| Darts in a bar | Entire minigame and reward presentation | Nothing unless win changes a relationship, lead or future access | When relevant, submit one reviewed result |
| Shop opens today | Door, hotspot, local shop UI | Persistent schedule/access/business state if authored | Rehydrate shop presentation on loading/revisit |

## Seam contract for the first integrations

1. Load authoritative Arkus snapshot/facts at an explicit scene/actor activation boundary; GC2 may cache a **read-only projection** for presentation. No double writes of the same meaning.
2. A GC2 Condition/Instruction (or equivalent public adapter accepted in 01A) queries a named fact or submits a bounded semantic intent with stable Juego2 identity. Arkus validates/commits under accepted H0 rules and returns success, refusal or conflict plus the current result.
3. Only after an accepted result does local gameplay present a durable consequence. A refused/conflicted mutation stays visible and retry/replan follows the inherited H0 boundary; no secret fallback write to GC2 Variables.
4. On scene re-entry or a clean reload, derive durable presentation from Arkus truth. GC2-local state can drive frame-to-frame animation/interaction and may use its own save mechanism only for a separately reviewed noncanonical need.
5. If a local sequence needs no persistent result, leave it local. A new permanent world meaning must name its Arkus owner and consumer before implementing a new schema or adapter.

H2F-01A probes Core; H2F-02 adopts only the admitted Core/version and H1 lifecycle host; GC2-00 proves the installed local player/interaction and a real small semantic round trip. Later modules (Dialogue, Inventory, Behavior, Perception, Melee, Quests) are **candidates**, each needing an exact license/version/provisioning and an explicit causal phase decision. A technically suitable module may be deferred if unnecessary. No speculative Game Creator wrapper or parallel universal framework is requested.

Unity prefab and GC2 graphs remain retained realization where admitted, not Arkus canonical state. Existing H1 materialize/observe/reconcile/rebuild behavior continues at its accepted boundary; H2F owns lifecycle classification of admitted plugin state. New gameplay must neither treat plugin-private objects as authoring semantics nor force each camera, door, gesture or minigame through Arkus.
