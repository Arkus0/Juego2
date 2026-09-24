#!/usr/bin/env python3
"""Run the H1-04 public catalogue through the real JSONL and MCP Unity hosts."""

import argparse
import json
import subprocess
from pathlib import Path


def require(condition, message):
    if not condition:
        raise RuntimeError(message)


class JsonlClient:
    def __init__(self, root):
        self.process = subprocess.Popen(
            ["dotnet", str(root / "src/Arkus.Harness.Cli/bin/Release/net8.0/Arkus.Harness.Cli.dll"), "--h1-unity"],
            cwd=root, stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
            text=True, encoding="utf-8", bufsize=1,
        )
        self.sequence = 0

    def invoke(self, capability, arguments):
        self.sequence += 1
        request = {
            "protocol": "arkus.reference.jsonl@1",
            "request": {
                "projectionVersion": "arkus.neutral-projection@1",
                "requestId": f"h1-04.jsonl.{self.sequence}",
                "capability": capability,
                "acceptedVersions": {"major": 1, "minimumMinor": 0, "maximumMinor": 0},
                "arguments": arguments,
            },
        }
        self.process.stdin.write(json.dumps(request, separators=(",", ":")) + "\n")
        self.process.stdin.flush()
        line = self.process.stdout.readline()
        require(line, "JSONL host ended before a catalogue response")
        return json.loads(line)["response"]

    def close(self):
        self.process.stdin.close()
        self.process.wait(timeout=30)
        errors = self.process.stderr.read()
        require(self.process.returncode == 0 and not errors, f"JSONL host failed: {errors[:500]}")


class McpClient:
    def __init__(self, root):
        self.process = subprocess.Popen(
            ["dotnet", str(root / "src/Arkus.Harness.Mcp/bin/Release/net8.0/Arkus.Harness.Mcp.dll"), "--h1-unity"],
            cwd=root, stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
            text=True, encoding="utf-8", bufsize=1,
        )
        self.sequence = 0
        result = self.request("initialize", {
            "protocolVersion": "2025-11-25", "capabilities": {},
            "clientInfo": {"name": "arkus.h1-04.local-proof", "version": "1.0.0"},
        })
        require(result["result"]["protocolVersion"] == "2025-11-25", "MCP protocol mismatch")
        self.process.stdin.write(json.dumps({"jsonrpc": "2.0", "method": "notifications/initialized", "params": {}}) + "\n")
        self.process.stdin.flush()
        tools = self.request("tools/list", {})["result"]["tools"]
        self.tool_names = {tool["_meta"]["dev.arkus/canonicalKey"]: tool["name"] for tool in tools}
        require(len(self.tool_names) == len(tools), "Duplicate MCP canonical tool key")

    def request(self, method, parameters):
        self.sequence += 1
        identity = self.sequence
        self.process.stdin.write(json.dumps({"jsonrpc": "2.0", "id": identity, "method": method, "params": parameters}, separators=(",", ":")) + "\n")
        self.process.stdin.flush()
        while True:
            line = self.process.stdout.readline()
            require(line, f"MCP host ended before {method} response")
            response = json.loads(line)
            if response.get("id") == identity:
                require("error" not in response, f"MCP protocol error: {response.get('error')}")
                return response

    def invoke(self, capability, arguments):
        key = capability + "@1.0"
        require(key in self.tool_names, f"MCP catalogue capability absent: {key}")
        result = self.request("tools/call", {"name": self.tool_names[key], "arguments": arguments})["result"]
        require("structuredContent" in result, "MCP omitted neutral structured result")
        return result["structuredContent"]

    def close(self):
        self.process.stdin.close()
        self.process.wait(timeout=30)
        errors = self.process.stderr.read()
        require(self.process.returncode == 0 and not errors, f"MCP host failed: {errors[:500]}")


def comparable(response):
    return {key: value for key, value in response.items() if key != "requestId"}


def run(root):
    reference = JsonlClient(root)
    mcp = McpClient(root)
    try:
        for name in ("unity.host.catalogue.query", "unity.host.catalogue.get", "unity.host.catalogue.resolve"):
            require(name + "@1.0" in mcp.tool_names, f"MCP discovery omitted {name}")
        descriptions = reference.invoke("system.describe", {})
        names = [value["name"] for value in descriptions["result"]["capabilities"]]
        for name in ("unity.host.catalogue.query", "unity.host.catalogue.get", "unity.host.catalogue.resolve"):
            require(names.count(name) == 1, f"JSONL discovery omitted or duplicated {name}")

        calls = [
            ("unity.host.catalogue.query", {"pageSize": 5}),
            ("unity.host.catalogue.get", {"kind": "material", "logicalId": "quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster"}),
            ("unity.host.catalogue.resolve", {"kind": "material", "logicalId": "quaternius.medieval.material.mi-plaster"}),
            ("unity.host.catalogue.resolve", {"kind": "prefab", "logicalId": "quaternius.medieval.prefab.wall-plaster-window-wide-flat"}),
        ]
        outcomes = []
        for name, arguments in calls:
            left = reference.invoke(name, arguments)
            right = mcp.invoke(name, arguments)
            require(comparable(left) == comparable(right), f"JSONL/MCP catalogue drift: {name}")
            outcomes.append(left)
        query, material, incompatible, prefab = outcomes
        require(query["status"] == "success" and query["result"]["total"] == 250, "Real catalogue inventory is not the reviewed 250-entry slice")
        require(material["status"] == "success" and material["result"]["entry"]["compatible"], "Source-derived material is not compatible")
        require(incompatible["status"] == "error" and incompatible["error"]["machineCode"] == "catalogue.incompatible-reference", "Incompatible Source material was silently usable")
        require(prefab["status"] == "success", "Source facade prefab did not resolve")

        first = query["result"]
        page_arguments = {"pageSize": 5, "offset": first["nextOffset"], "expectedSnapshotToken": first["snapshotToken"]}
        next_left = reference.invoke("unity.host.catalogue.query", page_arguments)
        next_right = mcp.invoke("unity.host.catalogue.query", page_arguments)
        require(comparable(next_left) == comparable(next_right), "JSONL/MCP second-page catalogue drift")
        require(next_left["status"] == "success" and next_left["result"]["fingerprint"] == first["fingerprint"], "Catalogue snapshot changed between pages")
        return {
            "schemaId": "arkus.h1-04-public-conformance@1", "result": "GREEN",
            "referenceAndMcpEqual": True, "discoveredCapabilities": 3,
            "effectiveCount": first["total"], "catalogueFingerprint": first["fingerprint"],
            "snapshotToken": first["snapshotToken"], "pageSize": 5,
            "firstPageCount": len(first["entries"]), "secondPageCount": len(next_left["result"]["entries"]),
            "compatibleSourceMaterial": material["result"]["entry"]["logicalId"],
            "incompatibleSourceDiagnostic": incompatible["error"]["machineCode"],
            "sourcePrefabResolved": prefab["result"]["entry"]["logicalId"],
        }
    finally:
        reference.close()
        mcp.close()


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()
    root = Path(__file__).resolve().parent.parent
    result = run(root)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(json.dumps(result, sort_keys=True))


if __name__ == "__main__":
    main()
