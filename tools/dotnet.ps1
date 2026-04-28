#!/usr/bin/env pwsh
# Wrapper around `dotnet` that runs either against the dotnet on PATH
# (default) or inside an official Microsoft .NET SDK container when
# -Docker (or MTCONNECT_DOTNET_USE_DOCKER=1) is set. Lets a contributor
# without a local SDK install build and test the repo, and pins the
# SDK version so two contributors don't drift on minor differences.
#
# The -Docker switch and the MTCONNECT_DOTNET_USE_DOCKER env var are
# deliberately kept as a dual API. The switch is the contributor-
# facing form; the env var lets the nested wrapper chain
# (tools/test.ps1 -Docker -> sets $env:MTCONNECT_DOTNET_USE_DOCKER=1
# -> calls tools/dotnet.ps1 per project) propagate the docker mode
# without splatting an extra positional switch through every dotnet
# invocation. Removing either form would either force test.ps1 to
# splat -Docker per call site or break the env-var propagation path.
#
# Default container image tag: 8.0. Override via
# MTCONNECT_DOTNET_SDK_TAG=9.0 or, for a fully custom image,
# MTCONNECT_DOTNET_IMAGE=mcr.microsoft.com/dotnet/sdk:9.0-noble.
#
# Cross-platform: Windows PowerShell, PowerShell Core on Linux/macOS.
#
# Usage:
#   tools/dotnet.ps1 build MTConnect.NET.sln
#   tools/dotnet.ps1 -Docker test tests/MTConnect.NET-Common-Tests
#   $env:MTCONNECT_DOTNET_USE_DOCKER='1'; tools/dotnet.ps1 --version

[CmdletBinding()]
param(
	[switch] $Docker,
	[Parameter(Position = 0, ValueFromRemainingArguments = $true)]
	[string[]] $DotnetArgs
)

$ErrorActionPreference = 'Stop'

$ToolsDir = Split-Path -Parent $PSCommandPath
$RepoRoot = (Resolve-Path (Join-Path $ToolsDir '..')).Path

$useDocker = $Docker -or ($env:MTCONNECT_DOTNET_USE_DOCKER -eq '1')

$sdkTag = if ($env:MTCONNECT_DOTNET_SDK_TAG) { $env:MTCONNECT_DOTNET_SDK_TAG } else { '8.0' }

if ($useDocker) {
	$image = if ($env:MTCONNECT_DOTNET_IMAGE) { $env:MTCONNECT_DOTNET_IMAGE } else { "mcr.microsoft.com/dotnet/sdk:${sdkTag}" }
	$nugetVol = if ($env:MTCONNECT_NUGET_VOLUME) { $env:MTCONNECT_NUGET_VOLUME } else { 'mtconnect-net-nuget' }
	$toolsVol = if ($env:MTCONNECT_DOTNET_TOOLS_VOLUME) { $env:MTCONNECT_DOTNET_TOOLS_VOLUME } else { 'mtconnect-net-dotnet-tools' }

	# E2E heuristic — matches the bash sibling.
	$e2eMode = ($env:MTCONNECT_DOTNET_E2E_DIND -eq '1')
	$joined = ' ' + ($DotnetArgs -join ' ') + ' '
	foreach ($hit in @(' tests/IntegrationTests', ' tests/E2E/', 'IntegrationTests.csproj', ' tests/Compliance/')) {
		if ($joined.Contains($hit)) { $e2eMode = $true; break }
	}

	$dindArgs = @()
	if ($e2eMode) {
		$dindArgs += @(
			'--network=host'
			'-v', '/var/run/docker.sock:/var/run/docker.sock'
			'-e', 'MTCONNECT_E2E_DOCKER=true'
			'-e', 'TESTCONTAINERS_RYUK_DISABLED=true'
			'-e', "MTCONNECT_E2E_HOST_REPO_ROOT=${RepoRoot}"
		)
		foreach ($hostBin in @('/usr/bin/docker', '/usr/local/bin/docker')) {
			if (Test-Path $hostBin) {
				$dindArgs += @('-v', "${hostBin}:${hostBin}:ro")
				break
			}
		}
	}

	& docker run --rm `
		-v "${RepoRoot}:/src" `
		-v "${nugetVol}:/root/.nuget/packages" `
		-v "${toolsVol}:/root/.dotnet/tools" `
		@dindArgs `
		-w /src `
		-e HOME=/root `
		-e 'PATH=/root/.dotnet/tools:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin' `
		-e DOTNET_NOLOGO=1 `
		-e DOTNET_CLI_TELEMETRY_OPTOUT=1 `
		$image `
		dotnet @DotnetArgs
	exit $LASTEXITCODE
}

Push-Location $RepoRoot
try {
	& dotnet @DotnetArgs
	exit $LASTEXITCODE
}
finally {
	Pop-Location
}
