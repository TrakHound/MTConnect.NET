# Assets

An **Asset** is a non-observation entity the device tracks: a cutting tool, a pallet, a fixture, a raw-material billet, an uploaded file. Assets exist outside the time-series stream — they are addressable by `AssetId`, they carry their own version history, and they are retrievable from the agent's `/asset/<assetId>` endpoint independently of any DataItem.

`MTConnect.NET` represents an Asset with [`MTConnect.Assets.Asset`](/api/MTConnect.Assets/Asset) implementing `IAsset`, and ships one concrete subclass per spec-defined Asset type:

- [`CuttingToolAsset`](/api/MTConnect.Assets.CuttingTools/CuttingToolAsset) and its archetype variant `CuttingToolArchetypeAsset`.
- [`PalletAsset`](/api/MTConnect.Assets.Pallet/PalletAsset).
- [`FileAsset`](/api/MTConnect.Assets.Files/FileAsset) and its archetype variant `FileArchetypeAsset`.
- [`RawMaterialAsset`](/api/MTConnect.Assets.RawMaterials/RawMaterialAsset).
- [`FixtureAsset`](/api/MTConnect.Assets.Fixture/FixtureAsset).
- [`ComponentConfigurationParametersAsset`](/api/MTConnect.Assets.ComponentConfigurationParameters/ComponentConfigurationParametersAsset).
- [`QIFDocumentWrapperAsset`](/api/MTConnect.Assets.QIF/QIFDocumentWrapperAsset).

## Identity

Every Asset carries:

- **`AssetId`** — globally unique, set once when the asset is authored. Used to retrieve the asset via `/asset/<assetId>`.
- **`DeviceUuid`** — the Device the asset is associated with.
- **`Type`** — `"CuttingTool"`, `"Pallet"`, `"File"`, etc. Matches the `TypeId` constant on the concrete subclass.
- **`Timestamp`** — when the asset was last updated.
- **`Removed`** — flag set true when the agent receives an `AssetRemoved` instruction; the asset stays in the buffer with `Removed=true` until the buffer recycles it.
- **`Hash`** — SHA-1 over the asset's identity and content; consumers detect a change without re-parsing the body.

```csharp
public partial class Asset : IAsset
{
    public string AssetId { get; set; }
    public string DeviceUuid { get; set; }
    public string Type { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Removed { get; set; }
    public string Hash { get; }
    public MTConnectEntityType EntityType => MTConnectEntityType.Asset;
}
```

## CuttingTool

The richest Asset type. A `CuttingToolAsset` describes one physical cutting tool with its serial number, life-cycle state, measurements, and per-edge `CuttingItem` records. The library exposes the spec's full structure:

```csharp
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.CuttingTools.Measurements;

var tool = new CuttingToolAsset
{
    AssetId = "TOOL-001234",
    DeviceUuid = device.Uuid,
    SerialNumber = "ABC-1234",
    ToolId = "T1",
    Description = "12mm carbide endmill, 4-flute",
};

var lifeCycle = new CuttingToolLifeCycle
{
    CutterStatus = CutterStatusType.NEW,
    Location = new Location { Type = "POT", PositiveOverlap = "0", NegativeOverlap = "0" },
};
lifeCycle.AddMeasurement(new CuttingDiameterMaxMeasurement { Value = 12.0m });
tool.CuttingToolLifeCycle = lifeCycle;
```

The `CuttingToolArchetypeAsset` variant (`Type == "CuttingToolArchetype"`) describes the tool's design-time blueprint — the shape every instance of the part-number-X conforms to — and the `CuttingToolAsset` references its archetype via `CuttingToolArchetypeReference`. Source: SysML `CuttingToolAsset` and `CuttingToolArchetype` classes ([`mtconnect/mtconnect_sysml_model`](https://github.com/mtconnect/mtconnect_sysml_model)).

## Pallet

A `PalletAsset` describes a pallet on a transfer system: its dimensions, its loaded vs unloaded shape, the work-piece(s) currently on it. The MTConnect Standard introduced typed measurements for pallets in v2.4, replacing the earlier free-form `Measurement` element with concrete classes:

- `HeightMeasurement`, `WidthMeasurement`, `LengthMeasurement`, `WeightMeasurement`, `SwingMeasurement` — pallet-empty geometry.
- `LoadedHeightMeasurement`, `LoadedWidthMeasurement`, `LoadedLengthMeasurement`, `LoadedWeightMeasurement`, `LoadedSwingMeasurement` — pallet-with-workpiece geometry.

