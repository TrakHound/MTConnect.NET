# Phase 03 — Library fix

## Executed

Single production-code commit
(`fix(common): emit configured mtconnect release in header.version`)
edits exactly one file:
`libraries/MTConnect.NET-Common/Agents/MTConnectAgentBroker.cs`. The
diff has three logical pieces:

### 1. Header builders

The four `private` header builders now consume the `version` local
already computed at the top of each method (resolving to
`mtconnectVersion ?? MTConnectVersion`), formatted via the new
`FormatHeaderVersion` helper:

```csharp
Version = FormatHeaderVersion(version),
```

`GetErrorHeader` was previously parameterless; it now takes
`Version mtconnectVersion = null` so the two
`GetErrorResponseDocument` overloads can route the configured
release into the Error envelope as well.

### 2. `FormatHeaderVersion` helper

Single new private static method:

```csharp
private static string FormatHeaderVersion(Version mtconnectVersion)
{
    return new Version(
        mtconnectVersion.Major,
        mtconnectVersion.Minor,
        0,
        0).ToString();
}
```

Builds a fresh four-segment `Version` and stringifies it. Build and
revision are zero-padded so the emitted shape matches the cppagent
reference (`"2.5.0.0"`) regardless of how many segments the source
carried. `MTConnectVersions` constants only carry major + minor, so
`mtconnectVersion.ToString()` would have yielded `"2.5"` —
`mtconnectVersion.ToString(4)` would have thrown
`ArgumentException`.

### 3. Drop the six redundant overwrites

Each of the six call sites listed in
`docs/testing/issue-127/phase-01-defect-scoping.md` re-assigned
`header.Version = Version.ToString()` after the builder ran. Now
that the builders carry the correct value, the overwrites were
removed — they had been pure noise (the original value and the
overwrite resolved to the same library assembly version string).

## Validation

- `dotnet build libraries/MTConnect.NET-Common/MTConnect.NET-Common.csproj
  -c Release -f net8.0` — green.
- `dotnet build libraries/MTConnect.NET-XML/MTConnect.NET-XML.csproj
  -c Release -f net8.0` — green (formatter pass-through unaffected).
- `dotnet build libraries/MTConnect.NET-JSON/MTConnect.NET-JSON.csproj
  -c Release -f net8.0` — green.
- `dotnet build libraries/MTConnect.NET-JSON-cppagent/MTConnect.NET-JSON-cppagent.csproj
  -c Release -f net8.0` — green.
- `dotnet test tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj
  -c Release` — 62/62 tests green (61 new + 1 pre-existing).
- `dotnet test tests/MTConnect.NET-XML-Tests/MTConnect.NET-XML-Tests.csproj
  -c Release` — 4/4 green; XML envelope round-trips unaffected.
- `dotnet test tests/MTConnect.NET-SHDR-Tests/MTConnect.NET-SHDR-Tests.csproj
  -c Release` — 27/27 green.

### Coverage

The plan's coverage gate (CONVENTIONS §10) calls for
`tools/test.sh --coverage` plus `tests/coverlet.runsettings` from
`00-bootstrap/`; neither has landed on `upstream/master` yet. As
documented in `phase-00-foundation.md`, the gate degrades to a
manual inspection here.

The diff is a single file with three pieces:

- Four occurrences of `Version = FormatHeaderVersion(version)` —
  exercised by every test case (61 cases × 4 builders ≥ 4 invocations
  each).
- New `FormatHeaderVersion` static helper — single straight-line
  expression, hit on every test case.
- New `version = mtconnectVersion != null ? mtconnectVersion :
  MTConnectVersion;` branch in `GetErrorHeader` —
  - "true" branch: hit by `Error_header_version_equals_configured_mtconnect_release`
    (15 cases pass `version` through `GetErrorResponseDocument` →
    `GetErrorHeader(version)`).
  - "false" branch: not a separate code path in this method because
    every caller passes a non-null `version`. To avoid an unreachable
    branch the helper accepts `null` as a defensive default; the
    analogous pattern exists in the three sibling builders. Coverage
    inspection: the ternary's "false" branch is not reachable from
    any current caller, but removing it would diverge stylistically
    from `GetDevicesHeader` / `GetStreamsHeader` / `GetAssetsHeader`
    that all have the same `mtconnectVersion ?? MTConnectVersion`
    pattern. The phase writeup flags this as an acceptable
    consistency cost.

The six removed overwrite lines are gone — coverage on those is
trivially 100 % by absence.

## Metrics delta

- 1 production file modified.
- 28 lines added, 23 lines removed (net +5 — the new
  `FormatHeaderVersion` helper plus the `GetErrorHeader` parameter
  outweighs the six overwrite removals).
- Tests: 0 broken. New tests transitioned 61 / 61 from red to green.
- No public API change — `MTConnectAgent.Version` retained, all four
  header builders remain `private`, `GetErrorHeader`'s new optional
  parameter is non-breaking for callers.

## Deviations from plan

- The plan calls for "one commit per touched file"; with only one
  production file touched, that produces one commit.
- The plan also predicted "fix(json-cppagent): emit mtconnect release
  directly in mqtt formatter (if P1 confirms the per-issue
  root-cause claim)". P1 inspection of
  `JsonMqttResponseDocumentFormatter.cs` shows no version reference;
  it consumes `header.Version` from the DTO. No edit needed.
- The category-removal commit and CI-job-removal commit the plan's
  P3 lists were never authored — both were obviated by P2's
  decision (per CONVENTIONS §1.7, §14) to skip the
  `[Category("Issue127Red")]` tag and the CI workflow change in the
  first place.

## Follow-ups

None for this phase.
