#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BASELINE_SHA="${DW_GATE_BASELINE_SHA:-068db8bb382a847e3fc1618ea6dced014b1e1da7}"
TARGET_SHA="${1:-$(git -C "${ROOT}" rev-parse HEAD)}"
cd "${ROOT}"

[[ "${BASELINE_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "DW_GATE_CLOSURE_RED invalid-baseline" >&2; exit 2; }
[[ "${TARGET_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "DW_GATE_CLOSURE_RED invalid-target" >&2; exit 2; }
git cat-file -e "${BASELINE_SHA}^{commit}" || { echo "DW_GATE_CLOSURE_RED baseline-unavailable" >&2; exit 2; }
git cat-file -e "${TARGET_SHA}^{commit}" || { echo "DW_GATE_CLOSURE_RED target-unavailable" >&2; exit 2; }
git merge-base --is-ancestor "${BASELINE_SHA}" "${TARGET_SHA}" || { echo "DW_GATE_CLOSURE_RED baseline-not-ancestor" >&2; exit 1; }

# DW-GATE is a composition/closure workpack. It may add gate-owned proof and
# canonical routing, but it may not rewrite predecessor semantics, tests,
# tools, accepted contracts/evidence, or predecessor proof machinery.
if ! git diff --quiet "${BASELINE_SHA}" "${TARGET_SHA}" -- \
  src \
  tests \
  tools \
  'scripts/dw0*' \
  '.github/workflows/dw04-*' \
  'Docs/workpacks/DW/WP-DW-0*.md' \
  'Docs/evidence/WP-DW-0*' \
  Docs/ROADMAP.md; then
  echo "DW_GATE_CLOSURE_RED predecessor-or-product-proof-surface-changed" >&2
  git diff --name-only "${BASELINE_SHA}" "${TARGET_SHA}" -- \
    src tests tools 'scripts/dw0*' '.github/workflows/dw04-*' \
    'Docs/workpacks/DW/WP-DW-0*.md' 'Docs/evidence/WP-DW-0*' Docs/ROADMAP.md >&2
  exit 1
fi

echo "DW_GATE_CLOSURE_GREEN baseline=${BASELINE_SHA} target=${TARGET_SHA}"