Each measurement is a separate concrete class with a `Value` (decimal), an optional `NativeUnits`, an optional `Code`, and `Units` defaulted from the spec ([`MTConnect.Assets.Pallet.HeightMeasurement`](/api/MTConnect.Assets.Pallet/HeightMeasurement) etc.). Older models with a free-form `Measurement` child still deserialize for backward compatibility.

```csharp
using MTConnect.Assets.Pallet;

var pallet = new PalletAsset
{
    AssetId = "PALLET-7",
    DeviceUuid = device.Uuid,
    Type = PalletAsset.TypeId,
};
pallet.Measurements = new IMeasurement[]
{
    new HeightMeasurement { Value = 200m },
    new WidthMeasurement { Value = 400m },
    new LoadedHeightMeasurement { Value = 250m },
};
```

## File

A `FileAsset` describes a file the device wants to publish: a CAD model, a control program, a calibration table, a measurement output. The asset carries:

- `FileLocation` — where to fetch the bytes.
- `FileProperty` — typed metadata (size, MIME type, etc.).
- `FileComment` — free-form notes.
- `Destination` — where the consumer should place the file.
- `ApplicationCategory` and `ApplicationType` — what the file is for.

The `FileArchetypeAsset` variant describes the file-class's design-time shape (a "G-code file" archetype with declared properties), and the `FileAsset` instance references the archetype. Source: SysML `File` and `FileArchetype` classes ([`mtconnect/mtconnect_sysml_model`](https://github.com/mtconnect/mtconnect_sysml_model)).

## RawMaterial

A `RawMaterialAsset` describes the raw material the device is consuming: stock dimensions, material composition, form (`BAR`, `BLOCK`, `SHEET`, etc. — enum `Form`). Carries optional `MaterialLifeCycle` records that track quantity consumed over time. Source: SysML `RawMaterial` class.

## Fixture, ComponentConfigurationParameters, QIF

Three less-common asset types, each shipped as a distinct subclass:

- `FixtureAsset` — a workholding fixture's identity and configuration.
- `ComponentConfigurationParametersAsset` — a snapshot of a Component's parameter set. Introduced in v2.2.
- `QIFDocumentWrapperAsset` — wraps a [QIF](https://qifstandards.org/) measurement document.

## Asset registration and lifecycle

The agent maintains an asset buffer separate from the observation buffer. Assets are added through:

```csharp
agent.AddAsset(deviceUuid, asset);
```

The buffer's size is bounded by `assetBufferSize` (default `1000` in `agent.config.yaml`); when full, the buffer evicts the least-recently-updated asset. Removal is logical: setting an asset's `Removed` flag (or sending `AssetRemoved` as an EVENT observation on the `AssetRemoved` DataItem) marks the asset removed without freeing its slot, so a consumer querying `/asset?removed=true` still sees the soft-deleted assets.

Every asset change drives an `AssetChanged` EVENT observation on the Device's `AssetChanged` DataItem (auto-installed on the Agent Device). Consumers subscribe to that DataItem to know when to re-fetch `/asset/<assetId>`. Source: `Part_4.0` Assets §3 ([docs.mtconnect.org](https://docs.mtconnect.org/)).

## AssetCount representation

The agent exposes an `AssetCount` DataItem that reports the count of each Asset type the agent currently holds. The Standard's three normative sources disagree on whether this DataItem is a scalar EVENT (`VALUE`) or a `DATA_SET`. `MTConnect.NET` follows the SysML XMI and the XSD, both of which declare a scalar EVENT — the cppagent reference auto-injects `representation="DATA_SET"`, but XMI is the source of truth. The library's posture is documented at [Compliance: Known divergences](/compliance/known-divergences) and tracked upstream at [Redmine #3890](https://projects.mtconnect.org/issues/3890).

## Wire shape

Assets serialize through the `MTConnectAssets_<version>.xsd` envelope. The XML codec lives in `MTConnect.NET-XML`; the JSON-CPPAGENT codec in `MTConnect.NET-JSON-cppagent`. The `/asset` endpoint returns one Asset per query, or the union of every asset matching the filter. See [Wire formats](/wire-formats/) for the envelope shape.

## Where to next

- [`IAsset` API reference](/api/MTConnect.Assets/IAsset).
- [Cookbook: Write an agent](/cookbook/write-an-agent) — agents that emit Assets.
- [Compliance: Per-version matrix](/compliance/per-version-matrix) — which Asset types ship in which spec version.
