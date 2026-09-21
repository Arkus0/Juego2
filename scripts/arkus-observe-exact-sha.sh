#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

resolve_wp() {
  if [[ -n "${ARKUS_WP:-}" ]]; then
    printf '%s\n' "${ARKUS_WP}"
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Eq '^WP:[[:space:]]*`?WP-H1-01`?[[:space:]]*$'; then
    printf '%s\n' 'WP-H1-01'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Eq '^WP:[[:space:]]*`?WP-H1-00`?[[:space:]]*$'; then
    printf '%s\n' 'WP-H1-00'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Eq '^WP:[[:space:]]*`?WP-HK-GATE`?[[:space:]]*$'; then
    printf '%s\n' 'WP-HK-GATE'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Eq '^WP:[[:space:]]*`?WP-HK-10`?[[:space:]]*$'; then
    printf '%s\n' 'WP-HK-10'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Eq '^WP:[[:space:]]*`?WP-HK-09B`?[[:space:]]*$'; then
    printf '%s\n' 'WP-HK-09B'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-09A`'; then
    printf '%s\n' 'WP-HK-09A'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-08B`'; then
    printf '%s\n' 'WP-HK-08B'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-08A`'; then
    printf '%s\n' 'WP-HK-08A'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-07B`'; then
    printf '%s\n' 'WP-HK-07B'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-07A`'; then
    printf '%s\n' 'WP-HK-07A'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-06C`'; then
    printf '%s\n' 'WP-HK-06C'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-06B`'; then
    printf '%s\n' 'WP-HK-06B'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-06A`'; then
    printf '%s\n' 'WP-HK-06A'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-05`'; then
    printf '%s\n' 'WP-HK-05'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-02A`'; then
    printf '%s\n' 'WP-HK-02A'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-04`'; then
    printf '%s\n' 'WP-HK-04'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-03`'; then
    printf '%s\n' 'WP-HK-03'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-02`'; then
    printf '%s\n' 'WP-HK-02'
    return
  fi
  if printf '%s\n' "${PR_BODY:-}" | grep -Fq 'WP: `WP-HK-01`'; then
    printf '%s\n' 'WP-HK-01'
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
  echo "Unable to resolve workpack for canonical observation" >&2
  exit 2
}

case "$(resolve_wp)" in
  WP-H1-01)
    exec bash scripts/h1-01-observe-exact-sha.sh "$@"
    ;;
  WP-H1-00)
    exec bash scripts/h1-00-observe-exact-sha.sh "$@"
    ;;
  WP-HK-GATE)
    exec bash scripts/hkgate-observe-exact-sha.sh "$@"
    ;;
  WP-HK-10)
    exec bash scripts/hk10-observe-exact-sha.sh "$@"
    ;;
  WP-HK-09B)
    exec bash scripts/hk09b-observe-exact-sha.sh "$@"
    ;;
  WP-HK-09A)
    exec bash scripts/hk09a-observe-exact-sha.sh "$@"
    ;;
  WP-HK-08B)
    exec bash scripts/hk08b-observe-exact-sha.sh "$@"
    ;;
  WP-HK-08A)
    exec bash scripts/hk08a-observe-exact-sha.sh "$@"
    ;;
  WP-HK-07B)
    exec bash scripts/hk07b-observe-exact-sha.sh "$@"
    ;;
  WP-HK-07A)
    exec bash scripts/hk07a-observe-exact-sha.sh "$@"
    ;;
  WP-HK-06C)
    exec bash scripts/hk06c-observe-exact-sha.sh "$@"
    ;;
  WP-HK-06B)
    exec bash scripts/hk06b-observe-exact-sha.sh "$@"
    ;;
  WP-HK-06A)
    exec bash scripts/hk06a-observe-exact-sha.sh "$@"
    ;;
  WP-HK-05)
    exec bash scripts/hk05-observe-exact-sha.sh "$@"
    ;;
  WP-HK-02A)
    exec bash scripts/hk02a-observe-exact-sha.sh "$@"
    ;;
  WP-HK-04)
    exec bash scripts/hk04-observe-exact-sha.sh "$@"
    ;;
  WP-HK-03)
    exec bash scripts/hk03-observe-exact-sha.sh "$@"
    ;;
  WP-HK-02)
    exec bash scripts/hk02-observe-exact-sha.sh "$@"
    ;;
  WP-HK-01)
    exec bash scripts/hk01-observe-exact-sha.sh "$@"
    ;;
  WP-HK-00A)
    exec bash scripts/hk00a-observe-exact-sha.sh "$@"
    ;;
  WP-HK-00)
    exec bash scripts/hk00-observe-exact-sha.sh "$@"
    ;;
  *)
    echo "No canonical observation entrypoint registered for requested workpack" >&2
    exit 2
    ;;
esac
