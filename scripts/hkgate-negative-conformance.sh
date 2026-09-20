#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
CANDIDATE="$(git rev-parse HEAD)"
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "HK GATE negative conformance requires a clean candidate" >&2; exit 2; }

TMP_ROOT="$(mktemp -d)"
WORKTREE="${TMP_ROOT}/mutant"
LOG="${TMP_ROOT}/gate-stage-execution-omission.log"
cleanup() {
  git worktree remove --force "${WORKTREE}" >/dev/null 2>&1 || true
  rm -rf "${TMP_ROOT}"
}
trap cleanup EXIT

git worktree add --detach "${WORKTREE}" "${CANDIDATE}" >/dev/null
cd "${WORKTREE}"

# G1 seeds the defect the matrix claims: remove the actual unfiltered stage-14
# execution command from the observation runner while deliberately leaving the
# declarative GateStepUniverse label intact.
python3 - <<'PY'
from pathlib import Path
import re

runner = Path("scripts/hkgate-observe-exact-sha.sh")
test_source = Path("tests/Arkus.Harness.Tests/HkGateReadinessTests.cs")
source = test_source.read_text(encoding="utf-8")
universe = re.search(
    r"private static readonly string\[\] GateStepUniverse\s*=\s*\{(?P<body>.*?)\n\s*\};",
    source,
    flags=re.DOTALL,
)
if universe is None or universe.group("body").count('"14-headless-full-validation"') != 1:
    raise SystemExit("HK GATE seeded-defect precondition failed: stage-14 universe label missing or ambiguous")

lines = runner.read_text(encoding="utf-8").splitlines(keepends=True)
prefix = "DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj"
matches = []
index = 0
while index < len(lines):
    stripped = lines[index].strip()
    if not stripped.startswith(prefix):
        index += 1
        continue

    start = index
    parts = []
    while True:
        current = lines[index].strip()
        continued = current.endswith("\\")
        parts.append(current[:-1].rstrip() if continued else current)
        if not continued:
            break
        index += 1
        if index >= len(lines):
            raise SystemExit("HK GATE seeded-defect target has unterminated dotnet test command")
    command = " ".join(parts)
    end = index
    if (
        "--configuration Release" in command
        and "--no-build" in command
        and "--no-restore" in command
        and "-m:1" in command
        and "--disable-build-servers" in command
        and "--filter" not in command
        and "--logger" not in command
    ):
        matches.append((start, end))
    index += 1

if len(matches) != 1:
    raise SystemExit(f"HK GATE seeded-defect target missing or ambiguous: matches={len(matches)}")

start, end = matches[0]
del lines[start:end + 1]
runner.write_text("".join(lines), encoding="utf-8")

# The defect is execution omission only; the declarative list must remain intact.
after_source = test_source.read_text(encoding="utf-8")
after_universe = re.search(
    r"private static readonly string\[\] GateStepUniverse\s*=\s*\{(?P<body>.*?)\n\s*\};",
    after_source,
    flags=re.DOTALL,
)
if after_universe is None or after_universe.group("body").count('"14-headless-full-validation"') != 1:
    raise SystemExit("HK GATE mutant unexpectedly changed GateStepUniverse")
PY

set +e
bash scripts/hkgate-proof-infrastructure-check.sh >"${LOG}" 2>&1
status=$?
set -e

if [[ ${status} -eq 0 ]]; then
  echo "HK GATE negative control: FALSE GREEN for omitted real headless full-validation execution" >&2
  cat "${LOG}" >&2
  exit 1
fi
if ! grep -Fq 'HK_GATE_PROOF_INFRA_RED missing-or-ambiguous-headless-full-validation-execution matches=0' "${LOG}"; then
  echo "HK GATE negative control: oracle RED for the wrong reason" >&2
  cat "${LOG}" >&2
  exit 2
fi

echo 'HK_GATE_NEGATIVE_RED real-headless-full-validation-execution-omitted'
cd "${ROOT}"
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "HK GATE candidate changed during disposable negative control" >&2; exit 2; }
echo 'HK_GATE_NEGATIVE_CONFORMANCE GREEN red_controls=1'
