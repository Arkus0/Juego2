"""Extract an owner-licensed .unitypackage into a disposable spike project, never touching its Packages/manifest.json.

Asset Store packages may embed a Packages/manifest.json (Game Creator 2 does). A non-interactive import would
replace the project's dependency set (it drops URP/Cinemachine/Splines and pins older Input System/AI Navigation),
so the spike extracts Assets/** only and records every excluded entry. Package bytes are never copied into the
repository; only this tool and the recorded hash are.

  python import_unitypackage.py <package> <unity-project> [--report out.json]
"""

from __future__ import annotations

import argparse
import hashlib
import json
import tarfile
from pathlib import Path


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("package", type=Path)
    parser.add_argument("project", type=Path)
    parser.add_argument("--report", type=Path)
    args = parser.parse_args()
    data = args.package.read_bytes()
    report = {"package": args.package.name, "sha256": hashlib.sha256(data).hexdigest(), "bytes": len(data),
              "imported": 0, "excluded": []}
    with tarfile.open(args.package, "r:gz") as tar:
        members = {m.name: m for m in tar.getmembers()}
        for name, member in members.items():
            if not name.endswith("/pathname"):
                continue
            guid = name.split("/")[0]
            path = tar.extractfile(member).read().decode("utf-8").splitlines()[0].strip()
            if not path.startswith("Assets/"):
                report["excluded"].append(path)
                continue
            target = args.project / path
            asset = members.get(f"{guid}/asset")
            meta = members.get(f"{guid}/asset.meta")
            if asset is not None:
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(tar.extractfile(asset).read())
            else:
                target.mkdir(parents=True, exist_ok=True)
            if meta is not None:
                Path(str(target) + ".meta").write_bytes(tar.extractfile(meta).read())
            report["imported"] += 1
    text = json.dumps(report, indent=2)
    if args.report:
        args.report.parent.mkdir(parents=True, exist_ok=True)
        args.report.write_text(text + "\n", encoding="utf-8")
    print(f"UNITYPACKAGE_EXTRACTED imported={report['imported']} excluded={report['excluded']} sha256={report['sha256']}")


if __name__ == "__main__":
    main()
