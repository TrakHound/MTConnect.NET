# Version-matrix convention (topic-first single-file-per-topic layout)

Established by the Phase 1 DRY-generator consolidation (PR TrakHound/MTConnect.NET#233, 2026-08-19). Enforced permanently by [`DryGenerator/PerVersionFolderProhibitionTests.cs`](https://github.com/TrakHound/MTConnect.NET/blob/master/tests/MTConnect.NET-Common-Tests/DryGenerator/PerVersionFolderProhibitionTests.cs); parity between pre- and post-migration assertions is pinned by [`DryGenerator/AssertionParityTests.cs`](https://github.com/TrakHound/MTConnect.NET/blob/master/tests/MTConnect.NET-Common-Tests/DryGenerator/AssertionParityTests.cs).

## The rule

A test fixture's name and folder must reflect the **topic** under test, never the spec version that introduced it.

- Correct: `tests/MTConnect.NET-Common-Tests/Devices/DataItems/DataItemTypeTests.cs`, `Devices/Components/ComponentTests.cs`, `Enums/EnumArmTests.cs`.
- Prohibited: `tests/MTConnect.NET-Common-Tests/V2_6_V2_7/*.cs`, `V2_8/DataItemTypeTests.cs`, `V2_8ComponentAndEnumTests.cs`. The prohibition guard flags any directory matching `V<N_M>/` or any fixture class matching `V<N_M>*Tests`.

Version becomes a **parameter**, not a **container**. A single fixture file houses every version's assertions for that topic; the fixture iterates over `MTConnectVersionMatrix.All` and gates each assertion with `Assume.That`.

## How to add a fixture for a new spec version

1. Ensure the version constant exists on [`MTConnect.MTConnectVersions`](https://github.com/TrakHound/MTConnect.NET/blob/master/libraries/MTConnect.NET-Common/MTConnectVersions.cs) (for example `public static readonly Version Version28 = new(2, 8);`). The matrix (`MTConnectVersionMatrix.All`) discovers it via reflection — no per-test edit is required.
2. Find the topic file the new element belongs to (or create a new one under `Devices/`, `Observations/`, `Enums/`, or `Assets/`). Never create a `V2_8/` folder.
3. Add a method with the matrix source and the version gate:

   ```csharp
   /// <summary>Pins the behavior expressed by the test name: my new spec type constructs with correct metadata.</summary>
   /// <param name="v">The MTConnect Standard version under test.</param>
   [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
   public void MyNewSpecType_constructs_with_correct_metadata(Version v)
   {
       Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version28),
           "MyNewSpecType was introduced in MTConnect v2.8.");

       var d = new MyNewSpecTypeDataItem();
       Assert.That(d.Type, Is.EqualTo("MY_NEW_SPEC_TYPE"));
       // …
   }
   ```

   Rows below the floor surface as `Inconclusive` in the test explorer (they neither pass nor fail); rows at or above the floor exercise the assertion.
4. Update the corresponding `docs/testing/v<N>-<M>.md` compliance matrix to point at the new method.
5. Do **not** name the method with a version prefix / suffix (`V2_8_*`, `*_in_v2_8`). Version is encoded in the matrix parameter, not the method name.

## When to keep a plain `[Test]` (no matrix)

Assertions that pin **constant-value invariants** — for example `MTConnectVersions.Version27 == new Version(2, 7)` — are not per-version behavior. Keep them as plain `[Test]` (see `tests/MTConnect.NET-Common-Tests/MTConnectVersionsTests.cs`). The prohibition guard does not flag topic-file `[Test]` methods; only fixture-class name and folder shape matter.

## Historical anchors

`PerVersionFolderProhibitionTests.HistoricalAnchors` is an allowlist for deliberately-pinned fixtures (for example a `CppAgentParityWorkflowTests` pinned to a specific spec version for spec-fidelity reasons). Each entry must include a rationale comment. At HEAD the list is empty — introducing a legitimate pin requires an edit visible in the PR diff, which reviewers must approve on the rationale.

## Migration-parity guard (`AssertionParityTests`)

`AssertionParityTests.MigrationMap` records the 34-entry baseline captured on 2026-08-19 (pre-migration methods under `V2_6_V2_7/`) and asserts every entry has a post-migration home. It is a permanent regression tripwire: accidental deletion of any of those 34 method names in the topic files fires the parity test immediately.

The `Every_baseline_assertion_has_a_post_migration_home` reflection sweep is cheap (≈ 5-30 ms on a warm CLR) and runs in the default `dotnet test` shape.

## References

- Migration PR: [TrakHound/MTConnect.NET#233](https://github.com/TrakHound/MTConnect.NET/pull/233).
- Prohibition guard: [`tests/MTConnect.NET-Common-Tests/DryGenerator/PerVersionFolderProhibitionTests.cs`](https://github.com/TrakHound/MTConnect.NET/blob/master/tests/MTConnect.NET-Common-Tests/DryGenerator/PerVersionFolderProhibitionTests.cs).
- Parity guard: [`tests/MTConnect.NET-Common-Tests/DryGenerator/AssertionParityTests.cs`](https://github.com/TrakHound/MTConnect.NET/blob/master/tests/MTConnect.NET-Common-Tests/DryGenerator/AssertionParityTests.cs).
- Matrix source: [`tests/MTConnect.NET-Common-Tests/TestHelpers/MTConnectVersionMatrix.cs`](https://github.com/TrakHound/MTConnect.NET/blob/master/tests/MTConnect.NET-Common-Tests/TestHelpers/MTConnectVersionMatrix.cs).
- Compliance-matrix pages: [`v2-6.md`](./v2-6.md), [`v2-7.md`](./v2-7.md).
