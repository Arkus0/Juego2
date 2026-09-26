#!/usr/bin/env python3
"""WP-H1-GATE hosted fresh independent AI-agent trial relay (owner-selected OpenRouter route).

The relay is plumbing, not a participant:

* it starts the fixed public MCP host (`Arkus.Harness.Mcp --h1-unity`) on the exact candidate, inside the pinned Unity
  image, exactly like the deterministic scenario's MCP path;
* it offers the model the host's `tools/list` result and nothing else. The model's only instructions are the text
  between the BRIEF markers of `Docs/evidence/WP-H1-GATE/AI_AGENT_TRIAL_BRIEF.md` plus one fixed kickoff line;
* it forwards only tool calls the model issued, unchanged, and returns the host's structured result. It never issues,
  edits or suggests a call;
* it records every model message, tool call and tool result in `trial-transcript.jsonl`, then derives the closing
  evidence (canonical hash, active generation, graph digest) from recorded tool results rather than the model's claims.

Usage: h1-gate-ai-trial.py --workspace PATH --out DIR --candidate-sha SHA [--launcher docker|local]
Requires OPENROUTER_API_KEY in the environment. The key is never written to disk or to the transcript.
"""

import argparse
import hashlib
import json
import os
import re
import subprocess
import sys
import time
import urllib.error
import urllib.request
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROTOCOL = ROOT / "Docs/evidence/WP-H1-GATE/AI_TRIAL_PROTOCOL.json"


def sha(value):
    return hashlib.sha256(value if isinstance(value, bytes) else value.encode("utf-8")).hexdigest()


def compact(value):
    return json.dumps(value, sort_keys=True, separators=(",", ":"), ensure_ascii=False)


def brief_text():
    text = (ROOT / "Docs/evidence/WP-H1-GATE/AI_AGENT_TRIAL_BRIEF.md").read_text(encoding="utf-8")
    match = re.search(r"<!-- BRIEF START -->\n(.*?)\n<!-- BRIEF END -->", text, re.S)
    if not match:
        raise SystemExit("H1_GATE_TRIAL_RED brief markers missing")
    return match.group(1)


class Recorder:
    def __init__(self, path):
        self.path = Path(path)
        self.path.parent.mkdir(parents=True, exist_ok=True)
        self.path.write_text("", encoding="utf-8")

    def write(self, record):
        with self.path.open("a", encoding="utf-8") as handle:
            handle.write(compact(dict(record, at=round(time.time(), 3))) + "\n")


class McpHost:
    def __init__(self, command, workspace):
        self.process = subprocess.Popen(command, cwd=workspace, stdin=subprocess.PIPE, stdout=subprocess.PIPE,
                                        stderr=subprocess.PIPE, text=True, bufsize=1)
        self.sequence = 0

    def rpc(self, method, params):
        self.sequence += 1
        wanted = self.sequence
        self.process.stdin.write(compact({"jsonrpc": "2.0", "id": wanted, "method": method, "params": params}) + "\n")
        self.process.stdin.flush()
        while True:
            line = self.process.stdout.readline()
            if not line:
                raise RuntimeError("MCP host ended: " + self.process.stderr.read()[-2000:])
            value = json.loads(line)
            if value.get("id") == wanted:
                return value

    def notify(self, method, params):
        self.process.stdin.write(compact({"jsonrpc": "2.0", "method": method, "params": params}) + "\n")
        self.process.stdin.flush()

    def close(self):
        if self.process.poll() is None:
            self.process.stdin.close()
            try:
                return self.process.wait(timeout=60)
            except subprocess.TimeoutExpired:
                self.process.kill()
                return -9
        return self.process.returncode


def host_command(args):
    dll = "src/Arkus.Harness.Mcp/bin/Release/net8.0/Arkus.Harness.Mcp.dll"
    if args.launcher == "local":
        return [args.dotnet, str(Path(args.workspace) / dll), "--h1-unity"]
    return ["docker", "run", "--rm", "--name", f"h1-gate-trial-{os.environ.get('GITHUB_RUN_ID', 'local')}", "-i",
            "--shm-size=1025m", "--workdir", "/github/workspace",
            "--volume", f"{args.license_home}:/root",
            "--volume", f"{Path(args.workspace).resolve()}:/github/workspace",
            "--volume", "/usr/share/dotnet:/usr/share/dotnet:ro", "--env", "DOTNET_ROOT=/usr/share/dotnet",
            args.unity_image, "/usr/share/dotnet/dotnet", f"/github/workspace/{dll}", "--h1-unity"]


def openai_name(name):
    return re.sub(r"[^a-zA-Z0-9_-]", "_", name)[:64]


