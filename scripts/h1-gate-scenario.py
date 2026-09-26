#!/usr/bin/env python3
"""WP-H1-GATE deterministic reference scenario, driven only through the public H1 hosts.

This client is the Gate's orchestration. It is not a product capability, and it has no product authority. It starts
the fixed public `--h1-unity` reference (JSONL) or MCP host and talks to it only over stdio. It discovers the composed
contract and executes stages 3-14 and 16 of `Docs/engineering/H1_UNITY_PARITY_GATE.md` through public capabilities.
It records every request/response frame in an append-only transcript. The workflow owns stages 1, 2, 15 (reference vs
MCP comparison, see `compare`) and 17.

The scenario touches the Unity project directly in exactly two places. Both are declared operator actions and neither
is a bridge effect:

* stage 13 edits one managed Transform in the published scene file. This simulates a human Unity edit without
  launching Unity, and only the public drift/proposal capabilities may interpret it;
* stage 14 deletes the generated projection output (`ManagedScenes`/`ManagedPrefabs`) between two host processes.

The client never launches the Unity Editor itself. Every Unity process is launched by the product's fixed launcher
behind a public composed capability, and the transcript/ledger audit in `h1-gate-verify.py` proves it.

Usage:
  h1-gate-scenario.py bootstrap --transport reference --workspace PATH --evidence DIR [launcher options]
  h1-gate-scenario.py scenario  --transport reference|mcp --workspace PATH --evidence DIR [launcher options]
  h1-gate-scenario.py compare   --reference DIR --mcp DIR --output FILE
"""

import argparse
import base64
import copy
import hashlib
import json
import os
import re
import shutil
import subprocess
import sys
import time
from pathlib import Path

SCENE = "arkus.h1-05.scene.potes"
BINDING_SCHEMA = "arkus.unity-binding@1"
CONVENTION = "unity-local-left-handed-y-up-z-forward"
SHARED_MATERIAL = "quaternius.medieval.material.wall-plaster-window-wide-flat-mi-plaster"
HUMANOID = "quaternius.ual1.prefab.ual1"
IDLE = "quaternius.ual1.animation-clip.armature-idle-loop"
WALK = "quaternius.ual1.animation-clip.armature-walk-loop"
SIT = "quaternius.ual1.animation-clip.armature-sitting-idle-loop"
MISSING_SOURCE = "quaternius.medieval.asset.h1-gate-missing-crate"
MISSING_CLIP = "quaternius.ual1.animation-clip.h1-gate-missing-clip"
# A materialize request over an invalid canonical binding is refused before any Editor launch. The accepted H1-09
# transport contract reports that refusal as `unity.lifecycle.corrupt-result` (encoder rejection). The actionable
# catalogue diagnostic comes from the public preflight `unity.projection.plan`.
MATERIALIZE_REJECTION = ["unity.lifecycle.corrupt-result", "projection.source-missing", "projection.reference-missing"]
EDIT_NODE = "river.edge"
EDIT_DELTA_MM = 375
GENERATED_OUTPUT = (
    "Unity/ArkusUnity/Assets/Arkus/H1/ManagedScenes",
    "Unity/ArkusUnity/Assets/Arkus/H1/ManagedScenes.meta",
    "Unity/ArkusUnity/Assets/Arkus/H1/ManagedPrefabs",
    "Unity/ArkusUnity/Assets/Arkus/H1/ManagedPrefabs.meta",
)
OPERATIONS = "Unity/ArkusUnity/Library/Arkus/H1Lifecycle/operations"
CHECKPOINT_ROOT = "Unity/ArkusUnity/ProjectSettings/Arkus/H1Checkpoint"

# Gate-owned bounded non-keeper conformance assembly (H1_UNITY_PARITY_GATE.md "Representative slice"). It uses only
# the H1-11 representative selected-item manifest. Fields: object, container, type, source kind, source logical ID,
# position mm, yaw milli-degrees, scale ppm, renderer material, animator clip, canonical link (relation, target).
NODES = [
    ("wedge.tip", None, "h1gate.root", "prefab", "quaternius.medieval.prefab.corner-exterior-wood", (0, 0, 0), 0, None, None, None, None),
    ("facade.front", "wedge.tip", "h1gate.module", "prefab", "quaternius.medieval.prefab.wall-plaster-window-wide-flat", (1000, 0, 0), 0, (10000, 10000, 10000), None, None, None),
    ("window.front", "wedge.tip", "h1gate.module", "prefab", "quaternius.medieval.prefab.window-wide-flat1", (1000, 0, 0), 0, None, None, None, None),
    ("shutters.front", "window.front", "h1gate.module", "prefab", "quaternius.medieval.prefab.windowshutters-wide-flat-open", (0, 0, 0), 0, None, None, None, None),
    ("vine.front", "wedge.tip", "h1gate.prop", "prefab", "quaternius.medieval.prefab.prop-vine1", (1600, 3000, 150), 0, None, None, None, None),
    ("wall.side", "wedge.tip", "h1gate.module", "prefab", "quaternius.medieval.prefab.wall-plaster-straight", (0, 0, -1000), 90000, None, None, None, None),
    ("wall.door", "wedge.tip", "h1gate.module", "prefab", "quaternius.medieval.prefab.wall-plaster-door-flat", (3000, 0, 0), 0, None, None, None, None),
    ("door.main", "wall.door", "h1gate.module", "prefab", "quaternius.medieval.prefab.door-1-flat", (535, 0, 0), 0, None, None, None, None),
    ("roof.house", "wedge.tip", "h1gate.module", "prefab", "quaternius.medieval.prefab.roof-roundtiles-4x4", (2000, 3120, -1500), 0, None, None, None, None),
    ("chimney.house", "roof.house", "h1gate.prop", "prefab", "quaternius.medieval.prefab.prop-chimney", (1200, 1500, 0), 0, None, None, None, None),
    ("market.cart", "wedge.tip", "h1gate.prop", "prefab", "quaternius.medieval.prefab.prop-wagon", (6500, 0, 2500), 30000, None, None, None, None),
    ("market.crate", "wedge.tip", "h1gate.prop", "asset", "quaternius.medieval.asset.prop-crate", (4200, 0, 800), 0, None, SHARED_MATERIAL, None, None),
    ("civilian.sit", "market.crate", "h1gate.humanoid", "prefab", HUMANOID, (0, 1060, 0), 180000, None, None, SIT, None),
    ("civilian.idle", "wedge.tip", "h1gate.humanoid", "prefab", HUMANOID, (1500, 0, 1500), 0, None, None, IDLE, ("faces", "door.main")),
    ("civilian.walk", "wedge.tip", "h1gate.humanoid", "prefab", HUMANOID, (3000, 0, 2500), 270000, None, None, WALK, ("walks-toward", "market.cart")),
    ("river.edge", None, "h1gate.marker", "asset", "quaternius.medieval.asset.prop-woodenfence-single", (9000, 0, 5000), 0, (1250000, 1000000, 1000000), SHARED_MATERIAL, None, None),
]

