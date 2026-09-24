#!/usr/bin/env python3
"""Compare a fresh local H1-04 round with the committed reviewed Unity evidence."""

import argparse
import json
from pathlib import Path


def read(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--inventory", required=True, type=Path)
    parser.add_argument("--mutation", required=True, type=Path)
    parser.add_argument("--public", required=True, type=Path)
    parser.add_argument("--summary", required=True, type=Path)
    args = parser.parse_args()
    root = Path(__file__).resolve().parent.parent / "Docs/evidence/WP-H1-04"
    for label, observed, committed in (
        ("inventory", args.inventory, root / "EFFECTIVE_INVENTORY.json"),
        ("public conformance", args.public, root / "PUBLIC_CONFORMANCE.json"),
        ("effective validation", args.summary, root / "EFFECTIVE_VALIDATION.json"),
    ):
        if read(observed) != read(committed):
            raise RuntimeError(f"Fresh {label} disagrees with committed H1-04 evidence")
    latest = read(args.mutation)
    reviewed = read(root / "EFFECTIVE_MUTATION.json")
    latest.pop("copiedGuid", None)
    reviewed.pop("copiedGuid", None)
    if latest != reviewed:
        raise RuntimeError("Fresh move/copy/delete/reimport observation disagrees with committed H1-04 evidence")
    print("H1-04 fresh local Unity/Source/public evidence equals the committed review evidence")


if __name__ == "__main__":
    main()
