#!/usr/bin/env bash
set -euo pipefail

TARGET_SHA="${1:-}"
if [[ ! "${TARGET_SHA}" =~ ^[0-9a-fA-F]{40}$ ]]; then
  echo "CITY04_VERIFY_RED: exact 40-character candidate SHA required" >&2
  exit 2
fi

if [[ "$(git rev-parse HEAD)" != "${TARGET_SHA}" ]]; then
  echo "CITY04_VERIFY_RED: checkout HEAD does not equal candidate SHA" >&2
  exit 2
fi

# The candidate workflow has already proven an initially clean checkout before
# writing VALIDATION_CONTEXT.json. From here on, reject tracked mutations while
# allowing that workflow-owned untracked context artifact.
if ! git diff --quiet || ! git diff --cached --quiet; then
  echo "CITY04_VERIFY_RED: tracked candidate bytes changed during validation" >&2
  exit 2
fi

expect_blob() {
  local path="$1" expected="$2"
  if [[ ! -f "${path}" ]]; then
    echo "CITY04_VERIFY_RED: missing ${path}" >&2
    exit 3
  fi
  local actual
  actual="$(git hash-object "${path}")"
  if [[ "${actual}" != "${expected}" ]]; then
    echo "CITY04_VERIFY_RED: blob drift ${path}: ${actual} != ${expected}" >&2
    exit 3
  fi
}

require_nonempty() {
  local path="$1"
  if [[ ! -s "${path}" ]]; then
    echo "CITY04_VERIFY_RED: missing/empty ${path}" >&2
    exit 4
  fi
}

require_text() {
  local pattern="$1" path="$2"
  if ! grep -Fq -- "${pattern}" "${path}"; then
    echo "CITY04_VERIFY_RED: evidence marker not found in ${path}: ${pattern}" >&2
    exit 5
  fi
}

# Accepted construction/source identities from durable CITY-04 local evidence.
expect_blob "Docs/production/CITY_PRODUCT_SEED.md" "3186b80d173cd20f961f33d5a28bf447b2c6971d"
expect_blob "Unity/ArkusUnity/Assets/Arkus/CITY/City04Greybox.unity" "6846ac662c01aa4ed05c6c3b9c2ee1559f9383f3"
expect_blob "Unity/ArkusUnity/Assets/Arkus/CITY/City04Layout.json" "7b1c83815188f323ae23a02f43aa08c085592f6e"
expect_blob "Unity/ArkusUnity/Packages/manifest.json" "19c29d4989182a73f559eefbf131775b99097a07"
expect_blob "Unity/ArkusUnity/Packages/packages-lock.json" "f1cd55ead4c8bd5254372af66942e1682c2e3225"

for path in \
  Unity/ArkusUnity/Assets/Arkus/CITY/City04TraversalProbe.cs \
  Unity/ArkusUnity/Assets/Arkus/CITY/Editor/City04GreyboxBuilder.cs \
  Docs/evidence/WP-CITY-04/overhead.png \
  Docs/evidence/WP-CITY-04/x1_to_casco.png \
  Docs/evidence/WP-CITY-04/plaza_to_casco.png \
  Docs/evidence/WP-CITY-04/landing_to_port.png; do
  require_nonempty "${path}"
done

require_text "TECHNICAL SCENE-HANDOFF PASS EVIDENCE" Docs/evidence/WP-CITY-04/LOCAL_GREYBOX_OBSERVATION.md
require_text "CITY04_BUILD_GREEN" Docs/evidence/WP-CITY-04/LOCAL_GREYBOX_OBSERVATION.md
require_text "CITY04_PHYSICAL_CUTS_GREEN" Docs/evidence/WP-CITY-04/LOCAL_GREYBOX_OBSERVATION.md
require_text "CITY04_CAPTURE_GREEN" Docs/evidence/WP-CITY-04/LOCAL_GREYBOX_OBSERVATION.md
require_text "FINAL CITY-04 DELIVERABLE / BINDING HANDOFF" Docs/evidence/WP-CITY-04/PROPOSED_CITY07_DEMO_HANDOFF.md
require_text "SCN-01..13" Docs/evidence/WP-CITY-04/PROPOSED_CITY07_DEMO_HANDOFF.md
require_text "Final human spatial verdict owner: **CITY-07**" Docs/evidence/WP-CITY-04/HUMAN_TRAVERSAL_RUN_SHEET.md
require_text "WORKER_PRE_REVIEW: CLEAN" Docs/evidence/WP-CITY-04/WORKER_PRE_REVIEW.md

echo "CITY04_EXACT_SHA_VALIDATION_V1"
echo "Candidate SHA: ${TARGET_SHA}"
echo "Construction identity: GREEN"
echo "Durable technical observation: GREEN"
echo "CITY-07 human-proof handoff: GREEN"
echo "Result: PASS"
