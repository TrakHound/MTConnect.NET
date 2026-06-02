#!/usr/bin/env bash
# Regenerate the auto-generated reference pages under docs/reference/.
#
# Walks the repo source tree with Roslyn (via MTConnect.NET-DocsGen)
# and writes:
#
#   docs/reference/http-api.md
#   docs/reference/environment-variables.md
#   docs/reference/configuration.md
#   docs/reference/index.md
#
# Usage:
#   bash docs/scripts/generate-reference.sh           # regenerate
#   bash docs/scripts/generate-reference.sh --check   # CI gate; non-zero on drift
#
# Requirements:
#   - dotnet 8 SDK
#
# Drift gate (`--check`) — why it runs before the build, not after:
#
# The VitePress build step in .github/workflows/docs.yml invokes the
# `prebuild` npm hook, which calls this script in regenerate mode and
# overwrites docs/reference/ on disk. A drift check after that step
# would therefore always pass: the just-overwritten files are
# byte-equivalent to whatever the generator just emitted, by
# definition. The drift gate is meaningful only against the committed
# state, so the workflow runs `--check` BEFORE the build.
#
# In `--check` mode the script exits non-zero when the committed
# docs/reference/ pages do not match what DocsGen would emit from the
# current source tree — surfacing as a CI failure that names the
# stale page and the diff. The author then re-runs the script without
# `--check`, commits the regenerated files, and pushes.

set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "${repo_root}"

mode="generate"
for arg in "$@"; do
  case "${arg}" in
    --check) mode="check" ;;
    -h|--help)
      sed -n '2,18p' "${BASH_SOURCE[0]}"
      exit 0
      ;;
    *)
      echo "unknown argument: ${arg}" >&2
      exit 1
      ;;
  esac
done

# Build once so subsequent runs are fast.
dotnet build build/MTConnect.NET-DocsGen/MTConnect.NET-DocsGen.csproj \
  -c Debug --nologo -v:quiet

if [ "${mode}" = "check" ]; then
  dotnet run --project build/MTConnect.NET-DocsGen --no-build \
    -- --repo . --check
else
  dotnet run --project build/MTConnect.NET-DocsGen --no-build \
    -- --repo .
fi
