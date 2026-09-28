# First urban adventure slice — player-facing target

Status: **ACCEPTED product target** via PR `#257`, Reviewer PASS `#5333313032`, merge `6bf2d6e74215be51e73d85f659a2a734752ea34c`; `GC2-SLICE` still owns implementation acceptance after the preceding workpacks. Duration target: **20–30 minutes of real first-play play**, measured on the retained route without counting setup, reload or debug steps.

The player leaves a lodging with a photograph or clue about a murder; asks people in a commercial area; learns something that points toward the port; obtains access through a person, job or another legible route; spots and follows a suspect; is discovered; plays a chase ending in confrontation or a bounded fight; gains a key or new lead; returns to an earlier place and finds a changed response based on the event. A short everyday activity (for example a bar game or small job) offers a reason to linger and can affect the encounter when authored. The final beat opens another thread.

The playable target is daily life, investigation and adventure in one continuous city experience. At least one named NPC must recall an action, at least one meaningful result must survive scene exit/reload/revisit, and the changed response must be visible without developer tools. More than one outcome to the pursuit or access problem is desirable when authored; if only one exists, the slice must be honest about it. A deterministic scripted sequence may carry a dramatic beat; it may not impersonate a systemic consequence it does not store.

| Moment | Player-visible check | Authority |
|---|---|---|
| Photograph and questions | Clue changes what a witness says | GC2 presents, Arkus stores relevant clue/knowledge |
| Activity/work | Short playable action has readable result | Local unless future consequence is authored |
| Following and chase | Player can notice, lose or catch the suspect with an explicit outcome | GC2/Unity plays; Arkus records consequential result |
| Fight/confrontation | Usable bounded combat or confrontation outcome | GC2 local; Arkus stores witness/access consequence when authored |
| Return visit | Prior encounter changes response, access or routine after leaving and reloading | Arkus durable state drives local presentation |

PASS requires a continuous playable capture, first-play timing and observation, a reload/revisit demonstration of the changed state, third-person visual approval on the urban keeper block, and a brief residual list. `GC2-SLICE` cannot pass using the inland pilot as evidence that port geography already exists; `CITY-URBAN-00` and a first urban block must be accepted before this slice claims the new setting. No claim of four or five finished districts, citywide actor simulation or 80–120 concurrent NPCs follows from this slice.
