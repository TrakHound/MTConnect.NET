#!/usr/bin/env bash
# Generate the API reference Markdown under docs/api/.
#
# Compiles every shipped MTConnect.NET library in Release / net8.0,
# then runs `docfx metadata` against the produced DLLs and XML doc
# files. The output is a flat tree of `Namespace.Type.md` pages
# alongside per-namespace landing pages.
#
# Usage:
#   bash docs/scripts/generate-api-ref.sh        # full regen
#   bash docs/scripts/generate-api-ref.sh --fast # skip the build
#
# Requirements:
#   - dotnet 8 SDK
#   - docfx (installed once via: dotnet tool install -g docfx)

set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "${repo_root}"

skip_build=false
for arg in "$@"; do
  case "${arg}" in
    --fast) skip_build=true ;;
    -h|--help)
      sed -n '2,15p' "${BASH_SOURCE[0]}"
      exit 0
      ;;
    *)
      echo "unknown argument: ${arg}" >&2
      exit 1
      ;;
  esac
done

libraries=(
  "MTConnect.NET-Common"
  "MTConnect.NET-DeviceFinder"
  "MTConnect.NET-HTTP"
  "MTConnect.NET-JSON"
  "MTConnect.NET-JSON-cppagent"
  "MTConnect.NET-MQTT"
  "MTConnect.NET-Protobuf"
  "MTConnect.NET-Services"
  "MTConnect.NET-SHDR"
  "MTConnect.NET-SysML"
  "MTConnect.NET-TLS"
  "MTConnect.NET-XML"
  "MTConnect.NET"
)

if ! "${skip_build}"; then
  echo "==> building libraries in Debug for net8.0"
  # Debug is the multi-target config that compiles ONLY net8.0 on every
  # project — Release multi-targets net4.6.1..net9.0 and fails on SDKs
  # that lack the legacy reference assemblies. The reference is content,
  # not packaged output, so a Debug build is fine.
  for lib in "${libraries[@]}"; do
    dotnet build "libraries/${lib}/${lib}.csproj" \
      -c Debug \
      -p:GenerateDocumentationFile=true \
      -p:NoWarn=CS1591 \
      --nologo \
      -v:quiet
  done
fi

echo "==> running docfx metadata"
docfx_bin="docfx"
if ! command -v "${docfx_bin}" >/dev/null 2>&1; then
  docfx_bin="${HOME}/.dotnet/tools/docfx"
fi
if ! command -v "${docfx_bin}" >/dev/null 2>&1; then
  echo "docfx not found on PATH and ~/.dotnet/tools/docfx is missing." >&2
  echo "install with: dotnet tool install -g docfx" >&2
  exit 1
fi

# Clean previous output so removed types do not linger.
find docs/api -mindepth 1 -maxdepth 1 -not -name 'index.md' -exec rm -rf {} +

"${docfx_bin}" metadata docs/.docfx/docfx.json --logLevel warning

echo "==> done. $(find docs/api -name '*.md' -not -name 'index.md' | wc -l) pages generated."