def model_parameters(schema):
    """Present the host's JSON schema to the provider without meaning loss.

    `$schema` is dropped, and the vendor keyword `x-arkus-reference-namespace` is moved into the node's
    description, because some providers reject unknown `x-*` keywords. Property names are never touched.
    """
    def node(value):
        if not isinstance(value, dict):
            return value
        out = {}
        for key, child in value.items():
            if key == "$schema":
                continue
            if key == "x-arkus-reference-namespace":
                continue
            if key == "properties" and isinstance(child, dict):
                out[key] = {name: node(sub) for name, sub in child.items()}
            elif key == "items":
                out[key] = node(child)
            else:
                out[key] = child
        namespace = value.get("x-arkus-reference-namespace")
        if namespace:
            prefix = (out.get("description", "") + " ").lstrip()
            out["description"] = f"{prefix}Arkus logical reference in namespace {namespace}.".strip()
        return out
    return node(dict(schema or {"type": "object"}))


def provider_call(protocol, messages, tools, key):
    body = {"model": protocol["model"], "messages": messages, "tools": tools, "tool_choice": "auto",
            "max_tokens": protocol["maxOutputTokensPerTurn"]}
    data = json.dumps(body, ensure_ascii=False).encode("utf-8")
    last = None
    for attempt in range(protocol["providerRetries"] + 1):
        request = urllib.request.Request(protocol["endpoint"], data=data, method="POST", headers={
            "Authorization": "Bearer " + key, "Content-Type": "application/json",
            "X-OpenRouter-Title": "Juego2 H1-GATE fresh AI-agent trial"})
        try:
            with urllib.request.urlopen(request, timeout=protocol["requestTimeoutSeconds"]) as response:
                value = json.loads(response.read())
            if value.get("choices"):
                return value
            last = "provider returned no choices: " + compact(value)[:500]
        except urllib.error.HTTPError as exc:
            last = f"HTTP {exc.code}: {exc.read().decode('utf-8', errors='replace')[:500]}"
            if exc.code not in (408, 409, 429, 500, 502, 503, 504):
                break
        except (urllib.error.URLError, TimeoutError, json.JSONDecodeError) as exc:
            last = f"transport: {exc}"
        time.sleep(10 * (attempt + 1))
    raise RuntimeError("OpenRouter call failed: " + str(last))


def final_report(text):
    match = re.search(r"```json\s*(\{.*?\})\s*```", text or "", re.S)
    if not match:
        return None
    try:
        return json.loads(match.group(1))
    except json.JSONDecodeError:
        return None


