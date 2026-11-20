# Contributing to MTConnect.NET

Thank you for taking the time to contribute — issues, discussions, and pull requests are all welcome. This file collects the mechanical checks a contributor is expected to run locally before pushing; the substantive discussion of what to work on lives on the [MTConnect.NET issue tracker](https://github.com/TrakHound/MTConnect.NET/issues) and on the [MTConnect.NET Docs](https://www.trakhound.com/docs/mtconnect-net).

## Running the formatting gate locally

Every push to `master` and every non-draft pull request runs `dotnet format MTConnect.NET.sln --verify-no-changes` in CI (job `format`, defined in [`.github/workflows/dotnet.yml`](.github/workflows/dotnet.yml)). The gate rejects any whitespace / indentation / brace-placement drift, so it is worth reproducing the check before pushing:

```bash
dotnet restore MTConnect.NET.sln
dotnet format MTConnect.NET.sln                                     # autofix in place
dotnet format MTConnect.NET.sln --verify-no-changes --verbosity diagnostic   # dry-run, same as CI
```

The two `dotnet new mtconnect.net-agent` template projects live outside the solution and CI verifies them in dedicated steps; run the same commands against them if you have touched files under `templates/`:

```bash
dotnet restore templates/mtconnect.net-agent/MTConnect-NET-Agent-Template.csproj
dotnet restore templates/mtconnect.net-agent/content/MTConnect.NET-Embedded-Agent/Agent.csproj
dotnet format templates/mtconnect.net-agent/MTConnect-NET-Agent-Template.csproj --verify-no-changes --verbosity diagnostic
dotnet format templates/mtconnect.net-agent/content/MTConnect.NET-Embedded-Agent/Agent.csproj --verify-no-changes --verbosity diagnostic
```

See [`contributing/testing/workflows.md`](contributing/testing/workflows.md#job-0--format) for the complete gate description, the `--severity warn` rationale, and the tracked follow-ups (root `.editorconfig` + `global.json` SDK pin).

## Running the test suite locally

The default sweep runs unit and light-integration tests across the whole solution:

```bash
./tools/test.sh          # Linux / macOS / Git Bash
./tools/test.ps1         # PowerShell, all platforms
```

Add `--e2e` / `-E2E` (Docker required) to also run the `Category=E2E` and `Category=RequiresDocker` workflow fixtures. `./tools/test.sh --help` (or `./tools/test.ps1 -?`) prints the full flag listing. The CI matrix and per-category filter rationale are documented on [`contributing/testing/workflows.md`](contributing/testing/workflows.md).

## Writing tests

New and edited tests follow these conventions. Existing tests that predate them are renamed only when a file is being changed anyway; a rename-only pass adds churn without catching bugs.

### Projects

- One test project per production project, named `MTConnect.NET-<Area>-Tests` and placed directly under `tests/` (for example `MTConnect.NET-SHDR-Tests` tests `libraries/MTConnect.NET-SHDR`).
- [`tests/Directory.Build.props`](tests/Directory.Build.props) applies to every project under `tests/`. Do **not** add version-pinned `PackageReference`s for `Microsoft.NET.Test.Sdk`, `NUnit`, `NUnit3TestAdapter` or `coverlet.collector` to a test `.csproj`; the props file adds them to every project whose name ends in `-Tests`, at one shared version.
- Package versions for the whole repository live in [`Directory.Packages.props`](Directory.Packages.props) (NuGet central package management). A `PackageReference` in a `.csproj` has no `Version`; add a `PackageVersion` for a new package there, and bump versions there. Use `VersionOverride` only when one project genuinely needs a different version.
- NUnit is the test framework. `MTConnect.NET-Integration-Tests` is the one xunit project (it sets `<TestFramework>xunit</TestFramework>`); do not add new xunit projects.
- Shared helpers used by more than one test project go in [`tests/Shared/`](tests/Shared) (namespace `MTConnect.Tests.Shared`), not in a copy per project.

### Namespaces and files

- Namespace: `MTConnect.Tests.<Area>`, plus the folder path below the project root, so `tests/MTConnect.NET-XML-Tests/Devices/Configurations/Foo.cs` is `MTConnect.Tests.XML.Devices.Configurations`. The props file sets `RootNamespace` to this, so the IDE's "new class" template produces the right namespace. Never put test code in a product namespace (`MTConnect.Devices`, `MTConnect.Shdr`, ...).
- Group files by feature folder that mirrors the production layout (`Devices/`, `Streams/`, `Assets/`, `Formatters/`), not by origin (`Regressions/`, `IssueNNN_*`). A regression test belongs in the fixture for the feature it protects; link the issue in a comment.
- One fixture class per file. The class is `<Subject>Tests` and the file is named after the class (`ShdrAdapterTests.cs`). Mark NUnit fixtures with `[TestFixture]`.
- Test-only infrastructure (fakes, builders, fixture loaders) goes under a `TestHelpers/` folder, which is excluded from coverage.

### Test methods

- Name tests `Subject_expected_behaviour_in_lower_case`, optionally followed by `_when_condition`:
  - `MapControllerMode_maps_known_modes`
  - `Constructor_with_source_header_copies_schemaVersion`
  - `Adapter_sends_ping_reply_when_agent_pings`

  The first segment is the member or scenario under test in PascalCase; the rest is a lower-case sentence. Avoid `Test1`, `RunXml` and `ShouldX`-style names.
- Arrange / act / assert. Name the values `actual` and `expected`.
- Use NUnit's constraint model: `Assert.That(actual, Is.EqualTo(expected))`. Don't use `Assert.AreEqual` / `Assert.IsTrue` (classic model), and never use `if (...) Assert.Pass(); else Assert.Fail();`: it throws away the actual and expected values that NUnit prints on failure.
- Use `[TestCase]` / `[TestCaseSource]` for data variations instead of copy-pasted tests, and use `Assert.Multiple` when one test checks several related properties.
- Don't sleep to wait for asynchronous work. Poll the condition with a deadline (or `Is.True.After(...)`), and connect to a listener only after it signals it is listening; fixed sleeps are the main source of flaky failures on shared CI runners.
- Bind network tests to port `0` (let the OS choose) and read the bound port back, rather than picking a "free" port and hoping it stays free.
- Tag slow or environment-dependent tests with the categories described in [`contributing/testing/workflows.md`](contributing/testing/workflows.md) (`E2E`, `RequiresDocker`), so CI can filter them.

## Opening a pull request

- Keep the PR body focused on the user-facing diff — what changed, why it changed, and how a reviewer can verify it. Internal process detail belongs in the commit trailer or in follow-up comments, not in the description.
- Every push re-runs the full CI matrix on non-draft PRs. Leave a PR in draft while it is still being iterated; flip it to ready when it is ready for review.
- If your change touches the `templates/` tree, please also run the template-project format commands above — the CI gate will catch drift, but reproducing locally saves a red-CI round-trip.

Thanks again for your interest in improving MTConnect.NET.
