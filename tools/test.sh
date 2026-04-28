#!/usr/bin/env bash
# Local test + coverage entry point for MTConnect.NET. Discovers every
# test project under tests/**/*.csproj — adding a new test project
# requires no edits to this script. The compliance harness under
# tests/Compliance/** and the Docker-gated end-to-end suites are
# skipped by default so the common loop stays fast; flags below opt
# into them.
#
# Pairs with tools/dotnet.sh: when --docker (or
# MTCONNECT_DOTNET_USE_DOCKER=1) is set, each dotnet invocation runs
# inside the pinned .NET SDK container via tools/dotnet.sh.
#
# This script reads the --docker flag, then exports
# MTCONNECT_DOTNET_USE_DOCKER=1 so the env-var form propagates into
# every nested tools/dotnet.sh call without needing to splat
# --docker per call site. The dual flag/env-var API on dotnet.sh
# exists specifically to support this nested-call pattern.
#
# Usage: tools/test.sh [--docker] [--compliance] [--e2e] [--only <pattern>]
#
# Flags:
#   -d, --docker        Run every dotnet invocation through tools/dotnet.sh
#                       --docker (also honoured via
#                       MTCONNECT_DOTNET_USE_DOCKER=1).
#   -c, --compliance    Include the MTConnect compliance harness under
#                       tests/Compliance/** (XSD validation, OCL checks,
#                       cppagent parity). Skipped by default because it
#                       is the slowest tier and many of its tests are
#                       gated behind Docker / [Category] tags. NOTE:
#                       runs every test in the project including
#                       `XsdLoadStrict`, expected to surface ~54
#                       failures until the XSD-1.1 validator lands.
#   -e, --e2e           Force the Docker-gated end-to-end suites
#                       (implies MTCONNECT_E2E_DOCKER=true;
#                       Testcontainers spins up mosquitto + cppagent
#                       containers per test class).
#   -o, --only PATTERN  Run only the test projects whose path matches
#                       PATTERN (grep -E). Example: --only 'XML|SHDR'
#                       runs only those two projects.
#   -h, --help          Print this help and exit.
set -euo pipefail

SCRIPT_SOURCE="${BASH_SOURCE[0]}"
while [ -h "${SCRIPT_SOURCE}" ]; do
	SCRIPT_DIR="$(cd -P "$(dirname "${SCRIPT_SOURCE}")" && pwd)"
	SCRIPT_SOURCE="$(readlink "${SCRIPT_SOURCE}")"
	[[ "${SCRIPT_SOURCE}" != /* ]] && SCRIPT_SOURCE="${SCRIPT_DIR}/${SCRIPT_SOURCE}"
done
TOOLS_DIR="$(cd -P "$(dirname "${SCRIPT_SOURCE}")" && pwd)"
REPO_ROOT="$(cd -P "${TOOLS_DIR}/.." && pwd)"

print_help() {
	cat <<'EOF'
Local test + coverage entry point for MTConnect.NET. Discovers every
test project under tests/**/*.csproj — adding a new test project
requires no edits to this script. The compliance harness under
tests/Compliance/** and the Docker-gated end-to-end suites are
skipped by default so the common loop stays fast; flags below opt
into them.

Pairs with tools/dotnet.sh: when --docker (or
MTCONNECT_DOTNET_USE_DOCKER=1) is set, each dotnet invocation runs
inside the pinned .NET SDK container via tools/dotnet.sh.

Usage: tools/test.sh [--docker] [--compliance] [--e2e] [--only <pattern>]

Flags:
  -d, --docker        Run every dotnet invocation through tools/dotnet.sh
                      --docker (also honoured via
                      MTCONNECT_DOTNET_USE_DOCKER=1).
  -c, --compliance    Include the MTConnect compliance harness under
                      tests/Compliance/** (XSD validation, OCL checks,
                      cppagent parity). Skipped by default because it
                      is the slowest tier and many of its tests are
                      gated behind Docker / [Category] tags.
  -e, --e2e           Force the Docker-gated end-to-end suites
                      (implies MTCONNECT_E2E_DOCKER=true;
                      Testcontainers spins up mosquitto + cppagent
                      containers per test class).
  -o, --only PATTERN  Run only the test projects whose path matches
                      PATTERN (grep -E). Example: --only 'XML|SHDR'
                      runs only those two projects.
  -h, --help          Print this help and exit.
EOF
}

USE_DOCKER=0
RUN_COMPLIANCE=0
FORCE_E2E=0
ONLY_PATTERN=""

while [[ $# -gt 0 ]]; do
	case "$1" in
		-d|--docker)      USE_DOCKER=1 ;;
		-c|--compliance)  RUN_COMPLIANCE=1 ;;
		-e|--e2e)         FORCE_E2E=1 ;;
		-o|--only)        ONLY_PATTERN="${2:-}"; shift ;;
		-h|--help)        print_help; exit 0 ;;
		--)               shift; break ;;
		*)
			echo "tools/test.sh: unknown argument '$1'. See --help." >&2
			exit 2
			;;
	esac
	shift
done

if [[ "${USE_DOCKER}" == "1" ]]; then
	export MTCONNECT_DOTNET_USE_DOCKER=1
fi

if [[ "${FORCE_E2E}" == "1" ]]; then
	export MTCONNECT_E2E_DOCKER=true
fi

DOTNET=("${TOOLS_DIR}/dotnet.sh")

cd "${REPO_ROOT}"
rm -rf TestResults coverage coverage-report
mkdir -p TestResults

"${DOTNET[@]}" tool restore

# --- Unit + integration tiers (the default, always runs) --------------
# Enumerate tests/**/*.csproj, minus Compliance (gated by --compliance)
# minus explicit E2E subtrees (run separately below).
mapfile -t ALL_TEST_PROJECTS < <(find tests -name '*.csproj' -not -path '*/Compliance/*' -not -path '*/E2E/*' | sort)

if [[ -n "${ONLY_PATTERN}" ]]; then
	FILTERED=()
	for proj in "${ALL_TEST_PROJECTS[@]}"; do
		if echo "${proj}" | grep -Eq "${ONLY_PATTERN}"; then
			FILTERED+=("${proj}")
		fi
	done
	ALL_TEST_PROJECTS=("${FILTERED[@]}")
fi

e2e_enabled_check() {
	local raw="${MTCONNECT_E2E_DOCKER:-false}"
	case "$(printf '%s' "${raw}" | tr '[:upper:]' '[:lower:]')" in
		true|yes|on|1) return 0 ;;
		*) return 1 ;;
	esac
}

