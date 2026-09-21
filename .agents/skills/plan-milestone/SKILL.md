# plan-milestone

Plan a Juego2 milestone without implementing it.

## Method

1. Read current `Docs/ROADMAP.md`, accepted predecessor contracts and current `main` state.
2. Define the milestone outcome and explicit gate.
3. Split into the smallest workpacks whose claims can be independently proven.
4. Encode real dependency edges; never rely on numeric order alone.
5. For each WP define objective, Allowed/Forbidden scope, acceptance, required tests/evidence and DoD.
6. Mark foundational WPs and bind `FOUNDATIONAL_PROOF_STANDARD.md` where downstream correctness would otherwise depend on an unproven architectural claim.
7. Avoid freezing downstream details that depend on contracts not yet accepted.
8. Update ROADMAP/contracts only; do not silently implement product code.

A milestone plan is successful when another fresh Worker can select and execute the first eligible WP without private chat context.
