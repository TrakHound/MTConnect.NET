# Phase 01 — Defect scoping

## Inventory at HEAD (`upstream/master` @ 3d6321ab)

### Hardcode site 1 — Streams envelope

`libraries/MTConnect.NET-JSON-cppagent/Streams/JsonMTConnectStreams.cs`,
both ctors:

```csharp
public JsonMTConnectStreams()
{
    JsonVersion = 2;
    SchemaVersion = "2.0";
}

public JsonMTConnectStreams(IStreamsResponseOutputDocument streamsDocument)
{
    JsonVersion = 2;
    SchemaVersion = "2.0";
    ...
}
```

Both stamp `"2.0"` unconditionally.

### Hardcode site 2 — Devices envelope

`libraries/MTConnect.NET-JSON-cppagent/Devices/JsonMTConnectDevices.cs`,
both ctors — same pattern:

```csharp
public JsonMTConnectDevices()
{
    JsonVersion = 2;
    SchemaVersion = "2.0";
}

public JsonMTConnectDevices(IDevicesResponseDocument document)
{
    JsonVersion = 2;
    SchemaVersion = "2.0";
    ...
}
```

### Assets — unaffected

`libraries/MTConnect.NET-JSON-cppagent/Assets/JsonMTConnectAssets.cs`
does not expose a `SchemaVersion` property; no fix required there.

## DefaultVersion → envelope flow

`AgentConfiguration.DefaultVersion` → `MTConnectAgent` populates the
response document's `Version` property → response document flows into
the envelope ctor. Both response documents already expose `Version`:

- `IStreamsResponseOutputDocument.Version` — `Version` (System.Version)
- `IDevicesResponseDocument.Version` — `Version` (System.Version)

So the fix is a one-liner per ctor: assign
`SchemaVersion = streamsDocument.Version.ToString()` (Streams) or
`SchemaVersion = document.Version.ToString()` (Devices), guarded by
the existing null check on the document.

## Segment-count decision

`System.Version.ToString()` defaults to the shortest meaningful form
when constructed via `new Version(major, minor)` — e.g.
`new Version(2, 5).ToString()` returns `"2.5"`. That matches cppagent's
two-segment wire output (the issue reports `"2.7"` for a v2.7 cppagent).

`MTConnectVersions` constructs every constant via `new Version(major,
minor)` so all 14 declared versions (v1.0-v1.8, v2.0-v2.5) round-trip
through `.ToString()` as two-segment strings.

Contrast with issue #127 (`Header.version`) where the four-segment form
is the spec-required output. Different field, different formatter.

## Existing-test audit

```
$ git grep -nE 'SchemaVersion.*"2\.0"' tests/
(no output)
```

No existing test pins the defective `"2.0"` literal. New tests are
free to assert the corrected behaviour without breaking anything green.

## Decisions

- **Assignment site**: in the document-accepting ctor, inside the
  existing `if (streamsDocument != null)` (Streams) /
  `if (document != null)` (Devices) block.
- **Default ctor behaviour**: leave `SchemaVersion` unset (null);
  consumers using the default ctor must set it via property-init.
  The default ctor today's `"2.0"` stamp is the bug.
- **String format**: `streamsDocument.Version.ToString()` —
  two-segment, matches cppagent reference.
- **Null safety**: existing null guard on the document parameter
  remains; if the document's `Version` is null (it shouldn't be at
  emit time, but the type allows it), `ToString()` would NRE — guard.
