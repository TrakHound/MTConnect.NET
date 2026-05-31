# Known divergences from the Standard

The MTConnect Standard is published through four artifacts — SysML XMI, XSDs, prose, cppagent reference — and they occasionally disagree. This page catalogues the divergences `MTConnect.NET` has identified, where they were filed upstream for resolution, and which artifact the library currently follows.

Every divergence has been filed against the standard body's tracker at [projects.mtconnect.org](https://projects.mtconnect.org/). The Redmine ticket numbers below are the canonical references; visit them for the standard maintainers' resolution timeline.

## ASSET_COUNT default representation

**Disagreement**: SysML XMI / XSD vs cppagent.

The MTConnect Standard's three formal sources disagree on whether an `ASSET_COUNT` DataItem defaults to `representation="VALUE"` (scalar) or `representation="DATA_SET"`:

- **SysML XMI** declares `AssetCount.result : integer` as a `uml:DataType`, not a `uml:Class`. The spec's own rule ("if `DataItem::representation` is not specified, it MUST be determined to be `VALUE`") yields **VALUE**.
- **XSD** declares the canonical `AssetCount` element as `substitutionGroup='IntegerEvent'` with `xs:simpleContent` — a scalar. Parallel `AssetCountDataSet` and `AssetCountTable` element declarations co-exist as opt-in alternates. → **VALUE**.
- **cppagent reference** `Agent::verifyDevice` auto-injects `representation="DATA_SET"` when no `ASSET_COUNT` DataItem is declared and `schemaVersion >= 2.0`. → **DATA_SET**.

**Library posture**: follows SysML XMI and XSD — `ASSET_COUNT` defaults to `VALUE`. The library does NOT auto-inject `representation="DATA_SET"` on devices that omit `ASSET_COUNT`. Consumers who need cppagent-parity DATA_SET behavior can author the DataItem with `representation="DATA_SET"` explicitly in the device model.

**Filed**: [Redmine #3890](https://projects.mtconnect.org/issues/3890) (Agent Working Group).

## v2.6 SysML XMI byte-identical to v2.7

**Disagreement**: spec release versioning.

The published v2.6 SysML XMI file (`MTConnectSysMLModel_V2.6.xml`) is byte-identical to v2.7. v2.6 and v2.7 should differ — the spec release cadence assumes each major increment has at least one substantive XMI change — and the byte-identical publication suggests a release-engineering mistake at the standard body, not a deliberate "no-change" version.

**Library posture**: `MTConnectVersions.Version26` and `Version27` are both shipped as distinct constants because the spec body advertises both as released versions. Any type's `MinimumVersion = Version26` is treated as equivalent to `Version27` for serialization gating.

**Filed**: [Redmine #3892](https://projects.mtconnect.org/issues/3892) (SysML Model Related Issues).

## DataSet vs Table result class disagreement

**Disagreement**: SysML XMI vs XSD.

For DataItems with `representation="DATA_SET"` or `representation="TABLE"`, the SysML XMI declares the `result` property's type as a single `DataSet` class shared between the two representations, while the XSD declares two distinct complex types (`DataSetType` and `TableType`) with non-overlapping element shapes.

**Library posture**: follows the XSD's two-class shape — `DataSetEntry` and `TableEntry` are distinct classes ([`MTConnect.Observations.DataSetEntry`](/api/MTConnect.Observations/DataSetEntry), [`MTConnect.Observations.TableEntry`](/api/MTConnect.Observations/TableEntry)). The on-the-wire output is XSD-validated; consumers parsing the XML never see ambiguity.

**Filed**: [Redmine #3893](https://projects.mtconnect.org/issues/3893) (SysML Model Related Issues).

## Feature persistent-id typo across SysML and XSD

**Disagreement**: spec internal consistency.

A typo in the property name `persistentId` is preserved as `persisitentId` across both the SysML XMI and the XSDs from v1.4 through the latest published release. The typo is in the same character position on both artifacts, suggesting a copy-and-paste origin rather than independent transcription.

**Library posture**: the library serializes the typo as-published, with no silent correction. Consumers receive `persisitentId` on the wire because that is what the XSD requires; a future spec release that fixes the typo will trigger a coordinated bump on both sides.

**Filed**: [Redmine #3894](https://projects.mtconnect.org/issues/3894) (SysML Model Related Issues).

## Component class hierarchy SysML vs XSD

**Disagreement**: SysML XMI vs XSD.

A subset of Component classes have a different parent class in SysML than they do in the XSD. For example, `LinearComponent` derives from `Axis` in SysML and from `AxisType` (the XSD-only intermediate) in the XSD. The hierarchy difference does not affect the wire shape — both forms produce the same element names — but it affects type-system code generators that compile against both artifacts.

**Library posture**: the source generator follows SysML for the C# class hierarchy and lets the XSD handle the wire shape independently. The generated `LinearComponent` inherits from `AxisComponent` in C#, and the XSD-side `LinearType` deriving from `AxisType` is handled at serialization time by the XML formatter.

**Filed**: [Redmine #3891](https://projects.mtconnect.org/issues/3891) (SysML Model Related Issues).

## JSON wire format not normative — no JSON Schema generator

**Disagreement**: missing normative artifact.

The MTConnect Standard's `Part_5` prose describes the JSON wire format descriptively, but no normative JSON Schema is published. Consumers parsing JSON output today rely on the cppagent reference's byte shape rather than a versioned schema. The library follows cppagent for the JSON-CPPAGENT codec but cannot validate its output against a missing-from-the-standard schema.

**Library posture**: the JSON-CPPAGENT codec is byte-tested against golden cppagent output; the JSON v1 codec is regression-tested against itself. A normative JSON Schema, if the standard body publishes one in a future release, would convert the byte-parity tests into schema-validated tests.

**Filed**: [Redmine #3889](https://projects.mtconnect.org/issues/3889) (Agent Working Group).

## How divergences are tracked

Every divergence flows through the same pipeline:

1. **Discovery** — a test, a code review, or a consumer report surfaces a spec inconsistency.
2. **Verification** — the divergence is reproduced against every published version of the affected artifact.
3. **Source citation** — the exact line numbers / element IDs / prose sections are recorded for every artifact involved.
4. **Library posture decision** — the library follows the artifact that wins under the [source-of-truth hierarchy](/compliance/spec-cross-references#source-of-truth-hierarchy).
5. **Upstream filing** — a Redmine ticket is filed against the standard body.
6. **Audit trail** — the ticket URL is referenced from this page and from the test fixtures that pin the library's posture.

When the standard body resolves a ticket (either accepting the library's posture or amending the standard), the library follows the resolution. Where the resolution requires a wire-format change, the change is gated on the spec version the resolution targets (existing wire output for earlier versions is unchanged).

## Where to next

- [Per-version matrix](/compliance/per-version-matrix) — version-by-version envelope coverage.
- [Wire-format compliance](/compliance/wire-format) — XML / JSON / SHDR posture.
- [Spec cross-references](/compliance/spec-cross-references) — the source-of-truth hierarchy that drives every posture decision above.
- [Test harness](/compliance/test-harness) — how the divergence-pinning tests run locally.
