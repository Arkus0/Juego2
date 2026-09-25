#!/usr/bin/env python3
"""Focused effective-Unity conformance for WP-H1-06.

The accepted H1-05 public path is reused rather than introducing a second authority.
This proof inspects the new normalized prefab realization fields, source immutability,
managed-root confinement, delete/rebuild convergence, and stable source diagnostics.
"""

import argparse
import hashlib
import importlib.util
import json
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
PROJECT = ROOT / "Unity/ArkusUnity"
H105_PATH = ROOT / "scripts/h1-05-public-conformance.py"
SOURCE = PROJECT / "Assets/Arkus/H1/SourceSlice/Wall_Plaster_Window_Wide_Flat.fbx"
MAPPING = PROJECT / "Assets/Arkus/H1/CatalogueMapping.json"
PREFAB = "quaternius.medieval.prefab.wall-plaster-window-wide-flat"
ASSET = "quaternius.medieval.asset.wall-plaster-window-wide-flat"
SOURCE_GUID = "a914dbae2609f0107a8bce353d33727c"
SOURCE_LOCAL_ID = "-927199367670048503"
SOURCE_SHA = "45825c565b9d1027036ce7fc922f7e7a7d69bc05e459d886eb738bf1eafc92b8"
MANAGED_PREFIX = "Assets/Arkus/H1/ManagedPrefabs/generations/"
UNITY = Path(r"C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe")
SCRATCH = PROJECT / "Library/Arkus/H1PrefabRealization"


def load_h105():
    spec = importlib.util.spec_from_file_location("arkus_h105_public", H105_PATH)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load accepted H1-05 public proof helper")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


h105 = load_h105()
require = h105.require


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def inject_stale_extra_relation(derivative_path):
    require(derivative_path.startswith(MANAGED_PREFIX), "Drift target escaped the managed derivative root")
    asset = PROJECT / derivative_path
    require(asset.is_file(), "Managed derivative is missing before drift injection")
    before = sha(asset)
    SCRATCH.mkdir(parents=True, exist_ok=True)
    request = SCRATCH / "stale-extra-request.json"
    log = SCRATCH / "stale-extra-injection.log"
    request.write_text(json.dumps({"derivativePath": derivative_path}), encoding="utf-8")
    log.unlink(missing_ok=True)
    process = subprocess.run([
        str(UNITY), "-batchmode", "-nographics", "-quit", "-projectPath", str(PROJECT),
        "-executeMethod", "Arkus.H1.Editor.H1PrefabNestedConformance.InjectExtraMaterial",
        "-arkus-h1-input", str(request), "-logFile", str(log),
    ], timeout=180, check=False)
    require(process.returncode == 0 and
            f"H106_STALE_EXTRA_RELATION_INJECTED:{derivative_path}" in log.read_text(encoding="utf-8"),
            f"Unity could not inject stale managed derivative relation; inspect {log}")
    require(sha(asset) != before, "Stale derivative fixture left the asset unchanged")


def normalized(node):
    return {
        "sourceLogicalId": node["sourceLogicalId"],
        "sourceKind": node["sourceKind"],
        "sourcePath": node["sourcePath"],
        "sourceGuid": node["sourceGuid"],
        "sourceLocalFileId": node["sourceLocalFileId"],
        "sourceContentSha256": node["sourceContentSha256"],
        "realizationKind": node["realizationKind"],
        "realizedPath": node["realizedPath"],
        "prefabGenerationId": node["prefabGenerationId"],
        "relationshipDigest": node["relationshipDigest"],
        "relationships": node["relationships"],
    }


