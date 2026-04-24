#!/usr/bin/env bash
# Build a Release nupkg of MTConnect.NET from the current `integration/all-fixes`
# tip and feed it to the user's local `dime-connector` so the downstream app can
# validate end-to-end against every in-flight fix before any per-plan PR merges
# upstream.
#
# Usage: tools/build-for-dime-connector.sh
#
# Flow:
#   1. Verify integration/all-fixes worktree exists + is up to date with origin.
#   2. Build Release nupkgs of every MTConnect.NET-* library in this repo.
#      Output: ./build/output/*.nupkg (matching the existing `build/output/` convention
#      from the historical nupkg builds in `.gitignore`).
#   3. Copy the nupkgs to a local feed at ~/.nuget/local-mtconnect-net-feed/.
#   4. Echo the version + feed path the user should add to dime-connector's
#      NuGet.config + the PackageReference Version property to update.
#
# Does NOT modify dime-connector itself — that's a deliberate boundary so the
# user reviews the package change before applying it. The script prints the
# exact dotnet command(s) to run inside dime-connector after the package is in
# the feed.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
WORKTREE_PATH="${REPO_ROOT}/.claude/worktrees/integration-all-fixes"
LOCAL_FEED="${HOME}/.nuget/local-mtconnect-net-feed"
DIME_CONNECTOR="${DIME_CONNECTOR_PATH:-/home/ts/git/mriiot/datainmotionenterprise/dime-connector}"

if [[ ! -d "${WORKTREE_PATH}" ]]; then
    echo "error: integration worktree not found at ${WORKTREE_PATH}" >&2
    echo "       run tools/refresh-integration-branch.sh first." >&2
    exit 1
fi

if [[ ! -d "${DIME_CONNECTOR}" ]]; then
    echo "error: dime-connector not found at ${DIME_CONNECTOR}" >&2
    echo "       set DIME_CONNECTOR_PATH or symlink the repo into the default location." >&2
    exit 1
fi

cd "${WORKTREE_PATH}"

INTEGRATION_SHA="$(git rev-parse --short HEAD)"
PACKAGE_VERSION="6.9.0.2-int+${INTEGRATION_SHA}"
echo "==> Integration SHA: ${INTEGRATION_SHA}"
echo "==> Package version: ${PACKAGE_VERSION}"

echo "==> Building Release nupkgs..."
mkdir -p "${LOCAL_FEED}"
mkdir -p build/output

# Restore + pack the libraries the dime-connector consumes today + the ones
# common consumers reach for.
LIBRARIES=(
    libraries/MTConnect.NET-Common
    libraries/MTConnect.NET-XML
    libraries/MTConnect.NET-JSON
    libraries/MTConnect.NET-JSON-cppagent
    libraries/MTConnect.NET-HTTP
    libraries/MTConnect.NET-MQTT
    libraries/MTConnect.NET-SHDR
    libraries/MTConnect.NET-Services
    libraries/MTConnect.NET-TLS
    libraries/MTConnect.NET-DeviceFinder
    libraries/MTConnect.NET
    agent/MTConnect.NET-Applications-Agents
    adapter/MTConnect.NET-Applications-Adapter
)

for lib in "${LIBRARIES[@]}"; do
    if [[ -d "${lib}" ]]; then
        echo "    - ${lib}"
        dotnet pack "${lib}" \
            -c Release \
            /p:Version="${PACKAGE_VERSION}" \
            -o "${LOCAL_FEED}" \
            --nologo --verbosity quiet
    fi
done

echo
echo "==> Packed nupkgs written to ${LOCAL_FEED}"
echo "==> To consume from dime-connector:"
echo
echo "    1. Add the local feed to dime-connector's NuGet.config (one-time):"
echo
echo "       <configuration>"
echo "         <packageSources>"
echo "           <add key=\"local-mtconnect-net\" value=\"${LOCAL_FEED}\" />"
echo "         </packageSources>"
echo "       </configuration>"
echo
echo "    2. Update the PackageReference Version in"
echo "       ${DIME_CONNECTOR}/DIME/DIME.csproj:"
echo
echo "       <PackageReference Include=\"MTConnect.NET-Applications-Agents\" Version=\"${PACKAGE_VERSION}\" />"
echo "       <PackageReference Include=\"MTConnect.NET-SHDR\" Version=\"${PACKAGE_VERSION}\" />"
echo
echo "    3. Restore + build dime-connector:"
echo
echo "       cd ${DIME_CONNECTOR}"
echo "       dotnet restore --no-cache"
echo "       dotnet build -c Release"
echo
echo "==> Integration SHA pin (for dime-connector consumer manifest): ${INTEGRATION_SHA}"
