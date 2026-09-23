#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

DOCSYNC="Docs/evidence/WP-DW-04/DOCSYNC.md"
[[ -f "${DOCSYNC}" ]] || { echo "DW_GATE_DW04_RED docsync-missing" >&2; exit 1; }
accepted_sha="$(python3 - "${DOCSYNC}" <<'PY'
from pathlib import Path
import re, sys
text = Path(sys.argv[1]).read_text(encoding='utf-8')
m = re.search(r"Frozen candidate SHA:\s*`([0-9a-fA-F]{40})`", text)
if not m:
    raise SystemExit(2)
print(m.group(1).lower())
PY
)" || { echo "DW_GATE_DW04_RED accepted-candidate-unresolved" >&2; exit 1; }

git cat-file -e "${accepted_sha}^{commit}" || { echo "DW_GATE_DW04_RED accepted-candidate-unavailable ${accepted_sha}" >&2; exit 1; }

# The inherited bytes on the current gate candidate must still bind scored
# answers to preserved provider response/request evidence.
python3 scripts/dw04-evidence-integrity.py

# Re-run the accepted DW-04 verifier at its own frozen historical candidate.
# Its chronology checks are intentionally exact-candidate checks and must not
# be weakened merely because DW-GATE is a later descendant.
tmp="$(mktemp -d)"
cleanup() {
  git worktree remove --force "${tmp}/accepted" >/dev/null 2>&1 || true
  rm -rf "${tmp}"
}
trap cleanup EXIT

git worktree add --detach "${tmp}/accepted" "${accepted_sha}" >/dev/null
(
  cd "${tmp}/accepted"
  python3 scripts/dw04-evidence-integrity.py
  bash scripts/dw04-verify-exact-sha.sh "${accepted_sha}"
)

echo "DW_GATE_DW04_GREEN accepted_candidate=${accepted_sha}"