REQUIRED_CAPABILITIES = [
    "system.describe@1.0",
    "authoring.change.plan@1.0", "authoring.change.dry-run@1.0", "authoring.change.apply@1.0", "authoring.change.validate@1.0",
    "authoring.snapshot.export@1.0", "authoring.snapshot.import@1.0", "authoring.journal.read@1.0", "authoring.diff.compare@1.0",
    "world.summary@1.0", "world.validation.current@1.0",
    "unity.binding.compile@1.0", "unity.binding.decode@1.0", "unity.binding.inspect@1.0",
    "unity.host.project-profile.inspect@1.0", "unity.lifecycle.operation-status@1.0",
    "unity.host.catalogue.query@1.0", "unity.host.catalogue.get@1.0", "unity.host.catalogue.resolve@1.0",
    "unity.projection.plan@1.0", "unity.host.projection.materialize@1.0", "unity.host.projection.observe@1.0",
    "unity.host.projection.drift@1.0", "unity.host.projection.import-proposal@1.0", "unity.host.projection.rematerialize@1.0",
    "unity.host.checkpoint.capture@1.0", "unity.host.checkpoint.current@1.0", "unity.host.checkpoint.restore@1.0",
    "unity.host.projection.clean-rebuild@1.0",
]


class GateError(RuntimeError):
    pass


def require(condition, message):
    if not condition:
        raise GateError(message)


def canonical_json(value):
    return json.dumps(value, sort_keys=True, separators=(",", ":"), ensure_ascii=False)


def sha(value):
    return hashlib.sha256(value.encode("utf-8") if isinstance(value, str) else value).hexdigest()


class Transcript:
    """Append-only record of every public frame. The verifier recomputes stage facts from it."""

    def __init__(self, path):
        self.path = Path(path)
        self.path.parent.mkdir(parents=True, exist_ok=True)
        self.sequence = 0
        if self.path.exists():
            for line in self.path.read_text(encoding="utf-8").splitlines():
                if line.strip():
                    self.sequence = max(self.sequence, json.loads(line)["seq"])

    def write(self, record):
        self.sequence += 1
        record = dict(record, seq=self.sequence)
        with self.path.open("a", encoding="utf-8") as handle:
            handle.write(canonical_json(record) + "\n")
        return record


