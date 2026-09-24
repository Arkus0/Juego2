#!/usr/bin/env python3
"""Normalize the effective H1-04 Unity, Source and public evidence for exact-SHA comparison."""

import argparse
import collections
import hashlib
import json
from pathlib import Path


ROOT = Path(__file__).resolve().parent.parent


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def require(value, message):
    if not value:
        raise RuntimeError(message)


def build(inventory_path, mutation_path, public_path):
    inventory = json.loads(inventory_path.read_text(encoding="utf-8-sig"))
    mutation = json.loads(mutation_path.read_text(encoding="utf-8-sig"))
    public = json.loads(public_path.read_text(encoding="utf-8-sig"))
    adoption = json.loads((ROOT / "Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json").read_text(encoding="utf-8"))
    rows = inventory["rows"]
    counts = dict(sorted(collections.Counter(row["kind"] for row in rows).items()))
    compact = json.dumps(inventory, ensure_ascii=False, separators=(",", ":")).encode("utf-8")
    inventory_sha = hashlib.sha256(compact).hexdigest()
    require(inventory["schemaId"] == "arkus.h1-catalogue-effective-inventory@1", "Wrong Unity inventory schema")
    require(inventory["editorVersion"] == mutation["editorVersion"] == "6000.3.24f1", "Effective Editor version mismatch")
    require(inventory["projectIdentity"] == mutation["projectIdentity"] == "arkus.unity-project@1:ArkusUnity", "Effective project identity mismatch")
    require(len(rows) == mutation["baselineCount"] == mutation["restoredCount"] == public["effectiveCount"] == 250, "Effective catalogue extent mismatch")
    require(counts == {"animation-clip": 240, "asset": 2, "component-schema": 3, "material": 2, "prefab": 2, "scene": 1}, "Reviewed kind universe mismatch")
    require(inventory_sha == mutation["baselineInventorySha256"] == mutation["restoredInventorySha256"], "Disposable Unity replay disagrees with committed inventory")
    require(mutation["result"] == public["result"] == "GREEN" and public["referenceAndMcpEqual"], "Effective or public proof is not green")
    require(mutation["originalGuid"] == mutation["movedGuid"] == mutation["restoredGuid"] != mutation["copiedGuid"], "Move/copy/reimport native identities disagree")
    require(mutation["copiedCount"] == 252 and mutation["deletedCount"] == 248, "Effective copy/delete counts disagree")
    require(adoption["schemaId"] == "arkus.h1-04-source-adoption@1" and len(adoption["sources"]) == 2 and len(adoption["slices"]) == 4, "Source adoption record is incomplete")
    for slice_row in adoption["slices"]:
        matches = [row for row in rows if row["path"] == slice_row["assetPath"]]
        require(matches and all(row["contentSha256"] == slice_row["contentSha256"] for row in matches), "Effective Source slice disagrees with adoption fingerprint")
    return {
        "schemaId": "arkus.h1-04-effective-validation@1",
        "result": "GREEN",
        "platform": "windows-x64",
        "projectIdentity": inventory["projectIdentity"],
        "editorVersion": inventory["editorVersion"],
        "editorRevision": "4e7b9b5b6244",
        "packageManifestSha256": digest(ROOT / "Unity/ArkusUnity/Packages/manifest.json"),
        "packageLockSha256": digest(ROOT / "Unity/ArkusUnity/Packages/packages-lock.json"),
        "projectVersionSha256": digest(ROOT / "Unity/ArkusUnity/ProjectSettings/ProjectVersion.txt"),
        "adoptedDistributionSha256": {source["sourceId"]: source["distributionSha256"] for source in adoption["sources"]},
        "inventoryNormalizedSha256": inventory_sha,
        "inventoryCount": len(rows),
        "entriesByKind": counts,
        "catalogueFingerprint": public["catalogueFingerprint"],
        "nativeMoveCopyDeleteReimport": "GREEN",
        "publicJsonlMcpConformance": "GREEN",
        "incompatibleSourceDiagnostic": public["incompatibleSourceDiagnostic"],
        "sourcePrefabResolved": public["sourcePrefabResolved"],
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--inventory", required=True, type=Path)
    parser.add_argument("--mutation", required=True, type=Path)
    parser.add_argument("--public", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()
    result = build(args.inventory, args.mutation, args.public)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(json.dumps(result, sort_keys=True))


if __name__ == "__main__":
    main()
