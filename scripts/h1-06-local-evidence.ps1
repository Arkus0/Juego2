param([Parameter(Mandatory=$true)][string]$AssetsRoot, [string]$ExpectedSha = '')

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location -LiteralPath $repoRoot
$actualSha = (git rev-parse HEAD).Trim()
if ($actualSha -notmatch '^[0-9a-f]{40}$') { throw 'Invalid candidate SHA' }
if ($ExpectedSha -and $ExpectedSha -ne $actualSha) { throw "Candidate SHA mismatch: $ExpectedSha != $actualSha" }
if (git status --porcelain --untracked-files=all) { throw 'H1-06 candidate is not clean before local evidence' }

$sdk = (Get-Content global.json -Raw | ConvertFrom-Json).sdk.version
$selectedSdk = (dotnet --version).Trim()
if ($sdk -ne $selectedSdk) { throw "Pinned .NET SDK mismatch: $selectedSdk != $sdk" }

$unity = 'C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe'
if (-not (Test-Path -LiteralPath $unity -PathType Leaf)) { throw "Pinned Unity Editor missing: $unity" }
$version = (Get-Item -LiteralPath $unity).VersionInfo.ProductVersion
if ($version -notlike '*6000.3.24f1_4e7b9b5b6244*') { throw "Pinned Unity revision mismatch: $version" }

# Reuse the accepted H1-04 importer: it verifies owner Source pins and copies only the
# already approved project-local slice. H1-06 never broadens source adoption.
Add-Type -AssemblyName System.IO.Compression.FileSystem
& (Join-Path $PSScriptRoot 'h1-04-import-source.ps1') -AssetsRoot $AssetsRoot
if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) { throw 'Approved Source import failed' }

$project = Join-Path $repoRoot 'Unity/ArkusUnity'
$scratch = Join-Path $project 'Library/Arkus/H1PrefabRealization'
New-Item -ItemType Directory -Force -Path $scratch | Out-Null

function Run-Unity([string]$method, [string]$logPath, [string]$outputPath = '') {
    $arguments = @('-batchmode','-nographics','-quit','-projectPath',$project,'-executeMethod',$method)
    if ($outputPath) { $arguments += @('-arkus-h1-output',$outputPath) }
    $arguments += @('-logFile',$logPath)
    $process = Start-Process -FilePath $unity -ArgumentList $arguments -PassThru -WindowStyle Hidden
    # Unity's Roslyn compiler server may outlive the Editor. Wait for the Editor itself.
    $process.WaitForExit()
    if ($process.ExitCode -ne 0) {
        Get-Content -LiteralPath $logPath -Tail 80 | Write-Output
        throw "Unity execution failed: $method (exit $($process.ExitCode))"
    }
}

# Import/observe the accepted source through the exact effective Unity before the public proof.
Run-Unity 'Arkus.H1.Editor.H1CatalogueInventory.CreateProofScene' (Join-Path $scratch 'prepare-catalogue.log')
$inventory = Join-Path $scratch 'inventory.json'
Run-Unity 'Arkus.H1.Editor.H1CatalogueInventory.WriteInventory' (Join-Path $scratch 'inventory.log') $inventory
$effectiveInventory = Get-Content -LiteralPath $inventory -Raw | ConvertFrom-Json
if ($effectiveInventory.projectIdentity -ne 'arkus.unity-project@1:ArkusUnity' -or
    $effectiveInventory.editorVersion -ne '6000.3.24f1') {
    throw 'Effective Unity project/editor identity mismatch'
}

