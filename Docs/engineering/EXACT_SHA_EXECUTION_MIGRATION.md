# Migration note — hosted CI to runner-neutral execution

Date: 2026-09-19

Reason: GitHub-hosted Actions quota exhaustion demonstrated that hosted-runner availability is an operational dependency unrelated to Arkus correctness.

Decision:

- hosted GitHub Actions are not part of the correctness contract;
- exact-SHA validation remains mandatory;
- canonical validation lives in repository scripts/tools;
- execution may occur in Worker, Reviewer, local/Codex Desktop or another capable environment;
- durable receipts bind execution to the exact SHA;
- provider outages are infrastructure backpressure, not implementation FAIL;
- final evidence-bearing SHA must still rerun read-only before freeze.

This migration does not waive any HK00 acceptance criterion, causal self-attack or independent review obligation. It changes only the execution substrate.
