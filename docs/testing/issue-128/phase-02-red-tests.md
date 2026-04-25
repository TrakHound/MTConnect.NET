# Phase 02 — Red tests

## Matrix

- **Envelope**: Streams, Devices.
- **Version**: every `public static readonly Version` field on
  `MTConnectVersions` (v1.0-v1.8, v2.0-v2.5 — 14 versions).

Two parametric tests × 14 versions = 30 NUnit cases (NUnit's
`TestCaseData` reflects each row separately). All Streams + Devices
cases except `v2.0` fail with
`Expected "<configured>" / But was: "2.0"`. The two `v2.0` cases pass
coincidentally because the hardcode literal happens to match the
formatted version string. Those will be green-on-arrival regression
pins after P3.

## Category

`SchemaVersionFromConfiguration` (descriptive — per CONVENTIONS §14
forbids `IssueNNNRed`-style labels).

## Sample failure

```
Failed Streams_envelope_schemaVersion_equals_configured_release(2.5) [< 1 ms]
  Error Message:
     Streams.schemaVersion must mirror AgentConfiguration.DefaultVersion (issue #128).
  Expected: "2.5"
  But was:  "2.0"
```

## Files

- `tests/MTConnect.NET-JSON-cppagent-Tests/Streams/JsonMTConnectStreamsSchemaVersionTests.cs`
- `tests/MTConnect.NET-JSON-cppagent-Tests/Devices/JsonMTConnectDevicesSchemaVersionTests.cs`
- `tests/MTConnect.NET-JSON-cppagent-Tests/TestHelpers/VersionMatrix.cs`
- `tests/MTConnect.NET-JSON-cppagent-Tests/TestHelpers/EnvelopeFixtures.cs`

## CI gate

Plan calls for an inverted-exit-code `schema-version-from-configuration`
job. The repo's `.github/workflows/dotnet.yml` rewrite lives on
`feat/issue-133`; this branch can't add a workflow that depends on it.
The category label is the durable assertion — once #133 lands, the
inverted job can be added in a follow-up commit; for the lifetime of
this draft PR the category label remains the contract.

## Validation

`dotnet test ...JSON-cppagent-Tests --filter Category=SchemaVersionFromConfiguration`
reports `Failed: 28, Passed: 2, Total: 30` (the 2 passing cases are the
coincidental v2.0 matches). Pre-existing tests (Sanity) green.