# A harness-only nested prefab fixture exercises the relationship collector through
# save/reload, then confirms that flattening the nested child turns the guard red.
$nested = Join-Path $scratch 'nested-conformance.json'
Run-Unity 'Arkus.H1.Editor.H1PrefabNestedConformance.Run' (Join-Path $scratch 'nested.log') $nested
$nestedResult = Get-Content -LiteralPath $nested -Raw | ConvertFrom-Json
if ($nestedResult.schemaId -ne 'arkus.h1-06-nested-prefab-conformance@1' -or
    $nestedResult.result -ne 'GREEN' -or
    $nestedResult.positive -ne 'two-same-name-nested-prefab-links-preserved-after-save-reload' -or
    $nestedResult.productPath -ne 'ObserveRealization:nested-prefab-multiplicity-and-digest' -or
    $nestedResult.negative -ne 'one-of-two-same-name-siblings-removed:projection.prefab-nested-lineage-missing') {
    throw 'Nested prefab conformance result is incomplete'
}

# Exploratory shape probe reads the exact already-approved archive only. It does not import
# or adopt the additional wall/roof/door/window/prop candidates it observes.
$archive = Join-Path $AssetsRoot 'Medieval Village/Engine Projects/Medieval Village MegaKit[Unity URP].zip'
$contentShape = Join-Path $scratch 'content-shape.json'
python scripts/h1-06-content-shape-probe.py --archive $archive --output $contentShape
if ($LASTEXITCODE -ne 0) { throw 'H1-06 content-shape probe failed' }

dotnet restore Juego2.sln --locked-mode --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw 'Locked restore failed' }
dotnet build Juego2.sln -c Release --no-restore -m:1 --disable-build-servers --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw 'Release build failed' }
dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj -c Release --no-build --no-restore --filter 'FullyQualifiedName~H1ManagedScenePlanTests' --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw 'Focused H1-06 plan/diagnostic/remap tests failed' }

$public = Join-Path $scratch 'public-conformance.json'
python scripts/h1-06-public-conformance.py --output $public
if ($LASTEXITCODE -ne 0) { throw 'Effective H1-06 prefab realization conformance failed' }

# Source immutability is checked both inside the public proof and again against the owner Source pins.
& (Join-Path $PSScriptRoot 'h1-04-import-source.ps1') -AssetsRoot $AssetsRoot -VerifyOnly
if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) { throw 'External Source changed during H1-06 evidence' }
if (git status --porcelain --untracked-files=all) { throw 'H1-06 candidate changed during local evidence' }

Write-Output 'EXECUTION_RECEIPT_V1'
Write-Output 'WP: WP-H1-06'
Write-Output "Candidate SHA: $actualSha"
Write-Output 'Executor role: WORKER'
Write-Output 'Execution environment: owner workstation, physical local Unity'
Write-Output 'OS: windows-x64'
Write-Output "Toolchain: Unity 6000.3.24f1 (4e7b9b5b6244); .NET $selectedSdk"
Write-Output 'Project identity: arkus.unity-project@1:ArkusUnity'
Write-Output "Package manifest SHA-256: $((Get-FileHash -LiteralPath (Join-Path $project 'Packages/manifest.json') -Algorithm SHA256).Hash.ToLowerInvariant())"
Write-Output "Package lock SHA-256: $((Get-FileHash -LiteralPath (Join-Path $project 'Packages/packages-lock.json') -Algorithm SHA256).Hash.ToLowerInvariant())"
Write-Output "Canonical command: scripts/h1-06-local-evidence.ps1 -AssetsRoot <owner-configured> -ExpectedSha $actualSha"
Write-Output 'Candidate clean before: YES'
Write-Output 'Candidate clean after: YES'
Write-Output 'Required gates: approved-Source=GREEN; pinned-Editor=GREEN; effective-catalogue=GREEN; nested-prefab-save-reload-and-flattened-control=GREEN; content-shape-probe=GREEN; locked-build=GREEN; focused-plan-tests=GREEN; effective-prefab-public-conformance=GREEN; source-immutability=GREEN; missing-wrong-type-rebound-controls=GREEN; delete-rebuild-normalization=GREEN'
Write-Output 'Result: GREEN'
Write-Output "Scratch evidence: $public; $nested; $contentShape; $inventory"
