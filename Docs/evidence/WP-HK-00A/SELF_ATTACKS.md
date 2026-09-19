# WP-HK-00A — Causal Self-Attacks

Canonical command:

```bash
bash scripts/self-attacks/run-hk00a-architecture-attacks.sh
```

The script never edits the candidate checkout. It copies `Docs/` to a temporary fixture, injects one causal architecture defect at a time, requires `scripts/hk00a-architecture-check.sh` to turn RED, discards the fixture, and finally re-runs the clean candidate to prove GREEN restoration.

## Attack set

| # | Injected defect | Expected causal rejection |
|---|---|---|
| 1 | Change MCP from non-canonical adapter to canonical semantic authority | one-source/upstream ownership contract is absent, architecture oracle RED |
| 2 | Add `UnityEngine.GameObject` to HK01 canonical contract | H0 engine-neutral contract check RED |
| 3 | Change Unity Biome from `BORROW + BENCHMARK` to wholesale `ADOPT` | engine-harness adoption boundary check RED |
| 4 | Permit unknown license/commercial terms before embedding code | exact-version fail-closed dependency gate RED |
| 5 | Define HK01 completeness universe only from the discovery registry being proved | independent-universe requirement RED |
| 6 | Permit transport/engine adapters to mutate canonical state directly | canonical transaction-boundary check RED |

## Required successful output shape

```text
SELF_ATTACK: GREEN baseline
SELF_ATTACK: RED mcp-canonical-authority
SELF_ATTACK: RED unity-leak-into-h0-contract
SELF_ATTACK: RED wholesale-engine-harness-adoption
SELF_ATTACK: RED unknown-license-accepted
SELF_ATTACK: RED self-shrinking-discovery-universe
SELF_ATTACK: RED adapter-bypasses-canonical-transaction
SELF_ATTACK: GREEN restored
HK00A_SELF_ATTACKS: GREEN (6/6 causal negative controls turned RED)
```

Any attack that stays GREEN fails the command. A failure caused by missing attack-fixture text also fails the command rather than being counted as a valid RED control.
