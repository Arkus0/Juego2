#!/usr/bin/env python3
"""Hardened owner-console facade.

The accepted transport implementation remains in local_wp_remote_console_core.py.
This PROCESS_ONLY facade adds state-aware /run and /work routing plus one shared
validity universe for Telegram owner decisions. Product semantics are untouched.
"""

from __future__ import annotations

import importlib.util
import json
import re
import sys
from pathlib import Path

_CORE_PATH = Path(__file__).with_name("local_wp_remote_console_core.py")
_spec = importlib.util.spec_from_file_location("arkus_local_wp_remote_console_core", _CORE_PATH)
if _spec is None or _spec.loader is None:
    raise SystemExit("Cannot load local_wp_remote_console_core.py")
_core = importlib.util.module_from_spec(_spec)
sys.modules[_spec.name] = _core
_spec.loader.exec_module(_core)

for _name in dir(_core):
    if not _name.startswith("_"):
        globals()[_name] = getattr(_core, _name)

WORK_BOUNDARY_RE = re.compile(
    r"^WORK_BOUNDARY (REVIEW_READY|REVIEWER_REQUIRED|CLOSED) WP=([A-Z][A-Z0-9-]*) "
    r"SHA=([0-9a-f]{40}|NONE)(?: STATE=([A-Z_]+))?$",
    re.MULTILINE,
)


def autopilot_command(root: Path, assets_root: Path, wp: str, mode: str = "run") -> list[str]:
    if mode not in {"run", "work"}:
        raise _core.ConsoleError(f"Modo de campaña inválido: {mode}")
    command = [
        sys.executable,
        str(Path(__file__).resolve().parent / "local_wp_autopilot_process.py"),
        "--root", str(root),
        "--assets-root", str(assets_root),
        "--wp", _core.normalize_wp(wp),
        "--one-wp",
    ]
    if mode == "work":
        command.append("--work-only")
    return command


_core.autopilot_command = autopilot_command


