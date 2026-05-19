#!/usr/bin/env pwsh
# Local test + coverage entry point for MTConnect.NET. Discovers every
# test project under tests/**/*.csproj — adding a new test project
# requires no edits to this script. The compliance harness under
# tests/Compliance/** and the Docker-gated end-to-end suites are
# skipped by default so the common loop stays fast; flags below opt
# into them.
#
# When -Docker (or MTCONNECT_DOTNET_USE_DOCKER=1) is set, each
# dotnet invocation runs inside the pinned .NET SDK container.
#
# This script reads the -Docker switch, then sets
# $env:MTCONNECT_DOTNET_USE_DOCKER=1 so the env-var form propagates
# into every nested dotnet wrapper call without needing to splat
# -Docker per call site.
#
# Usage:
#   tools/test.ps1 [-Docker] [-Compliance] [-E2E] [-Only <pattern>]
#
# Parameters:
#   -Docker        Run every dotnet invocation through tools/dotnet.ps1
#                  -Docker (also honored via MTCONNECT_DOTNET_USE_DOCKER=1).
#   -Compliance    Include the MTConnect compliance harness under
#                  tests/Compliance/** (XSD validation, OCL checks,
#                  cppagent parity). Skipped by default because it is
#                  the slowest tier and many of its tests are gated
#                  behind Docker / [Category] tags. NOTE: runs every
#                  test in the project including `XsdLoadStrict`,
#                  expected to surface ~54 failures until the XSD-1.1
#                  validator lands.
#   -E2E           Force the Docker-gated end-to-end suites (implies
#                  MTCONNECT_E2E_DOCKER=true; Testcontainers spins up
#                  mosquitto + cppagent containers per test class).
#   -Only PATTERN  Run only the test projects whose path matches PATTERN
#                  (regex). Example: -Only 'XML|SHDR' runs only those
#                  two projects.

[CmdletBinding()]
param(
	[Alias('d')][switch] $Docker,
	[Alias('c')][switch] $Compliance,
	[Alias('e')][switch] $E2E,
	[Alias('o')][string] $Only
)

$ErrorActionPreference = 'Stop'

$ToolsDir = Split-Path -Parent $PSCommandPath
$RepoRoot = (Resolve-Path (Join-Path $ToolsDir '..')).Path

if ($Docker) { $env:MTCONNECT_DOTNET_USE_DOCKER = '1' }
if ($E2E) { $env:MTCONNECT_E2E_DOCKER = 'true' }

function Invoke-Dotnet {
	param(
		[Parameter(Position = 0, ValueFromRemainingArguments = $true)]
		[string[]] $DotnetArgs
	)
	$wrapper = Join-Path $ToolsDir 'dotnet.ps1'
	& pwsh -File $wrapper @DotnetArgs
	if ($LASTEXITCODE -ne 0) {
		throw "dotnet $($DotnetArgs -join ' ') failed with exit code $LASTEXITCODE"
	}
}

function Get-E2EEnabled {
	$raw = [string]$env:MTCONNECT_E2E_DOCKER
	if (-not $raw) { return $false }
	return @('true', 'yes', 'on', '1') -contains $raw.ToLowerInvariant()
}

