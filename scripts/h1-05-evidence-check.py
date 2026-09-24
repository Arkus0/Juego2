#!/usr/bin/env python3
"""Check committed H1-05 observation against the accepted effective catalogue."""

import argparse
import json
from pathlib import Path


def require(value, message):
    if not value:
        raise RuntimeError(message)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--inventory", required=True, type=Path)
    parser.add_argument("--public", required=True, type=Path)
    parser.add_argument("--committed-public", type=Path)
    args = parser.parse_args()
    root = Path(__file__).resolve().parent.parent
    accepted = json.loads((root / "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json").read_text(encoding="utf-8"))
    effective = json.loads(args.inventory.read_text(encoding="utf-8"))
    require(accepted == effective, "Effective H1-04 catalogue differs from accepted snapshot")
    require(len(effective["rows"]) == 250, "H1-04 effective catalogue count changed")
    public = json.loads(args.public.read_text(encoding="utf-8"))
    if args.committed_public:
        committed = json.loads(args.committed_public.read_text(encoding="utf-8"))
        require(public == committed, "Fresh H1-05 public observation differs from committed evidence")
    require(public.get("schemaId") == "arkus.h1-05-public-conformance@1" and public.get("result") == "GREEN",
            "H1-05 public observation is not GREEN")
    require(public.get("finalCatalogueFingerprint") == "014d510bd8e9fe0d2754fed0075b3a81787e57a1717baed29e7e03f54295bd21",
            "H1-05 result used another accepted catalogue identity")
    require(public.get("potesNodeCount") == 4 and public.get("deletedNodeCount") == 3,
            "Representative hierarchy/deletion observation changed")
    for key in ("sameInputGenerationStable", "failedStagePreservedPriorGeneration",
                "deletedOutputRecreatedSameGraph", "canonicalHashAndJournalUnchangedByProjection",
                "referenceAndMcpEqual"):
        require(public.get(key) is True, f"Missing required effective proof: {key}")
    require(public.get("routes") == ["unity.projection.plan", "unity.host.projection.materialize",
                                      "unity.host.projection.observe"], "Public capability inventory changed")
    for key in ("initialGraphDigest", "finalGraphDigest", "finalInputDigest"):
        value = public.get(key)
        require(isinstance(value, str) and len(value) == 64 and all(char in "0123456789abcdef" for char in value),
                f"Invalid observed digest: {key}")
    print("H1-05 committed/effective catalogue and public observation: GREEN")


if __name__ == "__main__":
    main()