def assert_prefab_node(node):
    require(node["sourceLogicalId"] == PREFAB, "Observed prefab logical identity changed")
    require(node["sourceKind"] == "prefab", "Prefab binding was normalized as another source kind")
    require(node["sourcePath"] == "Assets/Arkus/H1/SourceSlice/Wall_Plaster_Window_Wide_Flat.fbx",
            "Observed prefab source escaped the accepted H1-04 Source slice")
    require(node["sourceGuid"] == SOURCE_GUID and node["sourceLocalFileId"] == SOURCE_LOCAL_ID,
            "Observed prefab native source identity changed")
    require(node["sourceContentSha256"] == SOURCE_SHA, "Observed prefab source content fingerprint changed")
    require(node["realizationKind"] == "managed-prefab-variant", "Source prefab was flattened or copied as an unmanaged realization")
    generation = node["prefabGenerationId"]
    require(len(generation) == 32 and all(c in "0123456789abcdef" for c in generation), "Managed prefab generation identity is malformed")
    expected_prefix = MANAGED_PREFIX + generation + "/"
    require(node["realizedPath"].startswith(expected_prefix), "Managed prefab derivative escaped its generation root")
    require("/" not in node["realizedPath"][len(expected_prefix):], "Managed prefab derivative escaped its immediate generation directory")
    require(len(node["relationshipDigest"]) == 64, "Normalized prefab relationship digest is malformed")
    relationships = node["relationships"]
    base = [row for row in relationships if row["kind"] == "variant-base"]
    require(len(base) == 1, "Managed prefab variant omitted or duplicated its source relationship")
    require(base[0]["assetPath"] == node["sourcePath"] and base[0]["assetGuid"] == SOURCE_GUID and
            base[0]["localFileId"] == SOURCE_LOCAL_ID, "Managed prefab variant source relationship is not exact")
    require(any(row["kind"] == "mesh-reference" for row in relationships), "Representative Quaternius prefab omitted its mesh relationship")
    require(any(row["kind"] == "material-reference" for row in relationships), "Representative Quaternius prefab omitted its material relationship")


def extension_with_source(client, subject, logical_id):
    binding = h105.binding(0)
    binding["source"] = {"kind": "prefab", "logicalId": logical_id}
    return h105.success(client, "unity.binding.compile", {"subjectId": subject, "binding": binding})["extensionMutation"]


def source_diagnostic(logical_id, expected_code, key):
    client = h105.JsonlClient()
    try:
        h105.mutate(client, key, [
            {"kind": "put-object", "id": "probe.potes", "typeId": "fixture.probe"},
            extension_with_source(client, "probe.potes", logical_id),
        ])
        before = h105.anchor(client)
        outcome = client.invoke("unity.projection.plan", {"sceneLogicalId": h105.SCENE})
        require(outcome["status"] == "error" and outcome["error"]["machineCode"] == expected_code,
                f"Expected {expected_code}, observed {outcome}")
        require(h105.anchor(client) == before, "Failed source resolution changed canonical world identity")
    finally:
        client.close()


def rebound_diagnostic():
    original = MAPPING.read_bytes()
    client = h105.JsonlClient()
    try:
        h105.seed(client)
        before = h105.anchor(client)
        journal = h105.success(client, "authoring.journal.read", {})
        document = json.loads(original.decode("utf-8"))
        row = next(entry for entry in document["entries"] if entry["logicalId"] == PREFAB)
        row["contentSha256"] = "0" * 64
        MAPPING.write_text(json.dumps(document, indent=2) + "\n", encoding="utf-8")
        outcome = client.invoke("unity.projection.plan", {"sceneLogicalId": h105.SCENE})
        require(outcome["status"] == "error" and outcome["error"]["machineCode"] == "projection.source-rebound",
                "Rebound catalogue mapping did not fail closed with projection.source-rebound")
        require(h105.anchor(client) == before and h105.success(client, "authoring.journal.read", {}) == journal,
                "Catalogue rebound control changed canonical world state or journal")
    finally:
        MAPPING.write_bytes(original)
        client.close()


