# Contributor documentation

Documentation for working on `MTConnect.NET` itself: how the tests, tooling, compliance checks and documentation site work. User documentation (installing, configuring and using the libraries and the agent) lives in [`docs/`](../docs/) and is published as the documentation site.

Start with [`CONTRIBUTING.md`](../CONTRIBUTING.md) for the checks to run before opening a pull request and the conventions for writing tests.

## Testing

- [Testing overview](testing.md)
- [CI workflows and local test harness](testing/workflows.md)
- [MTConnect v2.6 compliance matrix](testing/v2-6.md) and [v2.7 compliance matrix](testing/v2-7.md)
- [Compliance test harness](compliance/test-harness.md)
- [Spec sources and cross-references](compliance/spec-cross-references.md)

## Tools

- [`tools/test.sh`](tools/test-sh.md)
- [`tools/dotnet.sh`](tools/dotnet-sh.md)
- [SysML importer](tools/sysml-import.md)
- [Release builder](builder.md)

## Documentation site

- [Documentation site](docs-site.md): local builds, the base path, and the route check
- [House style](house-style.md)
