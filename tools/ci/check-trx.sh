#!/usr/bin/env bash
# Zero-test false-green guard for CI.
#
# Usage: tools/ci/check-trx.sh <label> <trx-file-or-glob>...
#
# dotnet test exits 0 when it discovers nothing, so a broken adapter
# wiring or an over-eager --filter would otherwise pass silently. This
# script fails when no TRX file is found, when a TRX has no parseable
# <Counters total=...>, or when ANY TRX file executed 0 tests. Each test
# project writes its own TRX (the workflow uses LogFilePrefix), so the
# check is per project rather than only on whichever project finished
# last.
set -euo pipefail

label="${1:?label required}"
shift

shopt -s nullglob
files=()
for pattern in "$@"; do
	# shellcheck disable=SC2206 # intentional glob expansion
	matches=( $pattern )
	files+=( "${matches[@]}" )
done

if [ "${#files[@]}" -eq 0 ]; then
	echo "::error::${label}: no TRX files matched '$*' — the test step did not produce results."
	exit 1
fi

grand_total=0
status=0
for trx in "${files[@]}"; do
	total="$(grep -o '<Counters[^>]*total="[0-9]*"' "$trx" \
		| grep -o 'total="[0-9]*"' | grep -o '[0-9]*' | head -n1 || true)"
	if [ -z "${total:-}" ]; then
		echo "::error::${label}: could not parse <Counters total=...> from $trx — refusing to false-green."
		status=1
		continue
	fi
	echo "${label}: ${total} tests in $(basename "$trx")"
	if [ "$total" -eq 0 ]; then
		echo "::error::${label}: $(basename "$trx") executed 0 tests — discovery is broken (zero-test false-green guard)."
		status=1
	fi
	grand_total=$((grand_total + total))
done

echo "${label}: ${grand_total} tests across ${#files[@]} TRX file(s)"
exit "$status"