# Category filter: by default exclude Docker-gated tests unless MTCONNECT_E2E_DOCKER.
FILTER_EXPR='Category!=RequiresDocker&Category!=XsdLoadStrict'
if e2e_enabled_check; then
	FILTER_EXPR=''
fi

for proj in "${ALL_TEST_PROJECTS[@]}"; do
	SETTINGS_ARGS=()
	if [[ -f tests/coverlet.runsettings ]]; then
		SETTINGS_ARGS+=(--settings tests/coverlet.runsettings)
	fi
	FILTER_ARGS=()
	if [[ -n "${FILTER_EXPR}" ]]; then
		FILTER_ARGS+=(--filter "${FILTER_EXPR}")
	fi

	"${DOTNET[@]}" test "${proj}" \
		--configuration Release \
		"${SETTINGS_ARGS[@]}" \
		"${FILTER_ARGS[@]}" \
		--collect:"XPlat Code Coverage" \
		--results-directory "TestResults/$(basename "${proj}" .csproj)"
done

# --- Compliance tier (tests/Compliance/**, opt-in) --------------------
if [[ "${RUN_COMPLIANCE}" == "1" ]]; then
	mapfile -t COMPLIANCE_PROJECTS < <(find tests/Compliance -name '*.csproj' 2>/dev/null | sort)
	for proj in "${COMPLIANCE_PROJECTS[@]}"; do
		"${DOTNET[@]}" test "${proj}" \
			--configuration Release \
			--collect:"XPlat Code Coverage" \
			--results-directory "TestResults/$(basename "${proj}" .csproj)"
	done
fi

# --- E2E tier (tests/IntegrationTests + tests/E2E/**, Docker-gated) ---
if e2e_enabled_check; then
	mapfile -t E2E_PROJECTS < <(find tests/IntegrationTests tests/E2E -name '*.csproj' 2>/dev/null | sort)
	for proj in "${E2E_PROJECTS[@]}"; do
		"${DOTNET[@]}" test "${proj}" \
			--configuration Release \
			--collect:"XPlat Code Coverage" \
			--results-directory "TestResults/$(basename "${proj}" .csproj)"
	done
fi

# --- Coverage report --------------------------------------------------
"${DOTNET[@]}" tool run reportgenerator \
	-reports:'TestResults/**/coverage.cobertura.xml' \
	-targetdir:coverage-report \
	-reporttypes:'Html;TextSummary;MarkdownSummary;Cobertura'

if [[ -f coverage-report/Summary.txt ]]; then
	cat coverage-report/Summary.txt
fi
