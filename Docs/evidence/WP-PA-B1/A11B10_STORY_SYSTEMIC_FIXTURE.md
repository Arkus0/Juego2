# WP-PA-B1 — A11B-10 Story/Systemic Composition Fixture

Status: **WORKER EVIDENCE / REVIEW PENDING**  
Scope: PA-09 A11B-10 only  
Execution class: `RESEARCH_BATCH`  
Date: 2026-09-26

## Purpose

Close the single A11B-10 research oracle required by the frozen PA-B1 plan: demonstrate one concrete collision between a systemic consequence and authored story reservation using an explicit resolution policy, while preserving prior causal history and leaving neither actor authority nor GameFlow authority stranded.

This fixture does not define a universal story/systemic scheduler, add runtime, reopen H0/GameFlow, or select final algorithms/tuning.

## Fixture SS-B1-01 — Authored reservation collides with a pending systemic consequence

### Prior systemic cause

Before 18:00, the Player validly returns `bar-service-stock` to Bar F01 through the normal PA-07 material seam.

Owned state becomes:

```text
bar-service-stock = RESTORED
cause = valid embodied Player action
service consequence = eligible to be consumed by normal work/service decisions
```

The causal fact is durable only at the bounded semantic level already required by PA-07/09: the stock was restored, by a legitimate actor action, and this may change later opportunities. It does **not** pre-command Carmen's next action.

### Authored reservation

A story beat has already reserved:

```text
actor: Carmen
place: Bar F01 back room
window: 18:00–18:20
reservation authority: GameFlow/authored story
restriction: Carmen cannot be allocated to ordinary bar-service work during this exclusive story window
release condition: story beat reaches its terminal exit OR its explicit abort path
```

The reservation is an authored restriction on actor/place/time. It does not own Carmen's beliefs, relationships, general routine, or future autonomous choices.

### Collision

At 18:05, the restored stock means ordinary service is again materially possible, and PA-07 exposes that changed service opportunity. Without composition, a naive systemic continuation could attempt to use Carmen immediately for service while GameFlow still owns the exclusive story reservation.

This is the A11B-10 collision.

## Selected collision policy: `DEFER`

For this fixture, the explicit resolution rule is:

> **DEFER the conflicting systemic actor allocation while the authored exclusive reservation is active. Preserve the systemic cause and shared material state; do not erase, duplicate, or convert them into a quest-only flag.**

Concretely:

```text
18:05 collision detected
 -> GameFlow reservation remains authoritative for Carmen + reserved place/window only
 -> no ordinary service allocation may seize Carmen during the reservation
 -> bar-service-stock remains RESTORED under PA-07
 -> causal lineage of the Player restoration remains intact
 -> no direct Carmen.nextAction write is created
 -> systemic opportunity is marked pending/re-evaluable, not completed
```

`DEFER` is the collision policy. The post-release re-evaluation below is normal PA-01/PA-02 ownership behavior, not a second conflict policy.

## Release and authority hand-back

When the story beat reaches its declared terminal exit or abort path, GameFlow must release the reservation exactly once.

After release:

```text
reservation(Carmen, Bar F01 back room, 18:00–18:20) = RELEASED
Carmen returns to current-context evaluation
PA-07 still owns service/material availability
PA-01 supplies current routine/time context
PA-02 owns Carmen's next autonomous decision
```

Carmen may now choose service, another valid obligation, or no service if the time/context has changed. The earlier Player restoration remains part of the real world state even if Carmen ultimately chooses something else.

GameFlow therefore retains authority only for the authored reservation while it is active; PA-02 retains actor choice authority once the reservation is released. Neither system steals the other's domain.

## Preserved causal state

The following survives the collision unchanged unless its normal owner later changes it:

1. `bar-service-stock = RESTORED` under PA-07;
2. the legitimate Player action that caused that material transition;
3. any already-created bounded service opportunity derived from the restored state, subject to current-context re-evaluation;
4. the fact that the authored reservation temporarily prevented Carmen from consuming that opportunity.

The story reservation may constrain *when/whether Carmen can act during its window*; it may not retroactively erase the stock restoration or rewrite the causal history so that the story appears to have restored it.

## Pass oracle — nothing stranded

SS-B1-01 passes only if all of the following are true:

- while the reservation is active, Carmen is not double-owned by ordinary service and GameFlow;
- the Player-caused material transition is still present after the collision;
- GameFlow has a declared terminal/abort release condition and releases its reservation exactly once;
- after release, Carmen is again eligible for normal PA-01/PA-02 current-context evaluation rather than remaining story-owned;
- the deferred systemic opportunity is either consumed through normal owners or becomes ineligible through normal time/context rules; it is not left as an immortal pending command;
- no direct actor-choice write, duplicated quest flag, or causal-history rewrite is needed to reconcile the collision.

The decisive anti-stranding condition is observable semantically:

```text
story reservation = RELEASED
AND no exclusive owner still claims Carmen for that beat
AND deferred systemic work contains no forced actor command
AND current material/service state can be evaluated by its normal owners
```

If the service window has ended by 18:20, the pending opportunity may simply become ineligible through current-context rules; that is a legitimate end state, not loss of causal history. The restored stock remains restored until PA-07 changes it normally.

## Negative controls

The fixture fails A11B-10 if any of these occur:

1. **Story erasure:** entering the story beat resets `bar-service-stock` or deletes the Player-caused transition merely to make the authored scene simpler.
2. **Systemic authority theft:** PA-07/09 cancels or ignores the active GameFlow reservation and assigns Carmen to ordinary service anyway.
3. **Actor authority theft:** release directly commands `Carmen.nextAction = SERVICE` instead of returning choice to PA-02.
4. **Stranded GameFlow:** the story ends but the exclusive reservation remains active.
5. **Stranded systemic command:** a deferred `serve-now` command survives forever after its context expires instead of being re-evaluated by the normal owners.
6. **Duplicate causality:** the integration creates a second quest-only "stock restored" truth that can disagree with PA-07 shared material state.

## A11B-10 disposition

**SATISFIED at research-model level by SS-B1-01.**

The fixture supplies the previously missing oracle: one real authored/systemic collision, one explicit `DEFER` policy, explicit preserved causal state, bounded GameFlow authority, actor authority hand-back, and a falsifiable no-stranding condition.

Runtime execution, universal conflict scheduling, save/load execution and final GameFlow implementation remain outside WP-PA-B1.