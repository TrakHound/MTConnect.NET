# Phase 2 — Red tests

## Executed

Authored a paired NUnit test project at `tests/MTConnect.NET-JSON-cppagent-Tests/` with three fixtures, each tagged `[Category("CppAgentHeaderFieldsPresent")]`:

- `Streams/JsonStreamsHeaderSchemaVersionTests.cs` — six cases covering forward mapping, serialized wire shape (`schemaVersion` + `testIndicator`), reverse mapping (`ToStreamsHeader`), null-source ctor branch, and the parameterless ctor.
- `Devices/JsonDevicesHeaderSchemaVersionTests.cs` — six cases mirroring the same matrix on the Devices envelope.
- `Assets/JsonAssetsHeaderSchemaVersionTests.cs` — six cases mirroring the same matrix on the Assets envelope.

Wired the test project into `MTConnect.NET.sln` under the existing `Tests` solution folder (GUID `{14375E03-6BF8-45E6-B868-D2399368992B}`) with project GUID `{A274A407-D9F4-4F54-B3BF-178B33FACBA3}`.

## Red proof

`dotnet build tests/MTConnect.NET-JSON-cppagent-Tests/MTConnect.NET-JSON-cppagent-Tests.csproj -c Debug` fails with 18 errors at HEAD, all of the form:

```
error CS0117: 'MTConnectStreamsHeader' does not contain a definition for 'SchemaVersion'
error CS1061: 'JsonStreamsHeader' does not contain a definition for 'SchemaVersion'
error CS0117: 'MTConnectDevicesHeader' does not contain a definition for 'SchemaVersion'
error CS1061: 'JsonDevicesHeader' does not contain a definition for 'SchemaVersion'
error CS0117: 'MTConnectAssetsHeader' does not contain a definition for 'SchemaVersion'
error CS1061: 'JsonAssetsHeader' does not contain a definition for 'SchemaVersion'
```

Compile-time red is the cleanest signal for "this surface is missing"; the production fix in P3 introduces the symbol on both the source DTO interfaces / impls and the JSON-cppagent header DTOs, at which point all 18 cases compile and pass.

## Metrics delta

- Test fixtures: +3 (18 cases total).
- Production code: untouched.
- Build state: red on the new test project (compile-time); the rest of the solution still builds Debug net8.0 green.

## Deviations from plan

The plan proposed leaning on a `VersionMatrix` fixture from `feat/issue-133`. That fixture is not on `upstream/master`, so the P2 cases use direct `MTConnect*Header` instantiations with explicit `SchemaVersion = "2.5"` values. The version-matrix coverage is reframed for P5 once a runnable agent fixture is available; the P2 fixtures already provide the tight unit-level pin per envelope.

## Follow-ups

- None — the green commit lands in P3.
