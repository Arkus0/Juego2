#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
CHECKER="${ROOT}/scripts/hk00a-architecture-check.sh"
TMP="$(mktemp -d)"
trap 'rm -rf "${TMP}"' EXIT

reset_fixture() {
  rm -rf "${TMP}/Docs"
  cp -R "${ROOT}/Docs" "${TMP}/Docs"
}

replace_once() {
  local file="$1"
  local old="$2"
  local new="$3"
  python3 - "$file" "$old" "$new" <<'PY'
from pathlib import Path
import sys
path = Path(sys.argv[1])
old = sys.argv[2]
new = sys.argv[3]
text = path.read_text()
if old not in text:
    raise SystemExit(f"attack setup failed; text not found in {path}: {old}")
path.write_text(text.replace(old, new, 1))
PY
}

expect_red() {
  local name="$1"
  local log="${TMP}/${name}.log"
  set +e
  ARKUS_ARCH_DOC_ROOT="${TMP}" bash "${CHECKER}" >"${log}" 2>&1
  local status=$?
  set -e
  if [[ ${status} -eq 0 ]]; then
    echo "SELF_ATTACK: FAIL: ${name} stayed GREEN" >&2
    cat "${log}" >&2
    exit 1
  fi
  echo "SELF_ATTACK: RED ${name}"
}

# Prove the clean architecture is accepted before injecting defects.
ARKUS_ARCH_DOC_ROOT="${ROOT}" bash "${CHECKER}" >/dev/null
echo "SELF_ATTACK: GREEN baseline"

# 1. MCP becomes the canonical authority.
reset_fixture
replace_once "${TMP}/Docs/engineering/PRODUCT_ARCHITECTURE.md" \
  "It is not the canonical semantic authority." \
  "It is the canonical semantic authority."
expect_red "mcp-canonical-authority"

# 2. Unity implementation concepts leak into the H0 canonical contract.
reset_fixture
printf '\n- Canonical request identity may directly use `UnityEngine.GameObject`.\n' >> \
  "${TMP}/Docs/workpacks/HK/WP-HK-01.md"
expect_red "unity-leak-into-h0-contract"

# 3. An engine-specific harness is wholesale-adopted despite missing Arkus semantics.
reset_fixture
replace_once "${TMP}/Docs/engineering/EXTERNAL_HARNESS_ADOPTION_AUDIT.md" \
  "BORROW + BENCHMARK" \
  "ADOPT"
expect_red "wholesale-engine-harness-adoption"

# 4. Unknown dependency terms are permitted before embedding.
reset_fixture
replace_once "${TMP}/Docs/engineering/DEPENDENCY_IP_POLICY.md" \
  "License claims must be reverified at the exact adopted version/commit before code is embedded, copied, vendored or added as a direct shipped dependency." \
  "Unknown license and commercial terms are allowed before code is embedded."
expect_red "unknown-license-accepted"

# 5. Completeness is circularly defined only by the discovery registry being proved.
reset_fixture
replace_once "${TMP}/Docs/workpacks/HK/WP-HK-01.md" \
  "The completeness universe is independently/effectively enumerable: removing or unregistering a public route cannot make both the capability and its proof obligation disappear." \
  "The completeness universe is defined only by the discovery registry being proved."
expect_red "self-shrinking-discovery-universe"

# 6. Adapter mutation bypasses the canonical transaction pipeline.
reset_fixture
replace_once "${TMP}/Docs/engineering/PRODUCT_ARCHITECTURE.md" \
  "Transport and engine adapters may not mutate canonical state directly." \
  "Transport and engine adapters may mutate canonical state directly."
expect_red "adapter-bypasses-canonical-transaction"

# Re-prove GREEN on the unmodified candidate after all negative controls.
ARKUS_ARCH_DOC_ROOT="${ROOT}" bash "${CHECKER}" >/dev/null
echo "SELF_ATTACK: GREEN restored"
echo "HK00A_SELF_ATTACKS: GREEN (6/6 causal negative controls turned RED)"
