<#
.SYNOPSIS
    Builds the MTConnect.NET NuGet Libraries, the Agent Installer and the Agent Docker Images, then publishes the
    Libraries to NuGet, pushes the Docker Images and creates a draft GitHub Release with the Installer.

.DESCRIPTION
    Runs the MTConnect.NET.Builder parts in Release against the repository set as 'input' in the Builder configuration
    (config.<ConfigurationId>.yaml, copied from config.template.yaml):
        libraries nuget build     (packs the Libraries to <output>\<version>\libraries)
        agent installer build     (Inno Setup, one installer with every configured Runtime and Framework)
        agent docker build        (one image per configured platform)
        libraries nuget publish   (pushes the Libraries to each group's nugetServer; existing versions are skipped)
        agent docker publish      (pushes the images and the multi-platform [version] and latest tags)
        github release publish    (a draft release on the built commit with the Installer attached)

    Package versions are [VersionPrefix].[Revision] (Directory.Build.props), where the Revision is the number of commits
    since the VersionPrefix was last changed, so publishing requires a clean working tree. After publishing, the built
    commit is tagged v[VersionPrefix] locally; push the tag with "git push origin v[VersionPrefix]".

    The Installer and NuGet packages can also be copied to a backup directory (-BackupDirectory) :
        [BackupDirectory]\[version]\[installer]
        [BackupDirectory]\[version]\libraries\[packages]

.EXAMPLE
    .\build.ps1

.EXAMPLE
    .\build.ps1 -SkipPublish  # Build only

.EXAMPLE
    .\build.ps1 -SkipBuild    # Publish the most recent build without rebuilding

.EXAMPLE
    .\build.ps1 -SkipDocker   # Build and publish everything except the Docker Images

.EXAMPLE
    .\build.ps1 -Quiet:$false # Show the dotnet / Inno Setup / docker output (hidden by default)
#>
[CmdletBinding()]
param(
    # Builder configuration (config.<id>.yaml in this directory)
    [string]$ConfigurationId = 'development',

    # Hide the build tool output (dotnet, Inno Setup, docker). On by default; use -Quiet:$false to show it.
    # Output is still shown when a step fails
    [switch]$Quiet = $true,

    # Don't build or publish the NuGet Libraries
    [switch]$SkipLibraries,

    # Don't build the Agent Installer or create the GitHub Release
    [switch]$SkipInstaller,

    # Don't build or push the Agent Docker Images
    [switch]$SkipDocker,

    # Publish the most recent build without rebuilding
    [switch]$SkipBuild,

    # Build only : don't publish the Libraries, push the Docker Images or create the GitHub Release
    [switch]$SkipPublish,

    # Copy the Installer and NuGet packages to [BackupDirectory]\[version] (a warning is shown if they can't be copied)
    [string]$BackupDirectory
)

$ErrorActionPreference = 'Stop'

$builderDirectory = $PSScriptRoot
$builderProject = Join-Path $builderDirectory 'MTConnect.NET.Builder.csproj'
$builderConfiguration = Join-Path $builderDirectory "config.$ConfigurationId.yaml"


function Write-Step([string]$message) {
    Write-Host ''
    Write-Host "==> $message" -ForegroundColor Cyan
}

function Get-ConfigurationValue([string]$path, [string]$name) {
    # Top level "name: value" from the Builder yaml
    $line = Get-Content $path | Where-Object { $_ -match "^$name\s*:" } | Select-Object -First 1
    if ($line) { return ($line -replace "^$name\s*:\s*", '').Trim().Trim('"', "'") }
    return $null
}

function Resolve-ConfigurationPath([string]$path) {
    # Relative paths in the Builder configuration are relative to the Builder directory
    if ([System.IO.Path]::IsPathRooted($path)) { return [System.IO.Path]::GetFullPath($path) }
    return [System.IO.Path]::GetFullPath((Join-Path $builderDirectory $path))
}

function Get-VersionPrefix([string]$path) {
    $match = Select-String -Path $path -Pattern '<VersionPrefix>\s*([^<]+?)\s*</VersionPrefix>' | Select-Object -First 1
    if ($match) { return $match.Matches[0].Groups[1].Value }
    throw "VersionPrefix not found in $path"
}

function Assert-Committed([string]$directory) {
    # Versions identify a commit, so uncommitted changes (to tracked files) can't be published under one
    $changes = & git -C $directory status --porcelain --untracked-files=no
    if ($LASTEXITCODE -ne 0) { throw "git status failed in $directory ($LASTEXITCODE)" }
    if ($changes) { throw "Uncommitted changes in $directory : commit them before publishing (or use -SkipPublish)`n$($changes -join "`n")" }
}

function Get-BuildInfo([string]$directory) {
    # Written by the Builder's Agent builds (version, revisionVersion, commit, dirty)
    $path = Join-Path $directory 'build.json'
    if (-not (Test-Path $path)) { throw "$path not found (rebuild without -SkipBuild)" }
    return Get-Content $path -Raw | ConvertFrom-Json
}

function Add-ReleaseTag($buildInfo) {
    # A local annotated tag on the built commit (a tag that already exists on another commit is left as is)
    $tag = "v$($buildInfo.version)"
    Write-Step "Tag $tag"

    $existing = & git -C $root rev-parse -q --verify "refs/tags/$tag^{commit}"
    if ($existing) {
        if ($existing -eq $buildInfo.commit) { Write-Host "$tag already tags $($buildInfo.commit)" -ForegroundColor Green }
        else { Write-Warning "$tag already exists on $existing (not $($buildInfo.commit))" }
        return
    }

    & git -C $root tag -a $tag $buildInfo.commit -m "MTConnect.NET $($buildInfo.revisionVersion)"
    if ($LASTEXITCODE -ne 0) { Write-Warning "Failed to create $tag ($LASTEXITCODE)"; return }
    Write-Host "Tagged $($buildInfo.commit) as $tag (push with: git push origin $tag)" -ForegroundColor Green
}

function Invoke-Builder([string[]]$arguments) {
    Write-Step "MTConnect.NET.Builder $($arguments -join ' ')"
    & $script:builderExe @arguments
    if ($LASTEXITCODE -ne 0) { throw "MTConnect.NET.Builder failed ($LASTEXITCODE) : $($arguments -join ' ')" }
}

function Copy-BackupFile([System.IO.FileInfo]$file, [string]$directory) {
    if (-not (Test-Path $directory)) { New-Item -ItemType Directory -Path $directory | Out-Null }

    $destination = Join-Path $directory $file.Name
    Copy-Item $file.FullName $destination -Force

    $hash = (Get-FileHash $file.FullName -Algorithm SHA256).Hash
    $copiedHash = (Get-FileHash $destination -Algorithm SHA256).Hash
    if ($copiedHash -ne $hash) { throw "Copied Hash ($copiedHash) does not match the local file ($hash)" }

    Write-Host "Copied $destination" -ForegroundColor Green
}

function Backup-Files([string]$label, [System.IO.FileInfo[]]$files, [string]$directory) {
    # A backup failure (ex. the share is offline) doesn't stop the build or the publish
    Write-Step "Backup $label to $directory"
    try {
        if (-not $files) { throw "No $label found" }
        foreach ($file in $files) { Copy-BackupFile $file $directory }
    }
    catch {
        Write-Warning "Backup of the $label failed : $($_.Exception.Message)"
    }
}


if (-not (Test-Path $builderConfiguration)) {
    throw "Builder Configuration not found : $builderConfiguration (copy config.template.yaml)"
}

# The MTConnect.NET repository being built (the Configuration's input), which is checked, versioned and tagged
$root = Get-ConfigurationValue $builderConfiguration 'input'
if (-not $root) { throw "'input' not set in $builderConfiguration" }
$root = Resolve-ConfigurationPath $root

$outputDirectory = Get-ConfigurationValue $builderConfiguration 'output'
if (-not $outputDirectory) { throw "'output' not set in $builderConfiguration" }
$outputDirectory = Resolve-ConfigurationPath $outputDirectory

$versionInfoPath = Get-ConfigurationValue $builderConfiguration 'versionInfoPath'
if ($versionInfoPath) { $versionInfoPath = Resolve-ConfigurationPath $versionInfoPath }
else { $versionInfoPath = Join-Path $root 'Directory.Build.props' }

# The version comes from the repository containing versionInfoPath, which can differ from the input repository,
# so both are checked before the build rather than only when publishing
if (-not $SkipPublish -and -not $SkipBuild) {
    foreach ($directory in @($root, (Split-Path $versionInfoPath -Parent)) | Select-Object -Unique) { Assert-Committed $directory }
}

$versionPrefix = Get-VersionPrefix $versionInfoPath
$versionDirectory = Join-Path $outputDirectory $versionPrefix
$started = Get-Date

$verbosity = @()
if (-not $Quiet) { $verbosity += '--verbose' }
$configurationArguments = @('--configurationId', $ConfigurationId)

Write-Step 'Build MTConnect.NET.Builder (Release)'
& dotnet build $builderProject -c Release -nologo -v q
if ($LASTEXITCODE -ne 0) { throw "MTConnect.NET.Builder build failed ($LASTEXITCODE)" }

# Debug builds of the Builder are interactive (args are ignored), so this script runs the Release build
$tfm = (Select-String -Path $builderProject -Pattern '<TargetFramework>\s*([^<]+?)\s*</TargetFramework>').Matches[0].Groups[1].Value
$script:builderExe = Join-Path $builderDirectory "bin\Release\$tfm\MTConnect.NET.Builder.exe"

Invoke-Builder (@('global', 'version') + $configurationArguments)

if (-not $SkipBuild) {
    if (-not $SkipLibraries) { Invoke-Builder (@('libraries', 'nuget', 'build') + $configurationArguments + $verbosity) }
    if (-not $SkipInstaller) { Invoke-Builder (@('agent', 'installer', 'build') + $configurationArguments + $verbosity) }
    if (-not $SkipDocker) { Invoke-Builder (@('agent', 'docker', 'build') + $configurationArguments + $verbosity) }
}

if ($BackupDirectory) {
    $since = if ($SkipBuild) { [datetime]::MinValue } else { $started }
    $backupVersionDirectory = Join-Path $BackupDirectory $versionPrefix

    if (-not $SkipInstaller) {
        $installers = Get-ChildItem $versionDirectory -Filter '*-install.exe' -File -ErrorAction SilentlyContinue | Where-Object { $_.LastWriteTime -ge $since }
        Backup-Files 'Installer' $installers $backupVersionDirectory
    }
    if (-not $SkipLibraries) {
        $packages = Get-ChildItem (Join-Path $versionDirectory 'libraries') -File -ErrorAction SilentlyContinue | Where-Object { $_.Name -match '\.s?nupkg$' -and $_.LastWriteTime -ge $since }
        Backup-Files 'NuGet Packages' $packages (Join-Path $backupVersionDirectory 'libraries')
    }
}

if (-not $SkipPublish) {
    $buildInfo = $null
    if (-not $SkipInstaller -or -not $SkipDocker) {
        $buildInfo = Get-BuildInfo $versionDirectory
        if ($buildInfo.dirty) { throw "The Agent was built with uncommitted changes ($versionDirectory\build.json) : commit them and rebuild" }
    }

    if (-not $SkipLibraries) { Invoke-Builder (@('libraries', 'nuget', 'publish') + $configurationArguments + $verbosity) }
    if (-not $SkipDocker) { Invoke-Builder (@('agent', 'docker', 'publish') + $configurationArguments + $verbosity) }
    if (-not $SkipInstaller) { Invoke-Builder (@('github', 'release', 'publish') + $configurationArguments) }

    if ($buildInfo) { Add-ReleaseTag $buildInfo }
}

Write-Step 'Done'
