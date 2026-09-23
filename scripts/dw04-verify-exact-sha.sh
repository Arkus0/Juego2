#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
PRECALIBRATION_COMMIT="9cbed950a3469897cf286c9a90624c240701528f"
cd "${ROOT}"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before DW-04 verification" >&2; candidate_dirty_status >&2; exit 2; }

bash scripts/dw04-observe-exact-sha.sh "${actual}"

required=(
  Docs/evidence/WP-DW-04/CALIBRATION_RESULTS.json
  Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json
  Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json
  Docs/evidence/WP-DW-04/ACCEPTANCE_TRANSCRIPT.jsonl
  Docs/evidence/WP-DW-04/CAMPAIGN_RECEIPT.json
  Docs/evidence/WP-DW-04/TRIAL_RESULT.json
  Docs/evidence/WP-DW-04/PROOF_MATRIX.md
  Docs/evidence/WP-DW-04/FINAL_CIRCUIT_BREAKER_AUDIT.md
  Docs/evidence/WP-DW-04/RESIDUAL_RISK.md
)
for path in "${required[@]}"; do
  [[ -f "${path}" ]] || { echo "DW-04 final proof is NOT_READY: missing ${path}" >&2; exit 2; }
done

freeze_path="Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json"
freeze_commit="$(git log --diff-filter=A --format=%H -- "${freeze_path}" | tail -n 1)"
[[ "${freeze_commit}" =~ ^[0-9a-f]{40}$ ]] || { echo "Unable to resolve immutable TASK_SELECTION_FREEZE introduction commit" >&2; exit 2; }
git merge-base --is-ancestor "${PRECALIBRATION_COMMIT}" "${freeze_commit}" || { echo "Acceptance freeze is outside the pre-calibration lineage" >&2; exit 2; }

audit_tmp="$(mktemp)"
trap 'rm -f "${audit_tmp}"' EXIT
PYTHONDONTWRITEBYTECODE=1 python3 scripts/dw04-trial.py audit \
  --pre-commit "${PRECALIBRATION_COMMIT}" \
  --freeze-commit "${freeze_commit}" \
  --transcript Docs/evidence/WP-DW-04/ACCEPTANCE_TRANSCRIPT.jsonl > "${audit_tmp}"

PYTHONDONTWRITEBYTECODE=1 python3 - "${audit_tmp}" <<'PY'
import json, pathlib, sys
observed = json.loads(pathlib.Path(sys.argv[1]).read_text(encoding='utf-8'))
committed = json.loads(pathlib.Path('Docs/evidence/WP-DW-04/TRIAL_RESULT.json').read_text(encoding='utf-8'))
if observed != committed:
    raise SystemExit('DW-04 committed TRIAL_RESULT differs from exact-SHA deterministic audit')
if observed.get('disposition') != 'PASS':
    raise SystemExit(f"DW-04 final disposition is {observed.get('disposition')}, not PASS")
if observed.get('structural') is not True:
    raise SystemExit('DW-04 structural context proof is not GREEN')
if observed.get('saving', 0) < 0.30:
    raise SystemExit('DW-04 median injected-source byte reduction is below 30%')
print('DW-04 exact-SHA deterministic trial audit: PASS')
PY

PYTHONDONTWRITEBYTECODE=1 python3 - "${freeze_commit}" <<'PY'
import hashlib, json, pathlib, sys
receipt = json.loads(pathlib.Path('Docs/evidence/WP-DW-04/CAMPAIGN_RECEIPT.json').read_text(encoding='utf-8'))
transcript = pathlib.Path('Docs/evidence/WP-DW-04/ACCEPTANCE_TRANSCRIPT.jsonl').read_bytes()
assert receipt.get('schema') == 'dw04-campaign-receipt-v1'
assert receipt.get('repository') == 'Arkus0/Juego2'
assert receipt.get('phase') == 'acceptance'
assert receipt.get('freeze_commit') == sys.argv[1]
assert receipt.get('transcript_sha256') == hashlib.sha256(transcript).hexdigest()
ids = receipt.get('provider_request_ids')
assert isinstance(ids, list) and len(ids) == 36 and len(set(ids)) == 36 and all(ids)
run_id = receipt.get('workflow_run_id')
assert isinstance(run_id, int) and run_id > 0
print(f'DW-04 campaign receipt structure: GREEN (run {run_id})')
PY

run_id="$(python3 -c "import json; print(json.load(open('Docs/evidence/WP-DW-04/CAMPAIGN_RECEIPT.json'))['workflow_run_id'])")"
repo="${GITHUB_REPOSITORY:-Arkus0/Juego2}"
api="https://api.github.com/repos/${repo}"
run_json="$(curl -fsSL -H 'Accept: application/vnd.github+json' "${api}/actions/runs/${run_id}")" || { echo "Cannot verify durable DW-04 campaign run on live GitHub" >&2; exit 2; }
[[ "$(jq -r '.conclusion // empty' <<<"${run_json}")" == "success" ]] || { echo "DW-04 campaign source run is not successful" >&2; exit 2; }
[[ "$(jq -r '.event // empty' <<<"${run_json}")" == "workflow_dispatch" ]] || { echo "DW-04 campaign source was not an explicit one-shot dispatch" >&2; exit 2; }
run_name="$(jq -r '.name // empty' <<<"${run_json}")"
[[ "${run_name}" == "DW-04 Paired Agent Campaign" ]] || { echo "Unexpected DW-04 campaign workflow identity: ${run_name}" >&2; exit 2; }
freeze_short="${freeze_commit:0:12}"
runs_json="$(curl -fsSL -H 'Accept: application/vnd.github+json' "${api}/actions/workflows/dw04-campaign.yml/runs?event=workflow_dispatch&per_page=100")" || { echo "Cannot enumerate DW-04 campaigns for uniqueness" >&2; exit 2; }
matching="$(jq --arg marker "acceptance-${freeze_short}" '[.workflow_runs[] | select((.display_title // "") | contains($marker))] | length' <<<"${runs_json}")"
[[ "${matching}" == "1" ]] || { echo "DW-04 acceptance campaign uniqueness violated: ${matching} runs for freeze ${freeze_short}" >&2; exit 2; }

for marker in \
  'FOUNDATIONAL_PROOF_VERDICT: READY' \
  'UNRESOLVED_PROOF_OBLIGATIONS: 0' \
  'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' \
  'PROOF_BUDGET_VERDICT: WITHIN_BUDGET'; do
  grep -Fq "${marker}" Docs/evidence/WP-DW-04/PROOF_MATRIX.md || { echo "Missing proof marker: ${marker}" >&2; exit 2; }
done
grep -Fq 'FINAL_CIRCUIT_BREAKER_AUDIT: CLEAN' Docs/evidence/WP-DW-04/FINAL_CIRCUIT_BREAKER_AUDIT.md
grep -Fq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-DW-04/RESIDUAL_RISK.md

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after DW-04 verification" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-DW-04
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/dw04-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; regression=GREEN; frozen-universe=GREEN; source-oracles=GREEN; typed-retrieval=GREEN; calibration=GREEN; structural-context=GREEN; paired-agent-quality=GREEN; context-reduction=GREEN; campaign-uniqueness=GREEN; provider-request-inventory=GREEN; foundational-proof=GREEN; final-circuit-breaker=GREEN; residual-risk=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-DW-04
EOF
