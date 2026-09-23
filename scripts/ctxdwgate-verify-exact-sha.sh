#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
TARGET_SHA="${1:-$(git -C "${ROOT}" rev-parse HEAD)}"
cd "${ROOT}"

[[ "${TARGET_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || {
  echo "CTX_DW_GATE_VERIFY_RED invalid-target-sha" >&2
  exit 2
}
actual="$(git rev-parse HEAD)"
[[ "${actual}" == "${TARGET_SHA}" ]] || {
  echo "CTX_DW_GATE_VERIFY_RED checkout-mismatch expected=${TARGET_SHA} actual=${actual}" >&2
  exit 1
}

python3 scripts/ctx-dw-gate-proof.py

echo "EXECUTION_RECEIPT_V1"
echo "WP: WP-CTX-DW-GATE"
echo "Candidate SHA: ${TARGET_SHA}"
echo "Executor role: ${ARKUS_EXECUTOR_ROLE:-UNSPECIFIED}"
echo "Execution substrate: ${ARKUS_EXECUTION_SUBSTRATE:-UNSPECIFIED}"
echo "Result: GREEN"
