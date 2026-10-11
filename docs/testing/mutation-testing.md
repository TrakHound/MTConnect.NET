# Mutation testing — Stryker.NET

Stryker.NET is the mutation-testing framework adopted as the Ultrareview coverage-quality gate in PR #233 (per user D3 directive, 2026-08-20). Mutation testing complements line / branch coverage by mutating the code under test and asserting that at least one test fails per mutation — surviving mutants are gaps the coverage report cannot see.

## Configuration

`stryker-config.json` at the repo root pins the entry-point project + test project:

```jsonc
// Baseline mutation score is 7.75% on 2026-08-20 (see the JSONC header
// on the shipped stryker-config.json for the full provenance block).
{
  "stryker-config": {
    "project": "MTConnect.NET-Common.csproj",
    "solution": "MTConnect.NET.sln",
    "test-projects": ["tests/MTConnect.NET-Common-Tests/MTConnect.NET-Common-Tests.csproj"],
    "target-framework": "net8.0",
    "reporters": ["progress", "cleartext", "html", "json"],
    "thresholds": { "high": 8, "low": 5, "break": 5 },
    "concurrency": 4,
    "mutation-level": "Complete",
    "mutate": [
      "!**/*.g.cs",
      "!libraries/MTConnect.NET-Common/Assets/**/*.g.cs",
      "!libraries/MTConnect.NET-Common/Devices/**/*.g.cs",
      "!libraries/MTConnect.NET-Common/Observations/**/*.g.cs"
    ],
    "ignore-mutations": ["Regex"]
  }
}
```

Key choices:

- **Target: `MTConnect.NET-Common`** — the largest hand-authored surface. Subsequent adoptions extend the roster (`MTConnect.NET-Generator-Tests`, `MTConnect.NET-XML`, `MTConnect.NET-JSON-cppagent`) once the Common project reaches zero surviving mutants.
- **Reporters** — `progress` + `cleartext` for the terminal replay, `html` for browsable maintainer report, `json` for CI ingestion. No `dashboard` reporter (no external API surface).
- **Thresholds: 8 / 5 / 5** — pinned above the 7.75 % baseline established on 2026-08-20 (Stryker.NET v4.16.0, Regex mutator ignored). `break: 5` and `low: 5` sit below the baseline so a baseline-conforming run passes CI; `high: 8` sits above so the same run reports yellow rather than green, keeping visible pressure on the phased campaign at TrakHound/MTConnect.NET#242 to raise the floor (20 -> 40 -> 60 -> 80 %+). The long-term Ultrareview target under CONVENTIONS §1.0d-trigies-septdecies remains 100 %.
- **Mutate excludes: every `*.g.cs`** — generator output is not hand-authored code; mutating it produces meaningless results. Fidelity of the generator emission is guarded by `tests/MTConnect.NET-Generator-Tests/` byte-identity + delta cross-verify tests.
- **Concurrency: 4** — matches the bluefin CPU budget without oversubscribing.

## Running locally

Install the tool once per machine:

```bash
dotnet tool install -g dotnet-stryker
```

Then, from the repo root:

```bash
dotnet stryker
```

A typical run takes 30 – 60 minutes on the `MTConnect.NET-Common` surface (four-way concurrency, ~250 hand-authored source files). Results land under `StrykerOutput/<timestamp>/` — open `reports/mutation-report.html` for the browsable report and `mutation-report.json` for automation.

## Running in CI

The Stryker gate is not wired into `dotnet.yml` yet; the config lands standalone in PR #233 with the runner integration deferred to a follow-up PR per user directive. When wired, the workflow shape is `dotnet stryker --config-file stryker-config.json --reporter json` on a nightly cron + label-triggered on-demand, uploading `StrykerOutput/**/*` as an artefact and failing the job on `--break-at 5` (the pinned baseline gate — see the top-of-file JSONC comment in `stryker-config.json` and TrakHound/MTConnect.NET#242 for the phased raise).

## Handling surviving mutants

Every surviving mutant has three acceptable dispositions:

1. **Killed by a new test.** Add a test that would fail if the mutation were shipped, land it in the same PR that introduced the surface. This is the default disposition — 99 % of surviving mutants deserve a matching test.
2. **Explicit exclusion with rationale.** Add the mutant to `stryker-config.json`'s `mutate.excluded-mutations` list (or use a `// Stryker disable next-line <mutator>` pragma at the source site) with a comment explaining why the mutation is spec-equivalent / performance-equivalent / defensively-unreachable. Rare — needs code-level rationale.
3. **Deferred to the coverage-quality campaign.** Until TrakHound/MTConnect.NET#242 raises the pinned break threshold in step, survivors that keep the score at or above the pinned break (5 %) do not block merge; catalog them per subsystem in the #242 phase plan. This disposition is a scoped transitional accommodation, not a general-purpose escape hatch — every survivor still needs an eventual disposition 1 or 2.

Zero surviving mutants (or fully-justified exclusions) remains the long-term merge gate; the pinned 7.75 % baseline is the interim floor per #242.

## References

- Stryker.NET: <https://stryker-mutator.io/docs/stryker-net/introduction/>
- Configuration options: <https://stryker-mutator.io/docs/stryker-net/configuration/>
- CONVENTIONS §1.0d-trigies-septdecies (Ultrareview coverage-quality gate)
- PR #233 adoption commit — `chore(tests): adopt Stryker.NET mutation-testing framework`
- TrakHound/MTConnect.NET#242 — phased coverage-quality campaign that raises the pinned thresholds toward 100 %
