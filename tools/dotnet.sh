#!/usr/bin/env bash
# Wrapper around `dotnet` that runs either against the dotnet on PATH
# (default) or inside an official Microsoft .NET SDK container when
# `--docker` (or `MTCONNECT_DOTNET_USE_DOCKER=1`) is set. Lets a
# contributor without a local SDK install build and test the repo, and
# pins the SDK version so two contributors don't drift on minor
# differences.
#
# Default container image tag: 8.0 (the TargetFramework every test
# project in this repo uses for Debug). Override via
# `MTCONNECT_DOTNET_SDK_TAG=9.0` or, for a fully custom image,
# `MTCONNECT_DOTNET_IMAGE=mcr.microsoft.com/dotnet/sdk:9.0-noble`.
#
# Cross-platform: Linux, macOS, Windows Git-Bash / WSL.
#
# Usage:
#   tools/dotnet.sh build MTConnect.NET.sln
#   tools/dotnet.sh --docker test tests/MTConnect.NET-Common-Tests
#   MTCONNECT_DOTNET_USE_DOCKER=1 tools/dotnet.sh --version
set -euo pipefail

# --- Locate repo root (macOS-safe; no readlink -f) ---------------------
SCRIPT_SOURCE="${BASH_SOURCE[0]}"
while [ -h "${SCRIPT_SOURCE}" ]; do
	SCRIPT_DIR="$(cd -P "$(dirname "${SCRIPT_SOURCE}")" && pwd)"
	SCRIPT_SOURCE="$(readlink "${SCRIPT_SOURCE}")"
	[[ "${SCRIPT_SOURCE}" != /* ]] && SCRIPT_SOURCE="${SCRIPT_DIR}/${SCRIPT_SOURCE}"
done
TOOLS_DIR="$(cd -P "$(dirname "${SCRIPT_SOURCE}")" && pwd)"
REPO_ROOT="$(cd -P "${TOOLS_DIR}/.." && pwd)"

# --- --docker short-circuit flag ---------------------------------------
USE_DOCKER="${MTCONNECT_DOTNET_USE_DOCKER:-0}"
if [[ "${1:-}" == "--docker" ]] || [[ "${1:-}" == "-d" ]]; then
	USE_DOCKER=1
	shift
fi

# --- SDK tag resolution ------------------------------------------------
# Default: net8.0 (matches the TFM alignment in plans/tests/01-foundation.md).
# Override via MTCONNECT_DOTNET_SDK_TAG (e.g. "6.0", "9.0") or swap the
# whole image via MTCONNECT_DOTNET_IMAGE.
SDK_TAG_DEFAULT="${MTCONNECT_DOTNET_SDK_TAG:-8.0}"

if [[ "${USE_DOCKER}" == "1" ]]; then
	IMAGE_DEFAULT="mcr.microsoft.com/dotnet/sdk:${SDK_TAG_DEFAULT}"
	IMAGE="${MTCONNECT_DOTNET_IMAGE:-${IMAGE_DEFAULT}}"
	NUGET_VOL="${MTCONNECT_NUGET_VOLUME:-mtconnect-net-nuget}"
	TOOLS_VOL="${MTCONNECT_DOTNET_TOOLS_VOLUME:-mtconnect-net-dotnet-tools}"

	# E2E tier needs host-network + docker-socket passthrough so
	# Testcontainers-spawned children (mosquitto, cppagent, etc.) are
	# reachable from inside this container. Enabled when the invocation
	# targets tests/IntegrationTests or any tests/E2E/** project, OR
	# when MTCONNECT_DOTNET_E2E_DIND=1 is set explicitly.
	E2E_MODE=0
	if [[ "${MTCONNECT_DOTNET_E2E_DIND:-0}" == "1" ]]; then
		E2E_MODE=1
	fi
	if [[ " $* " == *" tests/IntegrationTests"* ]] \
		|| [[ " $* " == *" tests/E2E/"* ]] \
		|| [[ " $* " == *"IntegrationTests.csproj"* ]] \
		|| [[ " $* " == *" tests/Compliance/"* ]]; then
		E2E_MODE=1
	fi

	DIND_ARGS=()
	if [[ "${E2E_MODE}" == "1" ]]; then
		DIND_ARGS+=(
			--network=host
			-v /var/run/docker.sock:/var/run/docker.sock
			-e MTCONNECT_E2E_DOCKER=true
			-e TESTCONTAINERS_RYUK_DISABLED=true
			-e "MTCONNECT_E2E_HOST_REPO_ROOT=${REPO_ROOT}"
		)
		# Bind the host's docker CLI so Testcontainers can invoke it
		# against the shared host daemon. Stdlib SDK image doesn't ship
		# docker CLI and installing it per test run is wasteful.
		for host_bin in /usr/bin/docker /usr/local/bin/docker; do
			if [ -x "${host_bin}" ]; then
				DIND_ARGS+=(-v "${host_bin}:${host_bin}:ro")
				break
			fi
		done
	fi

	exec docker run --rm \
		-v "${REPO_ROOT}:/src" \
		-v "${NUGET_VOL}:/root/.nuget/packages" \
		-v "${TOOLS_VOL}:/root/.dotnet/tools" \
		"${DIND_ARGS[@]}" \
		-w /src \
		-e HOME=/root \
		-e PATH=/root/.dotnet/tools:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin \
		-e DOTNET_NOLOGO=1 \
		-e DOTNET_CLI_TELEMETRY_OPTOUT=1 \
		"${IMAGE}" \
		dotnet "$@"
fi

# Native path: cd to repo root so `dotnet` picks up Directory.Build.props etc.
cd "${REPO_ROOT}"
exec dotnet "$@"
