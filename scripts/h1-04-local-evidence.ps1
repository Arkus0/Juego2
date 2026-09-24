param([Parameter(Mandatory=$true)][string]$AssetsRoot, [string]$ExpectedSha = '')

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location -LiteralPath $repoRoot
$actualSha = (git rev-parse HEAD).Trim()
if ($ExpectedSha -and $ExpectedSha -ne $actualSha) { throw "Candidate SHA mismatch: expected $ExpectedSha, observed $actualSha" }
if ($actualSha -notmatch '^[0-9a-f]{40}$') { throw 'Candidate SHA is not a 40-character Git commit identity' }
if (git status --porcelain --untracked-files=all) { throw 'Candidate is not clean before local H1-04 evidence execution' }

& (Join-Path $PSScriptRoot 'h1-04-import-source.ps1') -AssetsRoot $AssetsRoot -VerifyOnly
if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) { throw 'External Source fingerprint verification failed' }

$unity = 'C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe'
if (-not (Test-Path -LiteralPath $unity -PathType Leaf)) { throw "Pinned Unity Editor is missing: $unity" }
$version = (Get-Item -LiteralPath $unity).VersionInfo.ProductVersion
if ($version -notlike '*6000.3.24f1_4e7b9b5b6244*') { throw "Effective Unity revision mismatch: $version" }

$scratch = Join-Path $repoRoot '.h1-04-effective'
New-Item -ItemType Directory -Force -Path $scratch | Out-Null
$runDir = Join-Path $scratch ('exact-' + $actualSha.Substring(0, 12) + '-' + [guid]::NewGuid().ToString('N').Substring(0, 6))
New-Item -ItemType Directory -Path $runDir | Out-Null
$project = Join-Path $repoRoot 'Unity/ArkusUnity'

function Run-Unity([string]$projectPath, [string]$method, [string]$outputPath, [string]$logPath) {
    $info = [Diagnostics.ProcessStartInfo]::new($unity)
    $info.UseShellExecute = $false
    $info.CreateNoWindow = $true
    foreach ($argument in @('-batchmode','-nographics','-quit','-projectPath',$projectPath,
        '-executeMethod',$method,'-arkus-h1-output',$outputPath,'-logFile',$logPath)) {
        [void]$info.ArgumentList.Add($argument)
    }
    $process = [Diagnostics.Process]::Start($info)
    if (-not $process.WaitForExit(180000)) {
        $process.Kill($true)
        throw "Unity proof timed out: $method"
    }
    if ($process.ExitCode -ne 0) {
        Get-Content -LiteralPath $logPath -Tail 80 | Write-Output
        throw "Unity proof failed: $method (exit $($process.ExitCode))"
    }
    if (-not (Test-Path -LiteralPath $outputPath -PathType Leaf)) { throw "Unity proof omitted output: $outputPath" }
}

$observedInventory = Join-Path $runDir 'inventory.json'
$observedMutation = Join-Path $runDir 'mutation.json'
$observedPublic = Join-Path $runDir 'public.json'
$observedSummary = Join-Path $runDir 'summary.json'
Run-Unity $project 'Arkus.H1.Editor.H1CatalogueInventory.WriteInventory' $observedInventory (Join-Path $runDir 'inventory.log')

$disposable = Join-Path $runDir 'disposable-project'
New-Item -ItemType Directory -Path $disposable | Out-Null
foreach ($part in @('Assets','Packages','ProjectSettings')) {
    Copy-Item -LiteralPath (Join-Path $project $part) -Destination (Join-Path $disposable $part) -Recurse
}
Run-Unity $disposable 'Arkus.H1.Editor.H1CatalogueMutationProof.Run' $observedMutation (Join-Path $runDir 'mutation.log')

dotnet restore Juego2.sln --locked-mode | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Locked .NET restore failed before public conformance' }
dotnet build Juego2.sln -c Release --no-restore -m:1 --disable-build-servers --verbosity quiet | Out-Null
if ($LASTEXITCODE -ne 0) { throw '.NET build failed before public conformance' }
python scripts/h1-04-public-conformance.py --output $observedPublic
if ($LASTEXITCODE -ne 0) { throw 'Real JSONL/MCP catalogue conformance failed' }
python scripts/h1-04-evidence-summary.py --inventory $observedInventory --mutation $observedMutation --public $observedPublic --output $observedSummary
if ($LASTEXITCODE -ne 0) { throw 'Effective evidence normalization failed' }
python scripts/h1-04-evidence-check.py --inventory $observedInventory --mutation $observedMutation --public $observedPublic --summary $observedSummary
if ($LASTEXITCODE -ne 0) { throw 'Effective evidence differs from committed reviewed evidence' }

if (git status --porcelain --untracked-files=all) { throw 'Candidate is not clean after local H1-04 evidence execution' }
$summary = Get-Content -LiteralPath $observedSummary -Raw | ConvertFrom-Json
Write-Output "EXECUTION_RECEIPT_V1"
Write-Output "WP: WP-H1-04"
Write-Output "Candidate SHA: $actualSha"
Write-Output "Executor role: WORKER"
Write-Output "Execution environment: owner workstation, physical local Unity"
Write-Output "OS: windows-x64"
Write-Output "Toolchain: Unity 6000.3.24f1 (4e7b9b5b6244); .NET $(dotnet --version)"
Write-Output "Canonical command: scripts/h1-04-local-evidence.ps1 -AssetsRoot <owner-configured> -ExpectedSha $actualSha"
Write-Output "Candidate clean before: YES"
Write-Output "Candidate clean after: YES"
Write-Output "Required gates: external-source-fingerprint=GREEN; AssetDatabase-inventory=GREEN; mapping-and-adoption=GREEN; disposable-move-copy-delete-reimport=GREEN; JSONL-MCP-public-conformance=GREEN; committed-evidence-equality=GREEN"
Write-Output "Result: GREEN"
Write-Output "Evidence: Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json; Docs/evidence/WP-H1-04/EFFECTIVE_MUTATION.json; Docs/evidence/WP-H1-04/PUBLIC_CONFORMANCE.json; Docs/evidence/WP-H1-04/EFFECTIVE_VALIDATION.json"
Write-Output "Catalogue fingerprint: $($summary.catalogueFingerprint)"
