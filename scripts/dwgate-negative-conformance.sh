#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
CANDIDATE="$(git rev-parse HEAD)"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}
[[ -z "$(candidate_dirty_status)" ]] || { echo "DW GATE negative conformance requires a clean candidate" >&2; exit 2; }

TMP_ROOT="$(mktemp -d)"
cleanup() {
  git worktree remove --force "${TMP_ROOT}/g1" >/dev/null 2>&1 || true
  git worktree remove --force "${TMP_ROOT}/g2" >/dev/null 2>&1 || true
  git worktree remove --force "${TMP_ROOT}/g3" >/dev/null 2>&1 || true
  git worktree remove --force "${TMP_ROOT}/g4" >/dev/null 2>&1 || true
  rm -rf "${TMP_ROOT}"
}
trap cleanup EXIT

# G1: remove the real CITY invariant execution while leaving the declared gate universe intact.
git worktree add --detach "${TMP_ROOT}/g1" "${CANDIDATE}" >/dev/null
python3 - "${TMP_ROOT}/g1/scripts/dwgate-observe-exact-sha.sh" <<'PY'
from pathlib import Path
import sys
p = Path(sys.argv[1])
lines = p.read_text(encoding="utf-8").splitlines()
target = "run_gate_stage dw01-city-invariants "
hits = [i for i, line in enumerate(lines) if line.lstrip().startswith(target)]
if len(hits) != 1:
    raise SystemExit(f"G1 precondition failed hits={len(hits)}")
del lines[hits[0]]
p.write_text("\n".join(lines) + "\n", encoding="utf-8")
PY
set +e
g1_log="$(cd "${TMP_ROOT}/g1" && bash scripts/dwgate-proof-infrastructure-check.sh 2>&1)"
g1_status=$?
set -e
if [[ ${g1_status} -eq 0 ]] || ! grep -Fq 'DW_GATE_PROOF_INFRA_RED stage-execution-mismatch' <<<"${g1_log}"; then
  echo "DW GATE G1 false-green or wrong RED reason" >&2
  printf '%s\n' "${g1_log}" >&2
  exit 1
fi
echo "DW_GATE_NEGATIVE_RED G1 required-city-invariant-stage-omitted"

# G2: the gate handoff may not silently drop one accepted limitation from whole-file reconciliation.
git worktree add --detach "${TMP_ROOT}/g2" "${CANDIDATE}" >/dev/null
python3 - "${TMP_ROOT}/g2/Docs/evidence/WP-DW-GATE/H2_HANDOFF_V1.json" <<'PY'
from pathlib import Path
import json, sys
p = Path(sys.argv[1])
x = json.loads(p.read_text(encoding="utf-8"))
x["limitations_count"] -= 1
p.write_text(json.dumps(x, indent=2) + "\n", encoding="utf-8")
PY
set +e
g2_log="$(cd "${TMP_ROOT}/g2" && python3 scripts/dwgate-evidence-audit.py 2>&1)"
g2_status=$?
set -e
if [[ ${g2_status} -eq 0 ]] || ! grep -Fq 'DW_GATE_EVIDENCE_RED h2-handoff-limitation-reconciliation-mismatch' <<<"${g2_log}"; then
  echo "DW GATE G2 false-green or wrong RED reason" >&2
  printf '%s\n' "${g2_log}" >&2
  exit 1
fi
echo "DW_GATE_NEGATIVE_RED G2 accepted-limitation-omitted-from-handoff"

# G3: external-repository consumability remains an explicit non-claim.
git worktree add --detach "${TMP_ROOT}/g3" "${CANDIDATE}" >/dev/null
python3 - "${TMP_ROOT}/g3/Docs/evidence/WP-DW-GATE/H2_HANDOFF_V1.json" <<'PY'
from pathlib import Path
import json, sys
p = Path(sys.argv[1])
x = json.loads(p.read_text(encoding="utf-8"))
x["claims"]["external_repository_consumability"] = True
p.write_text(json.dumps(x, indent=2) + "\n", encoding="utf-8")
PY
set +e
g3_log="$(cd "${TMP_ROOT}/g3" && python3 scripts/dwgate-evidence-audit.py 2>&1)"
g3_status=$?
set -e
if [[ ${g3_status} -eq 0 ]] || ! grep -Fq 'DW_GATE_EVIDENCE_RED h2-handoff-overclaim external_repository_consumability' <<<"${g3_log}"; then
  echo "DW GATE G3 false-green or wrong RED reason" >&2
  printf '%s\n' "${g3_log}" >&2
  exit 1
fi
echo "DW_GATE_NEGATIVE_RED G3 external-repository-overclaim"

# G4: the gate may not rewrite predecessor proof machinery to manufacture PASS.
git worktree add --detach "${TMP_ROOT}/g4" "${CANDIDATE}" >/dev/null
(
  cd "${TMP_ROOT}/g4"
  printf '\n# gate-negative-tamper\n' >> scripts/dw04-verify-exact-sha.sh
  git add scripts/dw04-verify-exact-sha.sh
  git -c user.name='DW Gate Negative' -c user.email='dw-gate-negative@example.invalid' commit -m 'negative predecessor proof tamper' >/dev/null
)
g4_sha="$(git -C "${TMP_ROOT}/g4" rev-parse HEAD)"
set +e
g4_log="$(cd "${TMP_ROOT}/g4" && bash scripts/dwgate-closure-check.sh "${g4_sha}" 2>&1)"
g4_status=$?
set -e
if [[ ${g4_status} -eq 0 ]] || ! grep -Fq 'DW_GATE_CLOSURE_RED predecessor-or-product-proof-surface-changed' <<<"${g4_log}"; then
  echo "DW GATE G4 false-green or wrong RED reason" >&2
  printf '%s\n' "${g4_log}" >&2
  exit 1
fi
echo "DW_GATE_NEGATIVE_RED G4 predecessor-proof-machinery-tamper"

cd "${ROOT}"
[[ -z "$(candidate_dirty_status)" ]] || { echo "DW GATE candidate changed during disposable negative controls" >&2; exit 2; }
echo "DW_GATE_NEGATIVE_CONFORMANCE_GREEN red_controls=4"
