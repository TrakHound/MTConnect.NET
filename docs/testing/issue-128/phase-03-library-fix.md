# Phase 03 — Library fix

## Streams envelope

`libraries/MTConnect.NET-JSON-cppagent/Streams/JsonMTConnectStreams.cs`:

```diff
-        public JsonMTConnectStreams() 
+        public JsonMTConnectStreams()
         {
             JsonVersion = 2;
-            SchemaVersion = "2.0";
         }

         public JsonMTConnectStreams(IStreamsResponseOutputDocument streamsDocument)
         {
             JsonVersion = 2;
-            SchemaVersion = "2.0";

             if (streamsDocument != null)
             {
+                SchemaVersion = streamsDocument.Version?.ToString();
                 Header = new JsonStreamsHeader(streamsDocument.Header);
                 Streams = new JsonStreams(streamsDocument);
             }
         }
```

## Devices envelope

`libraries/MTConnect.NET-JSON-cppagent/Devices/JsonMTConnectDevices.cs`:

```diff
         public JsonMTConnectDevices()
         {
             JsonVersion = 2;
-            SchemaVersion = "2.0";
         }

         public JsonMTConnectDevices(IDevicesResponseDocument document)
         {
             JsonVersion = 2;
-            SchemaVersion = "2.0";

             if (document != null)
             {
+                SchemaVersion = document.Version?.ToString();
+
                 Header = new JsonDevicesHeader(document.Header);
                 Devices = new JsonDevices(document);
             }
         }
```

## Test result

```
$ dotnet test tests/MTConnect.NET-JSON-cppagent-Tests/...
Passed!  - Failed: 0, Passed: 31, Skipped: 0, Total: 31
```

All 28 previously-red cases (Streams + Devices, v1.0-v1.8 + v2.1-v2.5)
now pass. The 2 cases for v2.0 remain green. Sanity test green.

## Behaviour notes

- **Default ctor**: `SchemaVersion` is now `null` instead of `"2.0"`.
  This matches consumer expectation — the default ctor produces an
  unpopulated envelope that the caller fills in via property
  initializer or via the document-accepting ctor.
- **Null safety**: the new code uses `document.Version?.ToString()`.
  If `Version` is unset on the response document, `SchemaVersion`
  is null; this surfaces the misconfiguration at the wire (rather
  than masking it behind a stale `"2.0"`).
- **Format**: two-segment via `System.Version.ToString()` —
  `new Version(2, 5).ToString() == "2.5"`. Matches cppagent.

## Coverage

The two ctors are exercised by 28 NUnit cases (matrix × envelopes).
`SchemaVersion = document.Version?.ToString()` and the surrounding
null guard are covered by the same set. The default ctor is
exercised by the indirect path of the document-accepting ctor on
the null-document case (an existing concern; not introduced here).
