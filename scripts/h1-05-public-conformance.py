#!/usr/bin/env python3
"""Exercise the H1-05 canonical session and real Editor through JSONL and MCP."""

import argparse
import json
import os
import subprocess
from pathlib import Path

SCENE = "arkus.h1-05.scene.potes"
PREFAB = "quaternius.medieval.prefab.wall-plaster-window-wide-flat"
CONVENTION = "unity-local-left-handed-y-up-z-forward"
ROOT = Path(__file__).resolve().parent.parent
PROJECT = ROOT / "Unity/ArkusUnity"
MANIFEST = PROJECT / "Assets/Arkus/H1/ManagedScenes/current.json"
FAULT = PROJECT / "Library/Arkus/H1Projection/fail-before-publish"


def require(condition, message):
    if not condition:
        raise RuntimeError(message)


class JsonlClient:
    def __init__(self):
        self.process = subprocess.Popen(
            ["dotnet", str(ROOT / "src/Arkus.Harness.Cli/bin/Release/net8.0/Arkus.Harness.Cli.dll"), "--h1-unity"],
            cwd=ROOT, stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
            text=True, encoding="utf-8", bufsize=1,
        )
        self.sequence = 0
        self.names = None

    def invoke(self, capability, arguments):
        self.sequence += 1
        request = {
            "protocol": "arkus.reference.jsonl@1",
            "request": {
                "projectionVersion": "arkus.neutral-projection@1",
                "requestId": f"h1-05.jsonl.{self.sequence}",
                "capability": capability,
                "acceptedVersions": {"major": 1, "minimumMinor": 0, "maximumMinor": 0},
                "arguments": arguments,
            },
        }
        self.process.stdin.write(json.dumps(request, separators=(",", ":")) + "\n")
        self.process.stdin.flush()
        line = self.process.stdout.readline()
        require(line, f"JSONL host ended before {capability} returned")
        return json.loads(line)["response"]

    def close(self):
        self.process.stdin.close()
        self.process.wait(timeout=30)
        errors = self.process.stderr.read()
        require(self.process.returncode == 0 and not errors, f"JSONL host failed: {errors[:500]}")


class McpClient:
    def __init__(self):
        self.process = subprocess.Popen(
            ["dotnet", str(ROOT / "src/Arkus.Harness.Mcp/bin/Release/net8.0/Arkus.Harness.Mcp.dll"), "--h1-unity"],
            cwd=ROOT, stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
            text=True, encoding="utf-8", bufsize=1,
        )
        self.sequence = 0
        self.request("initialize", {
            "protocolVersion": "2025-11-25", "capabilities": {},
            "clientInfo": {"name": "arkus.h1-05.local-proof", "version": "1.0.0"},
        })
        self.process.stdin.write(json.dumps({"jsonrpc": "2.0", "method": "notifications/initialized", "params": {}}) + "\n")
        self.process.stdin.flush()
        tools = self.request("tools/list", {})["result"]["tools"]
        self.names = {tool["_meta"]["dev.arkus/canonicalKey"]: tool["name"] for tool in tools}
        require(len(self.names) == len(tools), "MCP canonical key collision")

    def request(self, method, parameters):
        self.sequence += 1
        identity = self.sequence
        self.process.stdin.write(json.dumps({"jsonrpc": "2.0", "id": identity, "method": method, "params": parameters}) + "\n")
        self.process.stdin.flush()
        while True:
            line = self.process.stdout.readline()
            require(line, f"MCP host ended before {method} returned")
            response = json.loads(line)
            if response.get("id") == identity:
                require("error" not in response, f"MCP protocol error: {response.get('error')}")
                return response

    def invoke(self, capability, arguments):
        key = capability + "@1.0"
        require(key in self.names, f"MCP omitted {key}")
        response = self.request("tools/call", {"name": self.names[key], "arguments": arguments})["result"]
        require("structuredContent" in response, f"MCP omitted structured outcome for {key}")
        return response["structuredContent"]

    def close(self):
        self.process.stdin.close()
        self.process.wait(timeout=30)
        errors = self.process.stderr.read()
        require(self.process.returncode == 0 and not errors, f"MCP host failed: {errors[:500]}")


def success(client, capability, arguments):
    response = client.invoke(capability, arguments)
    require(response["status"] == "success", f"{capability} failed: {response.get('error')}")
    return response["result"]


def binding(x, link=None):
    components = []
    if link:
        components.append({"kind": "canonical-link", "relation": "faces", "targetObjectId": link})
    return {
        "schemaId": "arkus.unity-binding@1", "targetSceneId": SCENE,
        "source": {"kind": "prefab", "logicalId": PREFAB},
        "transform": {
            "coordinateConvention": CONVENTION,
            "positionMm": {"x": x, "y": 0, "z": 0},
            "rotationMilliDegrees": {"x": 0, "y": 0, "z": 0},
            "scalePpm": {"x": 1000000, "y": 1000000, "z": 1000000},
        },
        "components": components,
    }


