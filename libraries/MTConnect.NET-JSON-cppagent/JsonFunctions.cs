// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTConnect
{
    /// <summary>
    /// Helpers for configuring the cppagent-compatible JSON serializer
    /// (option sets and one-shot convert wrappers around
    /// <see cref="JsonSerializer"/>). The shared option sets switch off
    /// indentation by default, ignore properties with default values,
    /// allow numbers to be read from strings (a common cppagent edge),
    /// and keep property lookup case-insensitive to tolerate equipment
    /// payload variation.
    /// </summary>
    public static class JsonFunctions
    {
        // A JsonSerializerOptions instance owns its own serialization
        // metadata cache, and building that cache emits reflection-based
        // property accessors (LCG DynamicMethods) for every property in
        // the reachable type graph. Allocating a fresh instance per call
        // therefore re-emits those accessors on every serialization, and
        // the emitted code accumulates in the runtime's loader heaps
        // where the GC cannot reclaim it — this is the mechanism behind
        // the ~3.3 MB/h RSS climb observed on DIME production hosts in
        // 2026-08. The cppagent-format assembly ships 25 Streams.Json
        // classes (2.5× the plain-JSON count), so the LCG cost per
        // serialization is proportionally larger. The instances below
        // are created once and reused; JsonSerializerOptions is
        // thread-safe for read after its first (de)serialization, and
        // nothing in this file mutates them after construction.
        private static readonly JsonSerializerOptions _defaultOptions = CreateOptions(false);
        private static readonly JsonSerializerOptions _indentOptions = CreateOptions(true);

        static JsonFunctions()
        {
#if NET8_0_OR_GREATER
            // Warm the default reflection resolver + serialization
            // metadata cache at assembly load rather than on the
            // first production /current | /sample | /assets request.
            // Serializing a real instance of each cppagent top-level
            // response surrogate forces STJ to walk that type's
            // reachable property graph, which is where the
            // LCG DynamicMethod property accessors are emitted; paying
            // that cost once at assembly load is the whole point of
            // the warm-up. This matters more for the cppagent assembly
            // than for the plain-JSON one — it ships 25 Streams.Json
            // classes vs 10, so the reflection cost per fresh options
            // is proportionally larger.
            //
            // A prior warm-up form (Serialize<object>(null, options))
            // only bootstrapped the shared reflection resolver — it
            // never named a concrete MTConnect type, so the cold LCG
            // emit for JsonStreamsResponseDocument /
            // JsonAssetsResponseDocument / JsonDevicesResponseDocument /
            // ErrorResponseDocument still fell on the first user-facing
            // request. The typed calls below fix that.
            // ErrorResponseDocument (from MTConnect.Errors) is the
            // surrogate-less envelope written directly by
            // JsonHttpResponseDocumentFormatter.Format(IErrorResponseDocument);
            // its cold LCG emit would otherwise hit on the first /probe
            // error, /current parse failure, or unsupported device
            // response — request paths that are already the symptom,
            // not the happy path we want to pay reflection cost on top of.
            //
            // Order matters: the warm-up must run BEFORE MakeReadOnly,
            // because MakeReadOnly(populateMissingResolver: false)
            // freezes the options WITHOUT choosing a TypeInfoResolver;
            // a subsequent Serialize on a resolver-less, frozen
            // options would throw NotSupportedException. Running
            // Serialize first lets STJ auto-populate the resolver via
            // its normal lazy path, after which MakeReadOnly(false)
            // is a pure lock with no side effect on serialization.
            WarmReachableGraph(_defaultOptions);
            WarmReachableGraph(_indentOptions);

            // Freeze both singletons so callers cannot mutate the
            // shared instance (adding a Converter, flipping
            // WriteIndented, etc.). Attempted mutation throws
            // InvalidOperationException — the fail-fast is preferable
            // to silent cross-caller pollution, and the cold-path
            // Convert branch stays open because it builds a fresh
            // (non-read-only) options object per call.
            _defaultOptions.MakeReadOnly(populateMissingResolver: false);
            _indentOptions.MakeReadOnly(populateMissingResolver: false);
#endif
        }

#if NET8_0_OR_GREATER
        // Serialize an instance of each cppagent top-level response
        // surrogate against <paramref name="options"/>, so STJ
        // configures JsonTypeInfo (and emits the LCG DynamicMethod
        // property accessors) for the reachable graph rooted at each
        // type. Called from the static constructor for both the
        // compact and indented option singletons.
        //
        // All three cppagent response envelopes expose public
        // parameterless constructors for JSON deserialization, so the
        // warm-up just news each one up and hands it to Serialize.
        // ErrorResponseDocument (from MTConnect.Errors) is the fourth
        // top-level envelope — no cppagent-specific surrogate, written
        // by the formatter's IErrorResponseDocument overload directly.
        //
        // The Error envelope populates a concrete
        // MTConnectErrorHeader + Error entry + Version, so STJ walks
        // the runtime types the production path actually serializes
        // (interface-typed properties resolve to their concrete
        // implementations only when the value is non-null); a naked
        // `new ErrorResponseDocument()` with null Header / Errors /
        // Version would only warm ErrorResponseDocument's own
        // accessors, leaving the first real /probe error or parse
        // failure to pay a cold LCG emit on MTConnectErrorHeader (9
        // properties), Error (2), and System.Version (6) — the exact
        // symptom class the singleton pattern exists to eliminate for
        // the error path (F-IMP-C5-001).
        private static void WarmReachableGraph(JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(new Streams.Json.JsonStreamsResponseDocument(), options);
            JsonSerializer.Serialize(new Assets.Json.JsonAssetsResponseDocument(), options);
            JsonSerializer.Serialize(new Devices.Json.JsonDevicesResponseDocument(), options);
            JsonSerializer.Serialize(
                new Errors.ErrorResponseDocument
                {
                    Header = new Headers.MTConnectErrorHeader(),
                    Errors = new[] { new Errors.Error() },
                    // Any non-null concrete Version warms the
                    // System.Version accessors identically; using the
                    // repo's canonical constant instead of a magic
                    // `new Version(2, 5)` tuple keeps the warm-up
                    // aligned with the rest of the codebase's
                    // MTConnectVersion sourcing (F-CR-C6-001).
                    Version = MTConnectVersions.Version25
                },
                options);
        }
#endif

        /// <summary>
        /// Builds a fresh <see cref="JsonSerializerOptions"/> instance
        /// with the cppagent option preset (compact / indented per
        /// <paramref name="indented"/>, default-value omission on net5+,
        /// number-from-string reading on net5+, case-insensitive property
        /// lookup, 1000-deep recursion limit).
        /// </summary>
        /// <remarks>
        /// This helper is intended for two callers only: (1) the
        /// static-init assignments that construct the shared
        /// <c>_defaultOptions</c> and <c>_indentOptions</c> singletons,
        /// and (2) the cold-path branch of <see cref="GetOptions"/> when
        /// a per-call converter forces a private options instance. Every
        /// call allocates a new object and re-emits the STJ reflection
        /// metadata cache for the reachable type graph — the very cost
        /// the singletons exist to amortize — so it MUST NOT be invoked
        /// on any hot-path serialization site.
        /// </remarks>
        /// <param name="indented">When true, sets <c>WriteIndented = true</c>; otherwise compact JSON.</param>
        private static JsonSerializerOptions CreateOptions(bool indented)
        {
            return new JsonSerializerOptions
            {
                WriteIndented = indented,
#if NET5_0_OR_GREATER
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
#endif
                PropertyNameCaseInsensitive = true,
                MaxDepth = 1000
            };
        }

        /// <summary>
        /// Resolves the <see cref="JsonSerializerOptions"/> instance
        /// used by <see cref="Convert(object, JsonConverter, bool)"/>,
        /// <see cref="ConvertBytes"/>, and <see cref="ConvertStream"/>
        /// for a given per-call converter + indentation combination.
        /// </summary>
        /// <remarks>
        /// Hot path (<paramref name="converter"/> is null, which is
        /// every in-tree caller): returns the shared frozen singleton
        /// (<c>_defaultOptions</c> or <c>_indentOptions</c>) so STJ
        /// reuses its metadata cache instead of re-emitting property
        /// accessors on every call.
        /// <para/>
        /// Cold path (<paramref name="converter"/> is non-null):
        /// allocates a fresh <see cref="JsonSerializerOptions"/> via
        /// <see cref="CreateOptions"/>, appends the caller's converter,
        /// and returns it. This branch pays the full reflection-emit
        /// cost per call and is NOT thread-safe under simultaneous cold
        /// callers because the fresh options is neither shared nor
        /// synchronized — each call allocates and mutates its own
        /// object, so per-call concurrent use is safe; multiple callers
        /// sharing one converter instance is safe iff the converter
        /// itself is thread-safe. The branch exists solely to preserve
        /// the public API contract for external consumers that pass a
        /// per-call converter.
        /// </remarks>
        /// <param name="converter">Optional caller-supplied converter. When non-null forces the cold path.</param>
        /// <param name="indented">Selects the compact or indented preset.</param>
        private static JsonSerializerOptions GetOptions(JsonConverter converter, bool indented)
        {
            // Hot path: no per-call converter, hand back the shared
            // instance and let System.Text.Json reuse its metadata cache
            // instead of re-emitting property accessors for the whole
            // type graph on every call.
            if (converter == null) return indented ? _indentOptions : _defaultOptions;

            // Cold path: a caller-supplied converter cannot be added to
            // a shared instance once it has been used, so build a
            // private one. Callers that mint many one-off converters
            // will still pay the LCG cost — this fallback preserves the
            // public API contract for external consumers.
            var options = CreateOptions(indented);
            options.Converters.Add(converter);
            return options;
        }

        /// <summary>
        /// Default serializer options used when no <c>indentOutput</c>
        /// option is requested. Produces compact JSON, omits properties
        /// at their default value, allows numbers to be read from
        /// strings, and ignores property-name casing.
        /// </summary>
        /// <remarks>
        /// This is a process-wide shared singleton; do NOT mutate the
        /// returned instance's <see cref="JsonSerializerOptions.Converters"/>
        /// collection or any writable property. The returned object is
        /// marked <c>MakeReadOnly()</c> under <c>net8.0</c> and later;
        /// attempted mutation throws <see cref="System.InvalidOperationException"/>.
        /// On older TFMs (netstandard2.0, net4.6.1–net4.8, net6.0, net7.0)
        /// the instance is not statically frozen but callers must still
        /// treat it as immutable — mutating it silently corrupts every
        /// other in-process serializer that shares the singleton.
        /// <para/>
        /// Why the invariant matters: every mutation of a
        /// <see cref="JsonSerializerOptions"/> instance that has already
        /// been used forces System.Text.Json to rebuild its serialization
        /// metadata cache, which re-emits reflection-based property
        /// accessors as <see cref="System.Reflection.Emit.DynamicMethod"/>s
        /// into the runtime's LCG (lightweight code generation) loader
        /// heaps. Those heaps are never reclaimed by the GC — every
        /// mutated-then-reused options object leaks the emit permanently.
        /// A peer measured this class of misuse at +3.2–3.8 MB/h RSS in
        /// production; the cppagent assembly ships 25 Streams.Json
        /// classes (2.5× the plain-JSON count), so the per-mutation LCG
        /// cost here is proportionally larger and the frozen singleton
        /// pays back correspondingly more.
        /// </remarks>
        public static JsonSerializerOptions DefaultOptions => _defaultOptions;

        /// <summary>
        /// Pretty-printed serializer options used when the
        /// <c>indentOutput</c> formatter option is enabled; otherwise
        /// identical to <see cref="DefaultOptions"/>.
        /// </summary>
        /// <remarks>
        /// This is a process-wide shared singleton; do NOT mutate the
        /// returned instance's <see cref="JsonSerializerOptions.Converters"/>
        /// collection or any writable property. The returned object is
        /// marked <c>MakeReadOnly()</c> under <c>net8.0</c> and later;
        /// attempted mutation throws <see cref="System.InvalidOperationException"/>.
        /// On older TFMs (netstandard2.0, net4.6.1–net4.8, net6.0, net7.0)
        /// the instance is not statically frozen but callers must still
        /// treat it as immutable — mutating it silently corrupts every
        /// other in-process serializer that shares the singleton.
        /// <para/>
        /// Why the invariant matters: same reflection-emit / LCG
        /// loader-heap accumulation as documented on
        /// <see cref="DefaultOptions"/>. Every per-call mutation of this
        /// shared instance re-emits property-accessor
        /// <see cref="System.Reflection.Emit.DynamicMethod"/>s into a
        /// heap the GC cannot free — the mechanism behind the peer's
        /// production +3.2–3.8 MB/h RSS climb before the singleton was
        /// frozen.
        /// </remarks>
        public static JsonSerializerOptions IndentOptions => _indentOptions;


        /// <summary>
        /// Serializes <paramref name="obj"/> to a JSON string using the
        /// cppagent option defaults, optionally with an extra converter
        /// and pretty-printing. Returns <c>null</c> on any
        /// serialization failure or when the input is null.
        /// </summary>
        public static string Convert(object obj, JsonConverter converter = null, bool indented = false)
        {
            if (obj != null)
            {
                try
                {
                    var options = GetOptions(converter, indented);

                    return JsonSerializer.Serialize(obj, options);
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Serializes <paramref name="obj"/> to a UTF-8 byte array
        /// using the cppagent option defaults, optionally with an extra
        /// converter and pretty-printing. Returns <c>null</c> on any
        /// serialization failure or when the input is null.
        /// </summary>
        public static byte[] ConvertBytes(object obj, JsonConverter converter = null, bool indented = false)
        {
            if (obj != null)
            {
                try
                {
                    var options = GetOptions(converter, indented);

                    return JsonSerializer.SerializeToUtf8Bytes(obj, options);
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Serializes <paramref name="obj"/> into a fresh
        /// <see cref="MemoryStream"/> using the cppagent option
        /// defaults, optionally with an extra converter and
        /// pretty-printing. Returns <c>null</c> on any serialization
        /// failure or when the input is null.
        /// </summary>
        public static Stream ConvertStream(object obj, JsonConverter converter = null, bool indented = false)
        {
            if (obj != null)
            {
                try
                {
                    var options = GetOptions(converter, indented);

                    var outputStream = new MemoryStream();
                    JsonSerializer.Serialize(outputStream, obj, options);
                    return outputStream;
                }
                catch { }
            }

            return null;
        }
    }
}
