# Releases

TrakHound builds and publishes the releases: the NuGet packages, the Windows agent installer, the agent Docker images (`trakhound/mtconnect.net-agent`) and the GitHub releases. Contributors don't run any part of that; `dotnet build` against `MTConnect.NET.sln` is enough for normal development.

A release is built from a commit on this repository, so a few things here decide what it contains:

- **Which projects are published.** A project is packed only when it sets `<IsPackable>true</IsPackable>`; test, example and tool projects stay unpublished by default. `dotnet pack MTConnect.NET.sln -c Package` builds every published package from source, the same way a release does.
- **Package metadata and version.** Authors, license, icon, project URL and the version (`VersionPrefix` / `VersionSuffix`) are in the root [`Directory.Build.props`](../Directory.Build.props). Don't set them in a project file.
- **The package count.** The `release-pack` CI job packs the solution and fails when the number of packages differs from `EXPECTED_PACKAGES` in [`.github/workflows/dotnet.yml`](../.github/workflows/dotnet.yml). A PR that adds or removes a published package sets `IsPackable` on the project and updates `EXPECTED_PACKAGES` in the same change.
