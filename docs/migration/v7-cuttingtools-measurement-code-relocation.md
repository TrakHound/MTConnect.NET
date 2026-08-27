---
title: 'v7 migration: CuttingTools Measurement.Code relocation'
description: The MTConnect SysML v2.7 model moves the Code property from the abstract Measurement base onto the concrete ToolingMeasurement subclass. MTConnect.NET-Common's regenerated .g.cs surface follows suit and removes Measurement.Code.
---

# v7 migration: CuttingTools `Measurement.Code` relocation

Prior to v7, [`MTConnect.Assets.CuttingTools.Measurement`](/api/MTConnect.Assets.CuttingTools.Measurement) — the abstract base class every specific cutting-tool measurement inherits — carried a `Code` property. That was a legacy of the pre-2.7 SysML model, which declared `Code` on `Measurement` and let `ToolingMeasurement` inherit it.

MTConnect SysML v2.7 relocates `Code` off the base and onto the concrete [`ToolingMeasurement`](/api/MTConnect.Assets.CuttingTools.ToolingMeasurement) subclass. v7 of MTConnect.NET-Common regenerates the C# surface from the v2.7 XMI and follows the same relocation: `Measurement.Code` is removed, and `ToolingMeasurement.Code` becomes the sole home for the property.

## What changed

| Symbol | Pre-v7 | v7 |
| --- | --- | --- |
| `MTConnect.Assets.CuttingTools.Measurement.Code` | Present on the abstract base | **Removed** |
| `MTConnect.Assets.CuttingTools.ToolingMeasurement.Code` | Inherited from `Measurement` | Declared directly on `ToolingMeasurement` |

`Measurement.DescriptionText` is also reworded to match the v2.7 XMI phrasing; the class-level description string changes but the API shape does not.

Every concrete measurement class that inherited `Code` transitively via `Measurement` (`CuttingDiameterMeasurement`, `WeightMeasurement`, `WiperEdgeLengthMeasurement`, and the rest of the 40-odd `Measurements.*` family) loses the `Code` property. `ToolingMeasurement` and its subclasses are the only measurement family that continues to carry it, matching the SysML v2.7 shape.

## Migration table

| Before (pre-v7) | After (v7) |
| --- | --- |
| `CuttingDiameterMeasurement { Code = "x" }` — sets `Measurement.Code` | Not applicable — the property no longer exists on this class; drop the `Code` initialiser. |
| `WiperEdgeLengthMeasurement instance = …; instance.Code = "x";` | Not applicable — drop the assignment. |
| `Measurement m = …; string c = m.Code;` — reads through the base | Read `Code` off the concrete `ToolingMeasurement` instead: cast to `ToolingMeasurement` (or the concrete tooling-measurement class) and read from there. |
| `((Measurement)toolingMeasurement).Code` — up-cast read | `toolingMeasurement.Code` — the `ToolingMeasurement`-declared property is on the concrete type. |

Any code path that reached `Code` via the abstract `Measurement` type will fail to compile after upgrading. The compiler error points at the exact call sites; each one becomes either a delete (if the pre-v7 code was setting `Code` on a non-tooling measurement class where the value was silently ignored by the XML wire format) or a cast to the concrete `ToolingMeasurement` subclass.

## Why the relocation

The SysML v2.7 shape reflects how the wire format actually behaves: `Code` is a `ToolingMeasurement`-specific attribute in the XSD, and every non-tooling measurement class ignored the value at serialisation time. Hoisting the property onto the abstract base pre-v7 was a code-side convenience that let callers set `Code` on any measurement, but the value was dropped on write for every measurement family except `ToolingMeasurement`. Relocating the declaration onto the concrete subclass restores the pre-condition the spec always intended: only `ToolingMeasurement` (and its subclasses) carry a `Code`.

## Before / after — measurement author

```csharp
// Before (pre-v7) — Code is set on the base Measurement type;
// XML serialisation silently drops the value for non-tooling measurements
using MTConnect.Assets.CuttingTools.Measurements;

var diameter = new CuttingDiameterMeasurement
{
    Value = 12.0m,
    Code = "CD",           // silently dropped on XML write
};
```

```csharp
// After (v7) — Code is no longer available on non-tooling measurements;
// the pre-v7 code was a latent bug (the value never reached the wire).
using MTConnect.Assets.CuttingTools.Measurements;

var diameter = new CuttingDiameterMeasurement
{
    Value = 12.0m,
};
```

```csharp
// Tooling-measurement authors — Code stays available, but reads shift
// off the abstract base onto the concrete type.
using MTConnect.Assets.CuttingTools;

var tooling = new ToolingMeasurement
{
    Value = 42.0m,
    Code = "TM",           // still available; declared directly on ToolingMeasurement in v7
};

// Reading — pre-v7 abstract-base read
Measurement m = tooling;
string c = ((ToolingMeasurement)m).Code;   // cast to the concrete type
```

## See also

- [Assets — CuttingTool](/concepts/assets#cuttingtool) — the concept-level overview of the CuttingTool asset family, including the note that `Code` lives on `ToolingMeasurement` in v7.
- [`MTConnect.Assets.CuttingTools.Measurement`](/api/MTConnect.Assets.CuttingTools.Measurement) — the abstract base with `Code` now removed.
- [`MTConnect.Assets.CuttingTools.ToolingMeasurement`](/api/MTConnect.Assets.CuttingTools.ToolingMeasurement) — the concrete subclass that carries `Code` directly.
- [TrakHound/MTConnect.NET#224](https://github.com/TrakHound/MTConnect.NET/pull/224) — the PR that regenerated the CuttingTools `.g.cs` surface from v2.7 XMI and performed the relocation.
