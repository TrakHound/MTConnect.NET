#!/usr/bin/env bash
# Run dotnet. By default uses the `dotnet` on PATH; when passed
# `--docker` (or `MTCONNECT_DOTNET_USE_DOCKER=1`) runs inside an
# official .NET SDK container. Portable across Linux, macOS, and
# Windows Git-Bash / WSL.
#
# Adapted from dime-connector/tools/dotnet.sh for MTConnect.NET
# conventions. Tuned for this repo's layout:
#   - no single "main" csproj to read TFM from — `MTConnect.NET.sln`
#     spans ~20+ projects targeting a mix of net6.0, net8.0, and
#     netstandard2.0. Default SDK image pinned to net8.0 (the target
#     used by every P0-aligned test project in plans/tests/); override
#     via MTCONNECT_DOTNET_IMAGE.
#   - test projects live under tests/**/*.csproj (not a hardcoded path).
#
# Usage: tools/dotnet.sh [--docker] <dotnet args ...>
#        tools/dotnet.sh build MTConnect.NET.sln
#        tools/dotnet.sh --docker test tests/MTConnect.NET-Common-Tests
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
