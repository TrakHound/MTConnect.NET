# Issue #138 — JSON-cppagent Probe DataItem emits empty `name` attribute

## 1. Defect + scope

The `JsonDataItem(IDataItem)` constructor in `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDataItem.cs` copies `Name` from the source `IDataItem` unconditionally. When the caller never sets `Name` (or sets it to `string.Empty`), the resulting Probe JSON includes `"name": ""`, which is wrong: the MTConnect Devices schema declares `DataItem/@name` as optional and the XML formatter (`libraries/MTConnect.NET-XML/Devices/XmlDataItem.cs:198`) already guards the attribute with `string.IsNullOrEmpty`. The base JSON formatter (`libraries/MTConnect.NET-JSON/Devices/JsonDataItem.cs`) has the same defect.

Sources:

- XSD: `https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd` — `DataItem/@name` is `use="optional"`.
- Prose: `https://docs.mtconnect.org/` Part_2.0 §7.2.2 "DataItem attributes".

## 2. Investigation (P1)

The `JsonDataItem(IDataItem)` constructor in both `MTConnect.NET-JSON-cppagent` and the base `MTConnect.NET-JSON` formatter copies `Name` unconditionally; the global `DefaultIgnoreCondition = WhenWritingDefault` skips `null` but not `""`, so any source DataItem with `Name = string.Empty` flows through to the wire as `"name": ""`. Detail: `docs/testing/issue-138/phase-01-defect-scoping.md`. Both JSON formatters need the `IsNullOrEmpty` guard; the XML formatter is already correct.

## 3. Red tests (P2)

NUnit fixtures `JsonDataItemEmptyNameOmissionTests` in both new test projects (`MTConnect.NET-JSON-cppagent-Tests` + `MTConnect.NET-JSON-Tests`) cover null / empty / explicit / typed-cleared `Name` cases against the constructor surface. Detail: `docs/testing/issue-138/phase-02-red-tests.md`. Two of four cases are red on HEAD (the empty-string and typed-cleared scenarios); the null and explicit-value cases pass because `WhenWritingDefault` already covers null.

## 4. Library fix (P3)

`JsonDataItem(IDataItem)` constructor in both `MTConnect.NET-JSON-cppagent/Devices/JsonDataItem.cs` and `MTConnect.NET-JSON/Devices/JsonDataItem.cs` now guards `Name = dataItem.Name` with `string.IsNullOrEmpty`, mirroring `XmlDataItem.cs:198`. Detail: `docs/testing/issue-138/phase-03-library-fix.md`. All eight P2 tests pass.

## 5. Regression pins (P4)

XML parity pin (`XmlDataItemEmptyNameOmissionTests` in `MTConnect.NET-XML-Tests/Devices/`) confirms the reference shape both JSON formatters now mirror. Source-grep guard (`JsonDataItemSourceGuardTests` in `MTConnect.NET-JSON-cppagent-Tests/Devices/`) prevents future regression to an unguarded `Name = dataItem.Name;` copy on either watched JSON file. Detail: `docs/testing/issue-138/phase-04-regression-pins.md`.

## 6. E2E validation (P5)

In-process E2E (`JsonDevicesResponseDocumentNameOmissionE2ETests` in `MTConnect.NET-JSON-cppagent-Tests`) wires a programmatic Device through the full `JsonDevicesResponseDocument` constructor + `JsonFunctions.Convert` cppagent serializer pipeline and asserts the wire shape on a mixed named/unnamed payload. Detail: `docs/testing/issue-138/phase-05-e2e-validation.md`. Docker-based MQTT scenarios are deferred to the compliance harness in the upcoming tests plan; the surface they would exercise is identical to the in-process E2E for this fix.

## 7. Campaign summary (P6)