class PublicHost:
    """One fixed public host process (`--h1-unity`). The transport is the only difference between reference and MCP."""

    def __init__(self, transport, session, launcher, workspace, transcript):
        self.transport = transport
        self.session = session
        self.transcript = transcript
        self.workspace = Path(workspace)
        self.sequence = 0
        self.tools = {}
        self.discovered = []
        self.stage = "S0"
        self.container = None
        command = launcher.command(transport, session, self.workspace)
        self.container = launcher.container_name(transport, session)
        self.started = time.time()
        self.process = subprocess.Popen(
            command, cwd=str(self.workspace), stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
            text=True, bufsize=1)
        public_command = [part for index, part in enumerate(command)
                          if part != "--volume" and (index == 0 or command[index - 1] != "--volume")]
        self.transcript.write({"event": "host-start", "session": session, "transport": transport, "command": public_command})
        if transport == "mcp":
            initialized = self._rpc("initialize", {
                "protocolVersion": "2025-11-25", "capabilities": {},
                "clientInfo": {"name": "arkus.h1-gate.reference-scenario", "version": "1.0.0"}})
            require(initialized.get("result", {}).get("protocolVersion") == "2025-11-25", "MCP initialize version mismatch")
            self._notify("notifications/initialized", {})

    # ----- wire ---------------------------------------------------------------------------------------------------
    def _readline(self, what):
        line = self.process.stdout.readline()
        if not line:
            raise GateError(f"{self.transport} host ended before {what}: {self.process.stderr.read()[-2000:]}")
        try:
            return json.loads(line)
        except json.JSONDecodeError as exc:
            raise GateError(f"{self.transport} stdout contamination: {line[:200]!r}") from exc

    def _rpc(self, method, params):
        self.sequence += 1
        wanted = self.sequence
        self.process.stdin.write(canonical_json({"jsonrpc": "2.0", "id": wanted, "method": method, "params": params}) + "\n")
        self.process.stdin.flush()
        while True:
            value = self._readline(method)
            if value.get("id") == wanted:
                return value

    def _notify(self, method, params):
        self.process.stdin.write(canonical_json({"jsonrpc": "2.0", "method": method, "params": params}) + "\n")
        self.process.stdin.flush()

    def discover(self, stage):
        """Stage 4: the complete composed capability/schema surface, discovered through this transport only."""
        self.stage = stage
        started = time.time()
        if self.transport == "reference":
            outcome = self._reference("system.describe", {})
            require(outcome.get("status") == "success", "system.describe failed")
            capabilities = outcome["result"]["capabilities"]
            keys = sorted(f'{item["name"]}@{item["version"]}' for item in capabilities)
            schemas = {f'{item["name"]}@{item["version"]}': sha(canonical_json({"request": item.get("requestSchema"), "success": item.get("successSchema")}))
                       for item in capabilities}
            raw = outcome
        else:
            response = self._rpc("tools/list", {})
            tools = response.get("result", {}).get("tools", [])
            self.tools = {}
            schemas = {}
            for tool in tools:
                key = tool.get("_meta", {}).get("dev.arkus/canonicalKey")
                require(key and tool.get("name") and key not in self.tools, "MCP canonical tool inventory is malformed")
                self.tools[key] = tool["name"]
                definition = tool.get("_meta", {}).get("dev.arkus/canonicalDefinition", {})
                schemas[key] = sha(canonical_json({"request": definition.get("requestSchema"), "success": definition.get("successSchema")}))
            keys = sorted(self.tools)
            raw = {"toolCount": len(tools), "keys": keys}
        self.discovered = keys
        self.transcript.write({"event": "discovery", "session": self.session, "transport": self.transport, "stage": stage,
                               "keys": keys, "schemaDigests": schemas, "elapsedMs": int((time.time() - started) * 1000),
                               "rawDigest": sha(canonical_json(raw))})
        missing = [key for key in REQUIRED_CAPABILITIES if key not in keys]
        require(not missing, f"composed discovery omitted required H1 capabilities: {missing}")
        return keys

    def _reference(self, capability, arguments, version=(1, 0)):
        self.sequence += 1
        frame = {"protocol": "arkus.reference.jsonl@1", "request": {
            "projectionVersion": "arkus.neutral-projection@1", "requestId": f"h1-gate.{self.session}.{self.sequence}",
            "capability": capability, "acceptedVersions": {"major": version[0], "minimumMinor": version[1], "maximumMinor": version[1]},
            "arguments": arguments}}
        self.process.stdin.write(canonical_json(frame) + "\n")
        self.process.stdin.flush()
        return self._readline(capability)["response"]

    def call(self, capability, arguments=None, stage=None, label=None):
        arguments = {} if arguments is None else arguments
        stage = stage or self.stage
        started = time.time()
        if self.transport == "reference":
            outcome = self._reference(capability, arguments)
        else:
            key = capability + "@1.0"
            require(key in self.tools, f"MCP discovery omitted {key}")
            response = self._rpc("tools/call", {"name": self.tools[key], "arguments": arguments})
            if "error" in response:
                outcome = {"status": "transport-error", "error": response["error"]}
            else:
                result = response.get("result", {})
                require("structuredContent" in result, f"MCP result for {key} omitted structuredContent")
                outcome = result["structuredContent"]
        self.transcript.write({"event": "call", "session": self.session, "transport": self.transport, "stage": stage,
                               "label": label or capability, "capability": capability + "@1.0", "arguments": arguments,
                               "outcome": outcome, "elapsedMs": int((time.time() - started) * 1000)})
        return outcome

    def ok(self, capability, arguments=None, stage=None, label=None):
        outcome = self.call(capability, arguments, stage, label)
        require(outcome.get("status") == "success", f"{label or capability} failed: {canonical_json(outcome)[:1500]}")
        return outcome["result"]

    def fail(self, capability, arguments, codes, stage=None, label=None):
        outcome = self.call(capability, arguments, stage, label)
        require(outcome.get("status") == "error", f"{label or capability} unexpectedly succeeded")
        code = outcome.get("error", {}).get("machineCode")
        require(code in codes, f"{label or capability} returned {code}, expected one of {codes}")
        return outcome["error"]

    def close(self):
        exit_code = None
        stderr = ""
        if self.process.poll() is None:
            self.process.stdin.close()
            try:
                exit_code = self.process.wait(timeout=60)
            except subprocess.TimeoutExpired:
                if self.container:
                    subprocess.run(["docker", "rm", "-f", self.container], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
                self.process.kill()
                exit_code = -9
        else:
            exit_code = self.process.returncode
        tail = self.process.stdout.read()
        stderr = self.process.stderr.read()
        self.transcript.write({"event": "host-exit", "session": self.session, "transport": self.transport, "exitCode": exit_code,
                               "extraStdout": bool(tail.strip()), "stderrDigest": sha(stderr),
                               "rawUnityOnHostStderr": "Unity Editor version:" in stderr or "ARKUS_H1_EDITOR_WORKER_FAILURE" in stderr,
                               "elapsedMs": int((time.time() - self.started) * 1000)})
        require(not tail.strip(), f"{self.transport} emitted extra protocol stdout")
        require(exit_code == 0, f"{self.transport} host exit={exit_code}: {stderr[-2000:]}")


class Launcher:
    """How the fixed public host process is started. `docker` runs it inside the pinned Unity image (H1-03A precedent)."""

    def __init__(self, args):
        self.mode = args.launcher
        self.image = args.unity_image
        self.license_home = args.license_home
        self.dotnet = args.dotnet
        self.run_id = os.environ.get("GITHUB_RUN_ID", "local")

    def container_name(self, transport, session):
        return None if self.mode == "local" else f"h1-gate-{transport}-{session}-{self.run_id}".lower()

    def command(self, transport, session, workspace):
        project = "Arkus.Harness.Cli" if transport == "reference" else "Arkus.Harness.Mcp"
        dll = f"src/{project}/bin/Release/net8.0/{project}.dll"
        if self.mode == "local":
            return [self.dotnet, str(Path(workspace) / dll), "--h1-unity"]
        require(self.image and self.license_home, "docker launcher needs --unity-image and --license-home")
        return ["docker", "run", "--rm", "--name", self.container_name(transport, session), "-i",
                "--shm-size=1025m", "--workdir", "/github/workspace",
                "--volume", f"{self.license_home}:/root",
                "--volume", f"{Path(workspace).resolve()}:/github/workspace",
                "--volume", "/usr/share/dotnet:/usr/share/dotnet:ro",
                "--env", "DOTNET_ROOT=/usr/share/dotnet",
                self.image, "/usr/share/dotnet/dotnet", f"/github/workspace/{dll}", "--h1-unity"]


# ----- scenario helpers -------------------------------------------------------------------------------------------

def vector(values):
    return {"x": values[0], "y": values[1], "z": values[2]}


def binding(node, source_kind=None, source_id=None, clip=None, position=None):
    _, _, _, kind, source, pos, yaw, scale, material, node_clip, link = node
    components = []
    if link:
        components.append({"kind": "canonical-link", "relation": link[0], "targetObjectId": link[1]})
    if material:
        components.append({"kind": "renderer", "materialId": material})
    if clip or node_clip:
        components.append({"kind": "animator", "clipId": clip or node_clip})
    return {
        "schemaId": BINDING_SCHEMA, "targetSceneId": SCENE,
        "source": {"kind": source_kind or kind, "logicalId": source_id or source},
        "transform": {"coordinateConvention": CONVENTION, "positionMm": vector(position or pos),
                      "rotationMilliDegrees": vector((0, yaw, 0)), "scalePpm": vector(scale or (1000000, 1000000, 1000000))},
        "components": components,
    }


def node_by_id(object_id):
    return next(node for node in NODES if node[0] == object_id)


def compile_binding(host, node, stage, **overrides):
    compiled = host.ok("unity.binding.compile", {"subjectId": node[0], "binding": binding(node, **overrides)}, stage,
                       f"compile:{node[0]}")
    mutation = dict(compiled["extensionMutation"])
    require(mutation.get("kind") == "put-extension" and mutation.get("payloadBase64"), "compile did not return an extension mutation")
    return compiled, mutation


def anchor(host, stage):
    world = host.ok("world.summary", {}, stage, "world.summary")["world"]
    return world["revision"], world["hash"]


def author(host, stage, key, operations):
    """Ordinary H0 plan -> dry-run -> apply against the live anchor. Returns the applied result."""
    revision, digest = anchor(host, stage)
    request = {"idempotencyKey": key, "expectedRevision": revision, "expectedHash": digest, "operations": operations}
    host.ok("authoring.change.plan", request, stage, f"plan:{key}")
    host.ok("authoring.change.dry-run", request, stage, f"dry-run:{key}")
    return host.ok("authoring.change.apply", request, stage, f"apply:{key}")


def published_scene(workspace, generation):
    path = Path(workspace) / "Unity/ArkusUnity/Assets/Arkus/H1/ManagedScenes/Generations" / f"{generation}.unity"
    require(path.is_file(), f"published generation scene is absent: {path}")
    return path


def edit_transform_x(scene_path, object_id, delta_mm):
    """Stage 13 external-actor stimulus: move one plain managed GameObject along X in the serialized scene."""
    text = scene_path.read_text(encoding="utf-8")
    documents = re.split(r"(?m)^(?=--- !u!)", text)
    game_object = None
    for document in documents:
        header = re.match(r"--- !u!1 &(-?\d+)", document)
        if header and re.search(rf"(?m)^  m_Name: {re.escape(object_id)}$", document):
            require(game_object is None, f"object name {object_id} is not unique in the published scene")
            game_object = header.group(1)
    require(game_object is not None, f"managed GameObject {object_id} is absent from the published scene")
    edited = 0
    for index, document in enumerate(documents):
        if re.match(r"--- !u!4 &", document) and re.search(rf"(?m)^  m_GameObject: {{fileID: {game_object}}}$", document):
            match = re.search(r"(?m)^(  m_LocalPosition: \{x: )([-0-9.eE+]+)(, y: [-0-9.eE+]+, z: [-0-9.eE+]+\})$", document)
            require(match is not None, f"managed Transform of {object_id} has no serialized local position")
            before = float(match.group(2))
            after = before + delta_mm / 1000.0
            documents[index] = document[:match.start()] + match.group(1) + repr(round(after, 6)) + match.group(3) + document[match.end():]
            edited += 1
    require(edited == 1, f"expected exactly one Transform for {object_id}, edited {edited}")
    scene_path.write_text("".join(documents), encoding="utf-8")
    return {"object": object_id, "deltaMm": delta_mm, "sceneSha256Before": sha(text), "sceneSha256After": sha("".join(documents))}


def remove_generated_output(workspace):
    removed = []
    for relative in GENERATED_OUTPUT:
        path = Path(workspace) / relative
        if path.is_dir():
            shutil.rmtree(path)
            removed.append(relative)
        elif path.exists():
            path.unlink()
            removed.append(relative)
    for relative in GENERATED_OUTPUT:
        require(not (Path(workspace) / relative).exists(), f"generated output still present: {relative}")
    return removed


def decode_env(path):
    fields = {}
    for line in Path(path).read_text(encoding="utf-8").splitlines():
        if "=" in line:
            key, value = line.split("=", 1)
            fields[key] = value
    decoded = {}
    for key, value in fields.items():
        try:
            decoded[key] = base64.b64decode(value, validate=True).decode("utf-8") if key not in ("schema", "mainThread") else value
        except Exception:
            decoded[key] = value
    return decoded


OWNED_ERROR = re.compile(
    r"ARKUS_H1_EDITOR_WORKER_FAILURE|error CS\d+|NullReferenceException|MissingReferenceException|Missing Prefab|"
    r"missing script|The referenced script .* is missing|Assets/Arkus/H1/.*(error|Error|failed)|"
    r"(Exception|Error).*Arkus\.H1")


def ledger_audit(workspace, since):
    """Every Unity process launched during the run, read from the product launcher's own operation directories."""
    root = Path(workspace) / OPERATIONS
    rows = []
    if not root.is_dir():
        return rows
    for directory in sorted(root.iterdir()):
        invocation = directory / "invocation.env"
        if not invocation.is_file() or invocation.stat().st_mtime < since:
            continue
        fields = decode_env(invocation)
        result = directory / "result.env"
        result_fields = decode_env(result) if result.is_file() else {}
        log = directory / "unity.log"
        owned = []
        loaded = False
        if log.is_file():
            content = log.read_text(encoding="utf-8", errors="replace")
            loaded = True
            owned = sorted({line.strip()[:240] for line in content.splitlines() if OWNED_ERROR.search(line)})
        rows.append({
            "invocationId": fields.get("invocation", directory.name),
            "capability": fields.get("capability", ""),
            "executor": fields.get("executor", ""),
            "entryPoint": fields.get("entryPoint", ""),
            "resultPresent": result.is_file(),
            "resultExecutor": result_fields.get("executor", ""),
            "mainThread": result_fields.get("mainThread", ""),
            "editorLogCaptured": loaded,
            "ownedErrorLines": owned,
        })
    return rows


# ----- stages ---------------------------------------------------------------------------------------------------------

def stage3_inspect(host):
    host.stage = "S03"
    last = None
    for attempt in range(1, 5):
        outcome = host.call("unity.host.project-profile.inspect", {}, "S03", f"project-profile.inspect#{attempt}")
        if outcome.get("status") == "success":
            result = outcome["result"]
            status = host.ok("unity.lifecycle.operation-status", {"invocationId": result["invocationId"]}, "S03", "operation-status")
            require(status.get("status") == "completed" and status.get("outcomeCode") == "success",
                    f"lifecycle recovery disagrees: {canonical_json(status)}")
            require(result.get("editorVersion") == "6000.3.24f1" and result.get("editorRevision") == "4e7b9b5b6244" and
                    result.get("mainThread") is True, "effective Unity editor identity or main-thread proof is wrong")
            return result
        last = outcome
        error = outcome.get("error", {})
        invocation = error.get("context", {}).get("invocationId")
        if invocation:
            host.call("unity.lifecycle.operation-status", {"invocationId": invocation}, "S03", "operation-status:retry-reconcile")
        # Only the read-only first inspection may be retried: it performs the project's first import and the
        # accepted lifecycle contract makes an interrupted inspection restartable under the same public semantics.
        require(error.get("retryable") is True and error.get("machineCode", "").startswith("unity."),
                f"project inspection failed non-retryably: {canonical_json(outcome)[:1500]}")
    raise GateError(f"project inspection never completed: {canonical_json(last)[:1500]}")


def selected_ids():
    ids = {("scene", SCENE)}
    for node in NODES:
        ids.add((node[3], node[4]))
        if node[8]:
            ids.add(("material", node[8]))
        if node[9]:
            ids.add(("animation-clip", node[9]))
    return sorted(ids)


def stage5_catalogue(host):
    host.stage = "S05"
    offset, token, entries, total = 0, None, [], None
    fingerprint = None
    while True:
        arguments = {"pageSize": 64, "offset": offset}
        if token is not None:
            arguments["expectedSnapshotToken"] = token
        page = host.ok("unity.host.catalogue.query", arguments, "S05", f"catalogue.query@{offset}")
        token = page["snapshotToken"] if token is None else token
        require(page["snapshotToken"] == token and (fingerprint is None or page["fingerprint"] == fingerprint), "catalogue snapshot moved during paging")
        fingerprint = page["fingerprint"]
        total = page["total"]
        entries.extend(page["entries"])
        if page["nextOffset"] in (None, -1, 0) or page["nextOffset"] >= total or not page["entries"]:
            break
        offset = page["nextOffset"]
    require(len(entries) == total and len({(e["kind"], e["logicalId"]) for e in entries}) == total, "catalogue pages are incomplete or duplicated")
    resolved = {}
    for kind, logical in selected_ids():
        result = host.ok("unity.host.catalogue.resolve", {"logicalId": logical, "kind": kind}, "S05", f"catalogue.resolve:{logical}")
        entry = result.get("entry", result)
        require(entry.get("logicalId") == logical and entry.get("kind") == kind and entry.get("compatible", True) is not False,
                f"catalogue did not resolve {kind}:{logical}")
        resolved[logical] = {"kind": kind, "path": entry.get("path"), "nativeGuid": entry.get("nativeGuid"), "contentSha256": entry.get("contentSha256")}
    return {"fingerprint": fingerprint, "snapshotToken": token, "total": total, "resolved": resolved}


def stage6_7_author(host):
    host.stage = "S06"
    objects, extensions, dependencies = [], [], {}
    for node in NODES:
        compiled, mutation = compile_binding(host, node, "S06")
        dependencies[node[0]] = {"canonical": compiled.get("canonicalDependencies", []), "catalogue": compiled.get("catalogueDependencies", [])}
        extensions.append(mutation)
        operation = {"kind": "put-object", "id": node[0], "typeId": node[2]}
        if node[1]:
            operation["containerId"] = node[1]
        objects.append(operation)
    host.stage = "S07"
    applied = author(host, "S07", "h1-gate.slice.create", objects + extensions)
    journal = host.ok("authoring.journal.read", {}, "S07", "journal.read")
    snapshot = host.ok("authoring.snapshot.export", {}, "S07", "snapshot.export:authored")
    revision, digest = anchor(host, "S07")
    return {"dependencies": dependencies, "revision": revision, "canonicalHash": digest,
            "journalEntries": journal.get("entryCount"), "appliedDigest": sha(canonical_json(applied)), "snapshot": snapshot}


def stage8_plan(host):
    host.stage = "S08"
    validation = host.ok("world.validation.current", {}, "S08", "world.validation.current")
    plan = host.ok("unity.projection.plan", {"sceneLogicalId": SCENE}, "S08", "projection.plan")
    require(sorted(plan["objectIds"]) == sorted(node[0] for node in NODES), "projection plan does not cover exactly the authored slice")
    return {"validationDigest": sha(canonical_json(validation)), "plan": plan}


def materialize(host, stage, label, plan):
    result = host.ok("unity.host.projection.materialize", {"sceneLogicalId": SCENE}, stage, label)
    require(result["active"] is True and result["current"] is True, f"{label}: publication is not the active current generation")
    require(result["inputDigest"] == plan["inputDigest"] and result["canonicalHash"] == plan["canonicalHash"] and
            result["catalogueFingerprint"] == plan["catalogueFingerprint"], f"{label}: observation is not bound to the plan")
    require(sorted(n["objectId"] for n in result["nodes"]) == sorted(plan["objectIds"]), f"{label}: observed node set differs from plan")
    return result


def summary_of(observation):
    return {key: observation.get(key) for key in ("active", "current", "generationId", "inputDigest", "canonicalHash",
                                                  "catalogueFingerprint", "graphDigest", "realizationDigest")}


def stage10_inspect(observation, resolved):
    """Stage 10: public inspection of hierarchy, prefab/source relationships, assets and allowlisted components."""
    nodes = {node["objectId"]: node for node in observation["nodes"]}
    facts = {"prefabRelationships": 0, "animatorClips": {}, "renderMaterials": {}, "canonicalLinks": {}, "maxDepth": 0}

    def identity(logical_id):
        entry = resolved[logical_id]
        return f'{entry["path"]}|{entry["nativeGuid"]}|'

    for node in NODES:
        observed = nodes[node[0]]
        require(observed["parentObjectId"] == (node[1] or ""), f"hierarchy lost for {node[0]}")
        require(observed["sourceLogicalId"] == node[4] and observed["sourceGuid"] == resolved[node[4]]["nativeGuid"],
                f"source identity lost for {node[0]}")
        rows = observed["componentRows"]
        if node[3] == "prefab":
            require(observed["relationships"], f"prefab/source relationships absent for {node[0]}")
            facts["prefabRelationships"] += len(observed["relationships"])
        if node[9]:
            require(any("animator" in row and "|clip=" + identity(node[9]) in row for row in rows), f"animator clip row absent for {node[0]}")
            facts["animatorClips"][node[0]] = node[9]
        if node[8]:
            require(any("|material=" + identity(node[8]) in row for row in rows), f"renderer material override absent for {node[0]}")
            facts["renderMaterials"][node[0]] = node[8]
        if node[10]:
            require(any(f"|relation={node[10][0]}|target={node[10][1]}" in row for row in rows), f"canonical link row absent for {node[0]}")
            facts["canonicalLinks"][node[0]] = list(node[10])
    parents = {node["objectId"]: node["parentObjectId"] for node in observation["nodes"]}
    for object_id in parents:
        depth, cursor = 1, parents[object_id]
        while cursor:
            depth, cursor = depth + 1, parents[cursor]
        facts["maxDepth"] = max(facts["maxDepth"], depth)
    require(facts["maxDepth"] >= 3, "representative hierarchy is shallower than three levels")
    require(len(facts["animatorClips"]) == 3 and set(facts["animatorClips"].values()) == {IDLE, WALK, SIT}, "humanoid idle/walk/sit clips not all realized")
    require(len(facts["renderMaterials"]) >= 2 and len(set(facts["renderMaterials"].values())) == 1, "shared material override not realized on two nodes")
    require(len(facts["canonicalLinks"]) >= 1, "no canonical-object reference realized")
    roots = sorted(object_id for object_id, parent in parents.items() if not parent)
    require(roots == ["river.edge", "wedge.tip"], f"wedge-tip root and separate river-edge marker not both realized as roots: {roots}")
    facts["roots"] = roots
    return facts


def drift(host, stage, label):
    return host.ok("unity.host.projection.drift", {"sceneLogicalId": SCENE}, stage, label)


def require_parity(report, label):
    require(report.get("parity") is True and not report.get("items") and not report.get("diagnostics"),
            f"{label}: effective Unity state is not at parity: {canonical_json(report)[:1500]}")


def run_scenario(args):
    workspace = Path(args.workspace).resolve()
    evidence = Path(args.evidence)
    evidence.mkdir(parents=True, exist_ok=True)
    transcript = Transcript(evidence / f"transcript-{args.transport}.jsonl")
    launcher = Launcher(args)
    started = time.time() - 5
    facts = {"schemaId": "arkus.h1-gate-scenario-facts@1", "transport": args.transport, "candidateSha": args.candidate_sha,
             "stages": {}}

    def stage(identifier, value):
        facts["stages"][identifier] = value
        (evidence / f"facts-{args.transport}.json").write_text(json.dumps(facts, indent=2, sort_keys=True) + "\n", encoding="utf-8")

    host = PublicHost(args.transport, f"{args.transport}-a", launcher, workspace, transcript)
    try:
        stage("S03", {"profile": stage3_inspect(host)})
        keys = host.discover("S04")
        stage("S04", {"capabilityCount": len(keys), "required": REQUIRED_CAPABILITIES})
        stage("S05", stage5_catalogue(host))
        authored = stage6_7_author(host)
        stage("S06", {"dependencies": authored["dependencies"]})
        stage("S07", {k: authored[k] for k in ("revision", "canonicalHash", "journalEntries", "appliedDigest")})
        authored_snapshot = authored["snapshot"]
        planned = stage8_plan(host)
        plan = planned["plan"]
        stage("S08", planned)

        host.stage = "S09"
        first = materialize(host, "S09", "materialize#1", plan)
        observed = host.ok("unity.host.projection.observe", {"sceneLogicalId": SCENE}, "S09", "observe#1")
        require(summary_of(observed) == summary_of(first), "public observe disagrees with the materialize observation")
        stage("S09", {"planInputDigest": plan["inputDigest"], "materialized": summary_of(first), "observed": summary_of(observed)})

        host.stage = "S10"
        inspected = stage10_inspect(observed, facts["stages"]["S05"]["resolved"])
        report = drift(host, "S10", "drift:after-first-publication")
        require_parity(report, "first publication")
        stage("S10", {"inspection": inspected, "driftState": report.get("state")})

        host.stage = "S11"
        second = materialize(host, "S11", "materialize#2", plan)
        for key in ("inputDigest", "canonicalHash", "catalogueFingerprint", "graphDigest", "realizationDigest"):
            require(second[key] == first[key], f"same-input materialization changed {key}")
        report = drift(host, "S11", "drift:after-second-materialization")
        require_parity(report, "second materialization")
        stage("S11", {"first": summary_of(first), "second": summary_of(second), "generationReused": second["generationId"] == first["generationId"]})

        host.stage = "S12"
        crate = node_by_id("market.crate")
        walker = node_by_id("civilian.walk")
        _, bad_source = compile_binding(host, crate, "S12", source_id=MISSING_SOURCE)
        author(host, "S12", "h1-gate.invalid.missing-source", [bad_source])
        source_plan = host.fail("unity.projection.plan", {"sceneLogicalId": SCENE}, ["projection.source-missing"], "S12", "plan:missing-source")
        source_materialize = host.fail("unity.host.projection.materialize", {"sceneLogicalId": SCENE}, MATERIALIZE_REJECTION, "S12", "materialize:missing-source")
        _, repaired_source = compile_binding(host, crate, "S12")
        _, bad_clip = compile_binding(host, walker, "S12", clip=MISSING_CLIP)
        author(host, "S12", "h1-gate.invalid.missing-component-reference", [repaired_source, bad_clip])
        clip_plan = host.fail("unity.projection.plan", {"sceneLogicalId": SCENE}, ["projection.reference-missing"], "S12", "plan:missing-clip")
        clip_materialize = host.fail("unity.host.projection.materialize", {"sceneLogicalId": SCENE}, MATERIALIZE_REJECTION, "S12", "materialize:missing-clip")
        stale = host.ok("unity.host.projection.observe", {"sceneLogicalId": SCENE}, "S12", "observe:after-invalid")
        require(stale["active"] is True and stale["generationId"] == second["generationId"] and stale["current"] is False and
                stale["graphDigest"] == second["graphDigest"] and stale["realizationDigest"] == second["realizationDigest"],
                "a rejected invalid reference changed or falsely re-labelled the active generation")
        _, repaired_clip = compile_binding(host, walker, "S12")
        author(host, "S12", "h1-gate.repair", [repaired_clip])
        repaired_plan = host.ok("unity.projection.plan", {"sceneLogicalId": SCENE}, "S12", "plan:repaired")
        repaired = materialize(host, "S12", "materialize:repaired", repaired_plan)
        repaired_snapshot = host.ok("authoring.snapshot.export", {}, "S12", "snapshot.export:repaired")
        same = host.ok("authoring.diff.compare", {"base": authored_snapshot, "target": repaired_snapshot}, "S12", "diff.compare:repaired-vs-authored")
        require(same.get("sameAuthorableState") is True, "canonical repair did not return to the authored content")
        stage("S12", {"diagnostics": [source_plan, source_materialize, clip_plan, clip_materialize],
                      "retainedGeneration": summary_of(stale), "repaired": summary_of(repaired)})

        host.stage = "S13"
        scene = published_scene(workspace, repaired["generationId"])
        stimulus = edit_transform_x(scene, EDIT_NODE, EDIT_DELTA_MM)
        transcript.write({"event": "operator-action", "session": host.session, "stage": "S13", "action": "unity-managed-edit", **stimulus})
        report = drift(host, "S13", "drift:after-unity-edit")
        changed = [item for item in report.get("items", []) if item.get("objectId") == EDIT_NODE]
        require(len(changed) == 1 and "transform" in changed[0].get("fields", []), f"supported Unity edit was not observed as drift: {canonical_json(report)[:1500]}")
        proposal = host.ok("unity.host.projection.import-proposal", {"sceneLogicalId": SCENE}, "S13", "import-proposal")
        require(proposal.get("available") is True, f"supported edit produced no proposal: {canonical_json(proposal)[:1500]}")
        request = proposal["mutationRequest"]
        before_revision, before_hash = anchor(host, "S13")
        require(request["expectedRevision"] == before_revision and request["expectedHash"] == before_hash, "proposal is not anchored to canonical truth")
        author(host, "S13", "h1-gate.concurrent-writer", [{"kind": "put-object", "id": "observer.gate", "typeId": "h1gate.observer"}])
        stale_error = host.fail("authoring.change.plan", request, ["world.change.stale_revision", "world.change.stale_hash"], "S13", "plan:stale-proposal")
        recovery = stale_error.get("context", {}).get("recovery", {})
        require(recovery.get("disposition") == "same-lineage-replan", f"stale proposal did not expose HK08B recovery: {canonical_json(stale_error)[:1500]}")
        revision, digest = anchor(host, "S13")
        recovered = dict(request, expectedRevision=revision, expectedHash=digest, idempotencyKey=request["idempotencyKey"] + ".recovered")
        host.ok("authoring.change.plan", recovered, "S13", "plan:recovered-proposal")
        host.ok("authoring.change.dry-run", recovered, "S13", "dry-run:recovered-proposal")
        host.ok("authoring.change.apply", recovered, "S13", "apply:recovered-proposal")
        revision, digest = anchor(host, "S13")
        objects = host.ok("world.object.get", {"revision": revision, "hash": digest, "id": "observer.gate"}, "S13", "object.get:concurrent-writer")
        rematerialized = host.ok("unity.host.projection.rematerialize", {"sceneLogicalId": SCENE}, "S13", "rematerialize")
        report = drift(host, "S13", "drift:after-rematerialize")
        require_parity(report, "rematerialized import")
        final = host.ok("unity.host.projection.observe", {"sceneLogicalId": SCENE}, "S13", "observe:after-import")
        moved = next(node for node in final["nodes"] if node["objectId"] == EDIT_NODE)
        require(moved["positionMm"]["x"] == node_by_id(EDIT_NODE)[5][0] + EDIT_DELTA_MM, "imported edit is not canonical truth after rematerialization")
        require(final["current"] is True, "rematerialized projection is not current")
        stage("S13", {"stimulus": stimulus, "staleCode": stale_error.get("machineCode"), "recovery": recovery.get("disposition"),
                      "concurrentWriterKept": bool(objects), "rematerializeDigest": sha(canonical_json(rematerialized)),
                      "final": summary_of(final)})

        host.stage = "S14"
        captured = host.ok("unity.host.checkpoint.capture", {"sceneLogicalId": SCENE}, "S14", "checkpoint.capture")
        require(captured["state"] == "ready", "checkpoint capture is not ready")
        current_before = host.ok("unity.host.checkpoint.current", {"sceneLogicalId": SCENE}, "S14", "checkpoint.current:before-close")
        require(current_before["checkpointId"] == captured["checkpointId"], "current checkpoint is not the captured one")
        pre_close_revision, pre_close_hash = anchor(host, "S14")
    finally:
        host.close()

    removed = remove_generated_output(workspace)
    transcript.write({"event": "operator-action", "session": f"{args.transport}-a", "stage": "S14", "action": "remove-generated-output", "paths": removed})

    fresh = PublicHost(args.transport, f"{args.transport}-b", launcher, workspace, transcript)
    try:
        fresh.discover("S14")
        empty = fresh.ok("world.summary", {}, "S14", "world.summary:fresh-process")
        require(empty["world"]["revision"] == 0 and empty["objectCount"] == 0, "fresh host did not start without canonical state")
        restored = fresh.ok("unity.host.checkpoint.restore", {"sceneLogicalId": SCENE}, "S14", "checkpoint.restore")
        revision, digest = anchor(fresh, "S14")
        require(digest == captured["canonicalHash"] == pre_close_hash, "restored canonical hash differs from the checkpoint")
        journal = fresh.ok("authoring.journal.read", {}, "S14", "journal.read:restored")
        rebuilt = fresh.ok("unity.host.projection.clean-rebuild", {"sceneLogicalId": SCENE}, "S14", "clean-rebuild")
        observed = fresh.ok("unity.host.projection.observe", {"sceneLogicalId": SCENE}, "S14", "observe:rebuilt")
        require(observed["current"] is True and observed["graphDigest"] == captured["observationGraphDigest"],
                "rebuilt observation graph digest differs from the checkpoint")
        report = drift(fresh, "S14", "drift:rebuilt")
        require_parity(report, "clean rebuild")
        recaptured = fresh.ok("unity.host.checkpoint.capture", {"sceneLogicalId": SCENE}, "S14", "checkpoint.capture:rebuilt")
        require(recaptured["observationReconstructionDigest"] == captured["observationReconstructionDigest"] and
                recaptured["observationGraphDigest"] == captured["observationGraphDigest"] and
                recaptured["canonicalHash"] == captured["canonicalHash"], "rebuilt normalized reconstruction digest differs")
        stage("S14", {"captured": captured, "restored": restored, "restoredAnchor": {"revision": revision, "hash": digest},
                      "restoredJournalEntries": journal.get("entryCount"), "rebuilt": summary_of(rebuilt) if "graphDigest" in rebuilt else rebuilt,
                      "observed": summary_of(observed), "recaptured": recaptured, "removedGeneratedOutput": removed,
                      "preCloseAnchor": {"revision": pre_close_revision, "hash": pre_close_hash}})

        fresh.stage = "S16"
        loaded = fresh.ok("unity.host.projection.observe", {"sceneLogicalId": SCENE}, "S16", "observe:load-capture")
        require(loaded["current"] is True, "real slice did not load as the current generation")
    finally:
        fresh.close()

    audit = ledger_audit(workspace, started)
    owned = [row for row in audit if row["ownedErrorLines"]]
    capture = [row for row in audit if row["capability"] == "unity.host.projection.observe@1.0" and row["editorLogCaptured"]]
    require(capture, "no Editor capture was recorded for the loaded slice")
    require(not owned, f"owned error diagnostics in Editor captures: {[row['ownedErrorLines'][:3] for row in owned]}")
    stage("S16", {"loaded": summary_of(loaded), "editorCaptures": len(capture), "ownedErrorLines": 0})
    (evidence / f"ledger-{args.transport}.json").write_text(json.dumps(audit, indent=2, sort_keys=True) + "\n", encoding="utf-8")

    parity = {
        "canonicalWorldHash": captured["canonicalHash"],
        "canonicalRevision": captured["revision"],
        "bindingSchema": BINDING_SCHEMA,
        "catalogueSnapshotFingerprint": facts["stages"]["S05"]["fingerprint"],
        "checkpointEnvironmentDigest": captured["environmentDigest"],
        "bridgeContract": "arkus.neutral-projection@1",
        "unityEditor": "6000.3.24f1 (4e7b9b5b6244)",
        "projectionPlanDigest": captured["planInputDigest"],
        "activeGenerationId": final["generationId"],
        "checkpointId": captured["checkpointId"],
        "normalizedObservationGraphDigest": captured["observationGraphDigest"],
        "normalizedObservationReconstructionDigest": captured["observationReconstructionDigest"],
    }
    facts["parityTuple"] = parity
    facts["h0sMeasurements"] = measurements(transcript.path, authored_snapshot, plan)
    facts["result"] = "GREEN"
    (evidence / f"facts-{args.transport}.json").write_text(json.dumps(facts, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(f"H1_GATE_SCENARIO_GREEN transport={args.transport} graph={parity['normalizedObservationGraphDigest']} "
          f"reconstruction={parity['normalizedObservationReconstructionDigest']} canonical={parity['canonicalWorldHash']}")


def measurements(path, snapshot, plan):
    """H0S relationship (H1_RISK_AND_RESIDUAL_PLAN.md): recorded, never optimized or turned into a Gate threshold."""
    records = [json.loads(line) for line in Path(path).read_text(encoding="utf-8").splitlines() if line.strip()]
    elapsed = {}
    for record in records:
        if record.get("event") == "call":
            elapsed.setdefault(record["capability"], []).append(record["elapsedMs"])
    stale = sum(1 for record in records if record.get("event") == "call" and
                record["outcome"].get("error", {}).get("machineCode", "").startswith("world.change.stale"))
    return {
        "canonicalObjectCount": max((r["outcome"]["result"]["objectCount"] for r in records if r.get("event") == "call" and
                                     r["capability"] == "world.summary@1.0" and r["outcome"].get("status") == "success"), default=None),
        "snapshotAuthoredStateBytes": len(base64.b64decode(snapshot["authoredStateBase64"])),
        "planNodeCount": len(plan["objectIds"]),
        "elapsedMsByCapability": {key: {"count": len(values), "total": sum(values), "max": max(values)} for key, values in sorted(elapsed.items())},
        "staleConflicts": stale,
    }


def run_bootstrap(args):
    """Stages 3-4 before the isolated MCP project copy is taken: the first public inspection imports the project."""
    workspace = Path(args.workspace).resolve()
    evidence = Path(args.evidence)
    evidence.mkdir(parents=True, exist_ok=True)
    transcript = Transcript(evidence / f"transcript-bootstrap.jsonl")
    host = PublicHost(args.transport, f"{args.transport}-bootstrap", Launcher(args), workspace, transcript)
    try:
        profile = stage3_inspect(host)
        host.discover("S04")
    finally:
        host.close()
    print(f"H1_GATE_BOOTSTRAP_GREEN editor={profile['editorVersion']} ({profile['editorRevision']})")


TRANSPORT_INVARIANT = [
    ("S05", ("fingerprint", "total")),
    ("S07", ("revision", "canonicalHash", "journalEntries")),
    ("S13", ("staleCode", "recovery", "concurrentWriterKept")),
]


def run_compare(args):
    """Stage 15: the MCP path reproduces the reference semantic delta from the same baseline."""
    reference = json.loads((Path(args.reference) / "facts-reference.json").read_text(encoding="utf-8"))
    mcp = json.loads((Path(args.mcp) / "facts-mcp.json").read_text(encoding="utf-8"))
    differences = []
    require(reference.get("result") == "GREEN" and mcp.get("result") == "GREEN", "both transports must complete the scenario")
    require(sorted(reference["stages"]) == sorted(mcp["stages"]), "transports executed different stage sets")
    for stage_id, keys in TRANSPORT_INVARIANT:
        for key in keys:
            if reference["stages"][stage_id].get(key) != mcp["stages"][stage_id].get(key):
                differences.append(f"{stage_id}.{key}")
    for key in ("canonicalWorldHash", "canonicalRevision", "bindingSchema", "catalogueSnapshotFingerprint", "bridgeContract",
                "unityEditor", "projectionPlanDigest", "normalizedObservationGraphDigest", "normalizedObservationReconstructionDigest"):
        if reference["parityTuple"][key] != mcp["parityTuple"][key]:
            differences.append("parity." + key)
    ref_codes = [d.get("machineCode") for d in reference["stages"]["S12"]["diagnostics"]]
    mcp_codes = [d.get("machineCode") for d in mcp["stages"]["S12"]["diagnostics"]]
    if ref_codes != mcp_codes:
        differences.append("S12.diagnostics")
    ref_s09 = reference["stages"]["S09"]["materialized"]
    mcp_s09 = mcp["stages"]["S09"]["materialized"]
    for key in ("inputDigest", "canonicalHash", "catalogueFingerprint", "graphDigest"):
        if ref_s09[key] != mcp_s09[key]:
            differences.append("S09." + key)
    result = {"schemaId": "arkus.h1-gate-transport-equivalence@1", "differences": differences,
              "reference": reference["parityTuple"], "mcp": mcp["parityTuple"],
              "diagnosticCodes": ref_codes, "result": "GREEN" if not differences else "RED"}
    Path(args.output).write_text(json.dumps(result, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    require(not differences, f"reference and MCP semantic deltas diverge: {differences}")
    print(f"H1_GATE_TRANSPORT_EQUIVALENCE_GREEN diagnostics={ref_codes}")


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    sub = parser.add_subparsers(dest="command", required=True)
    for name in ("bootstrap", "scenario"):
        command = sub.add_parser(name)
        command.add_argument("--transport", choices=("reference", "mcp"), required=True)
        command.add_argument("--workspace", required=True)
        command.add_argument("--evidence", required=True)
        command.add_argument("--candidate-sha", default=os.environ.get("TARGET_SHA", ""))
        command.add_argument("--launcher", choices=("docker", "local"), default="docker")
        command.add_argument("--unity-image", default=os.environ.get("PINNED_UNITY_IMAGE", ""))
        command.add_argument("--license-home", default=os.environ.get("UNITY_LICENSE_HOME", ""))
        command.add_argument("--dotnet", default=shutil.which("dotnet") or "dotnet")
    compare = sub.add_parser("compare")
    compare.add_argument("--reference", required=True)
    compare.add_argument("--mcp", required=True)
    compare.add_argument("--output", required=True)
    args = parser.parse_args()
    try:
        {"bootstrap": run_bootstrap, "scenario": run_scenario, "compare": run_compare}[args.command](args)
    except GateError as exc:
        print(f"H1_GATE_SCENARIO_RED {exc}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
