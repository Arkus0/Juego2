#!/usr/bin/env bash
# WP-H1-GATE frozen-candidate verifier (public-only; no Unity, no private source).
# The effective Unity evidence is produced by `.github/workflows/h1-gate-validation.yml` and the hosted fresh AI-agent
# trial by `.github/workflows/h1-gate-ai-trial.yml`. Both are bound to the exact candidate and recorded in PR metadata.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
expected="${1:-${CANDIDATE_SHA:-${actual}}}"
[[ "${expected}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid candidate SHA: ${expected}" >&2; exit 2; }
[[ "${actual}" == "${expected}" ]] || { echo "Candidate SHA mismatch: ${expected} != ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] \
  || { echo 'H1-GATE candidate is not clean' >&2; exit 2; }

for required in \
  Docs/workpacks/H1/WP-H1-GATE.md \
  Docs/workpacks/H1/H1_REMAINING_COMPRESSION_AMENDMENT.md \
  Docs/engineering/H1_UNITY_PARITY_GATE.md \
  Docs/architecture/ADR-H1-004-PUBLIC-EDITOR-EXECUTION-SEAM.md \
  Docs/evidence/WP-H1-GATE/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-H1-GATE/RECONCILIATION.json \
  Docs/evidence/WP-H1-GATE/PROOF_MATRIX.md \
  Docs/evidence/WP-H1-GATE/RESIDUAL_RISK.md \
  Docs/evidence/WP-H1-GATE/CONTENT_SHAPE_PROBE.md \
  Docs/evidence/WP-H1-GATE/AI_AGENT_TRIAL_BRIEF.md \
  Docs/evidence/WP-H1-GATE/AI_TRIAL_PROTOCOL.json \
  Docs/evidence/WP-H1-GATE/AI_TRIAL_HISTORY.md \
  scripts/h1-gate-scenario.py \
  scripts/h1-gate-verify.py \
  scripts/h1-gate-negative-controls.py \
  scripts/h1-gate-ai-trial.py \
  .github/workflows/h1-gate-validation.yml \
  .github/workflows/h1-gate-ai-trial.yml; do
  test -f "${required}" || { echo "Missing H1-GATE verification input: ${required}" >&2; exit 2; }
done

export PYTHONDONTWRITEBYTECODE=1
python3 -m py_compile scripts/h1-gate-scenario.py scripts/h1-gate-verify.py scripts/h1-gate-negative-controls.py scripts/h1-gate-ai-trial.py

base="${BASE_SHA:-}"
if [[ -z "${base}" ]]; then
  base="$(git merge-base HEAD origin/main 2>/dev/null || true)"
fi
[[ "${base}" =~ ^[0-9a-f]{40}$ ]] || { echo 'H1-GATE scope check needs the base (origin/main merge-base)' >&2; exit 2; }

python3 scripts/h1-gate-verify.py reconcile
python3 scripts/h1-gate-verify.py workflow
python3 scripts/h1-gate-verify.py scope --base "${base}"
python3 scripts/h1-gate-negative-controls.py static

# The trial relay must offer the model only the brief and the MCP tool list, and must never author a call itself.
grep -Fq 'BRIEF START' Docs/evidence/WP-H1-GATE/AI_AGENT_TRIAL_BRIEF.md
grep -Fq '"model": "openai/gpt-5.6-luna-20260709"' Docs/evidence/WP-H1-GATE/AI_TRIAL_PROTOCOL.json
! grep -Eq 'h1-gate-scenario|NODES|REQUIRED_CAPABILITIES' scripts/h1-gate-ai-trial.py

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq '^WP:[[:space:]]*`?WP-H1-GATE`?[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^H1-GATE Unity run:[[:space:]]*`?[0-9]+`?[[:space:]]+GREEN'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^AI trial run:[[:space:]]*`?[0-9]+`?[[:space:]]+PASS'
fi

echo "H1_GATE_EXACT_SHA_GREEN ${actual}"
echo "Result: GREEN"
