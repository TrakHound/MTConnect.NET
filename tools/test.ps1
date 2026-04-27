#!/usr/bin/env pwsh
# Local test + coverage entry point for MTConnect.NET. Discovers every
# test project under tests/**/*.csproj — adding a new test project
# requires no edits to this script. The compliance harness under
# tests/Compliance/** and the Docker-gated end-to-end suites are
# skipped by default so the common loop stays fast; flags below opt
# into them.
#
# Pairs with tools/dotnet.ps1: when -Docker (or
# MTCONNECT_DOTNET_USE_DOCKER=1) is set, each dotnet invocation runs
# inside the pinned .NET SDK container via tools/dotnet.ps1.
#
# Usage:
#   tools/test.ps1 [-Docker] [-Compliance] [-E2E] [-Only <pattern>]
#
# Parameters:
#   -Docker        Run every dotnet invocation through tools/dotnet.ps1
#                  -Docker (also honoured via MTCONNECT_DOTNET_USE_DOCKER=1).
#   -Compliance    Include the MTConnect compliance harness under
#                  tests/Compliance/** (XSD validation, OCL checks,
#                  cppagent parity). Skipped by default because it is
#                  the slowest tier and many of its tests are gated
#                  behind Docker / [Category] tags.
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
	param([Parameter(ValueFromRemainingArguments = $true)][string[]] $Args)
	$wrapper = Join-Path $ToolsDir 'dotnet.ps1'
	& pwsh -File $wrapper @Args
	if ($LASTEXITCODE -ne 0) {
		throw "dotnet $($Args -join ' ') failed with exit code $LASTEXITCODE"
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
	$allTestProjects = Get-ChildItem -Path tests -Recurse -Filter *.csproj `
		| Where-Object { $_.FullName -notmatch '[\\/]Compliance[\\/]' -and $_.FullName -notmatch '[\\/]E2E[\\/]' } `
		| ForEach-Object { $_.FullName } `
		| Sort-Object

	if ($Only) {
		$allTestProjects = $allTestProjects | Where-Object { $_ -match $Only }
	}

	$filterExpr = 'Category!=RequiresDocker'
	if (Get-E2EEnabled) { $filterExpr = '' }

	foreach ($proj in $allTestProjects) {
		$settingsArgs = @()
		if (Test-Path (Join-Path $RepoRoot 'tests/coverlet.runsettings')) {
			$settingsArgs = @('--settings', 'tests/coverlet.runsettings')
		}
		$filterArgs = @()
		if ($filterExpr) { $filterArgs = @('--filter', $filterExpr) }

		$projName = [IO.Path]::GetFileNameWithoutExtension($proj)
		Invoke-Dotnet test $proj `
			--configuration Release `
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

	# --- E2E tier (Docker-gated) --------------------------------------
	if (Get-E2EEnabled) {
		$e2eRoots = @('tests/IntegrationTests', 'tests/E2E') | Where-Object { Test-Path $_ }
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