class RemoteConsole(_core.RemoteConsole):
    def __init__(self, root: Path, control_dir: Path, token: str, chat_id: int,
                 assets_root: Path | None = None) -> None:
        super().__init__(root, control_dir, token, chat_id, assets_root)
        self._decision_snapshots: dict[str, dict] = {}
        self.command_mode = "run"
        self.boundary_state: str | None = None
        self.boundary_wp: str | None = None
        self.boundary_sha: str | None = None

    def _valid_pending_decisions(self) -> list[tuple[dict, str]]:
        """One authoritative universe for /status and Telegram advertisement."""
        if self.proc is None or not self.campaign_id:
            return []
        valid: list[tuple[dict, str]] = []
        for row in self._decision_requests():
            ident = row.get("decision_id", "")
            options = row.get("options") or []
            pr = row.get("pr")
            sha = str(row.get("sha") or "").lower()
            if (row.get("campaign_id") != self.campaign_id or row.get("completed_at") or
                    row.get("abandoned_at") or ident in self.owner_decisions or
                    not re.fullmatch(r"[0-9a-f]{32}", ident) or type(pr) is not int or pr < 1 or
                    not re.fullmatch(r"[0-9a-f]{40}", sha) or not isinstance(options, list) or
                    not 2 <= len(options) <= 3 or any(not isinstance(item, str) for item in options)):
                continue
            try:
                digest = _core.owner_auth.decision_request_digest(
                    self.campaign_id, ident, str(row.get("wp") or ""), pr, sha,
                    str(row.get("question") or ""), str(row.get("detail") or ""), options)
            except _core.owner_auth.OwnerProofError:
                continue
            if row.get("request_digest") != digest:
                continue
            valid.append((row, digest))
        return valid

    def pending_decision_count(self) -> int:
        return len(self._valid_pending_decisions())

    def launch(self, wp: str, mode: str | None = None) -> None:
        if self.proc is not None:
            raise _core.ConsoleError("Ya hay un WP en ejecución")
        wp = _core.normalize_wp(wp)
        if self.assets_root is None:
            raise _core.ConsoleError("No hay una carpeta externa de assets configurada")
        mode = mode or self.command_mode
        if mode not in {"run", "work"}:
            raise _core.ConsoleError(f"Modo de campaña inválido: {mode}")

        campaign_id = _core.uuid.uuid4().hex
        supervisor_url = self._ensure_ipc()
        logs = self.control_dir / "logs"
        logs.mkdir(parents=True, exist_ok=True)
        log_path = logs / f"{int(_core.time.time())}-{wp}.log"
        handle = log_path.open("w", encoding="utf-8")
        cmd = autopilot_command(self.root, self.assets_root, wp, mode)
        self.proc = _core.subprocess.Popen(
            cmd, cwd=self.root,
            env=_core.child_env(self.control_dir, campaign_id, supervisor_url),
            stdin=_core.subprocess.DEVNULL, stdout=handle,
            stderr=_core.subprocess.STDOUT, text=True,
        )
        self.log_handle = handle
        self.log_path = log_path
        self.current_wp = wp
        self.campaign_id = campaign_id
        self.active = True
        self.next_wp = None
        self.command_mode = mode
        self.boundary_state = None
        self.boundary_wp = None
        self.boundary_sha = None
        self.sent_decisions.clear()
        self.owner_decisions.clear()
        self._decision_snapshots.clear()
        if mode == "work":
            self.send(
                f"▶️ /work {wp} iniciado\n"
                "Se adopta el estado canónico existente y sólo avanzará Worker/Repair hasta REVIEW_READY. "
                "No se lanzará Reviewer."
            )
        else:
            self.send(
                f"▶️ /run {wp} iniciado\n"
                "Se adopta el estado canónico existente y continúa el ciclo completo desde ahí."
            )

    def check_child(self) -> None:
        if self.proc is None or self.proc.poll() is None:
            return
        if self.command_mode != "work":
            super().check_child()
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
        self._decision_snapshots.clear()
        try:
            self._pending_continue_path().unlink(missing_ok=True)
        except OSError:
            pass

        if code != 0:
            self.active = False
            self.next_wp = None
            self._clear_local_pending()
            self.send(
                f"🛑 /work detenido en {finished_wp}.\n"
                "El flujo pidió intervención o falló un control; no se lanzará Reviewer ni otra PR."
            )
            return

        matches = WORK_BOUNDARY_RE.findall(text)
        if not matches:
            self.active = False
            self.next_wp = None
            self._clear_local_pending()
            self.send(
                f"🛑 /work {finished_wp} terminó sin una frontera canónica reconocible. "
                "No se lanzará Reviewer automáticamente."
            )
            return

        kind, wp, sha, state = matches[-1]
        self.active = False
        self.next_wp = None
        self.boundary_state = kind if kind != "CLOSED" else (state or "CLOSED")
        self.boundary_wp = wp
        self.boundary_sha = None if sha == "NONE" else sha
        self._clear_local_pending()
        if kind == "REVIEW_READY":
            self.send(
                f"🟡 {wp} está REVIEW_READY\n"
                f"Candidate SHA: {sha}\n"
                "Espera Reviewer. /work se detiene aquí y no invoca Reviewer.\n"
                f"Usa /run {wp} para habilitar Reviewer automático y continuar el ciclo."
            )
        elif kind == "REVIEWER_REQUIRED":
            self.send(
                f"🟠 {wp} requiere una frontera Reviewer del protocolo\n"
                f"Candidate SHA: {sha}\n"
                "/work no invoca Reviewer; usa /run para continuar el ciclo completo."
            )
        else:
            self.send(
                f"⏹ {wp} ya está cerrado para /work ({state or 'CLOSED'}).\n"
                "No se reabre Worker ni se crea una segunda PR."
            )

    def status(self) -> str:
        if self.proc is not None:
            pause = "pausa al terminar este WP" if self.paused else (
                "stop al terminar este WP" if self.stop_after_wp else "ejecutando")
            boundary = "Worker/Repair → REVIEW_READY (sin Reviewer)" if self.command_mode == "work" else "ciclo completo desde estado canónico"
            return (
                f"⚙️ Autopilot: {pause}\n"
                f"Modo: /{self.command_mode} — {boundary}\n"
                f"WP: {self.current_wp}\n"
                f"Decisiones pendientes válidas: {self.pending_decision_count()}"
            )
        if self.boundary_state == "REVIEW_READY" and self.boundary_wp and self.boundary_sha:
            return (
                f"🟡 {self.boundary_wp}: REVIEW_READY\n"
                f"Candidate SHA: {self.boundary_sha}\n"
                f"Frontera /work alcanzada; espera Reviewer. /run {self.boundary_wp} habilita Reviewer automático."
            )
        if self.boundary_state and self.boundary_wp:
            return f"⏹ {self.boundary_wp}: {self.boundary_state}\n/work no reabre trabajo cerrado."
        if self.active and self.paused and self.next_wp:
            return f"⏸ Autopilot pausado\nSiguiente WP: {self.next_wp}\nModo: /{self.command_mode}"
        return "⏹ Autopilot parado"

    def advertise_decisions(self) -> None:
        for row, request_digest in self._valid_pending_decisions():
            ident = row["decision_id"]
            if ident in self.sent_decisions:
                continue
            options = row["options"]
            snapshot = {
                "campaign_id": self.campaign_id,
                "decision_id": ident,
                "request_digest": request_digest,
                "wp": row.get("wp"),
                "pr": row["pr"],
                "sha": str(row["sha"]).lower(),
                "question": str(row.get("question") or ""),
                "detail": str(row.get("detail") or ""),
                "options": list(options),
            }
            self._decision_snapshots[ident] = snapshot

            keyboard = []
            option_lines = []
            for index, option in enumerate(options):
                option_number = index + 1
                option_lines.append(f"{option_number}. {option}")
                keyboard.append([{
                    "text": f"{option_number}️⃣ Elegir opción {option_number}",
                    "callback_data": f"arkus:decision:{ident}:{index}",
                }])

            subject = row.get("wp") or self.current_wp or "WP"
            detail = str(row.get("detail") or "").strip()
            message = f"🟠 Decisión del owner — {subject}\n{row.get('question', '')}"
            if detail:
                message += f"\n\n{detail[:900]}"
            message += "\n\nOpciones:\n" + "\n".join(option_lines)
            message += (
                "\n\nLa elección se vinculará a esta solicitud exacta y sólo será válida "
                "cuando GitHub Actions autentique el HMAC del supervisor."
            )
            self.send(message, {"inline_keyboard": keyboard})
            self.sent_decisions.add(ident)

    def handle_decision(self, query: dict, decision_id: str, index: int) -> None:
        if self.proc is None or not self.campaign_id:
            self.answer_callback(query["id"], "La decisión ya no pertenece a una campaña activa.", True)
            return
        snapshot = self._decision_snapshots.get(decision_id)
        if snapshot is None or snapshot.get("campaign_id") != self.campaign_id:
            self.answer_callback(query["id"], "La decisión no fue anunciada por esta campaña.", True)
            return
        options = snapshot["options"]
        if not 0 <= index < len(options):
            self.answer_callback(query["id"], "Opción inválida.", True)
            return

        request_path = self.control_dir / "decisions" / f"{decision_id}.json"
        try:
            row = json.loads(request_path.read_text(encoding="utf-8"))
            if (row.get("campaign_id") != self.campaign_id or row.get("completed_at") or
                    row.get("abandoned_at")):
                raise ValueError("stale decision")
            current_digest = _core.owner_auth.decision_request_digest(
                self.campaign_id, decision_id, str(row.get("wp") or ""), row.get("pr"),
                str(row.get("sha") or ""), str(row.get("question") or ""),
                str(row.get("detail") or ""), row.get("options") or [])
        except (OSError, json.JSONDecodeError, _core.owner_auth.OwnerProofError, TypeError, ValueError):
            self.answer_callback(query["id"], "Solicitud local obsoleta o inválida.", True)
            return
        if current_digest != snapshot["request_digest"] or row.get("request_digest") != current_digest:
            self.answer_callback(query["id"], "La solicitud cambió después de mostrarse; decisión rechazada.", True)
            return

        selected = options[index]
        update_id = str(query.get("id", "")).strip()[:120]
        selected_digest = _core.owner_auth.decision_selected_digest(selected)
        try:
            proof = _core.owner_auth.sign_owner_decision(
                self.token, snapshot["pr"], snapshot["sha"], self.campaign_id, decision_id,
                snapshot["request_digest"], index, selected_digest, update_id)
        except _core.owner_auth.OwnerProofError as exc:
            raise _core.ConsoleError(f"No se pudo acuñar la decisión supervisor-only: {exc}") from exc

        payload = {
            "event_type": "arkus_owner_decision_local",
            "client_payload": {
                "pr": snapshot["pr"],
                "target_sha": snapshot["sha"],
                "campaign_id": self.campaign_id,
                "decision_id": decision_id,
                "request_digest": snapshot["request_digest"],
                "choice": index,
                "selected_digest": selected_digest,
                "telegram_update_id": update_id,
                "owner_proof_version": 1,
                "owner_proof": proof,
            },
        }
        _core.gh("api", "--method", "POST", f"repos/{_core.REPO}/dispatches", "--input", "-",
                 input_json=payload)
        self.owner_decisions[decision_id] = {
            "version": 2,
            "decision_id": decision_id,
            "campaign_id": self.campaign_id,
            "choice": index,
            "selected": selected,
            "request_digest": snapshot["request_digest"],
            "attestation": "github-actions[bot]-pending",
            "answered_at": _core.datetime.now(_core.timezone.utc).isoformat(),
        }
        self.answer_callback(query["id"], f"Elegido: {selected}")
        self.send(
            f"✅ Decisión del owner enviada para atestación GitHub exact-SHA ({snapshot['wp'] or self.current_wp}): "
            f"{selected}"
        )

    def command(self, text: str) -> None:
        parts = text.strip().split(maxsplit=1)
        command = parts[0].lower() if parts else ""
        arg = parts[1] if len(parts) > 1 else ""
        if command in {"/run", "/start", "/work"}:
            if self.proc is not None or self.active:
                raise _core.ConsoleError("Ya existe una campaña activa")
            self.paused = False
            self.stop_after_wp = False
            mode = "work" if command == "/work" else "run"
            self.launch(_core.normalize_wp(arg), mode)
        elif command == "/pause":
            if not self.active:
                raise _core.ConsoleError("No hay campaña activa")
            self.paused = True
            self.send("⏸ Pausa solicitada. Por seguridad se aplicará en la siguiente frontera segura.")
        elif command == "/resume":
            if self.proc is not None:
                self.paused = False
                self.send("▶️ Pausa cancelada; el WP actual sigue ejecutándose.")
            elif self.active and self.paused and self.next_wp:
                self.paused = False
                self.launch(self.next_wp, self.command_mode)
            else:
                raise _core.ConsoleError("No hay una campaña pausada que reanudar")
        elif command == "/stop":
            if not self.active:
                raise _core.ConsoleError("No hay campaña activa")
            if self.proc is None:
                self.active = False
                self.paused = False
                self.next_wp = None
                self._clear_local_pending()
                self.send("⏹ Campaña detenida.")
            else:
                self.stop_after_wp = True
                self.paused = False
                self.send("⏹ Stop seguro solicitado. Se aplicará en la siguiente frontera; no se mata un agente a mitad de turno.")
        elif command == "/status":
            self.send(self.status())
        elif command == "/note":
            self.queue_note(arg)
        elif command in {"/help", "/ayuda"}:
            self.send(
                "Comandos Arkus:\n"
                "/work <WP> — adoptar estado existente y avanzar sólo Worker/Repair hasta REVIEW_READY; nunca Reviewer\n"
                "/run <WP> — adoptar estado existente y continuar el ciclo completo desde ahí\n"
                "/status — estado real, modo/frontera y decisiones pendientes válidas\n"
                "/pause — pausar en la siguiente frontera segura\n"
                "/resume — continuar\n"
                "/stop — terminar de forma segura\n"
                "/note texto — instrucción autenticada para el próximo Worker/repair fresco"
            )
        else:
            raise _core.ConsoleError("Comando no reconocido. Usa /help")


_core.RemoteConsole = RemoteConsole


def main() -> int:
    return _core.main()


if __name__ == "__main__":
    raise SystemExit(main())
