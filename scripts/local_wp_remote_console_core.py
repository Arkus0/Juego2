#!/usr/bin/env python3
"""Owner-only Telegram console for the local Juego2 WP autopilot.

Run this idle supervisor on the workstation. It is the single Telegram
getUpdates consumer and can start a bounded WP campaign, report status, pause or
stop at the next WP boundary, queue one owner note for the next Worker-side role,
and route exact owner-decision buttons back to a blocked Worker.

The Telegram token and owner-response authority stay in this supervisor process.
Children receive neither Telegram secrets nor a write-capable owner-response
channel. Decisions and notes are exposed through read-only loopback IPC.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import subprocess
import sys
import threading
import time
import uuid
from datetime import datetime, timezone
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.error import URLError
from urllib.parse import parse_qs, urlparse
from urllib.request import Request, urlopen

import owner_control_auth as owner_auth

REPO = "Arkus0/Juego2"
WP_RE = re.compile(r"^(?:WP-)?([A-Z][A-Z0-9]*(?:-[A-Z0-9]+)+)$")
CONTINUE_RE = re.compile(r"^arkus:continue:([1-9][0-9]*):([0-9a-f]{40}):([23])$")
DECISION_RE = re.compile(r"^arkus:decision:([0-9a-f]{32}):([0-2])$")
NEXT_RE = re.compile(r"Stopped after 1 completed WP\(s\); next=([A-Z][A-Z0-9-]*|NONE)")
TOKEN_ENV = ("TELEGRAM_BOT_TOKEN", "TELEGRAM_CHAT_ID")


class ConsoleError(Exception):
    pass


def atomic_json(path: Path, payload: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    tmp = path.with_suffix(path.suffix + ".tmp")
    tmp.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    os.replace(tmp, path)


def normalize_wp(value: str) -> str:
    match = WP_RE.fullmatch(value.strip().upper())
    if not match:
        raise ConsoleError(f"WP inválido: {value!r}")
    return match.group(1)


def child_env(control_dir: Path, campaign_id: str, supervisor_url: str) -> dict[str, str]:
    env = os.environ.copy()
    for name in TOKEN_ENV:
        env.pop(name, None)
    env["ARKUS_REMOTE_CONTROL_DIR"] = str(control_dir)
    env["ARKUS_REMOTE_CAMPAIGN_ID"] = campaign_id
    env["ARKUS_REMOTE_SUPERVISOR_URL"] = supervisor_url
    return env


def telegram(token: str, method: str, payload: dict):
    request = Request(f"https://api.telegram.org/bot{token}/{method}",
                      data=json.dumps(payload).encode("utf-8"),
                      headers={"Content-Type": "application/json"})
    try:
        with urlopen(request, timeout=45) as response:
            data = json.load(response)
    except (OSError, URLError, ValueError):
        raise ConsoleError(f"Telegram {method} failed") from None
    if not data.get("ok"):
        raise ConsoleError(f"Telegram {method} rejected the request")
    return data.get("result")


def gh(*args: str, input_json: dict | None = None) -> dict | list | None:
    cmd = ["gh", *args]
    result = subprocess.run(cmd, input=(json.dumps(input_json) if input_json is not None else None),
                            capture_output=True, text=True, encoding="utf-8", errors="replace", check=False)
    if result.returncode:
        raise ConsoleError(f"gh failed ({result.returncode}): {result.stderr[-300:]}")
    return json.loads(result.stdout) if result.stdout.strip() else None


def private_owner(update: dict, chat_id: int) -> bool:
    message = update.get("message") or (update.get("callback_query") or {}).get("message") or {}
    chat = message.get("chat") or {}
    sender = (update.get("callback_query") or {}).get("from") or message.get("from") or {}
    return chat.get("type") == "private" and chat.get("id") == chat_id and sender.get("id") == chat_id


def parse_next(log_text: str) -> str | None:
    matches = NEXT_RE.findall(log_text)
    if not matches:
        return None
    return None if matches[-1] == "NONE" else normalize_wp(matches[-1])


class RemoteConsole:
    def __init__(self, root: Path, control_dir: Path, token: str, chat_id: int) -> None:
        self.root = root
        self.control_dir = control_dir
        self.token = token
        self.chat_id = chat_id
        self.offset: int | None = None
        self.proc: subprocess.Popen | None = None
        self.log_handle = None
        self.log_path: Path | None = None
        self.current_wp: str | None = None
        self.next_wp: str | None = None
        self.campaign_id: str | None = None
        self.active = False
        self.paused = False
        self.stop_after_wp = False
        self.sent_decisions: set[str] = set()
        self.owner_decisions: dict[str, dict] = {}
        self.pending_owner_note: str | None = None
        self._ipc_server: ThreadingHTTPServer | None = None
        self._ipc_thread: threading.Thread | None = None
        self.supervisor_url: str | None = None

    def send(self, text: str, reply_markup: dict | None = None) -> None:
        payload = {"chat_id": self.chat_id, "text": text[:4000], "disable_web_page_preview": True}
        if reply_markup:
            payload["reply_markup"] = reply_markup
        telegram(self.token, "sendMessage", payload)

    def answer_callback(self, ident: str, text: str, alert: bool = False) -> None:
        telegram(self.token, "answerCallbackQuery",
                 {"callback_query_id": ident, "text": text[:190], "show_alert": alert})

    def _pending_continue_path(self) -> Path:
        return self.control_dir / "pending-owner-continue.json"

    def _ipc_payload(self, path: str, query: dict[str, list[str]]) -> tuple[int, dict]:
        campaign = (query.get("campaign_id") or [""])[0]
        if not self.campaign_id or campaign != self.campaign_id:
            return 409, {"status": "wrong-campaign"}
        if path == "/v1/note":
            note = self.pending_owner_note or ""
            self.pending_owner_note = None
            return 200, {"status": "ok", "campaign_id": campaign, "note": note}
        if path == "/v1/decision":
            decision_id = (query.get("decision_id") or [""])[0]
            if not re.fullmatch(r"[0-9a-f]{32}", decision_id):
                return 400, {"status": "invalid-decision"}
            response = self.owner_decisions.get(decision_id)
            if response is None:
                return 200, {"status": "pending", "campaign_id": campaign, "decision_id": decision_id}
            return 200, dict(response, status="answered")
        return 404, {"status": "not-found"}

    def _ensure_ipc(self) -> str:
        if self._ipc_server is not None and self.supervisor_url:
            return self.supervisor_url
        console = self

        class Handler(BaseHTTPRequestHandler):
            def do_GET(self) -> None:
                parsed = urlparse(self.path)
                status, payload = console._ipc_payload(parsed.path, parse_qs(parsed.query))
                body = json.dumps(payload, ensure_ascii=False).encode("utf-8")
                self.send_response(status)
                self.send_header("Content-Type", "application/json")
                self.send_header("Content-Length", str(len(body)))
                self.end_headers()
                self.wfile.write(body)

            def do_POST(self) -> None:
                self.send_error(405, "read-only supervisor IPC")

            def do_PUT(self) -> None:
                self.send_error(405, "read-only supervisor IPC")

            def log_message(self, _format: str, *args) -> None:
                del args

        server = ThreadingHTTPServer(("127.0.0.1", 0), Handler)
        server.daemon_threads = True
        thread = threading.Thread(target=server.serve_forever, name="arkus-owner-ipc", daemon=True)
        thread.start()
        self._ipc_server = server
        self._ipc_thread = thread
        self.supervisor_url = f"http://127.0.0.1:{server.server_port}"
        return self.supervisor_url

    def _close_ipc(self) -> None:
        if self._ipc_server is not None:
            self._ipc_server.shutdown()
            self._ipc_server.server_close()
        self._ipc_server = None
        self._ipc_thread = None
        self.supervisor_url = None

    def _clear_local_pending(self) -> None:
        self.pending_owner_note = None
        self.owner_decisions.clear()
        try:
            self._pending_continue_path().unlink(missing_ok=True)
        except OSError:
            pass

    def launch(self, wp: str) -> None:
        if self.proc is not None:
            raise ConsoleError("Ya hay un WP en ejecución")
        wp = normalize_wp(wp)
        campaign_id = uuid.uuid4().hex
        supervisor_url = self._ensure_ipc()
        logs = self.control_dir / "logs"
        logs.mkdir(parents=True, exist_ok=True)
        log_path = logs / f"{int(time.time())}-{wp}.log"
        handle = log_path.open("w", encoding="utf-8")
        cmd = [sys.executable, str(self.root / "scripts" / "local_wp_autopilot_remote.py"),
               "--root", str(self.root), "--wp", wp, "--one-wp"]
        self.proc = subprocess.Popen(cmd, cwd=self.root,
                                     env=child_env(self.control_dir, campaign_id, supervisor_url),
                                     stdin=subprocess.DEVNULL, stdout=handle,
                                     stderr=subprocess.STDOUT, text=True)
        self.log_handle = handle
        self.log_path = log_path
        self.current_wp = wp
        self.campaign_id = campaign_id
        self.active = True
        self.next_wp = None
        self.sent_decisions.clear()
        self.owner_decisions.clear()
        self.send(f"▶️ Campaña Arkus iniciada\nWP actual: {wp}\nEl Reviewer seguirá siendo una sesión fresca e independiente.")

    def check_child(self) -> None:
        if self.proc is None or self.proc.poll() is None:
            return
        code = self.proc.returncode
        if self.log_handle:
            self.log_handle.close()
        text = self.log_path.read_text(encoding="utf-8", errors="replace") if self.log_path else ""
        finished_wp = self.current_wp or "?"
        self.proc = None
        self.log_handle = None
        self.campaign_id = None
        self.sent_decisions.clear()
        self.owner_decisions.clear()
        try:
            self._pending_continue_path().unlink(missing_ok=True)
        except OSError:
            pass
        if code != 0:
            self.active = False
            self.next_wp = None
            self._clear_local_pending()
            self.send(f"🛑 Autopilot detenido en {finished_wp}.\nEl flujo ha pedido intervención o falló un control. Revisa el aviso/GitHub; no se lanzará otro WP.")
            return
        next_wp = parse_next(text)
        if next_wp is None:
            self.active = False
            self.next_wp = None
            self._clear_local_pending()
            self.send(f"🏁 {finished_wp} completado y el handoff no declara otro WP. Campaña finalizada.")
            return
        self.next_wp = next_wp
        if self.stop_after_wp:
            self.active = False
            self.stop_after_wp = False
            self._clear_local_pending()
            self.send(f"⏹ {finished_wp} completado. Stop seguro aplicado antes de {next_wp}.")
            return
        if self.paused:
            self.send(f"⏸ {finished_wp} completado. Pausado antes de {next_wp}. Usa /resume para seguir.")
            return
        self.launch(next_wp)

    def queue_note(self, note: str) -> None:
        if not self.active:
            raise ConsoleError("/note requiere una campaña activa")
        note = note.strip()
        if not note:
            raise ConsoleError("Uso: /note <instrucción>")
        self.pending_owner_note = note[:4000]
        self.send("📝 Instrucción guardada en el supervisor para la próxima sesión Worker/repair fresca. No se inyecta en un Reviewer ni altera un turno ya iniciado.")

    def status(self) -> str:
        if self.proc is not None:
            mode = "pausa al terminar este WP" if self.paused else ("stop al terminar este WP" if self.stop_after_wp else "ejecutando")
            return f"⚙️ Autopilot: {mode}\nWP: {self.current_wp}\nDecisiones pendientes: {self.pending_decision_count()}"
        if self.active and self.paused and self.next_wp:
            return f"⏸ Autopilot pausado\nSiguiente WP: {self.next_wp}"
        return "⏹ Autopilot parado"

    def pending_decision_count(self) -> int:
        return sum(1 for item in self._decision_requests() if item["decision_id"] not in self.owner_decisions)

    def _decision_requests(self) -> list[dict]:
        directory = self.control_dir / "decisions"
        if not directory.exists():
            return []
        rows = []
        for path in sorted(directory.glob("*.json")):
            if path.name.endswith(".response.json"):
                continue
            try:
                row = json.loads(path.read_text(encoding="utf-8"))
            except (OSError, json.JSONDecodeError):
                continue
            if row.get("completed_at") or row.get("abandoned_at"):
                continue
            rows.append(row)
        return rows

    def advertise_decisions(self) -> None:
        if self.proc is None or not self.campaign_id:
            return
        for row in self._decision_requests():
            ident = row.get("decision_id", "")
            if ident in self.sent_decisions or row.get("campaign_id") != self.campaign_id:
                continue
            options = row.get("options") or []
            if (not re.fullmatch(r"[0-9a-f]{32}", ident) or not isinstance(options, list) or
                    not (2 <= len(options) <= 3)):
                continue
            keyboard = []
            for index, option in enumerate(options):
                callback = f"arkus:decision:{ident}:{index}"
                keyboard.append([{"text": f"{index + 1}️⃣ {str(option)[:48]}", "callback_data": callback}])
            subject = row.get("wp") or self.current_wp or "WP"
            detail = (row.get("detail") or "").strip()
            message = f"🟠 Decisión del owner — {subject}\n{row.get('question', '')}"
            if detail:
                message += f"\n\n{detail[:900]}"
            message += "\n\nLa respuesta sólo existirá dentro del supervisor; el Worker no puede escribirla en el canal IPC."
            self.send(message, {"inline_keyboard": keyboard})
            self.sent_decisions.add(ident)

    def handle_decision(self, query: dict, decision_id: str, index: int) -> None:
        request_path = self.control_dir / "decisions" / f"{decision_id}.json"
        if not request_path.exists() or self.proc is None or not self.campaign_id:
            self.answer_callback(query["id"], "La decisión ya no pertenece a una campaña activa.", True)
            return
        try:
            row = json.loads(request_path.read_text(encoding="utf-8"))
        except (OSError, json.JSONDecodeError):
            self.answer_callback(query["id"], "Solicitud local inválida.", True)
            return
        options = row.get("options") or []
        if row.get("campaign_id") != self.campaign_id or row.get("completed_at") or not 0 <= index < len(options):
            self.answer_callback(query["id"], "Solicitud obsoleta o inválida.", True)
            return
        response = {"version": 1, "decision_id": decision_id, "campaign_id": self.campaign_id,
                    "choice": index, "selected": options[index],
                    "answered_at": datetime.now(timezone.utc).isoformat()}
        self.owner_decisions[decision_id] = response
        self.answer_callback(query["id"], f"Elegido: {options[index]}")
        self.send(f"✅ Decisión autenticada en el supervisor para el Worker de {row.get('wp') or self.current_wp}: {options[index]}")

    def handle_continue(self, query: dict, pr: int, sha: str, fail_count: int) -> None:
        if self.proc is None or not self.campaign_id:
            self.answer_callback(query["id"], "No hay campaña local activa para esta autorización.", True)
            return
        pending_path = self._pending_continue_path()
        try:
            pending = json.loads(pending_path.read_text(encoding="utf-8"))
        except (OSError, json.JSONDecodeError):
            self.answer_callback(query["id"], "Esta campaña no está esperando esa autorización.", True)
            return
        expected = (pending.get("campaign_id"), pending.get("pr"), pending.get("sha"), pending.get("fail_count"))
        actual = (self.campaign_id, pr, sha, fail_count)
        if expected != actual:
            self.answer_callback(query["id"], "Botón obsoleto: no corresponde al bloqueo activo de esta campaña.", True)
            return
        script = self.root / "scripts" / "telegram_continue_receiver.py"
        import importlib.util
        spec = importlib.util.spec_from_file_location("arkus_tg_receiver_local", script)
        if spec is None or spec.loader is None:
            raise ConsoleError("No se puede cargar telegram_continue_receiver.py")
        module = importlib.util.module_from_spec(spec)
        sys.modules[spec.name] = module
        spec.loader.exec_module(module)
        module.fresh_offer = lambda created_at, now=None: True
        try:
            status = module.validate_current(pr, sha, fail_count)
        except module.ReceiverError as exc:
            self.answer_callback(query["id"], f"Ya no es válido: {exc}", True)
            return
        if status == "already":
            self.answer_callback(query["id"], "Ya estaba autorizado.")
            return
        update_id = str(query.get("id", "")).strip()[:120]
        try:
            proof = owner_auth.sign_owner_continue(self.token, pr, sha, fail_count,
                                                   self.campaign_id, update_id)
        except owner_auth.OwnerProofError as exc:
            raise ConsoleError(f"No se pudo acuñar la prueba supervisor-only: {exc}") from exc
        payload = {"event_type": "arkus_owner_continue_local",
                   "client_payload": {"pr": pr, "target_sha": sha, "fail_count": fail_count,
                                      "campaign_id": self.campaign_id,
                                      "telegram_update_id": update_id,
                                      "owner_proof_version": 1,
                                      "owner_proof": proof}}
        gh("api", "--method", "POST", f"repos/{REPO}/dispatches", "--input", "-", input_json=payload)
        self.answer_callback(query["id"], "Autorización autenticada enviada. GitHub validará origen + PR/SHA/FAIL/campaña antes de continuar.")

    def command(self, text: str) -> None:
        parts = text.strip().split(maxsplit=1)
        command = parts[0].lower() if parts else ""
        arg = parts[1] if len(parts) > 1 else ""
        if command in {"/run", "/start"}:
            if self.proc is not None or self.active:
                raise ConsoleError("Ya existe una campaña activa")
            self.paused = False
            self.stop_after_wp = False
            self.launch(normalize_wp(arg))
        elif command == "/pause":
            if not self.active:
                raise ConsoleError("No hay campaña activa")
            self.paused = True
            self.send("⏸ Pausa solicitada. Por seguridad no corto un agente a mitad de turno: se aplicará antes del siguiente WP.")
        elif command == "/resume":
            if self.proc is not None:
                self.paused = False
                self.send("▶️ Pausa cancelada; el WP actual sigue ejecutándose.")
            elif self.active and self.paused and self.next_wp:
                self.paused = False
                self.launch(self.next_wp)
            else:
                raise ConsoleError("No hay una campaña pausada que reanudar")
        elif command == "/stop":
            if not self.active:
                raise ConsoleError("No hay campaña activa")
            if self.proc is None:
                self.active = False
                self.paused = False
                self.next_wp = None
                self._clear_local_pending()
                self.send("⏹ Campaña detenida.")
            else:
                self.stop_after_wp = True
                self.paused = False
                self.send("⏹ Stop seguro solicitado. Terminará el WP actual y no arrancará otro. No se mata un Worker/Reviewer a mitad de turno.")
        elif command == "/status":
            self.send(self.status())
        elif command == "/note":
            self.queue_note(arg)
        elif command in {"/help", "/ayuda"}:
            self.send("Comandos Arkus:\n/run H1-04 — iniciar campaña\n/status — estado\n/pause — pausar antes del siguiente WP\n/resume — continuar\n/stop — terminar de forma segura tras el WP actual\n/note texto — instrucción autenticada por el supervisor para el próximo Worker/repair fresco")
        else:
            raise ConsoleError("Comando no reconocido. Usa /help")

    def handle_update(self, update: dict) -> None:
        if not private_owner(update, self.chat_id):
            return
        query = update.get("callback_query") or {}
        if query:
            data = query.get("data") or ""
            match = DECISION_RE.fullmatch(data)
            if match:
                self.handle_decision(query, match.group(1), int(match.group(2)))
                return
            match = CONTINUE_RE.fullmatch(data)
            if match:
                self.handle_continue(query, int(match.group(1)), match.group(2), int(match.group(3)))
                return
            self.answer_callback(query.get("id", ""), "Control obsoleto o no reconocido.", True)
            return
        message = update.get("message") or {}
        text = message.get("text") or ""
        if not text.startswith("/"):
            return
        try:
            self.command(text)
        except ConsoleError as exc:
            self.send(f"⚠️ {exc}")

    def _get_updates(self, *, timeout: int, offset: int | None = None) -> list[dict]:
        params = {"timeout": timeout, "limit": 100, "allowed_updates": ["message", "callback_query"]}
        if offset is not None:
            params["offset"] = offset
        updates = telegram(self.token, "getUpdates", params)
        if not isinstance(updates, list):
            raise ConsoleError("Respuesta getUpdates inválida")
        return updates

    def discard_offline_backlog(self) -> None:
        offset: int | None = None
        while True:
            updates = self._get_updates(timeout=0, offset=offset)
            if not updates:
                self.offset = offset
                return
            offset = max(int(update["update_id"]) for update in updates) + 1

    def poll_once(self) -> None:
        updates = self._get_updates(timeout=20, offset=self.offset)
        for update in updates:
            self.offset = max(self.offset or 0, int(update["update_id"]) + 1)
            self.handle_update(update)

    def run(self) -> None:
        webhook = telegram(self.token, "getWebhookInfo", {})
        if not isinstance(webhook, dict) or webhook.get("url"):
            raise ConsoleError("El bot tiene webhook; el modo local getUpdates requiere webhook vacío")
        self.control_dir.mkdir(parents=True, exist_ok=True)
        self._clear_local_pending()
        self._ensure_ipc()
        self.discard_offline_backlog()
        self.send("🟢 Arkus Telegram console online.\nNo hace falta tener una sesión Codex abierta. Los comandos enviados mientras el supervisor estaba offline se descartan; usa /run <WP> para iniciar y /help para controles.")
        try:
            while True:
                self.check_child()
                self.advertise_decisions()
                self.poll_once()
        finally:
            self._close_ipc()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", default=".")
    parser.add_argument("--control-dir", default=str(Path(os.environ.get("LOCALAPPDATA", str(Path.home()))) / "Arkus" / "Juego2" / "remote-control"))
    args = parser.parse_args()
    token = os.environ.get("TELEGRAM_BOT_TOKEN", "").strip()
    raw_chat = os.environ.get("TELEGRAM_CHAT_ID", "").strip()
    if not token or not raw_chat.isdecimal():
        print("REMOTE_CONSOLE_STOP: TELEGRAM_BOT_TOKEN/TELEGRAM_CHAT_ID local configuration missing", file=sys.stderr)
        return 2
    root = Path(args.root).resolve()
    if not (root / "scripts" / "local_wp_autopilot.py").exists():
        print("REMOTE_CONSOLE_STOP: --root is not a Juego2 checkout", file=sys.stderr)
        return 2
    try:
        RemoteConsole(root, Path(args.control_dir).resolve(), token, int(raw_chat)).run()
    except (ConsoleError, OSError, KeyboardInterrupt) as exc:
        print(f"REMOTE_CONSOLE_STOP: {exc}", file=sys.stderr)
        return 2
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
