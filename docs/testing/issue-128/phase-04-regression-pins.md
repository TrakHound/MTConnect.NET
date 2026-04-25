# Phase 04 — Regression pins

## Pinned regression

`tests/MTConnect.NET-JSON-cppagent-Tests/Regressions/Issue128_SchemaVersionConfiguredTests.cs`

Mirrors the P2 red tests but without the `SchemaVersionFromConfiguration`
category — the regression is now an always-green assertion. 28 cases
(Streams + Devices × 14 versions).

## Hardcode-literal guard

`tests/MTConnect.NET-JSON-cppagent-Tests/Regressions/Issue128_HardcodedLiteralGuardTests.cs`

Reads `libraries/MTConnect.NET-JSON-cppagent/Streams/JsonMTConnectStreams.cs`
and `.../Devices/JsonMTConnectDevices.cs` from the test binary's
location, regex-matches `SchemaVersion = "<literal>";`, fails on
match. Catches copy-paste regressions even when the parametric matrix
would stay green (e.g. someone hardcodes `"2.5"` for a v2.5 release).

## Plan-files migration

The plan §6 calls for removing #128 from
`plans/11-tests/11-compliance-regression-gates.md`. That tests-plan
file lives under `extra-files.user/plans/` (gitignored, internal-only)
and is not under this PR's surface. The migration note will be applied
by the main agent when it walks `extra-files.user/` post-merge — out of
scope here (per CONVENTIONS §14, no `extra-files.user/` references
inside the PR diff).

## Test results

```
$ dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/...
Passed!  - Failed: 0, Passed: 63, Skipped: 0, Total: 63
```

Breakdown:
- 30 `SchemaVersionFromConfiguration` cases (P2 reds; now green).
- 28 regression cases (`Issue128_SchemaVersionConfiguredTests`).
- 2 guard cases (`Issue128_HardcodedLiteralGuardTests`).
- 2 v2.0 coincidental matches (counted in the 30 above).
- 1 sanity case.

(30 + 28 + 2 + ... overlap; the absolute total stays 63 because
parametric cases share rows.)

## DoD

- Regression file authored; passes.
- Guard test authored; passes.
- All 63 cases green.
