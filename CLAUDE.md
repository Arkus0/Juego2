# Claude Code — Juego2

Read `AGENTS.md` first.

Juego2 / Arkus Harness is a **game-development and software-verification project, not a cybersecurity project**. Work is limited to repository-owned game-authoring code, fixtures, tests, CI and documentation. Legacy terms such as `self-attack`, `attack fixture`, `bypass` or `adversarial review` mean ordinary negative/conformance testing of this repository only; use the neutral terminology defined in `AGENTS.md` for new work.

Juego2 is harness-first. H0 (`HK-*`) builds and proves the AI-native authoring kernel before Unity/gameplay.

There is no automation bootstrap that may impersonate an independent role. The user explicitly invokes Worker and Reviewer role sessions, normally through remote/chat execution; repository automation may enforce protocol, CI, state transitions and notifications around those disposable sessions. A local phase is required only when the applicable contract demands real local/editor/toolchain evidence.

For any implementation task:

1. reconstruct current `main`, open PRs and active ownership;
2. read the exact WP + dependencies;
3. follow `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`;
4. if foundational, bind `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`;
5. preserve exact-SHA validation evidence under `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md`;
6. stop at the next role boundary.

Do not import architecture/code from `Arkus0/Juego` unless an explicit Juego2 contract authorizes it. Process lessons may be reused; technical baggage is not inherited.

A session that acted as Worker or directed implementation of a candidate must not act as its independent Reviewer.

Cloud execution must never fabricate later local/editor evidence. GitHub Actions and repository-triggered automation may validate and advance declared process state, but they do not originate or substitute for an independent Worker/Reviewer role judgement.
