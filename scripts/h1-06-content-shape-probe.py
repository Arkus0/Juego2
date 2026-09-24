#!/usr/bin/env python3
"""Bounded H1-06 omission probe over the already approved Quaternius archive.

This never imports or adopts additional assets. It only inspects the exact H1-04-pinned
archive to confirm that the representative content shape includes wall/roof/door/window/
prop candidates and reports prefab dependency-shaped GUID references when present.
"""

import argparse
import hashlib
import json
import re
import zipfile
from pathlib import Path

ARCHIVE_SHA = "b9d757dd2608a5cee4d9ee1e8183f6cb4cad9d27480841a905180def9c7d8b10"
CATEGORY_TERMS = {
    "wall": ("wall",),
    "roof": ("roof", "thatch"),
    "door": ("door", "gate"),
    "window": ("window",),
    "prop": ("prop", "barrel", "crate", "cart", "bench", "table", "lamp", "sign", "well", "fence"),
}
GUID_RE = re.compile(rb"guid:\s*([0-9a-fA-F]{32})")


def require(condition, message):
    if not condition:
        raise RuntimeError(message)


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def run(archive):
    require(archive.is_file(), f"Approved Quaternius archive is missing: {archive}")
    require(digest(archive) == ARCHIVE_SHA, "Quaternius archive does not match the H1-04 accepted fingerprint")
    with zipfile.ZipFile(archive) as zf:
        names = sorted(info.filename for info in zf.infolist() if not info.is_dir())
        candidates = {}
        for category, terms in CATEGORY_TERMS.items():
            rows = [
                name for name in names
                if any(term in name.lower() for term in terms)
                and name.lower().endswith((".prefab", ".fbx"))
            ]
            require(rows, f"Approved archive has no {category} prefab/FBX candidate")
            candidates[category] = rows[:12]

        prefab_relations = []
        selected_prefabs = []
        for category in CATEGORY_TERMS:
            selected_prefabs.extend(name for name in candidates[category] if name.lower().endswith(".prefab"))
        for name in sorted(set(selected_prefabs))[:40]:
            data = zf.read(name)
            refs = sorted({match.group(1).decode("ascii").lower() for match in GUID_RE.finditer(data)})
            prefab_relations.append({"entry": name, "guidReferenceCount": len(refs), "sampleGuids": refs[:8]})

    return {
        "schemaId": "arkus.h1-06-content-shape-probe@1",
        "archiveSha256": ARCHIVE_SHA,
        "classification": "exploratory-omission-detector-not-adoption",
        "categoryTerms": {key: list(value) for key, value in CATEGORY_TERMS.items()},
        "categories": candidates,
        "prefabDependencyShape": prefab_relations,
        "allRepresentativeCategoriesPresent": True,
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--archive", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = run(args.archive)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(json.dumps(result, sort_keys=True))


if __name__ == "__main__":
    main()
