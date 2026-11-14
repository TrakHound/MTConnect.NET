# Contributor documentation

Documentation for working on `MTConnect.NET` itself: how the tests, tooling and compliance checks work. User documentation (installing, configuring and using the libraries and the agent) is published as the [MTConnect.NET Docs](https://www.trakhound.com/docs/mtconnect-net) on www.trakhound.com.

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

## Documentation

- [Documentation](docs-site.md): where the documentation lives, the generated reference pages, and the redirects from the old documentation site
