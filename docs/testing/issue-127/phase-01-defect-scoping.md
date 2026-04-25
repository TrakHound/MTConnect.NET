# Phase 01 — Defect scoping

## Executed

Read-only audit confirming the defect surface against
`upstream/master` at the branch-cut SHA (3d6321ab).

### Defect origin sites

The library has **one** root cause and **six** redundant overwrites that
re-assign the same incorrect value.

Root cause — the four header builders in `MTConnectAgentBroker`:

| File | Line | Method |
|---|---|---|
| `libraries/MTConnect.NET-Common/Agents/MTConnectAgentBroker.cs` | 469 | `GetDevicesHeader` |
| `libraries/MTConnect.NET-Common/Agents/MTConnectAgentBroker.cs` | 493 | `GetStreamsHeader` |
| `libraries/MTConnect.NET-Common/Agents/MTConnectAgentBroker.cs` | 519 | `GetAssetsHeader` |
| `libraries/MTConnect.NET-Common/Agents/MTConnectAgentBroker.cs` | 538 | `GetErrorHeader` |

Each writes `Version = Version.ToString()` where the bare `Version`
identifier resolves (via inheritance from `MTConnectAgent`) to the
library assembly version returned by `GetAgentVersion()` —
`Assembly.GetExecutingAssembly().GetName().Version`. With the current
`VersionPrefix=6.9.0` in `Directory.Build.props`, every wire envelope
emits `Header.version="6.9.0.0"`.

Redundant overwrites — `MTConnectAgentBroker` rewrites `header.Version`
after calling the builder above:

| Line | Method |
|---|---|
| 564 | `GetDevicesResponseDocument(Version, string)` |
| 597 | `GetDevicesResponseDocument(string, Version)` |
| 1321 | `GetAssetsResponseDocument(string, string, bool, uint, Version)` |
| 1368 | `GetAssetsResponseDocument(IEnumerable<string>, Version)` |
| 1643 | `GetErrorResponseDocument(ErrorCode, string, Version)` |
| 1669 | `GetErrorResponseDocument(IEnumerable<IError>, Version)` |

Each line is `header.Version = Version.ToString();` — the same
incorrect value the builder already set. Removing them is part of P3
since the builders carry the correct value once fixed.

### Header DTO pass-through (untouched by P3)

The nine formatter sites listed in `00-overview.md` §1 (XML / JSON /
JSON-cppagent for Devices / Streams / Assets) read `header.Version`
from the DTO and copy it onto the wire DTO unchanged. They do not
need editing — fixing the origin propagates through them.

### `MTConnectAgent.Version` consumer audit

```text
libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs:52  private Version _version;
libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs:95  public Version Version => _version;
libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs:218 _version = GetAgentVersion();
libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs:237 _version = GetAgentVersion();
libraries/MTConnect.NET-Common/Agents/MTConnectAgent.cs:2339 private static Version GetAgentVersion() { return Assembly.GetExecutingAssembly().GetName().Version; }
```

The only public consumer outside the agent is the response-document
construction path inside `MTConnectAgentBroker`. After P3 fixes the
header path, the public `MTConnectAgent.Version` property remains —
it is the documented surface for diagnostics, logging, and startup
banners. No other library / agent / adapter source references it.

### Existing test coverage

`grep -rnE 'Header\.Version|header\.Version' tests/` returned zero
matches that assert a literal version string. Nothing in the existing
test suite needs to be rewritten by P3.

### Format decision — segment count

`MTConnectVersions` constants are constructed as `new Version(major,
minor)`. `System.Version.ToString()` then prints `"2.5"`;
`System.Version.ToString(4)` would throw `ArgumentException` because
the build and revision components are unset.

cppagent reports `Header.version="2.7.0.0"` — four-segment, padded
with zeros for build and revision. To match the cppagent reference
shape (and the historical MTConnect.NET shape, which was also
four-segment, just sourced from the wrong Version object), P3 emits
the configured release as `new Version(major, minor, 0, 0).ToString()`
— equivalent to the four-segment string `"<major>.<minor>.0.0"`.

This decision is captured here and consumed by P2's tests + P3's
implementation.

### `XmlFunctions.CreateHeaderComment` audit

`grep` confirms `XmlFunctions.WriteHeaderComment` /
`CreateHeaderComment` emit an XML *comment* prelude (the `<!-- ... -->`
block above the root element), not the `Header.version` attribute.
They are unrelated to issue #127 and are not edited.

### Out-of-scope adjacent issues

The audit confirms three adjacent defects that share the same DTOs
but are tracked separately:

- `Header.schemaVersion` hardcoded — issue #128.
- `Header.testIndicator` always emitted as `false` — issue #131.
- v2.6 / v2.7 support absent — issue #133.

These are not addressed by this plan and have their own branches.

## Metrics delta

Documentation only. No code or test changes in this phase.

## Deviations from plan

- The plan's checklist references a docker-run cppagent reproduction
  step. The local environment does not have docker available; the
  cppagent reference value `"2.7.0.0"` from the public issue text
  (https://github.com/TrakHound/MTConnect.NET/issues/127) is taken as
  authoritative for the format decision instead.
- `GetAgentVersion()` returns `Version`, not `string` — the redundant
  overwrites use `Version.ToString()` (current implicit `ToString()`,
  which yields `"6.9.0.0"` because the AssemblyVersion attribute is
  built with all four segments set). This is captured here so the P3
  diff is unambiguous.

## Follow-ups

None for this phase.
