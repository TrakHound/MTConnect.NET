# API reference

The API reference documents every public class, interface, record, struct, enum, and delegate in every shipped `MTConnect.NET` NuGet package. Each page is generated from the library's XML doc comments and the compiled assembly metadata, then surfaced under a flat `Namespace.Type.md` layout you can search from the top bar or browse from the sidebar.

## How the reference is generated

The pages under `docs/api/` are produced by [docfx](https://dotnet.github.io/docfx/) from the Release `net8.0` build of every shipped library. The build is reproducible — `bash docs/scripts/generate-api-ref.sh` rebuilds the libraries and regenerates the Markdown locally; the same script runs in CI ahead of every VitePress build.

The configuration lives at `docs/.docfx/docfx.json` and explicitly lists each library DLL the reference covers. Adding a new library is a two-line change there plus a row in the regen script.

## What each type page contains

- **Type signature**, namespace, the source assembly, base class, and implemented interfaces.
- **Every public member** — methods, properties, events, fields — with parameter docs, return docs, and exception docs sourced from the library's `<summary>` / `<param>` / `<returns>` / `<exception>` comments.
- **Inherited members** linked back to the declaring type.
- **Code samples** for any member whose XML doc comment includes an `<example>` block.

## Packages covered

| Library | Assembly | Audience |
| --- | --- | --- |
| `MTConnect.NET-Common` | `MTConnect.NET-Common.dll` | Agents, devices, components, data items, observations, assets, configurations. |
| `MTConnect.NET-DeviceFinder` | `MTConnect.NET-DeviceFinder.dll` | Network-scan helpers that locate live MTConnect agents on a subnet. |
| `MTConnect.NET-HTTP` | `MTConnect.NET-HTTP.dll` | HTTP client + server, REST endpoints, content negotiation. |
| `MTConnect.NET-JSON` | `MTConnect.NET-JSON.dll` | Legacy JSON v1 codec. |
| `MTConnect.NET-JSON-cppagent` | `MTConnect.NET-JSON-cppagent.dll` | cppagent-parity JSON v2 codec. |
| `MTConnect.NET-MQTT` | `MTConnect.NET-MQTT.dll` | MQTT client + broker integration, topic templates. |
| `MTConnect.NET-Protobuf` | `MTConnect.NET-Protobuf.dll` | Protobuf wire-format codec and gRPC plumbing. |
| `MTConnect.NET-Services` | `MTConnect.NET-Services.dll` | Long-running service host (Windows service / systemd) plumbing. |
| `MTConnect.NET-SHDR` | `MTConnect.NET-SHDR.dll` | SHDR adapters and clients. |
| `MTConnect.NET-SysML` | `MTConnect.NET-SysML.dll` | SysML XMI importer infrastructure consumed by the regen tool. |
| `MTConnect.NET-TLS` | `MTConnect.NET-TLS.dll` | TLS helpers shared by HTTP and MQTT. |
| `MTConnect.NET-XML` | `MTConnect.NET-XML.dll` | XML codec and XSD validation. |
| `MTConnect.NET` | `MTConnect.NET.dll` | Top-level umbrella package and convenience builders. |

`MTConnect.NET-HTTP-AspNetCore` is the only shipped library currently excluded from the reference. Its project targets `netcoreapp3.1`–`net7.0` and cannot resolve its `net8.0` `ProjectReference` dependencies under the current Debug build, so docfx has no assembly to load. Once the TFM matrix is widened to include `net8.0` the library joins the table here and the regen script picks it up automatically.

## Finding what you need

- Use the search box at the top of the site for a type or member by name.
- Use the left-hand sidebar to browse by namespace.
- If you arrived here from a narrative page, the link will resolve to the specific type and member that page references.

## Spec-version metadata

Every type added or deprecated under a specific MTConnect spec version carries `[MinimumVersion]` / `[MaximumVersion]` attributes. Those values appear in the type's XML doc summary; the docfx output preserves them. For the per-version spec-coverage matrix at the library level, see [Compliance](/compliance/).