def extension(client, subject, x, link=None):
    return success(client, "unity.binding.compile", {"subjectId": subject, "binding": binding(x, link)})["extensionMutation"]


def anchor(client):
    return success(client, "world.summary", {})["world"]


def mutate(client, key, operations):
    current = anchor(client)
    request = {"idempotencyKey": key, "expectedRevision": current["revision"],
               "expectedHash": current["hash"], "operations": operations}
    success(client, "authoring.change.plan", request)
    success(client, "authoring.change.dry-run", request)
    success(client, "authoring.change.apply", request)


def seed(client):
    children = [
        ("plaza.potes", "fixture.plaza", None, 0, None),
        ("market.potes", "fixture.market", "plaza.potes", 500, None),
        ("bar.potes", "fixture.bar", "market.potes", 1250, "plaza.potes"),
        ("workshop.potes", "fixture.workshop", "market.potes", -800, None),
    ]
    operations = []
    for object_id, type_id, parent, _, _ in children:
        row = {"kind": "put-object", "id": object_id, "typeId": type_id}
        if parent:
            row["containerId"] = parent
        operations.append(row)
    for object_id, _, _, x, link in children:
        operations.append(extension(client, object_id, x, link))
    mutate(client, "h1-05.seed", operations)


def projection(client, name):
    return success(client, name, {"sceneLogicalId": SCENE})


def manifest():
    require(MANIFEST.is_file(), "Projection manifest is missing")
    return json.loads(MANIFEST.read_text(encoding="utf-8"))


def managed_scene_path(record):
    expected = PROJECT / "Assets/Arkus/H1/ManagedScenes/generations" / (record["generationId"] + ".unity")
    actual = PROJECT / record["scenePath"]
    require(actual.resolve() == expected.resolve(), "Manifest scene path escapes the fixed managed generation root")
    return actual


def comparable(response):
    return {key: value for key, value in response.items() if key != "requestId"}


