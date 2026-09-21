param(
    [switch]$RequireUnity
)

$ErrorActionPreference = 'Stop'
$failures = New-Object System.Collections.Generic.List[string]
$warnings = New-Object System.Collections.Generic.List[string]

function Pass([string]$message) { Write-Host "[PASS] $message" }
function Warn([string]$message) { Write-Host "[WARN] $message"; $warnings.Add($message) }
function Fail([string]$message) { Write-Host "[FAIL] $message"; $failures.Add($message) }

function Has-Command([string]$name) {
    return $null -ne (Get-Command $name -ErrorAction SilentlyContinue)
}

Write-Host "Juego2 local agent readiness check"
Write-Host "=================================="

if (-not (Has-Command 'git')) {
    Fail 'git is not installed or not on PATH.'
} else {
    Pass "git: $((git --version) -join ' ')"
}

if (Has-Command 'git') {
    try {
        $root = (git rev-parse --show-toplevel 2>$null).Trim()
        if (-not $root) { throw 'not a git repository' }
        Pass "Git repository root: $root"

        $origin = (git remote get-url origin 2>$null).Trim()
        if ($origin -match '(^|[:/])Arkus0/Juego2(\.git)?$') {
            Pass "origin points to Arkus0/Juego2: $origin"
        } else {
            Fail "origin is not Arkus0/Juego2: $origin"
        }

        $dirty = @(git status --porcelain)
        if ($dirty.Count -eq 0) {
            Pass 'working tree is clean'
        } else {
            Warn "working tree has $($dirty.Count) uncommitted/untracked entries; a Worker should inspect them before taking ownership"
        }
    } catch {
        Fail 'run this script from inside a Juego2 checkout.'
    }
}

if (-not (Has-Command 'gh')) {
    Fail 'GitHub CLI (gh) is missing. Install with: winget install --id GitHub.cli --source winget'
} else {
    Pass "gh: $((gh --version | Select-Object -First 1) -join ' ')"

    & gh auth status --hostname github.com *> $null
    if ($LASTEXITCODE -eq 0) {
        Pass 'gh is authenticated to github.com'
    } else {
        Fail 'gh is not authenticated. Run: gh auth login'
    }

    if ($failures.Count -eq 0 -or (Has-Command 'git')) {
        try {
            $repoJson = gh repo view --json nameWithOwner 2>$null | ConvertFrom-Json
            if ($repoJson.nameWithOwner -eq 'Arkus0/Juego2') {
                Pass 'gh resolves this checkout to Arkus0/Juego2'
            } else {
                Fail "gh resolved repository '$($repoJson.nameWithOwner)' instead of Arkus0/Juego2"
            }
        } catch {
            Fail 'gh could not resolve the current repository. Check authentication and origin.'
        }
    }
}

if (Has-Command 'dotnet') {
    Pass "dotnet: $((dotnet --version) -join ' ')"
} else {
    Warn 'dotnet is not on PATH. Some WPs may still be blocked until the repository-pinned SDK/runtime is available.'
}

if (Has-Command 'codex') {
    try {
        $codexVersion = (codex --version 2>$null | Select-Object -First 1)
        Pass "Codex CLI: $codexVersion"
    } catch {
        Pass 'Codex CLI command is present'
    }
} else {
    Warn 'codex command was not found on PATH. If you use the Codex desktop/local UI instead of CLI this may be intentional.'
}

$unityHubCandidates = @(
    "$env:ProgramFiles\Unity Hub\Unity Hub.exe",
    "$env:LOCALAPPDATA\Programs\Unity Hub\Unity Hub.exe"
) | Where-Object { $_ -and (Test-Path $_) }

if ($unityHubCandidates.Count -gt 0) {
    Pass "Unity Hub found: $($unityHubCandidates[0])"
} else {
    if ($RequireUnity) { Fail 'Unity Hub was not found in standard Windows locations.' }
    else { Warn 'Unity Hub was not found in standard Windows locations. H1-02 may need it before local Unity evidence can run.' }
}

$editorRoot = "$env:ProgramFiles\Unity\Hub\Editor"
$editors = @()
if (Test-Path $editorRoot) {
    $editors = @(Get-ChildItem -Path $editorRoot -Directory -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Name)
}

if ($editors.Count -gt 0) {
    Pass "Installed Unity editors: $($editors -join ', ')"
} else {
    if ($RequireUnity) { Fail 'No Unity editor installations were found under the standard Unity Hub editor directory.' }
    else { Warn 'No Unity editor installation was found yet. H1-02 owns selecting/pinning the exact Unity 6.3 LTS patch; do not create an ad-hoc project to compensate.' }
}

Write-Host ""
Write-Host "Summary"
Write-Host "-------"
Write-Host "Failures: $($failures.Count)"
Write-Host "Warnings: $($warnings.Count)"

if ($failures.Count -gt 0) {
    Write-Host ""
    Write-Host 'Required fixes:'
    foreach ($item in $failures) { Write-Host " - $item" }
    exit 1
}

Write-Host ""
Write-Host 'READY: local GitHub/repository prerequisites are usable.'
if (-not $RequireUnity) {
    Write-Host 'For a Unity-required WP, rerun with -RequireUnity after the exact editor requirement is known/installed.'
}
exit 0
