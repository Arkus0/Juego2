#!/usr/bin/env bash
# Run exactly one WP-H1-11 Unity EditMode stage in its own GameCI Editor process and require that
# exact test case to be reported (a filter that selects nothing can never pass as GREEN).
#
# usage: scripts/h1-11-unity-stage.sh <label> <test-method> [allow-skip]
set -uo pipefail

label="$1"
method="$2"
allow_skip="${3:-no}"
: "${PINNED_GAMECI_BIN:?PINNED_GAMECI_BIN must point at the pinned GameCI CLI}"
fullname="Arkus.H1.Editor.Tests.H1RepresentativeSliceTests.${method}"
artifacts="artifacts/h1-11-${label}"

"${PINNED_GAMECI_BIN}" test \
  --docker \
  --dockerShmSize=1025m \
  --engine=unity \
  Unity/ArkusUnity \
  --testPlatforms=editmode \
  --customParameters="-testFilter ${fullname}" \
  --coverageEnabled false \
  --artifactsPath="${artifacts}" \
  --dockerIsolationMode=default \
  --containerRegistryRepository=unityci/editor \
  --containerRegistryImageVersion=3
code=$?
echo "H1_11_UNITY_PROCESS label=${label} exit=${code}"

python3 - "${artifacts}" "${fullname}" "${allow_skip}" <<'PY'
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

root, fullname, allow_skip = Path(sys.argv[1]), sys.argv[2], sys.argv[3] == "allow-skip"
cases = []
for path in sorted(root.rglob("*results.xml")) if root.exists() else []:
    try:
        tree = ET.parse(path)
    except Exception as exc:
        print(f"H1_11_STAGE_RESULT xml_parse_error={type(exc).__name__}")
        continue
    for node in tree.getroot().iter("test-case"):
        cases.append(node)
selected = [node for node in cases if node.attrib.get("fullname") == fullname]
others = [node.attrib.get("fullname") for node in cases if node.attrib.get("fullname") != fullname]
if others:
    print(f"H1_11_STAGE_RESULT unexpected_cases={len(others)}")
if len(selected) != 1:
    print(f"H1_11_STAGE_RESULT test={fullname} status=NOT_EXECUTED cases={len(cases)}")
    sys.exit(1)
result = selected[0].attrib.get("result", "")
print(f"H1_11_STAGE_RESULT test={fullname} result={result}")
if result != "Passed":
    failure = selected[0].find("failure/message")
    reason = selected[0].find("reason/message")
    for element in (failure, reason):
        if element is not None and element.text:
            for line in element.text.strip().splitlines()[:60]:
                print(f"H1_11_STAGE_MESSAGE {line}")
ok = result == "Passed" or (allow_skip and result == "Skipped")
sys.exit(0 if ok and not others else 1)
PY
verdict=$?
if [[ ${code} -ne 0 && ! ( "${allow_skip}" == "allow-skip" && ${verdict} -eq 0 ) ]]; then
  exit 1
fi
exit "${verdict}"
