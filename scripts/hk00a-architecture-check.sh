#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOC_ROOT="${ARKUS_ARCH_DOC_ROOT:-${REPO_ROOT}}"

arch="${DOC_ROOT}/Docs/engineering/PRODUCT_ARCHITECTURE.md"
ip="${DOC_ROOT}/Docs/engineering/DEPENDENCY_IP_POLICY.md"
audit="${DOC_ROOT}/Docs/engineering/EXTERNAL_HARNESS_ADOPTION_AUDIT.md"
proof="${DOC_ROOT}/Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md"
roadmap="${DOC_ROOT}/Docs/ROADMAP.md"
hk00a="${DOC_ROOT}/Docs/workpacks/HK/WP-HK-00A.md"
hk01="${DOC_ROOT}/Docs/workpacks/HK/WP-HK-01.md"
hk07="${DOC_ROOT}/Docs/workpacks/HK/WP-HK-07.md"

fail() {
  echo "HK00A_ARCHITECTURE_CHECK: FAIL: $*" >&2
  exit 1
}

require_file() {
  [[ -f "$1" ]] || fail "missing required document: $1"
}

require_text() {
  local file="$1"
  local text="$2"
  grep -Fq -- "$text" "$file" || fail "required contract text missing from ${file#${DOC_ROOT}/}: ${text}"
}

reject_regex() {
  local file="$1"
  local regex="$2"
  local reason="$3"
  if grep -Eqi -- "$regex" "$file"; then
    fail "$reason (${file#${DOC_ROOT}/})"
  fi
}

for f in "$arch" "$ip" "$audit" "$proof" "$roadmap" "$hk00a" "$hk01" "$hk07"; do
  require_file "$f"
done

# 1. MCP is a projection, never the canonical semantic/schema authority.
require_text "$arch" "MCP is a first-class interoperability adapter"
require_text "$arch" "It is not the canonical semantic authority."
require_text "$arch" "must derive from one canonical contract source"
require_text "$hk01" "The Arkus contract model is upstream of transports."
reject_regex "$arch" 'MCP (request|tool|schema).*(only|sole|canonical) (source|authority)' \
  "MCP-specific definitions cannot become the sole canonical source"

# 2. H0 canonical contract remains engine-neutral.
require_text "$hk01" "engine-specific object types"
reject_regex "$hk01" 'UnityEngine|UnityEditor|GameObject|MonoBehaviour|ScriptableObject|UnityEngine\.SceneManagement' \
  "HK01 canonical contract leaks Unity concepts"

# 3. External engine harnesses are benchmark/borrow inputs, not wholesale architecture masters.
require_text "$arch" "External harnesses are capability benchmarks, not architectural masters."
require_text "$arch" "Wholesale adoption of an engine harness is specifically rejected when required Arkus semantics are absent."
require_text "$audit" "| Unity Biome MCP |"
require_text "$audit" "BORROW + BENCHMARK"
reject_regex "$audit" '\| Unity Biome MCP \|.*\|[[:space:]]*ADOPT([[:space:]]|\|)' \
  "engine-specific harness cannot be wholesale-adopted as Arkus authority"

# 4. Unknown/incompatible commercial terms fail closed before embedding.
require_text "$ip" "License claims must be reverified at the exact adopted version/commit before code is embedded"
require_text "$ip" "Unknown, ambiguous or incompatible commercial terms are fail-closed"
require_text "$ip" "produce an SBOM for shipped artifacts"
reject_regex "$ip" '(unknown|ambiguous) (license|commercial terms).*(allowed|acceptable|may be embedded)' \
  "unknown dependency terms cannot be accepted by default"

# 5. Completeness may not be defined only by the registry/discovery surface under proof.
require_text "$proof" "Independent-universe rule"
require_text "$proof" "the universe of X is obtained independently"
require_text "$hk01" "The completeness universe is independently/effectively enumerable"
require_text "$hk01" "removing or unregistering a public route cannot make both the capability and its proof obligation disappear"
reject_regex "$hk01" 'completeness universe.*(only|solely).*discovery registry' \
  "HK01 completeness cannot be circularly defined by discovery"

# 6. Adapters may not mutate canonical state outside the accepted transaction pipeline.
require_text "$arch" "Transport and engine adapters may not mutate canonical state directly."
require_text "$arch" "Every canonical mutation must enter through the accepted Arkus authoring transaction pipeline"
require_text "$ip" "may not create a mutation path that bypasses the accepted Arkus authoring transaction pipeline"
require_text "$hk07" "MCP adapter introducing a hidden mutation path"

# H0 route/DAG reconciliation.
require_text "$hk00a" 'Depends on: `WP-HK-00` ✅ COMPLETE'
require_text "$hk01" 'Depends on: `WP-HK-00A`'
require_text "$hk07" 'Depends on: `WP-HK-06`'
require_text "$roadmap" '`WP-HK-00A` | Product architecture, adoption/IP boundary and anti-lock-in contract'
require_text "$roadmap" '`WP-HK-01` | Canonical contract model + machine-readable capability/schema discovery'
require_text "$roadmap" '`WP-HK-07` | Production headless host + reference JSONL + standards-compatible MCP projection'
require_text "$arch" "WP-HK-01"
require_text "$arch" "WP-HK-07"

# Semantic authority ownership is repeated at both architecture and dependency boundaries.
require_text "$arch" "No transport, engine adapter, SDK or third-party framework may become the only definition"
require_text "$ip" "No external component may be the only definition of"

echo "HK00A_ARCHITECTURE_CHECK: GREEN"
