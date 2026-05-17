# Compliance test harness

`MTConnect.NET` ships a compliance tier in its test suite — a battery of tests that validate the library's behavior against the MTConnect Standard's published artifacts (XSDs, SysML XMI, cppagent reference output). This page walks through running the tier locally, interpreting its output, and adding new pinning tests.

## What the tier covers

The compliance tier validates four claim levels:

- **XSD validation** — every wire-format output of every Streams / Devices / Assets / Error envelope at every spec version v1.0 through v2.7 validates against the matching `MTConnect<Envelope>_<version>.xsd`.
- **SysML XMI parity** — every type's `MinimumVersion` / `MaximumVersion` / property set matches the SysML model.
- **cppagent JSON parity** — every JSON-CPPAGENT and JSON-CPPAGENT-MQTT output is byte-identical to a captured cppagent reference output for the same model.
- **SHDR prose conformance** — every example SHDR line in `Part_5.0` round-trips through the SHDR parser and writer.

The tier is opt-in. The default `dotnet test` run skips it because the embedded XSDs make the tier slower than the unit + integration tiers.

## Running the tier

The repo's wrapper script handles tier selection:

```sh
./tools/test.sh -c
# or
./tools/test.sh --compliance
```

The flag flips the `RUN_COMPLIANCE` gate in `tools/test.sh`. The script then:

1. Runs the default unit + integration tier.
2. Enumerates every `*.csproj` under `tests/Compliance/`.
3. Runs `dotnet test --configuration Release --collect:"XPlat Code Coverage"` on each.
4. Writes results to `TestResults/<csproj-name>/`.
5. Runs `reportgenerator` to produce a combined coverage report at `coverage-report/`.

The Windows-side equivalent is `./tools/test.ps1 -Compliance`.

To run just the compliance tier without the unit tier, pass the `--only` filter:

```sh
./tools/test.sh -c --only Compliance
```

## What you should see

A successful run prints, per compliance project:

```text
Passed!  - Failed:     0, Passed:   <N>, Skipped:     0, Total:   <N>
```

The combined coverage report at `coverage-report/Summary.txt` shows line, branch, and method coverage. The compliance tier targets 100 % of the wire-format codecs and the version-aware serializers.

## Failure categories

When a compliance test fails, the failure category is encoded in the test class name and the assertion message:

- **`XsdValidationTests_<envelope>_<version>`** — output failed XSD validation. The assertion message includes the XSD line number that rejected the output. Resolution: fix the codec; the XSD is the wire-shape authority. See [Troubleshooting: XSD validation failures](/troubleshooting/xsd-validation-failures) for the diagnostic flow.
- **`SysMLParityTests_<class>`** — a class's `MinimumVersion` or property set drifted from the SysML model. Resolution: re-run the source generator against the latest XMI; the `.g.cs` files should regenerate to match.
- **`CppAgentParityTests_<format>`** — JSON output diverged from cppagent's reference. Resolution: inspect the byte diff; either fix the codec (if the divergence is unintentional) or update the captured golden file and document the divergence at [Known divergences](/compliance/known-divergences).
- **`ShdrConformanceTests`** — SHDR parser or writer failed a `Part_5.0` example. Resolution: fix the SHDR codec; the spec prose is the authority.

## Interpreting a typical run

Sample output for one envelope at one version:

```text
[12:34:56 INF] Running XsdValidationTests_Streams_v2_5...
[12:34:56 INF] Loading MTConnectStreams_2.5.xsd from embedded resources.
[12:34:56 INF] Validating golden fixture streams_v2_5_minimal.xml...
[12:34:56 INF] Validating golden fixture streams_v2_5_dataset.xml...
[12:34:56 INF] Validating golden fixture streams_v2_5_condition.xml...
[12:34:56 INF] Validating golden fixture streams_v2_5_table.xml...
Passed!  - Failed:     0, Passed:     4, Skipped:     0, Total:     4
```

Per envelope per version, the suite runs the same battery of golden fixtures (minimal, with-dataset, with-condition, with-table, with-timeseries, with-asset-events). A version-introduction regression (a type that should be elided at v1.0 but appeared in the output) fails the golden-fixture comparison loudly.

## Adding a new pinning test

When the library asserts a new spec behavior — a type added in a newer spec version, a Known divergence resolution, a wire-format change — add a test to the relevant compliance project. The test class:

```csharp
using NUnit.Framework;

namespace MTConnect.Compliance.Tests;

[TestFixture]
public class MyNewSpecAssertion
{
    // Source: SysML class <X> introduced at version V.
    // https://github.com/mtconnect/mtconnect_sysml_model/blob/v<V>/MTConnectSysMLModel_V<V>.xml
    // Cross-reference: Part_3.0 prose §N.
    // https://docs.mtconnect.org/
    [Test]
    public void Class_X_MinimumVersion_Is_V()
    {
        var minVer = typeof(MTConnect.Devices.X).GetField("DefaultMinimumVersion",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.GetValue(null) as System.Version;

        Assert.That(minVer, Is.EqualTo(new System.Version(V_major, V_minor)));
    }
}
```

The test must cite its source — XMI / XSD / prose URL — in the fixture comment. Tests without source references are blocked at review.

## Running the tier in CI

The repo's CI workflow runs the unit + integration tier on every PR and the full compliance + E2E tier nightly. The compliance tier is gated in CI by the same `--compliance` flag, set as an environment variable. PRs that touch wire-format code paths trigger the full tier on the PR itself.

## Where to next

- [Per-version matrix](/compliance/per-version-matrix) — which spec versions the tier exercises.
- [Wire-format compliance](/compliance/wire-format) — what each format's compliance claim level means.
- [Spec cross-references](/compliance/spec-cross-references) — the citation pattern every test fixture follows.
- [Troubleshooting: XSD validation failures](/troubleshooting/xsd-validation-failures) — when the XSD validator rejects a model.
