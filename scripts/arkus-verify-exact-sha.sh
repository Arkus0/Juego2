#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

# Bounded CITY planning/product routes precede the legacy dispatcher.
# Preserve every pre-existing legacy route and its semantics.
if [[ "${ARKUS_WP:-}" == "WP-CITY-URBAN-00" ]]; then
  exec bash scripts/city-urban-00-verify-exact-sha.sh "$@"
fi

if [[ "${ARKUS_WP:-}" == "WP-CITY-04" ]]; then
  exec bash scripts/city04-verify-exact-sha.sh "$@"
fi

exec bash scripts/arkus-verify-exact-sha-base.sh "$@"
