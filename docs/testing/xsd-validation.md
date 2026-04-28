# XSD validation — strategy, scope, and known gaps

This page describes how the L1 layer of the compliance harness validates
each shipped MTConnect XSD, and which parts of the validation gate require
follow-up work.

## What L1 does

`tests/Compliance/MTConnect-Compliance-Tests/L1_XsdValidation/` parametrically
loads every official MTConnect XSD shipped under
`tests/Compliance/MTConnect-Compliance-Tests/Schemas/v*/` and verifies that
each schema compiles cleanly. The XSD set covers v1.0 through the current
ceiling and is bundled as embedded resources in the test assembly so
adding a new version directory automatically extends coverage.

Two test suites live side-by-side:

- `[Category("XsdLoadStrict")]` — the stock `System.Xml.Schema.XmlSchemaSet`
  (.NET BCL) baseline. The BCL implements XSD 1.0 only and forbids
  cross-schema imports unless an `XmlResolver` is provided.
- `[Category("XsdValidate")]` — same parametric surface, but with two
  additional resolvers wired up: an embedded-resource resolver that streams
  any sibling `<xs:include>` / `<xs:import>` target out of the test assembly
  manifest, and a pre-load step that adds the W3C XLink XSD into the schema
  set before any MTConnect XSD that references the
  `http://www.w3.org/1999/xlink` namespace.

The two categories are filterable independently. CI runs `XsdValidate` as
the active gate; `XsdLoadStrict` remains a "this is what the BCL alone
does" reference, useful for tracking how the spec's XSD-1.1 adoption
diverges from the BCL's XSD-1.0 ceiling.

## XLink import

Every MTConnect XSD from v1.5 onwards declares
`<xs:import namespace="http://www.w3.org/1999/xlink" schemaLocation="xlink.xsd"/>`,
but the published spec bundles do not ship `xlink.xsd` next to the
MTConnect XSDs. The W3C XLink 1.1 XSD (`http://www.w3.org/1999/xlink.xsd`)
is bundled here under `Schemas/W3C/xlink.xsd` and pre-loaded into the
schema set for any MTConnect XSD that references it. Provenance is
recorded in `Schemas/W3C/PROVENANCE.md`.

## XSD 1.1 features in the spec

54 of 122 official MTConnect XSDs declare `vc:minVersion='1.1'` and use
features specific to XSD 1.1:

| XSD 1.1 feature | Where it appears | Affected XSDs |
|---|---|---|
| `notNamespace` attribute on `<xs:any>` / `<xs:anyAttribute>` | XSD 1.1 wildcard restriction | `Assets_*` v1.3-v2.5; `Devices_*` v1.3-v1.5 |
| `<xs:all>` particles with `maxOccurs > 1` | XSD 1.1 lifted the cardinality limit on `<xs:all>` particles | `Devices_*` v1.5+, `Streams_*` v1.5+ |
| `<xs:any>` placement extended into `<xs:group>` / `<xs:complexType>` model groups | XSD 1.1 broadens the legal placement of wildcards | several v1.7+ envelopes |

`System.Xml.Schema.XmlSchemaSet` (.NET BCL) implements XSD 1.0 only, so
these XSDs cannot load through the BCL path even after the XLink fix-up.

## Validator selection

Loading XSD 1.1 schemas in .NET requires a non-BCL processor. The two
candidates evaluated:

- **Saxon-HE 10.x** (`Saxon-HE` on NuGet, `lib/net35/`). MPL-2.0 licence.
  IKVM-cross-compiled from Java SaxonJ HE. **Does not ship XSD validation
  in the HE tier** — the Saxon feature matrix marks XML Schema 1.0 and 1.1
  validation as Saxon-EE-V minimum on Java SaxonJ, which is the underlying
  product the .NET package wraps. The `net35`-only target framework also
  prevents direct restore on a `net8.0` test project without a downlevel
  shim.
- **Saxon 11/12 SaxonCS** (Saxonica's native .NET product). XSD 1.1
  validation is included, but the entire SaxonCS .NET line is
  Enterprise-Edition-only — no free tier.

Until the XSD 1.1 validator decision lands (see follow-up tracker), the
`XsdValidate` category covers exactly the XSDs whose compile failures were
caused by the missing XLink import. XSD-1.1-only schemas remain on the
`XsdLoadStrict` baseline only and are not asserted as a green gate.

## Per-envelope validation

`SchemaLoadTests` only checks that each XSD compiles. Validating actual
library output against each version's XSD is the next L1 layer (out of
this page's scope). When that lands, every envelope kind
(`MTConnectDevices`, `MTConnectStreams`, `MTConnectAssets`,
`MTConnectError`) is built via the library's emitters using a fixture
device, then validated against the XSD for the version it advertises.

## Status

| Component | State |
|---|---|
| 122 XSDs bundled as embedded resources | done |
| BCL `XsdLoadStrict` baseline | done |
| W3C XLink 1.1 XSD bundled + provenance | done |
| Embedded-resource `XmlResolver` for sibling imports | done |
| `XsdValidate` category — XLink fix applied, BCL XSD 1.0 ceiling | done |
| XSD 1.1 validator (Saxon-EE / SaxonCS / alternative) | follow-up |
| Per-envelope validation matrix | follow-up |
