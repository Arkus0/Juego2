#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
RUNNER="${ROOT}/scripts/dwgate-observe-exact-sha.sh"

python3 - "${RUNNER}" <<'PY'
from pathlib import Path
import re
import sys

runner = Path(sys.argv[1])
if not runner.is_file():
    print("DW_GATE_PROOF_INFRA_RED runner-missing", file=sys.stderr)
    raise SystemExit(1)

text = runner.read_text(encoding="utf-8")
decl = re.search(r"GATE_STAGES=\((?P<body>.*?)\)", text, flags=re.DOTALL)
if decl is None:
    print("DW_GATE_PROOF_INFRA_RED stage-universe-missing", file=sys.stderr)
    raise SystemExit(1)

declared = re.findall(r"^\s*([a-z0-9-]+)\s*$", decl.group("body"), flags=re.MULTILINE)
expected = [
    "dw00-generic",
    "dw01-city-invariants",
    "dw02-city-queries",
    "dw03-pa-losslessness",
    "dw04-paired-agent-evidence",
    "dw05-generic-boundary",
    "gate-evidence-audit",
    "gate-negative-conformance",
    "full-regression",
]
executed = re.findall(r"^\s*run_gate_stage\s+([a-z0-9-]+)\b", text, flags=re.MULTILINE)

if declared != expected:
    print(f"DW_GATE_PROOF_INFRA_RED stage-universe-mismatch declared={declared}", file=sys.stderr)
    raise SystemExit(1)
if executed != expected:
    print(f"DW_GATE_PROOF_INFRA_RED stage-execution-mismatch executed={executed}", file=sys.stderr)
    raise SystemExit(1)

checks = {
    "dw00-generic": "FullyQualifiedName~Dw00",
    "dw01-city-invariants": "FullyQualifiedName~Dw01",
    "dw02-city-queries": "FullyQualifiedName~Dw02",
    "dw03-pa-losslessness": "FullyQualifiedName~Dw03",
    "dw04-paired-agent-evidence": "scripts/dw04-verify-exact-sha.sh",
    "dw05-generic-boundary": "FullyQualifiedName~Dw05",
    "gate-evidence-audit": "scripts/dwgate-evidence-audit.py",
    "gate-negative-conformance": "scripts/dwgate-negative-conformance.sh",
}
lines = text.splitlines()
for stage, token in checks.items():
    matches = [line for line in lines if re.match(rf"\s*run_gate_stage\s+{re.escape(stage)}\b", line)]
    if len(matches) != 1 or token not in matches[0]:
        print(f"DW_GATE_PROOF_INFRA_RED stage-wiring-mismatch stage={stage}", file=sys.stderr)
        raise SystemExit(1)

reg = [line for line in lines if re.match(r"\s*run_gate_stage\s+full-regression\b", line)]
if len(reg) != 1 or "dotnet test" not in reg[0] or "--filter" in reg[0]:
    print("DW_GATE_PROOF_INFRA_RED full-regression-not-unfiltered", file=sys.stderr)
    raise SystemExit(1)

print("DW_GATE_PROOF_INFRA_GREEN declared=9 executed=9 full_regression=1")
PY
