#!/usr/bin/env python3
"""Hardened owner-console facade.

The accepted transport implementation remains in local_wp_remote_console_core.py.
This PROCESS_ONLY facade adds state-aware /run and /work routing plus one shared
validity universe for Telegram owner decisions. Product semantics are untouched.
"""

from __future__ import annotations

import importlib.util
import json
import os
import re
import sys
from pathlib import Path

from local_wp_recovery_policy import (Disposition, campaign_proof, child_disposition,
                                      valid_campaign_proof)
import request_owner_decision as decision_contract

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


class MaterialCampaignConflict(_core.ConsoleError):
    """Two authoritative identities disagree; automatic recovery must stop."""


class RemoteConsole(_core.RemoteConsole):
    def __init__(self, root: Path, control_dir: Path, token: str, chat_id: int,
                 assets_root: Path | None = None) -> None:
        super().__init__(root, control_dir, token, chat_id, assets_root)
        self._decision_snapshots: dict[str, dict] = {}
        self.command_mode = "run"
        self.boundary_state: str | None = None
        self.boundary_wp: str | None = None
        self.boundary_sha: str | None = None
        self.campaign_status = "IDLE"
        self.campaign_reason = ""
        self.detached_pid: int | None = None
        self.blocked_sha: str | None = None

    def _campaign_path(self) -> Path:
        return self.control_dir / "campaign.json"

    def _persist_campaign(self) -> None:
        if not self.campaign_id or not self.current_wp:
            return
        payload = {
            "version": 1, "campaign_id": self.campaign_id, "wp": self.current_wp,
            "mode": self.command_mode, "status": self.campaign_status,
            "reason": self.campaign_reason,
            "pid": getattr(self.proc, "pid", None) if self.proc else self.detached_pid,
            "log_path": str(self.log_path) if self.log_path else None,
            "next_wp": self.next_wp, "paused": self.paused,
            "stop_after_wp": self.stop_after_wp,
            "blocked_sha": self.blocked_sha,
            "owner_decisions": self.owner_decisions,
        }
        payload["proof"] = campaign_proof(self.token, payload)
        _core.atomic_json(self._campaign_path(), payload)

    def _github_campaign_pr(self) -> dict | None:
        """Re-read PR ownership and exact HEAD before any automatic resume."""
        pages = _core.gh("api", "--paginate", "--slurp",
                         f"repos/{_core.REPO}/pulls?state=all&per_page=100")
        if not isinstance(pages, list) or any(not isinstance(page, list) for page in pages):
            raise MaterialCampaignConflict("GitHub devolvió un inventario de PR inválido")
        target = f"WP-{self.current_wp}"
        matches = []
        for page in pages:
            for pr in page:
                body = str(pr.get("body") or "")
                if re.search(rf"(?im)^WP:\s*`?{re.escape(target)}`?\s*$", body):
                    matches.append(pr)
        open_prs = [pr for pr in matches if pr.get("state") == "open"]
        if len(open_prs) > 1:
            raise MaterialCampaignConflict(f"Dos PR canónicas abiertas para {target}")
        if not open_prs:
            # A damaged WP field cannot authorize bootstrapping a replacement PR.
            possible = [pr for page in pages for pr in page if pr.get("state") == "open" and
                        (str((pr.get("head") or {}).get("ref") or "").lower() ==
                         f"worker/{self.current_wp.lower()}" or
                         str(pr.get("title") or "").upper().startswith(target))]
            if possible:
                raise MaterialCampaignConflict(
                    f"PR de {target} con ownership/body incongruente: {[pr.get('number') for pr in possible]}")
        chosen = open_prs[0] if open_prs else max(
            (pr for pr in matches if pr.get("merged_at")),
            key=lambda pr: pr.get("merged_at", ""), default=None)
        if chosen is None:
            return None
        current = _core.gh("api", f"repos/{_core.REPO}/pulls/{chosen['number']}")
        if not isinstance(current, dict) or current.get("number") != chosen["number"]:
            raise MaterialCampaignConflict("La PR canónica cambió durante recovery")
        sha = ((current.get("head") or {}).get("sha") or "").lower()
        if not re.fullmatch(r"[0-9a-f]{40}", sha):
            raise MaterialCampaignConflict("La PR canónica no tiene HEAD exacto")
        return current

    def restore_campaign(self) -> None:
        """Campaign JSON is a hint; GitHub decides the frontier on every restart."""
        if self.proc is not None:
            return
        path = self._campaign_path()
        if not path.exists():
            return
        try:
            row = json.loads(path.read_text(encoding="utf-8"))
        except (OSError, json.JSONDecodeError):
            # A corrupt derived campaign file does not take down Telegram.
            self.campaign_status = "HARD_BLOCKER"
            self.campaign_reason = "Campaign JSON inválido; requiere reconstrucción desde GitHub"
            return
        if (not isinstance(row, dict) or not valid_campaign_proof(self.token, row) or
                row.get("version") != 1 or
                not re.fullmatch(r"[0-9a-f]{32}", str(row.get("campaign_id") or "")) or
                row.get("mode") not in {"run", "work"} or
                row.get("status") not in {"STARTING", "RUNNING", "PAUSED_RECOVERABLE", "HARD_BLOCKER", "PAUSED", "DETACHED"}):
            self.campaign_status = "HARD_BLOCKER"
            self.campaign_reason = "Campaign JSON no autenticado o malformado; no se reanuda automáticamente"
            return
        try:
            self.current_wp = _core.normalize_wp(str(row.get("wp") or ""))
        except _core.ConsoleError:
            self.campaign_status = "HARD_BLOCKER"
            self.campaign_reason = "WP inválido en campaign JSON"
            return
        self.campaign_id = row["campaign_id"]
        self.command_mode = row["mode"]
        self.campaign_status = row["status"]
        self.campaign_reason = str(row.get("reason") or "")
        self.next_wp = row.get("next_wp")
        self.paused = bool(row.get("paused"))
        self.stop_after_wp = bool(row.get("stop_after_wp"))
        saved_decisions = row.get("owner_decisions")
        self.owner_decisions = saved_decisions if isinstance(saved_decisions, dict) else {}
        self.active = True
        self.log_path = Path(row["log_path"]) if row.get("log_path") else None
        self.detached_pid = row.get("pid") if type(row.get("pid")) is int else None
        self.blocked_sha = row.get("blocked_sha") if re.fullmatch(
            r"[0-9a-f]{40}", str(row.get("blocked_sha") or "")) else None
        try:
            pr = self._github_campaign_pr()
        except MaterialCampaignConflict as exc:
            self.campaign_status = "HARD_BLOCKER"
            self.campaign_reason = str(exc)
            self._persist_campaign()
            return
        except _core.ConsoleError as exc:
            if self.campaign_status != "HARD_BLOCKER":
                self.campaign_status = "PAUSED_RECOVERABLE"
                self.campaign_reason = f"GitHub temporalmente inaccesible: {exc}"
            self._persist_campaign()
            return
        if pr is None and self.campaign_status not in {"STARTING", "RUNNING", "PAUSED_RECOVERABLE", "DETACHED"}:
            self.campaign_status = "HARD_BLOCKER"
            self.campaign_reason = "No hay PR canónica inequívoca para recuperar la campaña"
        elif self.campaign_status in {"STARTING", "RUNNING", "DETACHED"} and (
                self._pid_alive(self.detached_pid) or self._controller_busy()):
            self.campaign_status = "DETACHED"
        elif self.campaign_status in {"STARTING", "RUNNING", "DETACHED"}:
            if self.log_path and self.log_path.exists():
                log = self.log_path.read_text(encoding="utf-8", errors="replace")
                disposition = child_disposition(log, 2)
                self.campaign_status = ("HARD_BLOCKER" if disposition == Disposition.HARD
                                        else "PAUSED_RECOVERABLE")
            else:
                self.campaign_status = "PAUSED_RECOVERABLE"
        self._persist_campaign()
        if self.campaign_status == "PAUSED_RECOVERABLE" and not self.paused and not self.stop_after_wp:
            self.launch(self.current_wp, self.command_mode, resume=True)

    @staticmethod
    def _pid_alive(pid: int | None) -> bool:
        if not pid or pid <= 0:
            return False
        try:
            os.kill(pid, 0)
            return True
        except OSError:
            return False

    @staticmethod
    def _controller_busy() -> bool:
        state = Path(os.environ.get("LOCALAPPDATA", str(Path.home()))) / "Arkus" / "Juego2" / "autopilot"
        lock = state / "controller.lock"
        if not lock.exists():
            return False
        with lock.open("a+b") as handle:
            try:
                if os.name == "nt":
                    import msvcrt
                    handle.seek(0)
                    msvcrt.locking(handle.fileno(), msvcrt.LK_NBLCK, 1)
                    handle.seek(0)
                    msvcrt.locking(handle.fileno(), msvcrt.LK_UNLCK, 1)
                else:
                    import fcntl
                    fcntl.flock(handle.fileno(), fcntl.LOCK_EX | fcntl.LOCK_NB)
                    fcntl.flock(handle.fileno(), fcntl.LOCK_UN)
                return False
            except OSError:
                return True

    def _valid_pending_decisions(self) -> list[tuple[dict, str]]:
        """One authoritative universe for /status and Telegram advertisement."""
        if not self.active or not self.campaign_id:
            return []
        valid: list[tuple[dict, str]] = []
        pr_cache: dict[int, tuple[str, list[dict]]] = {}
        for row in self._decision_requests():
            ident = row.get("decision_id", "")
            options = row.get("options") or []
            pr = row.get("pr")
            sha = str(row.get("sha") or "").lower()
            if (row.get("campaign_id") != self.campaign_id or row.get("wp") != self.current_wp or
                    row.get("completed_at") or
                    row.get("abandoned_at") or
                    not isinstance(ident, str) or not re.fullmatch(r"[0-9a-f]{32}", ident) or
                    type(pr) is not int or pr < 1 or
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
            if pr not in pr_cache:
                try:
                    current = _core.gh("api", f"repos/{_core.REPO}/pulls/{pr}")
                    if (current.get("number") != pr or not re.search(
                            rf"(?im)^WP:\s*`?WP-{re.escape(self.current_wp)}`?\s*$",
                            str(current.get("body") or ""))):
                        continue
                    pages = _core.gh("api", "--paginate", "--slurp",
                                     f"repos/{_core.REPO}/issues/{pr}/comments?per_page=100")
                    if not isinstance(pages, list) or any(not isinstance(page, list) for page in pages):
                        continue
                    pr_cache[pr] = (((current.get("head") or {}).get("sha") or "").lower()
                                    if current.get("state") == "open" else "",
                                    [item for page in pages for item in page])
                except (_core.ConsoleError, AttributeError, TypeError):
                    # Infrastructure failure never consumes or invalidates the request.
                    continue
            current_sha, comments = pr_cache[pr]
            if current_sha != sha:
                continue
            attested: set[int] = set()
            contradiction = ""
            for item in comments:
                if (item.get("user") or {}).get("login") != "github-actions[bot]":
                    continue
                body = item.get("body") or ""
                if "ARKUS_LOCAL_AUTOPILOT" not in body:
                    continue
                marker = decision_contract._marker_fields(body)
                if (marker.get("state") != "OWNER_DECISION" or
                        marker.get("campaign id") != self.campaign_id or
                        marker.get("decision id") != ident):
                    continue
                if (marker.get("target sha", "").lower() != sha or
                        marker.get("request digest", "").lower() != digest or
                        marker.get("authority proof") != "supervisor-HMAC-v1"):
                    contradiction = "Identidad o autoridad de OWNER_DECISION contradictoria"
                    break
                try:
                    choice = int(marker.get("choice", ""))
                    selected = _core.owner_auth.decision_selected_digest(options[choice])
                    if choice < 0 or marker.get("selected digest", "").lower() != selected:
                        raise ValueError("selected option mismatch")
                except (ValueError, IndexError, _core.owner_auth.OwnerProofError):
                    contradiction = "OWNER_DECISION atestada con opción inválida"
                    break
                attested.add(choice)
            sent = self.owner_decisions.get(ident)
            if len(attested) > 1 or (attested and isinstance(sent, dict) and
                                     sent.get("choice") not in attested):
                contradiction = "Decisiones owner contradictorias para una solicitud"
            if contradiction:
                self.campaign_status = "HARD_BLOCKER"
                self.campaign_reason = contradiction
                self._persist_campaign()
                continue
            if attested:
                continue
            valid.append((row, digest))
        return valid

    def pending_decision_count(self) -> int:
        return len(self._valid_pending_decisions()) + int(self._pending_continue_valid() is not None)

    def _pending_continue_valid(self) -> dict | None:
        if not self.active or not self.campaign_id:
            return None
        try:
            row = json.loads(self._pending_continue_path().read_text(encoding="utf-8"))
        except (OSError, json.JSONDecodeError):
            return None
        if not isinstance(row, dict):
            return None
        if (row.get("campaign_id") != self.campaign_id or type(row.get("pr")) is not int or
                not re.fullmatch(r"[0-9a-f]{40}", str(row.get("sha") or "")) or
                row.get("fail_count") not in {2, 3}):
            return None
        script = self.root / "scripts" / "telegram_continue_receiver.py"
        spec = importlib.util.spec_from_file_location("arkus_tg_receiver_recovery", script)
        if spec is None or spec.loader is None:
            return None
        receiver = importlib.util.module_from_spec(spec)
        sys.modules[spec.name] = receiver
        spec.loader.exec_module(receiver)
        receiver.fresh_offer = lambda created_at, now=None: True
        try:
            if receiver.validate_current(row["pr"], row["sha"], row["fail_count"]) == "already":
                return None
        except receiver.ReceiverError:
            return None
        return row

    def launch(self, wp: str, mode: str | None = None, *, resume: bool = False) -> None:
        if self.proc is not None:
            raise _core.ConsoleError("Ya hay un WP en ejecución")
        if self._controller_busy():
            raise _core.ConsoleError("Otro controlador WP sigue activo; no se duplica el work plane")
        wp = _core.normalize_wp(wp)
        if self.assets_root is None:
            raise _core.ConsoleError("No hay una carpeta externa de assets configurada")
        mode = mode or self.command_mode
        if mode not in {"run", "work"}:
            raise _core.ConsoleError(f"Modo de campaña inválido: {mode}")

        campaign_id = self.campaign_id if resume else _core.uuid.uuid4().hex
        if not campaign_id:
            raise _core.ConsoleError("La campaña recuperada no tiene identidad durable")
        supervisor_url = self._ensure_ipc()
        logs = self.control_dir / "logs"
        logs.mkdir(parents=True, exist_ok=True)
        log_path = logs / f"{int(_core.time.time() * 1000)}-{_core.uuid.uuid4().hex[:8]}-{wp}.log"
        handle = log_path.open("w", encoding="utf-8")
        cmd = autopilot_command(self.root, self.assets_root, wp, mode)
        self.current_wp = wp
        self.campaign_id = campaign_id
        self.command_mode = mode
        if not resume:
            self.owner_decisions.clear()
        self.active = True
        self.campaign_status = "STARTING"
        self.detached_pid = None
        self.log_path = log_path
        self._persist_campaign()
        try:
            self.proc = _core.subprocess.Popen(
                cmd, cwd=self.root,
                env=_core.child_env(self.control_dir, campaign_id, supervisor_url),
                stdin=_core.subprocess.DEVNULL, stdout=handle,
                stderr=_core.subprocess.STDOUT, text=True,
            )
        except OSError as exc:
            handle.close()
            self.campaign_status = "PAUSED_RECOVERABLE"
            self.campaign_reason = f"No se pudo iniciar el work plane: {exc}"
            self._persist_campaign()
            raise _core.ConsoleError(self.campaign_reason) from exc
        self.log_handle = handle
        self.log_path = log_path
        self.current_wp = wp
        self.campaign_id = campaign_id
        self.active = True
        self.next_wp = None
        self.command_mode = mode
        self.campaign_status = "RUNNING"
        self.campaign_reason = ""
        self.detached_pid = None
        self.blocked_sha = None
        self.boundary_state = None
        self.boundary_wp = None
        self.boundary_sha = None
        self.sent_decisions.clear()
        if not resume:
            self.owner_decisions.clear()
        self._decision_snapshots.clear()
        self._persist_campaign()
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
        code = self.proc.returncode
        if self.log_handle:
            self.log_handle.close()
        text = self.log_path.read_text(encoding="utf-8", errors="replace") if self.log_path else ""
        finished_wp = self.current_wp or "?"
        self.proc = None
        self.log_handle = None
        self.sent_decisions.clear()
        self._decision_snapshots.clear()
        if code != 0:
            disposition = child_disposition(text, code)
            self.campaign_status = ("PAUSED_RECOVERABLE" if disposition in
                                    {Disposition.RECOVER, Disposition.RETRY, Disposition.PAUSE} else "HARD_BLOCKER")
            self.campaign_reason = next((line.partition(": ")[2] for line in reversed(text.splitlines())
                                         if line.startswith("REMOTE_AUTOPILOT_STOP: ")), "Work plane detenido")
            try:
                current = self._github_campaign_pr()
                self.blocked_sha = ((current.get("head") or {}).get("sha") or "").lower() if current else None
            except _core.ConsoleError:
                self.blocked_sha = None
            self._persist_campaign()
            label = "Pausa recuperable" if self.campaign_status == "PAUSED_RECOVERABLE" else "Bloqueo del WP"
            self.send(f"🛑 {label} en {finished_wp}. El supervisor sigue disponible. "
                      "Usa /status, /resume o /abandon; las decisiones válidas permanecen visibles.")
            return

        matches = WORK_BOUNDARY_RE.findall(text) if self.command_mode == "work" else []
        if self.command_mode == "work" and not matches:
            self.active = True
            self.next_wp = None
            self.campaign_status = "HARD_BLOCKER"
            self.campaign_reason = "El work plane terminó sin frontera canónica reconocible"
            self._persist_campaign()
            self.send(
                f"🛑 /work {finished_wp} terminó sin una frontera canónica reconocible. "
                "No se lanzará Reviewer automáticamente."
            )
            return

        if self.command_mode != "work":
            next_wp = _core.parse_next(text)
            if next_wp is None:
                if not (_core.NEXT_RE.search(text) or "No next WP (DOCSYNC_COMPLETE says NONE)" in text):
                    self.campaign_status = "HARD_BLOCKER"
                    self.campaign_reason = "El work plane terminó sin handoff canónico"
                    self._persist_campaign()
                    self.send(f"🛑 {finished_wp} terminó sin handoff canónico. /status sigue disponible.")
                    return
                self.active = False
                self.campaign_status = "COMPLETE"
                self.next_wp = None
                self._clear_local_pending()
                self._persist_campaign()
                self.send(f"🏁 {finished_wp} completado. Campaña finalizada.")
                return
            self.next_wp = next_wp
            if self.stop_after_wp or self.paused:
                self.campaign_status = "PAUSED" if self.paused else "ABANDONED"
                self.active = self.paused
                self._persist_campaign()
                self.send(f"⏸ {finished_wp} completado. Siguiente WP: {next_wp}.")
                return
            self.campaign_status = "COMPLETE"
            self._persist_campaign()
            self.campaign_id = None
            self.launch(next_wp, "run")
            return

        kind, wp, sha, state = matches[-1]
        self.active = False
        self.next_wp = None
        self.campaign_status = "COMPLETE"
        self.boundary_state = kind if kind != "CLOSED" else (state or "CLOSED")
        self.boundary_wp = wp
        self.boundary_sha = None if sha == "NONE" else sha
        self._clear_local_pending()
        self._persist_campaign()
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
        if not self.active and self.campaign_status == "HARD_BLOCKER":
            return f"🛑 HARD_BLOCKER: {self.campaign_reason}. Supervisor activo."
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
        if self.active and self.campaign_status in {"HARD_BLOCKER", "PAUSED_RECOVERABLE", "DETACHED"}:
            return (f"🛑 {self.current_wp}: {self.campaign_status}\n"
                    f"{self.campaign_reason}\n"
                    f"Decisiones pendientes válidas: {self.pending_decision_count()}\n"
                    "Supervisor activo. /resume para reconciliar; /abandon para cerrar la campaña.")
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
        pending_continue = self._pending_continue_valid()
        if pending_continue:
            key = f"continue:{pending_continue['pr']}:{pending_continue['sha']}:{pending_continue['fail_count']}"
            if key not in self.sent_decisions:
                self.send(
                    f"🟠 Decisión pendiente — {self.current_wp}\n"
                    f"PR #{pending_continue['pr']} / SHA {pending_continue['sha']} / FAIL {pending_continue['fail_count']}\n"
                    "El owner puede autorizar la continuación del ciclo aceptado.",
                    {"inline_keyboard": [[{
                        "text": "Continuar reparación",
                        "callback_data": (f"arkus:continue:{pending_continue['pr']}:"
                                          f"{pending_continue['sha']}:{pending_continue['fail_count']}"),
                    }]]},
                )
                self.sent_decisions.add(key)
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
        if not self.active or not self.campaign_id:
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
        prior_choice = self.owner_decisions.get(decision_id)
        if prior_choice and prior_choice.get("choice") != index:
            self.answer_callback(query["id"], "Ya se envió otra opción para esta decisión; espera su atestación.", True)
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
        if not any(item[0].get("decision_id") == decision_id for item in self._valid_pending_decisions()):
            self.answer_callback(query["id"], "La decisión quedó superada por GitHub o no puede verificarse ahora.", True)
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
        self._persist_campaign()
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
            elif self.active and self.campaign_status in {"PAUSED_RECOVERABLE", "HARD_BLOCKER", "DETACHED"}:
                if self.campaign_status == "DETACHED" and (
                        self._pid_alive(self.detached_pid) or self._controller_busy()):
                    self.send("⏳ El work plane anterior sigue vivo; se espera sin crear otro controlador.")
                    return
                current = self._github_campaign_pr()
                if current is None and self.campaign_status == "HARD_BLOCKER":
                    raise _core.ConsoleError("No hay PR canónica inequívoca; la campaña sigue detenida")
                sha = ((current.get("head") or {}).get("sha") or "").lower() if current else ""
                if (self.campaign_status == "HARD_BLOCKER" and not current.get("merged") and
                        (not self.blocked_sha or sha == self.blocked_sha)):
                    self.send("🛑 El bloqueo material permanece en la misma SHA. "
                              "El supervisor sigue activo; resuelve la contradicción antes de reanudar.")
                    return
                self.paused = False
                self.stop_after_wp = False
                self.launch(self.current_wp, self.command_mode, resume=True)
            elif self.active and self.paused and self.next_wp:
                self.paused = False
                self.launch(self.next_wp, self.command_mode)
            else:
                raise _core.ConsoleError("No hay una campaña pausada que reanudar")
        elif command in {"/stop", "/abandon"}:
            if not self.active:
                raise _core.ConsoleError("No hay campaña activa")
            if self.proc is None:
                self.active = False
                self.paused = False
                self.next_wp = None
                self.campaign_status = "ABANDONED"
                self._persist_campaign()
                self._clear_local_pending()
                self.campaign_id = None
                self.send("⏹ Campaña abandonada. El supervisor sigue disponible.")
            else:
                self.stop_after_wp = True
                self.paused = False
                self._persist_campaign()
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
                "/resume — reconciliar y continuar si el bloqueo está resuelto\n"
                "/abandon — abandonar campaña detenida; en ejecución, parar en la siguiente frontera\n"
                "/stop — terminar de forma segura\n"
                "/note texto — instrucción autenticada para el próximo Worker/repair fresco"
            )
        else:
            raise _core.ConsoleError("Comando no reconocido. Usa /help")

    def run(self) -> None:
        """The owner control plane outlives every work-plane exception."""
        webhook = _core.telegram(self.token, "getWebhookInfo", {})
        if not isinstance(webhook, dict) or webhook.get("url"):
            raise _core.ConsoleError("El bot tiene webhook; getUpdates requiere webhook vacío")
        self.control_dir.mkdir(parents=True, exist_ok=True)
        self._ensure_ipc()
        self.discard_offline_backlog()
        try:
            self.restore_campaign()
        except Exception as exc:
            print(f"REMOTE_CONSOLE_RECOVERY_PAUSED: {type(exc).__name__}: {exc}", file=sys.stderr, flush=True)
            self.campaign_status = "PAUSED_RECOVERABLE"
            self.campaign_reason = str(exc)
        try:
            self.send("🟢 Arkus Telegram console online. Usa /status para consultar la campaña y /help para controles.")
        except _core.ConsoleError:
            pass
        try:
            while True:
                try:
                    self.check_child()
                    if (self.campaign_status == "DETACHED" and
                            not self._pid_alive(self.detached_pid) and not self._controller_busy()):
                        self.campaign_status = "PAUSED_RECOVERABLE"
                        self._persist_campaign()
                        self.launch(self.current_wp, self.command_mode, resume=True)
                    self.advertise_decisions()
                    self.poll_once()
                except Exception as exc:
                    # Keep /status and owner controls reachable after a work-plane
                    # failure or one transient Telegram/GitHub operation.
                    print(f"REMOTE_CONSOLE_RECOVERABLE: {type(exc).__name__}: {exc}", file=sys.stderr, flush=True)
                    _core.time.sleep(2)
        finally:
            self._close_ipc()

    def poll_once(self) -> None:
        updates = self._get_updates(timeout=20, offset=self.offset)
        for update in updates:
            try:
                self.handle_update(update)
            except Exception as exc:
                print(f"REMOTE_CONSOLE_UPDATE_ERROR: {type(exc).__name__}: {exc}", file=sys.stderr, flush=True)
            finally:
                self.offset = max(self.offset or 0, int(update["update_id"]) + 1)


_core.RemoteConsole = RemoteConsole


def main() -> int:
    return _core.main()


if __name__ == "__main__":
    raise SystemExit(main())
