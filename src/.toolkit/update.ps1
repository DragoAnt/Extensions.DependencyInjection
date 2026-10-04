#!/usr/bin/env pwsh
<#
.SYNOPSIS
Installs or updates DragoAnt.MSBuildKit in a repository's .toolkit/ folder.

.DESCRIPTION
Downloads a release zip of the kit from GitHub, checks its SHA-256 against the published
checksum (and against .toolkit/kit.json when the version is unchanged), and rewrites
.toolkit/msbuild/ with the default parts plus the optional parts you selected.

.EXAMPLE
pwsh .toolkit/update.ps1                         # update to the version in kit.json, else the latest
pwsh .toolkit/update.ps1 -Version 0.2.0 -DryRun  # show what 0.2.0 would change
pwsh .toolkit/update.ps1 -Add PackageAsProj      # install an optional part
#>
[CmdletBinding()]
param(
    # Kit version to install; default: the version in .toolkit/kit.json, else the latest release.
    [string] $Version,
    # Optional parts to install, e.g. PackageAsProj, EF, Project.SourceGenerator.
    [string[]] $Add = @(),
    # Optional parts to uninstall.
    [string[]] $Remove = @(),
    # Show what would change and change nothing.
    [switch] $DryRun,
    # Install from a local kit build (a folder containing .toolkit/ or a release zip).
    [string] $Source,
    # Expected SHA-256 of the release zip (or of a -Source zip).
    [string] $Sha256,
    # GitHub repository to download releases from.
    [string] $Repo = 'DragoAnt/MSBuildKit',
    # Repository root; default: the current folder.
    [string] $Root
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Say([string] $text) { Write-Host "update: $text" }
function Fail([string] $text) { throw "update: error: $text" }

if (-not $Root) { $Root = (Get-Location).Path }
$Root = (Resolve-Path $Root).Path
$toolkit = Join-Path $Root '.toolkit'
$kitJsonPath = Join-Path $toolkit 'kit.json'
$kitJson = if (Test-Path $kitJsonPath) { Get-Content -Raw $kitJsonPath | ConvertFrom-Json } else { $null }
function KitJsonValue([string] $name) {
    if ($null -ne $kitJson -and $kitJson.PSObject.Properties.Name -contains $name) { return $kitJson.$name }
    return $null
}

if ($Version) { $Version = $Version -replace '^[vV]', '' }
if ($Sha256) { $Sha256 = $Sha256.ToLowerInvariant() }
$recordedRepo = KitJsonValue 'repository'
if ($recordedRepo -and $Repo -eq 'DragoAnt/MSBuildKit') { $Repo = $recordedRepo }

$work = Join-Path ([System.IO.Path]::GetTempPath()) ("msbuildkit-" + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
try {
    $actualSha = ''
    $kitDir = $null
    if ($Source) {
        if (Test-Path $Source -PathType Container) {
            if (-not (Test-Path (Join-Path $Source '.toolkit/msbuild/init.props'))) { Fail "'$Source' is not a kit build: no .toolkit/msbuild/init.props" }
            $kitDir = (Resolve-Path $Source).Path
        }
        elseif (Test-Path $Source -PathType Leaf) {
            $actualSha = (Get-FileHash -Algorithm SHA256 $Source).Hash.ToLowerInvariant()
            if ($Sha256 -and $actualSha -ne $Sha256) { Fail "SHA-256 mismatch for '$Source': expected $Sha256, got $actualSha" }
            $kitDir = Join-Path $work 'kit'
            Expand-Archive -Path $Source -DestinationPath $kitDir
        }
        else { Fail "-Source '$Source' does not exist" }
        if (-not $Version) {
            $versionFile = Join-Path $kitDir '.toolkit/kit.version'
            $Version = if (Test-Path $versionFile) { (Get-Content $versionFile -TotalCount 1).Trim() } else { '0.0.0-local' }
        }
    }
    else {
        if (-not $Version) { $Version = KitJsonValue 'version' }
        if (-not $Version) {
            $latest = Invoke-RestMethod -Uri "https://api.github.com/repos/$Repo/releases/latest" -Headers @{ 'User-Agent' = 'msbuildkit-update' }
            $Version = ($latest.tag_name -replace '^[vV]', '')
            if (-not $Version) { Fail "no release found in $Repo" }
        }
        $zipName = "msbuildkit-$Version.zip"
        $base = "https://github.com/$Repo/releases/download/v$Version"
        $zipPath = Join-Path $work $zipName
        Say "downloading $base/$zipName"
        Invoke-WebRequest -Uri "$base/$zipName" -OutFile $zipPath
        Invoke-WebRequest -Uri "$base/$zipName.sha256" -OutFile "$zipPath.sha256"
        $published = ((Get-Content "$zipPath.sha256" -TotalCount 1) -split '\s+')[0].ToLowerInvariant()
        $actualSha = (Get-FileHash -Algorithm SHA256 $zipPath).Hash.ToLowerInvariant()
        if ($actualSha -ne $published) { Fail "SHA-256 mismatch for ${zipName}: published $published, downloaded $actualSha" }
        $pinned = KitJsonValue 'sha256'
        if (-not $Sha256 -and (KitJsonValue 'version') -eq $Version -and $pinned) { $Sha256 = $pinned.ToLowerInvariant() }
        if ($Sha256 -and $actualSha -ne $Sha256) { Fail "SHA-256 mismatch for ${zipName}: expected $Sha256 (kit.json or -Sha256), got $actualSha" }
        $kitDir = Join-Path $work 'kit'
        Expand-Archive -Path $zipPath -DestinationPath $kitDir
    }

    $newToolkit = Join-Path $kitDir '.toolkit'
    if (-not (Test-Path (Join-Path $newToolkit 'msbuild/init.props'))) { Fail 'the kit build has no .toolkit/msbuild/init.props' }
    $partsPath = Join-Path $newToolkit 'kit.parts'
    if (-not (Test-Path $partsPath)) { Fail 'the kit build has no .toolkit/kit.parts' }
    if ((Test-Path $toolkit) -and (Resolve-Path $newToolkit).Path -eq (Resolve-Path $toolkit).Path) {
        Fail 'the target .toolkit is the kit source itself; run from the repository you want to update, or pass -Root'
    }
    $parts = [ordered]@{}
    foreach ($line in Get-Content $partsPath) {
        $line = $line.Trim()
        if (-not $line -or $line.StartsWith('#')) { continue }
        $fields = $line -split '\s+'
        $parts[$fields[0]] = [pscustomobject]@{ Kind = $fields[1]; Requires = @($fields | Select-Object -Skip 2) }
    }
    function PartDir([string] $name) { if ($name -eq 'Trunk') { 'DragoAnt.MSBuildKit' } else { "DragoAnt.MSBuildKit.$name" } }

    $optional = [System.Collections.Generic.List[string]]::new()
    $recordedParts = @(KitJsonValue 'parts' | Where-Object { $_ })
    foreach ($p in @($recordedParts) + @($Add)) {
        if (-not $parts.Contains($p)) { Fail "unknown part '$p'; known parts: $($parts.Keys -join ' ')" }
        if ($parts[$p].Kind -eq 'optional' -and -not $optional.Contains($p)) { $optional.Add($p) }
    }
    foreach ($p in $Remove) {
        if (-not $parts.Contains($p)) { Fail "unknown part '$p'" }
        if ($parts[$p].Kind -ne 'optional') { Fail "'$p' is a default part and cannot be removed" }
        [void]$optional.Remove($p)
    }

    $selected = [System.Collections.Generic.List[string]]::new()
    foreach ($name in $parts.Keys) { if ($parts[$name].Kind -eq 'default') { $selected.Add($name) } }
    $pending = [System.Collections.Generic.Queue[string]]::new([string[]]$optional)
    while ($pending.Count -gt 0) {
        $p = $pending.Dequeue()
        if ($selected.Contains($p)) { continue }
        $selected.Add($p)
        foreach ($d in $parts[$p].Requires) { $pending.Enqueue($d) }
    }
    $recorded = @($parts.Keys | Where-Object { $selected.Contains($_) -and $parts[$_].Kind -eq 'optional' })

    $stage = Join-Path $work 'stage/msbuild'
    New-Item -ItemType Directory -Path $stage -Force | Out-Null
    Get-ChildItem -File (Join-Path $newToolkit 'msbuild') | Copy-Item -Destination $stage
    foreach ($p in $selected) {
        $d = PartDir $p
        $from = Join-Path $newToolkit "msbuild/$d"
        if (-not (Test-Path $from)) { Fail "the kit build has no part folder msbuild/$d" }
        Copy-Item -Recurse $from (Join-Path $stage $d)
    }

    $oldVersion = KitJsonValue 'version'
    Say "kit $Repo $(if ($oldVersion) { $oldVersion } else { 'none' }) -> $Version$(if ($actualSha) { " (sha256 $actualSha)" })"
    Say "parts: $($selected -join ' ')"

    $currentMsbuild = Join-Path $toolkit 'msbuild'
    function RelativeFiles([string] $dir) {
        if (-not (Test-Path $dir)) { return @() }
        $full = (Resolve-Path $dir).Path
        @(Get-ChildItem -Recurse -File $full | ForEach-Object { $_.FullName.Substring($full.Length + 1).Replace('\', '/') } | Sort-Object)
    }
    $old = RelativeFiles $currentMsbuild
    $new = RelativeFiles $stage
    $changes = 0
    foreach ($f in $new) {
        if ($old -notcontains $f) { Say "  add    .toolkit/msbuild/$f"; $changes++ }
        elseif ((Get-FileHash (Join-Path $currentMsbuild $f)).Hash -ne (Get-FileHash (Join-Path $stage $f)).Hash) { Say "  change .toolkit/msbuild/$f"; $changes++ }
    }
    foreach ($f in $old) { if ($new -notcontains $f) { Say "  remove .toolkit/msbuild/$f"; $changes++ } }

    $dbp = Join-Path $Root 'Directory.Build.props'
    $dbt = Join-Path $Root 'Directory.Build.targets'
    if ($DryRun) {
        Say "$changes file(s) under .toolkit/msbuild would change"
        if (-not (Test-Path $dbp)) { Say '  would create Directory.Build.props' }
        if (-not (Test-Path $dbt)) { Say '  would create Directory.Build.targets' }
        Say 'dry run: nothing changed'
        return
    }

    New-Item -ItemType Directory -Path $toolkit -Force | Out-Null
    if (Test-Path $currentMsbuild) { Remove-Item -Recurse -Force $currentMsbuild }
    Copy-Item -Recurse $stage $currentMsbuild
    $res = Join-Path $newToolkit 'res'
    if (Test-Path $res) { New-Item -ItemType Directory -Path (Join-Path $toolkit 'res') -Force | Out-Null; Copy-Item -Recurse -Force (Join-Path $res '*') (Join-Path $toolkit 'res') }
    foreach ($f in 'update.sh', 'update.ps1', 'kit.parts') {
        $from = Join-Path $newToolkit $f
        if (Test-Path $from) { Copy-Item -Force $from (Join-Path $toolkit $f) }
    }
    Remove-Item -Force -ErrorAction SilentlyContinue (Join-Path $toolkit 'kit.version')

    $partsJson = ($recorded | ForEach-Object { "`"$_`"" }) -join ', '
    $json = "{`n  `"repository`": `"$Repo`",`n  `"version`": `"$Version`",`n  `"sha256`": `"$actualSha`",`n  `"parts`": [$partsJson]`n}`n"
    [System.IO.File]::WriteAllText($kitJsonPath, $json)
    Say "$changes file(s) under .toolkit/msbuild changed"

    if (-not (Test-Path $dbp)) {
        [System.IO.File]::WriteAllText($dbp, "<Project>`n`n  <Import Project=`"`$(MSBuildThisFileDirectory).toolkit/msbuild/init.props`" />`n`n</Project>`n")
        Say 'created Directory.Build.props'
    }
    elseif ((Get-Content -Raw $dbp) -notmatch '\.toolkit[/\\]msbuild[/\\]init\.props') {
        Say 'add to Directory.Build.props: <Import Project="$(MSBuildThisFileDirectory).toolkit/msbuild/init.props" />'
    }
    if (-not (Test-Path $dbt)) {
        [System.IO.File]::WriteAllText($dbt, "<Project>`n`n  <Import Project=`"`$(MSBuildThisFileDirectory).toolkit/msbuild/init.targets`" />`n`n</Project>`n")
        Say 'created Directory.Build.targets'
    }
    elseif ((Get-Content -Raw $dbt) -notmatch '\.toolkit[/\\]msbuild[/\\]init\.targets') {
        Say 'add to Directory.Build.targets: <Import Project="$(MSBuildThisFileDirectory).toolkit/msbuild/init.targets" />'
    }
    Say "done: DragoAnt.MSBuildKit $Version installed in $toolkit"
}
finally {
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $work
}
