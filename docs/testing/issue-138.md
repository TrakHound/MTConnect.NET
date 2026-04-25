# Issue #138 — JSON-cppagent Probe DataItem emits empty `name` attribute

## 1. Defect + scope

The `JsonDataItem(IDataItem)` constructor in `libraries/MTConnect.NET-JSON-cppagent/Devices/JsonDataItem.cs` copies `Name` from the source `IDataItem` unconditionally. When the caller never sets `Name` (or sets it to `string.Empty`), the resulting Probe JSON includes `"name": ""`, which is wrong: the MTConnect Devices schema declares `DataItem/@name` as optional and the XML formatter (`libraries/MTConnect.NET-XML/Devices/XmlDataItem.cs:198`) already guards the attribute with `string.IsNullOrEmpty`. The base JSON formatter (`libraries/MTConnect.NET-JSON/Devices/JsonDataItem.cs`) has the same defect.

Sources:

- XSD: `https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd` — `DataItem/@name` is `use="optional"`.
- Prose: `https://docs.mtconnect.org/` Part_2.0 §7.2.2 "DataItem attributes".

## 2. Investigation (P1)

## 3. Red tests (P2)

## 4. Library fix (P3)

## 5. Regression pins (P4)

## 6. E2E validation (P5)

## 7. Campaign summary (P6)
