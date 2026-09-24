# Arkus Telegram Console — Windows launcher

Convenience launcher for the accepted opt-in Telegram owner console.

## Use

1. Keep `Arkus-Console.cmd` and `Arkus-Console.ps1` together.
2. Double-click `Arkus-Console.cmd`.
3. On first run, provide the local `Juego2` checkout path if needed, the external `Juego2-Assets` path if it is not the checkout's sibling, `TELEGRAM_BOT_TOKEN`, and numeric `TELEGRAM_CHAT_ID`.
4. The bot token is stored encrypted with Windows DPAPI for the current Windows user under `%LOCALAPPDATA%\Arkus\Juego2\console-launcher`; the repo and assets paths are stored there as ordinary local configuration.
5. Later runs verify Git/GitHub CLI/Codex/Python, require the external assets directory to remain readable, refuse to modify a dirty checkout, fast-forward `main`, and launch `scripts/local_wp_remote_console.py` with both roots.
6. Keep the console window open. Use Telegram commands such as `/help`, `/status`, `/run <WP>`, `/pause`, `/resume`, `/stop`, and `/note ...`.
7. Stop the local supervisor with `Ctrl+C` in its console window.

## Important

GitHub Actions secrets cannot be read back from GitHub. Even if `TELEGRAM_BOT_TOKEN` and `TELEGRAM_CHAT_ID` already exist as repository secrets, the local supervisor needs them once on the workstation.

The launcher contains no embedded Telegram secret and does not change the accepted owner-console authority model. It is local convenience tooling only.

Every fresh Codex role receives the external assets path through `codex exec --add-dir` and explicit prompt context. The source directory remains external input rather than repository or canonical state, and upstream bytes are treated as read-only unless an exact authoritative workpack permits a bounded change.
