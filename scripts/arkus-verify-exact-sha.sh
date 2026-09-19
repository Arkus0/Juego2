#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

resolve_wp() {
  if [[ -n "${ARKUS_WP:-}" ]]; then
    printf '%s\n' "${ARKUS_WP}"
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-00A`'; then
    printf '%s\n' 'WP-HK-00A'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-00`'; then
    printf '%s\n' 'WP-HK-00'
    return
  fi
  echo "Unable to resolve workpack for canonical verification" >&2
  exit 2
}

case "$(resolve_wp)" in
  WP-HK-00A)
    exec bash scripts/hk00a-verify-exact-sha.sh "$@"
    ;;
  WP-HK-00)
    exec bash scripts/hk00-verify-exact-sha.sh "$@"
    ;;
  *)
    echo "No canonical verification entrypoint registered for requested workpack" >&2
    exit 2
    ;;
esac
