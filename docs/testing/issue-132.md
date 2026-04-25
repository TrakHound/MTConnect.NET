# Issue #132 — Auto-generated `ASSET_COUNT` DataItem must use `DATA_SET` representation

## 1. Defect + scope

The MTConnect Standard (Part 2, v2.0+) defines `ASSET_COUNT` as a `DATA_SET`
representation: a map of asset-type to the count of assets of that type
currently held in the agent's asset buffer. The `MTConnect.NET-Common`
agent auto-injects an `AssetCountDataItem` for every device whose
`assetBufferSize > 0`, but the injected DataItem inherits the generated
default `Representation = VALUE` and is therefore emitted on the Probe as
a scalar EVENT instead of a DataSet.

Pragmatic, narrow scope for this PR: stamp
`Representation = DataItemRepresentation.DATA_SET` on the auto-injected
`AssetCountDataItem` at the injection site in
`libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs` (private
`NormalizeDevice` method, near line 1157). User-declared `ASSET_COUNT`
DataItems in devices.xml are untouched.

Out of scope for this PR (tracked as follow-ups):

- Generator-side fix to `AssetCountDataItem.g.cs` (`DefaultRepresentation`
  is declared as `VALUE`; should be `DATA_SET` per the SysML model). The
  generator output belongs in the SysML-importer plan.
- Per-asset-type bookkeeping on the asset buffer
  (`GetCountsByType()`-style helper).
- Renaming the streamed observation from `AssetCount` to
  `AssetCountDataSet` and emitting the per-type map.
- MQTT Docker E2E and cppagent v2.7.0.7 parity scenarios.

## 2. Investigation (P1)

See `docs/testing/issue-132/phase-01-defect-scoping.md`.

## 3. Red tests (P2)

See `docs/testing/issue-132/phase-02-red-tests.md`.

## 4. Library fix (P3)

See `docs/testing/issue-132/phase-03-library-fix.md`.

## 5. Regression pins (P4)

See `docs/testing/issue-132/phase-04-regression-pins.md`.

## 6. E2E validation (P5)

Out of scope for this PR — recorded as follow-up. See
`docs/testing/issue-132/phase-05-e2e-validation.md`.

## 7. Campaign summary (P6)

See `docs/testing/issue-132/phase-06-finalisation.md`.
