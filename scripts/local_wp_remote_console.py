#!/usr/bin/env python3
"""Hardened owner-console facade.

The accepted transport/command implementation from the previous candidate is
kept byte-for-byte in local_wp_remote_console_core.py. This facade changes only
soft owner decisions: the request shown to Telegram is snapshotted, the owner
click is HMAC-bound to that exact request + PR/SHA, and GitHub Actions must
attest the choice before a Worker may consume it.
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


class RemoteConsole(_core.RemoteConsole):
    def __init__(self, root: Path, control_dir: Path, token: str, chat_id: int) -> None:
        super().__init__(root, control_dir, token, chat_id)
        self._decision_snapshots: dict[str, dict] = {}

    def launch(self, wp: str) -> None:
        self._decision_snapshots.clear()
        super().launch(wp)

    def advertise_decisions(self) -> None:
        if self.proc is None or not self.campaign_id:
            return
        for row in self._decision_requests():
            ident = row.get("decision_id", "")
            if ident in self.sent_decisions or row.get("campaign_id") != self.campaign_id:
                continue
            options = row.get("options") or []
            pr = row.get("pr")
            sha = str(row.get("sha") or "").lower()
            if (not re.fullmatch(r"[0-9a-f]{32}", ident) or type(pr) is not int or pr < 1 or
                    not re.fullmatch(r"[0-9a-f]{40}", sha) or not isinstance(options, list) or
                    not 2 <= len(options) <= 3 or any(not isinstance(item, str) for item in options)):
                continue
            try:
                request_digest = _core.owner_auth.decision_request_digest(
                    self.campaign_id, ident, str(row.get("wp") or ""), pr, sha,
                    str(row.get("question") or ""), str(row.get("detail") or ""), options)
            except _core.owner_auth.OwnerProofError:
                continue
            if row.get("request_digest") != request_digest:
                continue

            snapshot = {
                "campaign_id": self.campaign_id,
                "decision_id": ident,
                "request_digest": request_digest,
                "wp": row.get("wp"),
                "pr": pr,
                "sha": sha,
                "question": str(row.get("question") or ""),
                "detail": str(row.get("detail") or ""),
                "options": list(options),
            }
            self._decision_snapshots[ident] = snapshot
            keyboard = []
            for index, option in enumerate(options):
                callback = f"arkus:decision:{ident}:{index}"
                keyboard.append([{"text": f"{index + 1}️⃣ {option[:48]}", "callback_data": callback}])
            subject = row.get("wp") or self.current_wp or "WP"
            detail = str(row.get("detail") or "").strip()
            message = f"🟠 Decisión del owner — {subject}\n{row.get('question', '')}"
            if detail:
                message += f"\n\n{detail[:900]}"
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
            current_digest = _core.owner_auth.decision_request_digest(
                self.campaign_id, decision_id, str(row.get("wp") or ""), row.get("pr"),
                str(row.get("sha") or ""), str(row.get("question") or ""),
                str(row.get("detail") or ""), row.get("options") or [])
        except (OSError, json.JSONDecodeError, _core.owner_auth.OwnerProofError, TypeError):
            self.answer_callback(query["id"], "Solicitud local inválida.", True)
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


_core.RemoteConsole = RemoteConsole


def main() -> int:
    return _core.main()


if __name__ == "__main__":
    raise SystemExit(main())
