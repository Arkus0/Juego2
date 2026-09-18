#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SDK_VERSION="8.0.425"
OUT="${ROOT}/artifacts/bootstrap-proof"
DLL="${OUT}/Arkus.HK00.Proof.dll"
PDB="${OUT}/Arkus.HK00.Proof.pdb"
RUNTIMECONFIG="${OUT}/Arkus.HK00.Proof.runtimeconfig.json"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

cd "${ROOT}"

actual_sdk="$(dotnet --version)"
[[ "${actual_sdk}" == "${SDK_VERSION}" ]] || {
  echo "BOOTSTRAP FAIL: expected SDK ${SDK_VERSION}, observed ${actual_sdk}" >&2
  exit 2
}

sdk_base="$(
  dotnet --list-sdks |
  awk -v v="${SDK_VERSION}" '
    $1 == v {
      gsub(/^\[/, "", $2)
      gsub(/\]$/, "", $2)
      print $2
      exit
    }'
)"
[[ -n "${sdk_base}" ]] || { echo "BOOTSTRAP FAIL: selected SDK directory not found" >&2; exit 2; }
sdk_dir="${sdk_base}/${SDK_VERSION}"
csc="${sdk_dir}/Roslyn/bincore/csc.dll"
[[ -f "${csc}" ]] || { echo "BOOTSTRAP FAIL: csc.dll missing at ${csc}" >&2; exit 2; }

dotnet_root="$(dirname "${sdk_base}")"
ref_dir="$(
  find "${dotnet_root}/packs/Microsoft.NETCore.App.Ref" \
    -mindepth 3 -maxdepth 3 -type d -path '*/ref/net8.0' -print |
  sort -V |
  tail -n 1
)"
[[ -n "${ref_dir}" && -d "${ref_dir}" ]] || {
  echo "BOOTSTRAP FAIL: net8.0 reference pack not found under ${dotnet_root}" >&2
  exit 2
}

mapfile -d '' sources < <(git ls-files -z -- 'tools/Arkus.HK00.Proof/*.cs')
[[ ${#sources[@]} -gt 0 ]] || { echo "BOOTSTRAP FAIL: no tracked proof sources" >&2; exit 2; }

mkdir -p "${OUT}"
before="${OUT}/sources.before.sha256"
after="${OUT}/sources.after.sha256"
: >"${before}"
for source in "${sources[@]}"; do
  [[ -f "${ROOT}/${source}" ]] || { echo "BOOTSTRAP FAIL: tracked proof source missing: ${source}" >&2; exit 2; }
  sha256sum "${ROOT}/${source}" >>"${before}"
done

refs=()
while IFS= read -r -d '' ref; do
  refs+=("-r:${ref}")
done < <(find "${ref_dir}" -maxdepth 1 -type f -name '*.dll' -print0 | sort -z)
[[ ${#refs[@]} -gt 0 ]] || { echo "BOOTSTRAP FAIL: no reference assemblies found" >&2; exit 2; }

src_args=()
for source in "${sources[@]}"; do
  src_args+=("${ROOT}/${source}")
done

dotnet "${csc}" \
  -nologo \
  -noconfig \
  -nostdlib+ \
  -target:exe \
  -main:Arkus.HK00.Proof.Program \
  -langversion:9.0 \
  -nullable:enable \
  -warnaserror+ \
  -deterministic+ \
  -optimize+ \
  -debug:portable \
  "-out:${DLL}" \
  "-pdb:${PDB}" \
  "${refs[@]}" \
  "${src_args[@]}"

: >"${after}"
for source in "${sources[@]}"; do
  sha256sum "${ROOT}/${source}" >>"${after}"
done
cmp -s "${before}" "${after}" || {
  echo "BOOTSTRAP FAIL: tracked proof source bytes changed during direct compiler bootstrap" >&2
  exit 2
}

cat >"${RUNTIMECONFIG}" <<'JSON'
{
  "runtimeOptions": {
    "tfm": "net8.0",
    "framework": {
      "name": "Microsoft.NETCore.App",
      "version": "8.0.0"
    },
    "rollForward": "LatestPatch"
  }
}
JSON

[[ -f "${DLL}" && -f "${PDB}" && -f "${RUNTIMECONFIG}" ]] || {
  echo "BOOTSTRAP FAIL: proof oracle output incomplete" >&2
  exit 2
}

printf '%s\n' "${sources[@]}" >"${OUT}/sources.txt"
echo "HK00 proof oracle bootstrapped directly from tracked C# sources with pinned csc."
