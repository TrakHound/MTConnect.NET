---
title: 'v7 migration: ValidationResult consolidation'
description: Three pre-v7 per-domain validation result structs collapse into one universal MTConnect.ValidationResult carrying IsValid, Code, and Message.
---

# v7 migration: ValidationResult consolidation

Prior to MTConnect.NET v7, three near-identical validation result structs lived side by side in the codebase — one per "domain" that produced validation outcomes. Each carried the same shape (`IsValid` + `Message`) but a different type identity, forcing handler authors to write the same shape three times and the agent to allocate per-domain wrappers at every emit site.

v7 collapses all three into a single universal type — `MTConnect.ValidationResult` — and adds a new `Code` property so subscribers can branch on the failure category without parsing the message string. Every member of the [agent validation events](/concepts/agent-validation-events) family now carries the universal type.

## What changed

| Pre-v7 type | Namespace | Replaced by |
| --- | --- | --- |
| `AssetValidationResult` | `MTConnect.Assets` | [`MTConnect.ValidationResult`](/api/MTConnect.ValidationResult) |
| `ValidationResult` (DataItems) | `MTConnect.Devices.DataItems` | [`MTConnect.ValidationResult`](/api/MTConnect.ValidationResult) |
| `ObservationValidationResult` | `MTConnect.Observations` | [`MTConnect.ValidationResult`](/api/MTConnect.ValidationResult) |

The three legacy structs are removed from the public surface. Any handler signature that mentioned one of them is now phrased in terms of the universal `MTConnect.ValidationResult`.

## The new shape

`MTConnect.ValidationResult` is a `readonly struct` with three properties and a pair of static factory methods for the common cases:

```csharp
namespace MTConnect
{
    public readonly struct ValidationResult
    {
        public bool   IsValid { get; }
        public string Code    { get; }   // machine-readable discriminator
        public string Message { get; }   // human-readable description

        public ValidationResult(bool isValid, string message = null, string code = null);

        public static ValidationResult Valid();
        public static ValidationResult Invalid(string code, string message);
    }
}
```

The `Code` property is the only structural addition. Both legacy structs already had `IsValid` and `Message` — those move across unchanged.

### Why `Code`

The pre-v7 result types only carried a free-form `Message`. Subscribers wanting to react differently to different failure categories had to substring-match the message, which broke whenever the message text was tightened or localised. `Code` is a stable, lower-case-camelCase string the agent stamps on every failure (e.g. `DeviceNull`, `DeviceUuidMissing`). Subscribers branch on `Code`; logs render `Message`.

## Migration table

| Before | After |
| --- | --- |
| `using MTConnect.Assets; … AssetValidationResult` | `using MTConnect; … ValidationResult` |
| `using MTConnect.Devices.DataItems; … ValidationResult` | `using MTConnect; … ValidationResult` |
| `using MTConnect.Observations; … ObservationValidationResult` | `using MTConnect; … ValidationResult` |
| `new AssetValidationResult(false, "Missing SerialNumber")` | `ValidationResult.Invalid("SerialNumberMissing", "Missing SerialNumber")` (or `new ValidationResult(false, "Missing SerialNumber")` if you don't yet have a code) |
| `new ValidationResult(true)` | `ValidationResult.Valid()` |
| handler signature `(IAsset, AssetValidationResult)` | handler signature `(IAsset, ValidationResult)` |

The constructor remains compatible by parameter order — `new ValidationResult(bool, string)` still works — so the minimum migration for a callsite is to drop the legacy type name and the `using` directive.

## Before / after — handler

```csharp
// Before — three different result types, three different using statements
using MTConnect.Assets;             // AssetValidationResult
using MTConnect.Observations;       // ObservationValidationResult
using MTConnect.Devices.DataItems;  // ValidationResult (DataItems variant)

agent.InvalidAssetAdded += (asset, AssetValidationResult result) =>
    Log.Warn($"asset {asset.AssetId}: {result.Message}");

agent.InvalidObservationAdded += (deviceUuid, dataItemKey, ObservationValidationResult result) =>
    Log.Warn($"[{deviceUuid}] {dataItemKey}: {result.Message}");
```

```csharp
// After — one type, one using, plus a stable Code for branching
using MTConnect;                    // universal ValidationResult

agent.InvalidAssetAdded += (asset, result) =>
    Log.Warn("asset {AssetId} failed ({Code}): {Message}",
             asset.AssetId, result.Code, result.Message);

agent.InvalidObservationAdded += (deviceUuid, dataItemKey, result) =>
    Log.Warn("[{DeviceUuid}] {DataItemKey} failed ({Code}): {Message}",
             deviceUuid, dataItemKey, result.Code, result.Message);
```

## Before / after — Asset author

`Asset.IsValid(Version)` and its overrides previously returned `AssetValidationResult`; they now return the universal `ValidationResult`:

```csharp
// Before
public override AssetValidationResult IsValid(Version mtconnectVersion)
{
    if (string.IsNullOrEmpty(SerialNumber))
        return new AssetValidationResult(false, "SerialNumber property is Required");

    return new AssetValidationResult(true);
}
```

```csharp
// After
public override ValidationResult IsValid(Version mtconnectVersion)
{
    if (string.IsNullOrEmpty(SerialNumber))
        return ValidationResult.Invalid("SerialNumberMissing",
                                        "SerialNumber property is Required");

    return ValidationResult.Valid();
}
```

The structural change is type-name only; the per-asset validation logic is unaffected.

## See also

- [Agent validation events](/concepts/agent-validation-events) — the family of `Invalid*Added` events that now carry the universal type.
- [`MTConnect.ValidationResult`](/api/MTConnect.ValidationResult) — full API reference for the universal type.
- [TrakHound/MTConnect.NET#169](https://github.com/TrakHound/MTConnect.NET/pull/169) — the PR that performed the consolidation, added `InvalidDeviceAdded`, and introduced `ValidateDevice` as a pre-flight helper.
