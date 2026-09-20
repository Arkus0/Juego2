#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
RUNNER="${ROOT}/scripts/hkgate-observe-exact-sha.sh"
TEST_SOURCE="${ROOT}/tests/Arkus.Harness.Tests/HkGateReadinessTests.cs"

python3 - "${RUNNER}" "${TEST_SOURCE}" <<'PY'
from pathlib import Path
import sys

runner = Path(sys.argv[1])
test_source = Path(sys.argv[2])

if not runner.is_file() or not test_source.is_file():
    print("HK_GATE_PROOF_INFRA_RED required-proof-file-missing", file=sys.stderr)
    raise SystemExit(1)

text = runner.read_text(encoding="utf-8")
lines = text.splitlines()
commands = []
index = 0
prefix = "DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj"
while index < len(lines):
    stripped = lines[index].strip()
    if not stripped.startswith(prefix):
        index += 1
        continue

    parts = []
    while True:
        current = lines[index].strip()
        continued = current.endswith("\\")
        parts.append(current[:-1].rstrip() if continued else current)
        if not continued:
            break
        index += 1
        if index >= len(lines):
            print("HK_GATE_PROOF_INFRA_RED unterminated-dotnet-test-command", file=sys.stderr)
            raise SystemExit(1)
    commands.append(" ".join(parts))
    index += 1

full_validation = [
    command for command in commands
    if "--configuration Release" in command
    and "--no-build" in command
    and "--no-restore" in command
    and "-m:1" in command
    and "--disable-build-servers" in command
    and "--filter" not in command
    and "--logger" not in command
]

if len(full_validation) != 1:
    print(
        "HK_GATE_PROOF_INFRA_RED missing-or-ambiguous-headless-full-validation-execution "
        f"matches={len(full_validation)}",
        file=sys.stderr,
    )
    raise SystemExit(1)

source = test_source.read_text(encoding="utf-8")
if source.count('"14-headless-full-validation"') != 1:
    print("HK_GATE_PROOF_INFRA_RED stage-14-universe-label-missing-or-ambiguous", file=sys.stderr)
    raise SystemExit(1)

print("HK_GATE_PROOF_INFRA GREEN stage14_execution_matches=1 stage14_universe_labels=1")
PY