def run():
    require(SOURCE.is_file(), "Accepted H1-04 Quaternius Source prefab input is missing")
    require(sha(SOURCE) == SOURCE_SHA, "Accepted H1-04 Quaternius Source prefab hash is stale before H1-06 proof")
    source_before = sha(SOURCE)

    reference = h105.JsonlClient()
    try:
        h105.seed(reference)
        canonical_before = h105.anchor(reference)
        journal_before = h105.success(reference, "authoring.journal.read", {})
        plan = h105.projection(reference, "unity.projection.plan")
        first = h105.projection(reference, "unity.host.projection.materialize")
        require(first["active"] and first["current"] and first["canonicalHash"] == plan["canonicalHash"],
                "Initial H1-06 materialization was not current for the canonical plan")
        require(len(first["nodes"]) == 4 and len(first["realizationDigest"]) == 64,
                "Initial H1-06 normalized realization is incomplete")
        for node in first["nodes"]:
            assert_prefab_node(node)
        profile = {node["objectId"]: normalized(node) for node in first["nodes"]}

        second = h105.projection(reference, "unity.host.projection.materialize")
        require(second["generationId"] == first["generationId"], "Same input caused managed-scene generation churn")
        require({node["objectId"]: normalized(node) for node in second["nodes"]} == profile,
                "Same input caused prefab realization churn")

        # A valid deterministic derivative is now made stale without touching its lineage or source.
        # Materialize must reject reuse, rebuild the derivative, stage and publish clean relationships.
        inject_stale_extra_relation(first["nodes"][0]["realizedPath"])
        recovered = h105.projection(reference, "unity.host.projection.materialize")
        require(recovered["generationId"] != second["generationId"],
                "Stale extra-bearing derivative was reused without staging a new scene")
        require({node["objectId"]: normalized(node) for node in recovered["nodes"]} == profile,
                "Stale extra-bearing derivative was adopted instead of rebuilt from source")
        observed_recovery = h105.projection(reference, "unity.host.projection.observe")
        require(observed_recovery["current"] and observed_recovery["generationId"] == recovered["generationId"],
                "Recovered derivative was not published as the active current generation")

        paths = sorted({PROJECT / node["realizedPath"] for node in recovered["nodes"]})
        require(paths, "No managed prefab derivative was observed")
        for path in paths:
            require(path.is_file(), f"Managed derivative is missing: {path}")
            path.unlink()
            Path(str(path) + ".meta").unlink(missing_ok=True)

        rebuilt = h105.projection(reference, "unity.host.projection.materialize")
        rebuilt_profile = {node["objectId"]: normalized(node) for node in rebuilt["nodes"]}
        require(rebuilt_profile == profile, "Deleting managed derivatives did not reproduce the same normalized prefab relationships")
        require(rebuilt["generationId"] != recovered["generationId"], "Deleted managed derivative was not causally rebuilt")
        require(h105.anchor(reference) == canonical_before and h105.success(reference, "authoring.journal.read", {}) == journal_before,
                "Prefab materialization/rebuild changed canonical world state or journal")
    finally:
        reference.close()

    source_diagnostic("prefab.absent", "projection.source-missing", "h1-06.missing")
    source_diagnostic(ASSET, "projection.source-wrong-type", "h1-06.wrong-type")
    rebound_diagnostic()

    require(sha(SOURCE) == source_before == SOURCE_SHA, "H1-06 mutated the accepted Quaternius source prefab")

    return {
        "schemaId": "arkus.h1-06-public-conformance@1",
        "result": "GREEN",
        "sourceLogicalId": PREFAB,
        "sourceGuid": SOURCE_GUID,
        "sourceLocalFileId": SOURCE_LOCAL_ID,
        "sourceContentSha256": SOURCE_SHA,
        "sourceUnchanged": True,
        "nodeCount": 4,
        "sameInputRealizationStable": True,
        "staleExtraDerivativeRejectedAndRebuilt": True,
        "staleExtraRecoveryPublished": True,
        "deletedDerivativeRebuiltSameNormalizedRelationships": True,
        "managedDerivativeRoot": MANAGED_PREFIX,
        "missingDiagnostic": "projection.source-missing",
        "wrongTypeDiagnostic": "projection.source-wrong-type",
        "reboundDiagnostic": "projection.source-rebound",
        "canonicalHashAndJournalUnchangedByRealization": True,
        "initialGraphDigest": first["graphDigest"],
        "initialRealizationDigest": first["realizationDigest"],
        "rebuiltGraphDigest": rebuilt["graphDigest"],
        "rebuiltRelationshipDigests": sorted({node["relationshipDigest"] for node in rebuilt["nodes"]}),
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = run()
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(json.dumps(result, sort_keys=True))


if __name__ == "__main__":
    main()