def derive_final(results):
    final = {"canonicalHash": None, "revision": None, "activeGenerationId": None, "graphDigest": None,
             "journalEntryCount": None, "checkpointId": None}
    for key, result in results:
        if not isinstance(result, dict):
            continue
        world = result.get("world") if isinstance(result.get("world"), dict) else None
        if world and "hash" in world:
            final["canonicalHash"], final["revision"] = world["hash"], world.get("revision")
        if key.startswith("unity.host.projection.") and result.get("generationId") and result.get("graphDigest"):
            final["activeGenerationId"], final["graphDigest"] = result["generationId"], result["graphDigest"]
        if key.startswith("authoring.journal.read") and "entryCount" in result:
            final["journalEntryCount"] = result["entryCount"]
        if key.startswith("unity.host.checkpoint.") and result.get("checkpointId"):
            final["checkpointId"] = result["checkpointId"]
    return final


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--workspace", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--candidate-sha", required=True)
    parser.add_argument("--launcher", choices=("docker", "local"), default="docker")
    parser.add_argument("--unity-image", default=os.environ.get("PINNED_UNITY_IMAGE", ""))
    parser.add_argument("--license-home", default=os.environ.get("UNITY_LICENSE_HOME", ""))
    parser.add_argument("--dotnet", default="dotnet")
    args = parser.parse_args()
    key = os.environ.get("OPENROUTER_API_KEY", "")
    if not key:
        raise SystemExit("H1_GATE_TRIAL_RED OPENROUTER_API_KEY missing")
    protocol = json.loads(PROTOCOL.read_text(encoding="utf-8"))
    out = Path(args.out)
    recorder = Recorder(out / "trial-transcript.jsonl")
    brief = brief_text()
    host = McpHost(host_command(args), args.workspace)
    results = []
    verdict = "INCOMPLETE"
    report = None
    turns = 0
    relay_error = None
    try:
        initialized = host.rpc("initialize", {"protocolVersion": "2025-11-25", "capabilities": {},
                                              "clientInfo": {"name": "arkus.h1-gate.ai-trial-relay", "version": "1.0.0"}})
        host.notify("notifications/initialized", {})
        listed = host.rpc("tools/list", {})["result"]["tools"]
        by_openai, tools = {}, []
        for tool in listed:
            name = openai_name(tool["name"])
            if name in by_openai:
                raise RuntimeError("tool name collision after sanitizing: " + name)
            by_openai[name] = tool
            # `strict` is declared explicitly (protocol `toolStrict`, false): strict function calling makes every
            # property required, which would misrepresent the host's optional properties to the model.
            tools.append({"type": "function", "function": {
                "name": name, "description": (tool.get("description") or "")[:1000],
                "parameters": model_parameters(tool.get("inputSchema")), "strict": bool(protocol["toolStrict"])}})
        recorder.write({"event": "discovery", "protocolVersion": initialized.get("result", {}).get("protocolVersion"),
                        "canonicalKeys": sorted(t.get("_meta", {}).get("dev.arkus/canonicalKey", "") for t in listed),
                        "offeredToolNames": sorted(by_openai), "briefSha256": sha(brief), "protocolSha256": sha(PROTOCOL.read_bytes())})
        messages = [{"role": "system", "content": brief}, {"role": "user", "content": protocol["userKickoff"]}]
        while turns < protocol["maxModelTurns"]:
            turns += 1
            response = provider_call(protocol, messages, tools, key)
            message = response["choices"][0]["message"]
            calls = message.get("tool_calls") or []
            recorder.write({"event": "model-message", "turn": turns, "responseId": response.get("id"),
                            "model": response.get("model"), "provider": response.get("provider"), "content": message.get("content"),
                            "toolCalls": [{"id": c["id"], "name": c["function"]["name"], "arguments": c["function"].get("arguments")} for c in calls],
                            "usage": response.get("usage")})
            assistant = {"role": "assistant", "content": message.get("content") or ""}
            if calls:
                assistant["tool_calls"] = calls
            messages.append(assistant)
            if not calls:
                report = final_report(message.get("content"))
                verdict = (report or {}).get("verdict", "NO_REPORT")
                break
            for call in calls:
                name = call["function"]["name"]
                tool = by_openai.get(name)
                started = time.time()
                if tool is None:
                    outcome = {"status": "relay-error", "error": {"machineCode": "relay.unknown-tool", "message": f"No offered tool named {name}."}}
                    canonical = None
                else:
                    canonical = tool.get("_meta", {}).get("dev.arkus/canonicalKey")
                    try:
                        arguments = json.loads(call["function"].get("arguments") or "{}")
                    except json.JSONDecodeError:
                        arguments = None
                    recorder.write({"event": "tool-call", "callId": call["id"], "toolName": name, "mcpName": tool["name"],
                                    "canonicalKey": canonical, "arguments": arguments})
                    if not isinstance(arguments, dict):
                        outcome = {"status": "relay-error", "error": {"machineCode": "relay.arguments-not-json-object", "message": "Tool arguments must be a JSON object."}}
                    else:
                        response_frame = host.rpc("tools/call", {"name": tool["name"], "arguments": arguments})
                        if "error" in response_frame:
                            outcome = {"status": "transport-error", "error": response_frame["error"]}
                        else:
                            outcome = response_frame.get("result", {}).get("structuredContent") or response_frame.get("result", {})
                error = outcome.get("error", {}) if isinstance(outcome, dict) else {}
                recorder.write({"event": "tool-result", "callId": call["id"], "canonicalKey": canonical,
                                "status": outcome.get("status") if isinstance(outcome, dict) else None,
                                "machineCode": error.get("machineCode"), "outcome": outcome,
                                "elapsedMs": int((time.time() - started) * 1000)})
                if canonical and isinstance(outcome, dict) and outcome.get("status") == "success":
                    results.append((canonical, outcome.get("result")))
                shown = compact(outcome)
                if len(shown) > protocol["maxToolResultCharsShownToModel"]:
                    shown = shown[:protocol["maxToolResultCharsShownToModel"]] + f"...[relay truncated {len(shown) - protocol['maxToolResultCharsShownToModel']} chars for context size; the full result is recorded]"
                messages.append({"role": "tool", "tool_call_id": call["id"], "content": shown})
    except Exception as exc:  # recorded, never silently turned into a verdict
        relay_error = f"{type(exc).__name__}: {str(exc)[:1500]}"
        verdict = "RELAY_ERROR"
        recorder.write({"event": "relay-error", "error": relay_error})
    finally:
        exit_code = host.close()
    transcript = out / "trial-transcript.jsonl"
    record = {
        "schemaId": "arkus.h1-gate-ai-trial-record@1", "candidateSha": args.candidate_sha,
        "transport": "MCP_STDIO", "implementationSourceAccess": "NONE", "model": protocol["model"],
        "briefSha256": sha(brief), "protocolSha256": sha(PROTOCOL.read_bytes()), "modelTurns": turns,
        "hostExitCode": exit_code, "transcriptSha256": sha(transcript.read_bytes()),
        "final": derive_final(results), "agentReport": report, "verdict": verdict,
        "relayError": relay_error, "githubRunId": os.environ.get("GITHUB_RUN_ID"),
    }
    (out / "trial-record.json").write_text(json.dumps(record, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(f"H1_GATE_TRIAL_RECORDED verdict={verdict} turns={turns} final={compact(record['final'])}")
    if relay_error:
        print(f"H1_GATE_TRIAL_RED relay error: {relay_error}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
