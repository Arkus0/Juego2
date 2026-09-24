param([switch]$ResetConfig)

$ErrorActionPreference = "Stop"
$Host.UI.RawUI.WindowTitle = "Arkus Telegram Console"

function Fail([string]$Message, [int]$Code = 2) {
    Write-Host ""
    Write-Host "ERROR: $Message" -ForegroundColor Red
    exit $Code
}

function Plain([Security.SecureString]$Secure) {
    $ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Secure)
    try { return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) }
}

$config = Join-Path $env:LOCALAPPDATA "Arkus\Juego2\console-launcher"
$tokenFile = Join-Path $config "telegram-token.dpapi"
$chatFile = Join-Path $config "telegram-chat-id.txt"
$repoFile = Join-Path $config "repo-path.txt"
New-Item -ItemType Directory -Force -Path $config | Out-Null

if ($ResetConfig) {
    Remove-Item -Force -ErrorAction SilentlyContinue $tokenFile,$chatFile,$repoFile
    Write-Host "Configuracion local eliminada."
}

Write-Host ""
Write-Host "=== Arkus Telegram Console ===" -ForegroundColor Cyan

$repo = $null
if (Test-Path (Join-Path $PSScriptRoot "scripts\local_wp_remote_console.py")) {
    $repo = (Resolve-Path $PSScriptRoot).Path
} elseif (Test-Path $repoFile) {
    $saved = (Get-Content $repoFile -Raw).Trim()
    if ($saved -and (Test-Path (Join-Path $saved "scripts\local_wp_remote_console.py"))) {
        $repo = $saved
    }
}
if (-not $repo) {
    $entered = (Read-Host "Ruta local de Juego2").Trim().Trim('"')
    if (-not (Test-Path (Join-Path $entered "scripts\local_wp_remote_console.py"))) {
        Fail "Esa ruta no parece un checkout actualizado de Juego2."
    }
    $repo = (Resolve-Path $entered).Path
    Set-Content $repoFile $repo -Encoding UTF8
}
Write-Host "Repo: $repo"

foreach ($tool in @("git.exe","gh.exe","codex.exe")) {
    if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) { Fail "No encuentro $tool en PATH." }
}

$py = Get-Command py.exe -ErrorAction SilentlyContinue
$python = Get-Command python.exe -ErrorAction SilentlyContinue
if ($py) {
    $pythonExe = $py.Source
    $pythonPrefix = @("-3")
} elseif ($python) {
    $pythonExe = $python.Source
    $pythonPrefix = @()
} else {
    Fail "No encuentro Python 3."
}

& gh auth status *> $null
if ($LASTEXITCODE -ne 0) { Fail "GitHub CLI no esta autenticado. Ejecuta: gh auth login" }

& codex --version *> $null
if ($LASTEXITCODE -ne 0) { Fail "Codex CLI no responde correctamente." }

if (-not (Test-Path $tokenFile)) {
    Write-Host ""
    Write-Host "Primera ejecucion: pega TELEGRAM_BOT_TOKEN. No se mostrara." -ForegroundColor Yellow
    $secureToken = Read-Host "TELEGRAM_BOT_TOKEN" -AsSecureString
    Set-Content $tokenFile (ConvertFrom-SecureString $secureToken) -Encoding ASCII
}
try {
    $secureToken = ConvertTo-SecureString ((Get-Content $tokenFile -Raw).Trim())
    $token = Plain $secureToken
} catch {
    Fail "No puedo descifrar el token local. Ejecuta este PS1 con -ResetConfig."
}
if (-not $token) { Fail "El token local esta vacio." }

if (-not (Test-Path $chatFile)) {
    $chat = (Read-Host "TELEGRAM_CHAT_ID (numero)").Trim()
    if ($chat -notmatch '^\d+$') { Fail "TELEGRAM_CHAT_ID debe ser numerico." }
    Set-Content $chatFile $chat -Encoding ASCII
}
$chat = (Get-Content $chatFile -Raw).Trim()
if ($chat -notmatch '^\d+$') { Fail "TELEGRAM_CHAT_ID guardado invalido." }

Write-Host ""
Write-Host "=== Actualizando main ===" -ForegroundColor Cyan
$dirty = (& git -C $repo status --porcelain)
if ($LASTEXITCODE -ne 0) { Fail "git status fallo." }
if ($dirty) {
    Write-Host $dirty
    Fail "Hay cambios locales. No toco el repo hasta que este limpio."
}

& git -C $repo fetch origin main
if ($LASTEXITCODE -ne 0) { Fail "git fetch fallo." }
& git -C $repo switch main
if ($LASTEXITCODE -ne 0) { Fail "No pude cambiar a main." }
& git -C $repo pull --ff-only origin main
if ($LASTEXITCODE -ne 0) { Fail "git pull --ff-only fallo." }

Write-Host ""
Write-Host "=== Supervisor online ===" -ForegroundColor Green
Write-Host "Deja esta ventana abierta."
Write-Host "En Telegram: /help, /status y /run <WP>."
Write-Host "Para apagarlo: Ctrl+C aqui."
Write-Host ""

$env:TELEGRAM_BOT_TOKEN = $token
$env:TELEGRAM_CHAT_ID = $chat
try {
    Push-Location $repo
    $args = @()
    $args += $pythonPrefix
    $args += @("scripts\local_wp_remote_console.py","--root",".")
    & $pythonExe @args
    $code = $LASTEXITCODE
} finally {
    Pop-Location
    Remove-Item Env:\TELEGRAM_BOT_TOKEN -ErrorAction SilentlyContinue
    Remove-Item Env:\TELEGRAM_CHAT_ID -ErrorAction SilentlyContinue
    $token = $null
}
exit $code
