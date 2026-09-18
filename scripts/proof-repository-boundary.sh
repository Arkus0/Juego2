#!/usr/bin/env bash
#
# Independent repository-boundary oracle for WP-HK-00.
#
# This intentionally does not parse kernel-manifest.json and does not share the
# C# enumerator. It proves properties of the checkout itself before restore/build
# can create generated files. The C# proof then handles ownership, MSBuild,
# assemblies, PDBs and compiler inputs independently.
#
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

fail() {
  local id="$1"
  shift
  printf '%s: %s\n' "${id}" "$*" >&2
  exit 1
}

if ! top="$(git -C "${ROOT}" rev-parse --show-toplevel 2>/dev/null)"; then
  fail HK00-BOUNDARY-NOT-GIT-CHECKOUT "repository boundary oracle requires a Git checkout"
fi

if [[ "$(cd "${top}" && pwd)" != "${ROOT}" ]]; then
  fail HK00-BOUNDARY-WRONG-GIT-ROOT "script root is not the Git worktree root"
fi

is_csharp_input() {
  local lower="${1,,}"
  [[ "${lower}" == *.cs || "${lower}" == *.csproj ]]
}

check_case() {
  local rel="$1"
  local lower="${rel,,}"
  if [[ "${lower}" == *.cs && "${rel}" != *.cs ]]; then
    fail HK00-BOUNDARY-NONCANONICAL-CASE "C# source uses non-canonical extension casing: ${rel}"
  fi
  if [[ "${lower}" == *.csproj && "${rel}" != *.csproj ]]; then
    fail HK00-BOUNDARY-NONCANONICAL-CASE "C# project uses non-canonical extension casing: ${rel}"
  fi
}

# The generated root-level artifacts workspace is excluded from the C# inventory.
# Therefore no repository input may be tracked there. Otherwise the exclusion
# would become a proof escape hatch. Symlinks and gitlinks are also forbidden:
# the candidate must own the bytes it proves, not an external target/tree.
while IFS= read -r -d '' rel; do
  case "${rel}" in
    artifacts/*)
      fail HK00-BOUNDARY-TRACKED-ARTIFACT "tracked repository input is hidden under excluded artifacts/: ${rel}"
      ;;
  esac

  check_case "${rel}"

  mode="$(git -C "${ROOT}" ls-files -s -- "${rel}" | awk 'NR == 1 { print $1 }')"
  case "${mode}" in
    120000)
      fail HK00-BOUNDARY-SYMLINK "tracked symlinks are forbidden in the HK00 proof boundary: ${rel}"
      ;;
    160000)
      fail HK00-BOUNDARY-GITLINK "tracked gitlinks/submodules are forbidden in the HK00 proof boundary: ${rel}"
      ;;
  esac

  # A tracked source/project must also be present as a regular file in this
  # checkout. This makes sparse/missing worktrees fail closed instead of silently
  # shrinking the physical universe inspected by the C# proof.
  if is_csharp_input "${rel}" && [[ ! -f "${ROOT}/${rel}" ]]; then
    fail HK00-BOUNDARY-TRACKED-SOURCE-MISSING "tracked C# source/project is absent from the checkout: ${rel}"
  fi
done < <(git -C "${ROOT}" ls-files -z)

# Before restore/build, every physical C# source/project outside the generated
# artifacts workspace must be a tracked repository input. This catches files a
# developer or automation could otherwise place in the checkout without making
# them part of the exact candidate commit.
while IFS= read -r -d '' path; do
  rel="${path#${ROOT}/}"
  check_case "${rel}"

  if ! git -C "${ROOT}" ls-files --error-unmatch -- "${rel}" >/dev/null 2>&1; then
    fail HK00-BOUNDARY-UNTRACKED-SOURCE "physical C# source/project is not part of the candidate commit: ${rel}"
  fi
done < <(
  find "${ROOT}" \
    -path "${ROOT}/.git" -prune -o \
    -path "${ROOT}/artifacts" -prune -o \
    -type f \( -iname '*.cs' -o -iname '*.csproj' \) -print0
)

printf 'repository boundary oracle: GREEN\n'
