#!/usr/bin/env bash
# Generate the API reference Markdown under docs/api/.
#
# Compiles every committed MTConnect.NET project (libraries, agent and
# adapter applications and modules, in-tree build / generator tools,
# examples, the embedded-agent template, and the test suites) in
# Debug, then runs `docfx metadata` against the produced DLLs and XML
# doc files. The output is a flat tree of `Namespace.Type.md` pages
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
      sed -n '2,17p' "${BASH_SOURCE[0]}"
      exit 0
      ;;
    *)
      echo "unknown argument: ${arg}" >&2
      exit 1
      ;;
  esac
done

# Path to the project directory. The csproj filename is inferred as
# "<basename>.csproj" except where overridden in the matching entries
# of csproj_overrides below.
projects=(
  "libraries/MTConnect.NET-Common"
  "libraries/MTConnect.NET-DeviceFinder"
  "libraries/MTConnect.NET-HTTP"
  "libraries/MTConnect.NET-JSON"
  "libraries/MTConnect.NET-JSON-cppagent"
  "libraries/MTConnect.NET-MQTT"
  "libraries/MTConnect.NET-Protobuf"
  "libraries/MTConnect.NET-Services"
  "libraries/MTConnect.NET-SHDR"
  "libraries/MTConnect.NET-SysML"
  "libraries/MTConnect.NET-TLS"
  "libraries/MTConnect.NET-XML"
  "libraries/MTConnect.NET"
  "agent/MTConnect.NET-Agent"
  "agent/MTConnect.NET-Applications-Agents"
  "agent/Modules/MTConnect.NET-AgentModule-HttpServer"
  "agent/Modules/MTConnect.NET-AgentModule-HttpAdapter"
  "agent/Modules/MTConnect.NET-AgentModule-MqttAdapter"
  "agent/Modules/MTConnect.NET-AgentModule-MqttBroker"
  "agent/Modules/MTConnect.NET-AgentModule-MqttRelay"
  "agent/Modules/MTConnect.NET-AgentModule-ShdrAdapter"
  "agent/Processors/MTConnect.NET-AgentProcessor-Python"
  "adapter/MTConnect.NET-Adapter"
  "adapter/MTConnect.NET-Applications-Adapter"
  "adapter/Modules/MTConnect.NET-AdapterModule-MQTT"
  "adapter/Modules/MTConnect.NET-AdapterModule-SHDR"
  "build/MTConnect.NET-SysML-Import"
  "build/MTConnect.NET-DocsGen"
  "build/MTConnect.NET.Builder"
  "examples/MTConnect.NET-Agent-Embedded"
  "examples/MTConnect.NET-Client-HTTP"
  "examples/MTConnect.NET-Client-MQTT"
  "examples/MTConnect.NET-Client-SHDR"
  "templates/mtconnect.net-agent/content/MTConnect.NET-Embedded-Agent:Agent.csproj"
  "tests/Compliance/MTConnect-Compliance-Tests"
  "tests/MTConnect.NET-AgentModule-MqttRelay-Tests"
  "tests/MTConnect.NET-Common-Tests"
  "tests/MTConnect.NET-Docs-Tests"
  "tests/MTConnect.NET-HTTP-Tests"
  "tests/MTConnect.NET-Integration-Tests"
  "tests/MTConnect.NET-JSON-cppagent-Tests"
  "tests/MTConnect.NET-JSON-Tests"
  "tests/MTConnect.NET-SHDR-Tests"
  "tests/MTConnect.NET-Tests-Agents"
  "tests/MTConnect.NET-XML-Tests"
)

if ! "${skip_build}"; then
  echo "==> building projects in Debug"
  # Debug is the multi-target config that compiles ONLY net8.0 on
  # every shipped project — Release multi-targets net4.6.1..net9.0 and
  # fails on SDKs that lack the legacy reference assemblies. The
  # reference is content, not packaged output, so a Debug build is
  # fine.
  for entry in "${projects[@]}"; do
    proj_dir="${entry%%:*}"
    if [[ "${entry}" == *:* ]]; then
      csproj_name="${entry##*:}"
    else
      csproj_name="$(basename "${proj_dir}").csproj"
    fi
    # Pass no NoWarn overrides. TreatWarningsAsErrors=true in
    # Directory.Build.props turns every XML-doc warning — including
    # CS1591 (missing XML doc on a public surface) — into a build
    # failure, which is the campaign's 100 % XML-doc-coverage gate.
    dotnet build "${proj_dir}/${csproj_name}" \
      -c Debug \
      -p:GenerateDocumentationFile=true \
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

# docfx renders <c>…</c> as raw <code>…</code> HTML in the emitted
# markdown. VitePress's underlying Vue template compiler then treats
# {{ … }} inside that <code> element as a mustache interpolation —
# e.g. <code>{{term(data set)}}</code> from the SysML representation
# markers is parsed as the JavaScript expression `term(data set)` and
# rejects the unquoted space. Mark every emitted <code> as v-pre so
# Vue treats its contents as literal text rather than a template
# fragment. Safe to re-run because the clean step above wipes the
# output tree before docfx writes fresh files.
find docs/api -name '*.md' -not -name 'index.md' -print0 \
  | xargs -0 sed -i -E 's#<code( [^>]*)?>#<code v-pre\1>#g'

echo "==> done. $(find docs/api -name '*.md' -not -name 'index.md' | wc -l) pages generated."
