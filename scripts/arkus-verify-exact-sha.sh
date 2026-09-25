#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

# CITY-04 is a bounded local-product verifier. Preserve every pre-existing
# dispatcher route byte-for-byte in the legacy dispatcher below.
if [[ "${ARKUS_WP:-}" == "WP-CITY-04" ]]; then
  exec bash scripts/city04-verify-exact-sha.sh "$@"
fi

exec bash scripts/arkus-verify-exact-sha-base.sh "$@"
