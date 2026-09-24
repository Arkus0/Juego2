param([Parameter(Mandatory=$true)][string]$AssetsRoot)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../Unity/ArkusUnity'
$target = Join-Path $project 'Assets/Arkus/H1/SourceSlice'
$village = Join-Path $AssetsRoot 'Medieval Village'
$animation = Join-Path $AssetsRoot 'Animation'
$archive = Join-Path $village 'Engine Projects/Medieval Village MegaKit[Unity URP].zip'

function Assert-Hash([string]$path, [string]$expected) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Missing approved Source input: $path" }
    $actual = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($actual -ne $expected) { throw "Source fingerprint mismatch: $path (observed $actual)" }
}

Assert-Hash $archive 'b9d757dd2608a5cee4d9ee1e8183f6cb4cad9d27480841a905180def9c7d8b10'
Assert-Hash (Join-Path $village 'License_Source.txt') '7310dfa8512d7ca12b6591329fda4adab7e8b9cdbe04c7d7061c2fe5c7dc38ee'
Assert-Hash (Join-Path $village 'FBX (Unity)/Wall_Plaster_Window_Wide_Flat.fbx') '45825c565b9d1027036ce7fc922f7e7a7d69bc05e459d886eb738bf1eafc92b8'
Assert-Hash (Join-Path $animation 'License.txt') '6d01f55c6e4c49a2c9963e147e561945ae2c83958c8ca667d90a6bffdbfac061'
Assert-Hash (Join-Path $animation 'Unity/UAL1.fbx') '0556d52f6bce01c0982b3548ee3cdfa1b8270977507001f62cbdfcc405570842'

New-Item -ItemType Directory -Force -Path $target | Out-Null
$facade = Join-Path $target 'Wall_Plaster_Window_Wide_Flat.fbx'
$clips = Join-Path $target 'UAL1.fbx'
Copy-Item -LiteralPath (Join-Path $village 'FBX (Unity)/Wall_Plaster_Window_Wide_Flat.fbx') -Destination $facade -Force
Copy-Item -LiteralPath (Join-Path $animation 'Unity/UAL1.fbx') -Destination $clips -Force

Add-Type -AssemblyName System.IO.Compression
$zip = [System.IO.Compression.ZipFile]::OpenRead($archive)
try {
    foreach ($item in @(
        @{Suffix='/Materials/MI_Plaster.mat'; Name='MI_Plaster.mat'; Hash='3fcfc0359d1460009858533461893c626e25ea52364ae4e85f4a6bf84fc3ce69'},
        @{Suffix='/Materials/MI_Plaster.mat.meta'; Name='MI_Plaster.mat.meta'; Hash='1d4843119c9f80b78a8e4466bf5041e531d0c8072a72f1770e4d013070e9b042'}
    )) {
        $entry = @($zip.Entries | Where-Object { $_.FullName.EndsWith($item.Suffix, [StringComparison]::Ordinal) })
        if ($entry.Count -ne 1) { throw "Approved archive member is missing or ambiguous: $($item.Suffix)" }
        $destination = Join-Path $target $item.Name
        $sourceStream = $entry[0].Open()
        try { $destinationStream = [IO.File]::Create($destination); try { $sourceStream.CopyTo($destinationStream) } finally { $destinationStream.Dispose() } } finally { $sourceStream.Dispose() }
        Assert-Hash $destination $item.Hash
    }
} finally { $zip.Dispose() }

[IO.File]::WriteAllText($facade + '.meta', "fileFormatVersion: 2`nguid: a914dbae2609f0107a8bce353d33727c`n", [Text.UTF8Encoding]::new($false))
[IO.File]::WriteAllText($clips + '.meta', "fileFormatVersion: 2`nguid: 06d37381cd6d9bece36de7794e2fc74a`n", [Text.UTF8Encoding]::new($false))
Write-Output "H1-04 approved Source slice staged at $target"