Push-Location $RepoRoot
try {
	Remove-Item -Recurse -Force TestResults, coverage, coverage-report -ErrorAction SilentlyContinue | Out-Null
	New-Item -ItemType Directory -Path TestResults | Out-Null

	Invoke-Dotnet tool restore

	# --- Unit + integration tier (default) ----------------------------
	# Exclude the MTConnect.NET-Tests-Agents support library (an AgentRunner
	# host with no test adapter; it builds as a dependency of
	# MTConnect.NET-HTTP-Tests and discovers zero tests, so running it here
	# is a redundant no-op cycle). Parity with tools/test.sh.
	$allTestProjects = Get-ChildItem -Path tests -Recurse -Filter *.csproj `
		| Where-Object { $_.FullName -notmatch '[\\/]Compliance[\\/]' -and $_.FullName -notmatch '[\\/]E2E[\\/]' -and $_.Name -notmatch '-Tests-Agents\.csproj$' } `
		| ForEach-Object { $_.FullName } `
		| Sort-Object

	if ($Only) {
		$allTestProjects = $allTestProjects | Where-Object { $_ -match $Only }
	}

	$filterExpr = 'Category!=XsdLoadStrict'
	if (Get-E2EEnabled) { $filterExpr = '' }

	foreach ($proj in $allTestProjects) {
		$settingsArgs = @()
		$extraBuildArgs = @()
		$projFilterExpr = $filterExpr
		# The integration suite drives the in-process Agent/HTTP hot
		# path; coverlet IL-instrumenting MTConnect.NET-Common /
		# MTConnect.NET-HTTP makes sample delivery race the test's
		# wait. It uses its own runsettings (the shared config plus
		# those two assemblies excluded — covered faster by the
		# dedicated unit suites: MTConnect.NET-HTTP-Tests and its
		# AgentRunner support project MTConnect.NET-Tests-Agents are
		# in MTConnect.NET.sln, so the solution-wide run executes
		# them under the shared runsettings against a real broker +
		# HTTP server and their Cobertura merges with this one — and
		# MaxCpuCount=1, scoped here only)
		# and is built with -p:IntegrationCoverage=true (IsTestProject
		# is false without it; see the .csproj comment). It is owned by
		# exactly this loop — NOT re-run in the E2E tier below. The
		# Category=E2E / RequiresDocker workflow fixtures are excluded
		# by default (mirroring the CI integration step) and run only
		# when -E2E widens the filter.
		if ($proj -match 'MTConnect\.NET-Integration-Tests') {
			if (Test-Path (Join-Path $RepoRoot 'tests/coverlet.integration.runsettings')) {
				$settingsArgs = @('--settings', 'tests/coverlet.integration.runsettings')
			}
			$extraBuildArgs = @('-p:IntegrationCoverage=true')
			if (Get-E2EEnabled) {
				$projFilterExpr = ''
			}
			else {
				$projFilterExpr = 'Category!=RequiresDocker&Category!=XsdLoadStrict&Category!=E2E'
			}
		}
		elseif (Test-Path (Join-Path $RepoRoot 'tests/coverlet.runsettings')) {
			$settingsArgs = @('--settings', 'tests/coverlet.runsettings')
		}
		$filterArgs = @()
		if ($projFilterExpr) { $filterArgs = @('--filter', $projFilterExpr) }

		$projName = [IO.Path]::GetFileNameWithoutExtension($proj)
		Invoke-Dotnet test $proj `
			--configuration Release `
			@extraBuildArgs `
			@settingsArgs `
			@filterArgs `
			'--collect:XPlat Code Coverage' `
			--results-directory "TestResults/$projName"
	}

	# --- Compliance tier (opt-in) -------------------------------------
	if ($Compliance) {
		$compliance = Get-ChildItem -Path tests/Compliance -Recurse -Filter *.csproj -ErrorAction SilentlyContinue
		foreach ($proj in $compliance) {
			$projName = $proj.BaseName
			Invoke-Dotnet test $proj.FullName `
				--configuration Release `
				'--collect:XPlat Code Coverage' `
				--results-directory "TestResults/$projName"
		}
	}

	# --- E2E tier (tests/E2E/**, Docker-gated) ------------------------
	# The integration project is NOT re-run here: it is owned by the
	# default loop above, which widens its filter to include the
	# Category=E2E / RequiresDocker workflow fixtures when -E2E is
	# set. This tier only covers any dedicated tests/E2E/** projects.
	if (Get-E2EEnabled) {
		$e2eRoots = @('tests/E2E') | Where-Object { Test-Path $_ }
		foreach ($root in $e2eRoots) {
			$e2eProjects = Get-ChildItem -Path $root -Recurse -Filter *.csproj -ErrorAction SilentlyContinue
			foreach ($proj in $e2eProjects) {
				$projName = $proj.BaseName
				Invoke-Dotnet test $proj.FullName `
					--configuration Release `
					'--collect:XPlat Code Coverage' `
					--results-directory "TestResults/$projName"
			}
		}
	}

	# --- Coverage report ----------------------------------------------
	Invoke-Dotnet tool run reportgenerator `
		'-reports:TestResults/**/coverage.cobertura.xml' `
		-targetdir:coverage-report `
		'-reporttypes:Html;TextSummary;MarkdownSummary;Cobertura'

	$summary = Join-Path $RepoRoot 'coverage-report/Summary.txt'
	if (Test-Path $summary) {
		Get-Content $summary
	}
}
finally {
	Pop-Location
}
