param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("configure", "batch", "editmode")]
    [string]$Action,

    [string]$UnityEditorPath = $env:UNITY_EDITOR_PATH,

    [string]$OutputPath,

    [string]$LogPath
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($UnityEditorPath)) {
    throw "UNITY_EDITOR_PATH_REQUIRED: pass -UnityEditorPath or set UNITY_EDITOR_PATH."
}

$editor = [System.IO.Path]::GetFullPath($UnityEditorPath)
if (-not (Test-Path -LiteralPath $editor -PathType Leaf)) {
    throw "UNITY_EDITOR_NOT_FOUND: $editor"
}

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$projectPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot "Unity/ArkusUnity"))

if (-not (Test-Path -LiteralPath (Join-Path $projectPath "ProjectSettings/ProjectVersion.txt") -PathType Leaf)) {
    throw "H1_PROJECT_NOT_FOUND: $projectPath"
}

if ([string]::IsNullOrWhiteSpace($LogPath)) {
    $LogPath = Join-Path $repoRoot "artifacts/h1-02/$Action.log"
}
$LogPath = [System.IO.Path]::GetFullPath($LogPath)
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $LogPath) | Out-Null

$common = @(
    "-batchmode",
    "-nographics",
    "-projectPath", $projectPath,
    "-logFile", $LogPath
)

switch ($Action) {
    "configure" {
        if ([string]::IsNullOrWhiteSpace($OutputPath)) {
            $OutputPath = Join-Path $repoRoot "Docs/evidence/WP-H1-02/effective-inventory.json"
        }
        $OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $OutputPath) | Out-Null

        $arguments = $common + @(
            "-executeMethod", "Arkus.H1.Editor.H1Bootstrap.ConfigureBaseline",
            "-arkus-h1-output", $OutputPath,
            "-quit"
        )
    }
    "batch" {
        if ([string]::IsNullOrWhiteSpace($OutputPath)) {
            $OutputPath = Join-Path $repoRoot "artifacts/h1-02/batch-inventory.json"
        }
        $OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $OutputPath) | Out-Null

        $arguments = $common + @(
            "-executeMethod", "Arkus.H1.Editor.H1Batch.Run",
            "-arkus-h1-output", $OutputPath,
            "-quit"
        )
    }
    "editmode" {
        if ([string]::IsNullOrWhiteSpace($OutputPath)) {
            $OutputPath = Join-Path $repoRoot "artifacts/h1-02/editmode-results.xml"
        }
        $OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $OutputPath) | Out-Null

        $arguments = $common + @(
            "-runTests",
            "-testPlatform", "EditMode",
            "-testResults", $OutputPath,
            "-quit"
        )
    }
}

function ConvertTo-UnityProcessArgument {
    param([Parameter(Mandatory = $true)][string]$Value)

    if ($Value.Contains('"')) {
        throw "UNITY_ARGUMENT_QUOTE_UNSUPPORTED: $Value"
    }

    if ($Value -match '\s') {
        return '"' + $Value + '"'
    }

    return $Value
}

Write-Host "H1-02 project: $projectPath"
Write-Host "H1-02 editor:  $editor"
Write-Host "H1-02 action:  $Action"

# Unity.exe is a Windows GUI-subsystem executable. Invoking it directly from
# Windows PowerShell can return control before the Editor process has exited,
# leaving $LASTEXITCODE unset. Start-Process returns the exact process object;
# waiting on that object avoids Start-Process -Wait's process-tree semantics
# while still binding success to Unity's real process lifetime and exit code.
$processArguments = ($arguments | ForEach-Object {
    ConvertTo-UnityProcessArgument ([string]$_)
}) -join ' '

$process = Start-Process -FilePath $editor -ArgumentList $processArguments -PassThru
if ($null -eq $process) {
    throw "UNITY_PROCESS_START_FAILED: action=$Action editor=$editor"
}

$process.WaitForExit()
$process.Refresh()
$exitCode = $process.ExitCode
Write-Host "H1-02 Unity exit: $exitCode"

if ($exitCode -ne 0) {
    throw "UNITY_BATCH_FAILED: action=$Action exit=$exitCode log=$LogPath"
}

Write-Host "H1_02_UNITY_${Action}_GREEN"