def run():
    reference = JsonlClient()
    try:
        names = [value["name"] for value in success(reference, "system.describe", {})["capabilities"]]
        routes = ("unity.projection.plan", "unity.host.projection.materialize", "unity.host.projection.observe")
        for route in routes:
            require(names.count(route) == 1, f"JSONL discovery omitted or duplicated {route}")
        seed(reference)
        plan = projection(reference, "unity.projection.plan")
        require(plan["objectIds"] == ["bar.potes", "market.potes", "plaza.potes", "workshop.potes"], "Plan omitted Potes hierarchy")
        before = anchor(reference)
        journal_before = success(reference, "authoring.journal.read", {})
        first = projection(reference, "unity.host.projection.materialize")
        require(first["active"] and first["current"] and len(first["nodes"]) == 4, "Initial effective scene is incomplete")
        require(first["inputDigest"] == plan["inputDigest"], "Materialization used another canonical plan")
        second = projection(reference, "unity.host.projection.materialize")
        require(second["generationId"] == first["generationId"] and second["graphDigest"] == first["graphDigest"], "Same input caused semantic or generation churn")
        observed = projection(reference, "unity.host.projection.observe")
        require(observed["graphDigest"] == first["graphDigest"] and observed["current"], "Fresh batch observation disagrees")
        require(anchor(reference) == before and success(reference, "authoring.journal.read", {}) == journal_before,
                "Materialization changed canonical state or journal")

        mutate(reference, "h1-05.update", [extension(reference, "bar.potes", 1750, "plaza.potes")])
        expected_new_plan = projection(reference, "unity.projection.plan")
        prior_manifest = manifest()
        FAULT.parent.mkdir(parents=True, exist_ok=True)
        FAULT.write_text("bounded prepublication proof\n", encoding="utf-8")
        try:
            failed = reference.invoke("unity.host.projection.materialize", {"sceneLogicalId": SCENE})
        finally:
            FAULT.unlink(missing_ok=True)
        require(failed["status"] == "error" and failed["error"]["machineCode"] == "projection.forced-prepublication-failure", "Forced staging interruption returned success")
        require(manifest() == prior_manifest, "Failed stage replaced the active manifest")
        stale = projection(reference, "unity.host.projection.observe")
        require(stale["active"] and not stale["current"] and stale["generationId"] == first["generationId"], "Stale generation was presented as current")
        updated = projection(reference, "unity.host.projection.materialize")
        require(updated["current"] and updated["inputDigest"] == expected_new_plan["inputDigest"] and updated["generationId"] != first["generationId"], "Update failed to publish new generation")
        require(next(node for node in updated["nodes"] if node["objectId"] == "bar.potes")["positionMm"]["x"] == 1750,
                "Updated Transform was not observed")

        mutate(reference, "h1-05.delete", [
            {"kind": "remove-extension", "owner": "arkus.unity-binding", "schemaVersion": 1, "subjectId": "workshop.potes"},
            {"kind": "remove-object", "id": "workshop.potes"},
        ])
        deleted = projection(reference, "unity.host.projection.materialize")
        require(deleted["current"] and len(deleted["nodes"]) == 3 and all(node["objectId"] != "workshop.potes" for node in deleted["nodes"]),
                "Deletion did not converge")
        deletion_digest = deleted["graphDigest"]
        output = managed_scene_path(manifest())
        output.unlink()
        output.with_suffix(".unity.meta").unlink(missing_ok=True)
        recreated = projection(reference, "unity.host.projection.materialize")
        require(recreated["current"] and recreated["graphDigest"] == deletion_digest and recreated["generationId"] != deleted["generationId"],
                "Deleting generated output did not reconstruct the same normalized graph")
        scene_file = managed_scene_path(manifest())
        original_scene = scene_file.read_bytes()
        scene_text = original_scene.decode("utf-8")
        for old, replacement, expected_code in (
            ("canonicalObjectId: bar.potes", "canonicalObjectId: market.potes", "projection.duplicate-or-invalid-marker"),
            ("canonicalObjectId: bar.potes", "canonicalObjectId: ", "projection.duplicate-or-invalid-marker"),
        ):
            require(scene_text.count(old) == 1, "Managed scene defect control has no unique marker target")
            scene_file.write_text(scene_text.replace(old, replacement, 1), encoding="utf-8")
            try:
                outcome = reference.invoke("unity.host.projection.observe", {"sceneLogicalId": SCENE})
                require(outcome["status"] == "error" and outcome["error"]["machineCode"] == expected_code,
                        f"Marker defect was not rejected for {expected_code}")
            finally:
                scene_file.write_bytes(original_scene)
            require(projection(reference, "unity.host.projection.observe")["graphDigest"] == deletion_digest,
                    "Restored marker fixture did not return GREEN")
        final_anchor = anchor(reference)
        final_journal = success(reference, "authoring.journal.read", {})
    finally:
        reference.close()

    mcp = McpClient()
    try:
        for route in routes:
            require(route + "@1.0" in mcp.names, f"MCP discovery omitted {route}")
        seed(mcp)
        mutate(mcp, "h1-05.update", [extension(mcp, "bar.potes", 1750, "plaza.potes")])
        mutate(mcp, "h1-05.delete", [
            {"kind": "remove-extension", "owner": "arkus.unity-binding", "schemaVersion": 1, "subjectId": "workshop.potes"},
            {"kind": "remove-object", "id": "workshop.potes"},
        ])
        require(anchor(mcp) == final_anchor and success(mcp, "authoring.journal.read", {}) == final_journal,
                "MCP canonical session disagrees with JSONL")
        mcp_plan = projection(mcp, "unity.projection.plan")
        reference_again = JsonlClient()
        try:
            seed(reference_again)
            mutate(reference_again, "h1-05.update", [extension(reference_again, "bar.potes", 1750, "plaza.potes")])
            mutate(reference_again, "h1-05.delete", [
                {"kind": "remove-extension", "owner": "arkus.unity-binding", "schemaVersion": 1, "subjectId": "workshop.potes"},
                {"kind": "remove-object", "id": "workshop.potes"},
            ])
            ref_plan = projection(reference_again, "unity.projection.plan")
            require(mcp_plan == ref_plan, "Composed plan differs between transports")
            mcp_materialized = projection(mcp, "unity.host.projection.materialize")
            ref_materialized = projection(reference_again, "unity.host.projection.materialize")
            require(mcp_materialized == ref_materialized, "Composed materialize outcome differs between transports")
            mcp_observed = projection(mcp, "unity.host.projection.observe")
            ref_observed = projection(reference_again, "unity.host.projection.observe")
            require(mcp_observed == ref_observed, "Composed observe outcome differs between transports")
        finally:
            reference_again.close()
    finally:
        mcp.close()

    return {
        "schemaId": "arkus.h1-05-public-conformance@1", "result": "GREEN",
        "routes": list(routes), "potesNodeCount": 4, "deletedNodeCount": 3,
        "initialGraphDigest": first["graphDigest"], "finalGraphDigest": recreated["graphDigest"],
        "finalInputDigest": recreated["inputDigest"], "finalCatalogueFingerprint": recreated["catalogueFingerprint"],
        "sameInputGenerationStable": second["generationId"] == first["generationId"],
        "failedStagePreservedPriorGeneration": True,
        "deletedOutputRecreatedSameGraph": recreated["graphDigest"] == deletion_digest,
        "canonicalHashAndJournalUnchangedByProjection": True,
        "referenceAndMcpEqual": True,
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
