# LIVING WORLD CROSS-CUTTING AMENDMENT 02 — INTENTIONAL TRANSFORMATION VS ACCIDENTAL COLLAPSE

Status: **FROZEN CROSS-CUTTING RESEARCH AMENDMENT**  
Date: 2026-09-20  
Repository: `Arkus0/Juego2`  
Applies to: canonical PA-09, PA-10, PA-12, PA-13, PA-14 and any later combat/destruction integration that can materially perturb Living World state  
Process class: **RESEARCH / PRODUCT-DESIGN INPUT — NON-BINDING UNTIL REVIEWED**

> **The normal town is an attractor for ordinary play, not a protected canonical ending.**
>
> The simulation should resist accidental/noisy collapse. It should not resist a player who knowingly and persistently chooses to transform or destabilise the town.

This amendment clarifies the anti-chaos thesis established by the Living World programme. It does not require literal destructible-city simulation or any particular criminal/political feature.

---

## 1. Product distinction

Juego2 must distinguish three qualitatively different causal regimes:

```text
A. accident / low-salience mistake
   -> limited propagation
   -> legible correction/recovery

B. deliberate meaningful intervention
   -> persistent consequences
   -> adaptation/resistance/opportunity

C. sustained deliberate transformation
   -> stabilisers can be overcome
   -> town may reach a materially different equilibrium or crisis
```

The same mechanisms should not flatten all three into either:

- consequence-free sandbox;
- one-click catastrophe;
- invisible restoration of default state.

---

## 2. Cross-cutting rules

### XCT-01 — No accidental campaign destruction from trivial input

Low-severity ambiguous or mistaken actions should not normally create self-amplifying cascades capable of destroying a long-running campaign without additional causal reinforcement.

### XCT-02 — No invisible immortality for the default town

If the player repeatedly supplies strong, coherent destabilising causes after receiving understandable feedback, recovery systems may fail, actors may reorganise, services/routines may degrade and a new equilibrium may emerge.

The simulation MUST NOT silently erase or damp all such consequences merely because the resulting town is inconvenient or unlike the starting state.

### XCT-03 — Recovery has finite causal capacity

Recovery/de-escalation should itself be understandable in-world:

- people repair;
- relationships cool;
- resources are replaced;
- services reorganise;
- officials intervene;
- memories decay/compact;
- routines replan.

Repeated pressure can consume or defeat those mechanisms.

### XCT-04 — Deliberate transformation may be constructive, neutral or destructive

Freedom is not only destruction.

The player may push toward:

- stronger cooperation;
- different activity/community patterns;
- altered municipal order;
- new social coalitions;
- reduced/increased institutional intervention;
- severe disorder;
- conflict and fragmentation;
- later reconstruction.

The product should test breadth of transformation, not only crime/destruction.

### XCT-05 — Severe consequences should remain playable where possible

A radically altered town is preferably a new play state, not an automatic invalid save.

Hard fail states require a specific authored/product reason rather than generic discomfort with divergence.

### XCT-06 — Salience and propagation remain local/owned

Allowing deliberate chaos does not authorize magical global propagation.

Consequences still require normal owners:

- material state;
- perception;
- information flow;
- relationships;
- memory;
- schedules/opportunities;
- institutional rules;
- event chains.

### XCT-07 — Player intent is inferred from causal persistence, not mind-reading

The simulation does not need an `isPlayerTryingToCauseChaos` flag.

Intentional transformation can emerge operationally from repeated accepted high-impact actions, choices made after feedback, sustained policy direction and reinforcement of prior consequences.

### XCT-08 — Anti-chaos budgets regulate emergence, not outcome morality

PA-13 budgets should answer:

- how much can propagate at once;
- how often actors reconsider;
- what becomes persistent;
- how chains terminate;
- how background simulation degrades safely;

They should NOT answer:

- which political/social outcome is acceptable;
- whether the player is allowed to make the town worse;
- whether the default equilibrium must be restored.

---

## 3. Ownership deltas

### PA-09 — Player Causal Agency

Must preserve enough expressive player action that deliberate transformation is possible through repeated social/material/embodied interventions where supported.

It does not define the final stability budgets.

### PA-10 — Autonomous Events & Causal Chains

Must allow player-originated changes to seed chains that can continue without player presence while still terminating/reaching new equilibria.

Termination does **not** mean restoration to starting conditions.

### PA-12 — Governance

Owns high-leverage institutional transformation and its macro↔micro gameplay loop.

See `PA-12_AMENDMENT_01_EMERGENT_GOVERNANCE.md`.

### PA-13 — Simulation Control

Must demonstrate both:

1. accidental/noisy play does not drift naturally into systemic anarchy;
2. sustained deliberate pressure can overcome normal stabilisers and produce a materially different stable/unstable state.

A system that passes (1) but fails (2) is over-damped.

A system that passes (2) but fails (1) is unstable.

### PA-14 — Integration Review

Must reject both false positives:

```text
"Nothing can really break because the simulation heals everything."

"Everything eventually breaks because every consequence amplifies."
```

The integrated product target lies between them.

---

## 4. Required adversarial proofs

### X-A — Clumsy player

```text
GIVEN ordinary free play with occasional low/medium-severity mistakes
WHEN no sustained destabilising intent is expressed through further actions
THEN the town remains broadly functional
AND mistakes can leave consequences without cascading automatically into collapse.
```

### X-B — Persistent destabiliser

```text
GIVEN equivalent starting state
WHEN player repeatedly applies high-impact destabilising actions despite legible feedback
THEN the town may materially transform/degrade
AND anti-chaos systems MUST NOT restore the default state by fiat
AND resulting consequences remain causally explainable.
```

### X-C — Constructive transformer

```text
GIVEN equivalent starting state
WHEN player repeatedly invests in cooperation/services/activities or other accepted constructive interventions
THEN a materially different positive equilibrium may emerge
AND this change uses the same causal owners rather than a scripted "good town" ending state.
```

### X-D — Stop pushing

```text
GIVEN a destabilised but recoverable town
WHEN player stops adding destabilising pressure
THEN actors/institutions may attempt recovery through normal systems
AND the degree of recovery depends on actual remaining state/resources/relationships
NOT an unconditional reset timer.
```

### X-E — Two long-horizon runs

One ordinary run and one intentionally transformational run from equivalent initial state must diverge materially over a bounded long horizon.

If they converge despite sustained different causes, agency is too weak.

If ordinary run collapses without sustained causes, stability is too weak.

---

## 5. Failure modes

| ID | Failure | Signal |
|---|---|---|
| XCT-F1 | Butterfly apocalypse | Trivial mistakes routinely trigger runaway collapse. |
| XCT-F2 | Rubber town | Serious consequences vanish because the town always snaps back. |
| XCT-F3 | Chaos flag | System uses a direct player-intent/chaos mode instead of causal state. |
| XCT-F4 | Moral budget | Simulation-control layer secretly permits only designer-approved outcomes. |
| XCT-F5 | Destruction-only freedom | Transformation is meaningful only through violence/property damage. |
| XCT-F6 | Irreversible-by-default | Ordinary experimentation permanently ruins the campaign. |
| XCT-F7 | Fake recovery | Fixed timer restores state without causal repair. |
| XCT-F8 | Endless crisis | Deliberate disruption creates chains that never settle enough to remain playable. |

---

## Freeze rule

**STATUS: FROZEN CROSS-CUTTING RESEARCH AMENDMENT**

Future findings may tune severity, recovery, propagation and supported transformation channels. They may not silently reinterpret `normality as an attractor` to mean that the default town is protected against sustained intentional player causality.