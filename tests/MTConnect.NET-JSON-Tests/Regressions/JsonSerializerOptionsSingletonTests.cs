// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MTConnect.NET_JSON_Tests.Regressions
{
    /// <summary>
    /// Regression pin for the DIME-connector native-heap leak (peer
    /// diagnosis dated 2026-08-21). A fresh <see cref="JsonSerializerOptions"/>
    /// instance owns its own serialization-metadata cache; building that
    /// cache emits reflection-based property accessors for the entire type
    /// graph, and the emitted code accumulates in the runtime's loader
    /// heaps where the GC cannot reclaim it. Allocating one on every
    /// serialization call therefore leaked ~3.3 MB/h RSS in production
    /// (tempco-001, tim-001).
    ///
    /// The <see cref="JsonFunctions.DefaultOptions"/> and
    /// <see cref="JsonFunctions.IndentOptions"/> properties must therefore
    /// return the same instance on every access, and the
    /// <c>Convert</c>/<c>ConvertBytes</c>/<c>ConvertStream</c> hot paths
    /// must reuse those instances when no per-call converter is supplied.
    /// </summary>
    [TestFixture]
    public class JsonSerializerOptionsSingletonTests
    {
        // Small POCO used by the Convert/ConvertBytes/ConvertStream overload
        // smoke tests and the thread-safety pin. Kept tiny on purpose so
        // the metadata cache warms in a single JIT pass and each thread's
        // work is bounded, but with a nested type so the reachable-graph
        // JIT emit still runs on cold DefaultOptions instances.
        internal class SamplePayload
        {
            public string Name { get; set; } = "sample";
            public int Count { get; set; } = 42;
            public SampleChild Child { get; set; } = new SampleChild();
        }

        internal class SampleChild
        {
            public string Label { get; set; } = "child";
            public double Value { get; set; } = 3.14;
        }

        private sealed class NoopConverter : JsonConverter<SamplePayload>
        {
            public override SamplePayload Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
                => new SamplePayload();

            public override void Write(Utf8JsonWriter writer, SamplePayload value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("Name", value.Name);
                writer.WriteNumber("Count", value.Count);
                writer.WriteEndObject();
            }
        }

        /// <summary>Pins the behavior expressed by the test name: default options returns the same instance on repeat access.</summary>
        [Test]
        public void DefaultOptions_returns_the_same_instance_on_repeat_access()
        {
            var a = JsonFunctions.DefaultOptions;
            var b = JsonFunctions.DefaultOptions;
            Assert.That(ReferenceEquals(a, b), Is.True,
                "JsonFunctions.DefaultOptions must be a shared singleton — a fresh JsonSerializerOptions per call leaks LCG-emitted metadata into the loader heap.");
        }

        /// <summary>Pins the behavior expressed by the test name: indent options returns the same instance on repeat access.</summary>
        [Test]
        public void IndentOptions_returns_the_same_instance_on_repeat_access()
        {
            var a = JsonFunctions.IndentOptions;
            var b = JsonFunctions.IndentOptions;
            Assert.That(ReferenceEquals(a, b), Is.True,
                "JsonFunctions.IndentOptions must be a shared singleton — a fresh JsonSerializerOptions per call leaks LCG-emitted metadata into the loader heap.");
        }

        /// <summary>Pins the behavior expressed by the test name: default options and indent options are distinct instances.</summary>
        [Test]
        public void DefaultOptions_and_IndentOptions_are_distinct_instances()
        {
            Assert.That(ReferenceEquals(JsonFunctions.DefaultOptions, JsonFunctions.IndentOptions), Is.False,
                "DefaultOptions (compact) and IndentOptions (pretty-printed) must be separate instances so WriteIndented differs.");
        }

        /// <summary>Pins the behavior expressed by the test name: default options write indented is false.</summary>
        [Test]
        public void DefaultOptions_WriteIndented_is_false()
        {
            Assert.That(JsonFunctions.DefaultOptions.WriteIndented, Is.False);
        }

        /// <summary>Pins the behavior expressed by the test name: indent options write indented is true.</summary>
        [Test]
        public void IndentOptions_WriteIndented_is_true()
        {
            Assert.That(JsonFunctions.IndentOptions.WriteIndented, Is.True);
        }

        /// <summary>Pins the behavior expressed by the test name: json functions holds a static readonly options field.</summary>
        [Test]
        public void JsonFunctions_holds_a_static_readonly_options_field()
        {
            // Structural guard: at least one static readonly field of type
            // JsonSerializerOptions must exist on JsonFunctions so the
            // shared instance survives across serialization calls.
            var fields = typeof(JsonFunctions).GetFields(BindingFlags.NonPublic | BindingFlags.Static);
            var optionsFields = System.Array.FindAll(fields, f => f.FieldType == typeof(JsonSerializerOptions) && f.IsInitOnly);
            Assert.That(optionsFields.Length, Is.GreaterThanOrEqualTo(2),
                "JsonFunctions must declare shared static readonly JsonSerializerOptions fields (compact + indented) so the instances outlive each serialization call.");
        }

        /// <summary>
        /// Thread-safety pin: 100 concurrent Convert calls against the shared
        /// DefaultOptions must all succeed, must not throw, and must produce
        /// byte-identical output. JsonSerializerOptions is documented as
        /// thread-safe for read after its first (de)serialization call, so
        /// concurrent Serialize calls sharing one instance is legal by
        /// contract — this test pins that contract against any future
        /// refactor that would put a mutation on the hot path.
        /// </summary>
        [Test]
        public void Convert_is_thread_safe_across_100_concurrent_callers()
        {
            const int threadCount = 100;
            var payload = new SamplePayload();
            var outputs = new ConcurrentBag<string>();
            var exceptions = new ConcurrentBag<System.Exception>();
            var startGate = new ManualResetEventSlim(false);

            var threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    try
                    {
                        startGate.Wait();
                        var s = JsonFunctions.Convert(payload);
                        outputs.Add(s);
                    }
                    catch (System.Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                });
                threads[i].Start();
            }

            // Warm the singleton once on the driver thread so the first-use
            // metadata build is not itself concurrent with the race window.
            // The point of the pin is steady-state hot-path concurrency, not
            // first-touch racing (STJ handles that internally).
            _ = JsonFunctions.Convert(payload);

            startGate.Set();
            foreach (var t in threads) t.Join();

            Assert.That(exceptions, Is.Empty,
                "Concurrent Convert calls against the shared DefaultOptions must not throw.");
            Assert.That(outputs.Count, Is.EqualTo(threadCount));

            var canonical = JsonFunctions.Convert(payload);
            foreach (var s in outputs)
            {
                Assert.That(s, Is.EqualTo(canonical),
                    "Every concurrent Convert output must be byte-identical to the single-threaded canonical output.");
            }
        }

        /// <summary>
        /// Cold-path pin: when a caller supplies a per-call converter, the
        /// implementation must NOT append that converter to the shared
        /// DefaultOptions.Converters collection. Doing so would (a) permanently
        /// pollute the singleton and (b) throw InvalidOperationException
        /// because JsonSerializerOptions freezes its converter list after
        /// first use. The pin observes the singleton before and after a
        /// converter-supplied Convert call and asserts the count is unchanged.
        /// </summary>
        [Test]
        public void Convert_with_custom_converter_does_not_mutate_DefaultOptions_converters()
        {
            // Warm the singleton so its converters list is frozen — if the
            // implementation ever tried to append to it, the test would
            // observe an exception path rather than silent pollution.
            _ = JsonFunctions.Convert(new SamplePayload());

            var beforeCount = JsonFunctions.DefaultOptions.Converters.Count;
            var beforeIndentCount = JsonFunctions.IndentOptions.Converters.Count;

            var converter = new NoopConverter();
            var result = JsonFunctions.Convert(new SamplePayload(), converter);
            var resultIndented = JsonFunctions.Convert(new SamplePayload(), converter, indented: true);

            Assert.That(result, Is.Not.Null,
                "Convert with a custom converter must succeed via the cold-path fresh-options branch.");
            Assert.That(resultIndented, Is.Not.Null,
                "Convert(indented:true) with a custom converter must succeed via the cold-path fresh-options branch.");

            Assert.That(JsonFunctions.DefaultOptions.Converters.Count, Is.EqualTo(beforeCount),
                "The cold-path branch must build a fresh JsonSerializerOptions — a caller-supplied converter must never leak into DefaultOptions.Converters.");
            Assert.That(JsonFunctions.IndentOptions.Converters.Count, Is.EqualTo(beforeIndentCount),
                "The cold-path branch must build a fresh JsonSerializerOptions — a caller-supplied converter must never leak into IndentOptions.Converters.");
        }

        /// <summary>
        /// Cold-path pin: the custom-converter Convert output must reflect
        /// the caller's converter (proving the fresh options object used
        /// it), while a following converter-less Convert on the same input
        /// must NOT reflect the converter (proving the singleton was untouched).
        /// </summary>
        [Test]
        public void Convert_with_custom_converter_uses_converter_without_affecting_singleton_output()
        {
            var payload = new SamplePayload();
            var canonicalWithoutConverter = JsonFunctions.Convert(payload);

            var converter = new NoopConverter();
            var withConverter = JsonFunctions.Convert(payload, converter);

            // The custom converter drops the Child property, so its output
            // must not equal the canonical singleton-emitted output.
            Assert.That(withConverter, Is.Not.EqualTo(canonicalWithoutConverter),
                "The cold-path fresh options must apply the caller's converter — otherwise the singleton was reused, defeating the branch.");
            Assert.That(withConverter, Does.Not.Contain("Child"),
                "NoopConverter drops the Child field; the cold-path output should reflect that.");

            // Following singleton-path call must be untouched.
            var afterCanonical = JsonFunctions.Convert(payload);
            Assert.That(afterCanonical, Is.EqualTo(canonicalWithoutConverter),
                "After a converter-supplied call, the singleton-path output must be byte-identical to before.");
            Assert.That(afterCanonical, Does.Contain("Child"),
                "The singleton must still emit the Child field — the cold-path converter must not have polluted it.");
        }

        /// <summary>
        /// Overload smoke pin: Convert/ConvertBytes/ConvertStream must all
        /// produce the same JSON for the same input under the compact
        /// preset, and the indented variants must be internally consistent.
        /// This guards against a future refactor that accidentally routes
        /// one overload through a divergent options instance.
        /// </summary>
        [Test]
        public void Convert_ConvertBytes_ConvertStream_produce_identical_output_for_compact()
        {
            var payload = new SamplePayload();

            var s = JsonFunctions.Convert(payload);
            var bytes = JsonFunctions.ConvertBytes(payload);
            var stream = JsonFunctions.ConvertStream(payload);

            Assert.That(s, Is.Not.Null);
            Assert.That(bytes, Is.Not.Null);
            Assert.That(stream, Is.Not.Null);

            var bytesString = Encoding.UTF8.GetString(bytes!);
            Assert.That(bytesString, Is.EqualTo(s),
                "ConvertBytes(UTF-8) must decode to the same string ConvertBytes emits.");

            stream!.Position = 0;
            using var reader = new System.IO.StreamReader(stream);
            var streamString = reader.ReadToEnd();
            Assert.That(streamString, Is.EqualTo(s),
                "ConvertStream must contain the same JSON Convert returns.");
        }

        /// <summary>Overload smoke pin — indented variant of the same guard.</summary>
        [Test]
        public void Convert_ConvertBytes_ConvertStream_produce_identical_output_for_indented()
        {
            var payload = new SamplePayload();

            var s = JsonFunctions.Convert(payload, indented: true);
            var bytes = JsonFunctions.ConvertBytes(payload, indented: true);
            var stream = JsonFunctions.ConvertStream(payload, indented: true);

            Assert.That(s, Is.Not.Null);
            Assert.That(bytes, Is.Not.Null);
            Assert.That(stream, Is.Not.Null);

            var bytesString = Encoding.UTF8.GetString(bytes!);
            Assert.That(bytesString, Is.EqualTo(s));

            stream!.Position = 0;
            using var reader = new System.IO.StreamReader(stream);
            var streamString = reader.ReadToEnd();
            Assert.That(streamString, Is.EqualTo(s));

            // Cross-check indentation is present (crude, but sufficient
            // to pin that IndentOptions actually took effect on all
            // three overloads).
            Assert.That(s, Does.Contain("\n"),
                "Indented Convert output must contain newlines.");
            Assert.That(bytesString, Does.Contain("\n"),
                "Indented ConvertBytes output must contain newlines.");
            Assert.That(streamString, Does.Contain("\n"),
                "Indented ConvertStream output must contain newlines.");
        }

        /// <summary>
        /// Null-input pin: every overload must swallow a null input and
        /// return null / null / null — this is documented behavior and
        /// the singleton refactor must preserve it.
        /// </summary>
        [Test]
        public void Convert_overloads_return_null_for_null_input()
        {
            Assert.That(JsonFunctions.Convert(null), Is.Null);
            Assert.That(JsonFunctions.ConvertBytes(null), Is.Null);
            Assert.That(JsonFunctions.ConvertStream(null), Is.Null);
        }

        /// <summary>
        /// Async-throughput pin: 100 parallel Convert calls via
        /// Parallel.For to complement the Thread-based pin above. The
        /// ThreadPool scheduling differs from raw Thread scheduling, and
        /// System.Text.Json's internal metadata-lock behavior has
        /// historically been sensitive to the difference; both paths are
        /// pinned so future STJ upgrades are covered.
        /// </summary>
        [Test]
        public void Convert_is_thread_safe_under_ParallelFor()
        {
            _ = JsonFunctions.Convert(new SamplePayload()); // warm

            var canonical = JsonFunctions.Convert(new SamplePayload());
            var results = new ConcurrentBag<string>();

            Parallel.For(0, 100, _ =>
            {
                var s = JsonFunctions.Convert(new SamplePayload());
                results.Add(s);
            });

            Assert.That(results.Count, Is.EqualTo(100));
            foreach (var r in results)
            {
                Assert.That(r, Is.EqualTo(canonical));
            }
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// Freeze pin (net8+): the shared DefaultOptions and IndentOptions
        /// singletons must be marked read-only at static-ctor time, so any
        /// attempt to mutate their <see cref="JsonSerializerOptions.Converters"/>
        /// collection throws <see cref="System.InvalidOperationException"/>.
        /// The freeze is the enforcement half of the singleton pattern —
        /// documentation alone would let a careless caller silently pollute
        /// every other in-process serializer; the read-only lock makes the
        /// misuse fail loudly at the point of the offending Add.
        /// </summary>
        [Test]
        public void DefaultOptions_Converters_Add_throws_InvalidOperationException_when_frozen()
        {
            Assert.Throws<System.InvalidOperationException>(
                () => JsonFunctions.DefaultOptions.Converters.Add(new NoopConverter()),
                "DefaultOptions is a shared singleton and must be frozen on net8+ so a stray Converters.Add fails fast rather than silently mutating the process-wide instance.");
        }

        /// <summary>
        /// Freeze pin (net8+) — IndentOptions mirror of the DefaultOptions guard.
        /// </summary>
        [Test]
        public void IndentOptions_Converters_Add_throws_InvalidOperationException_when_frozen()
        {
            Assert.Throws<System.InvalidOperationException>(
                () => JsonFunctions.IndentOptions.Converters.Add(new NoopConverter()),
                "IndentOptions is a shared singleton and must be frozen on net8+ so a stray Converters.Add fails fast rather than silently mutating the process-wide instance.");
        }

        /// <summary>
        /// Warm-up-before-freeze ordering pin (net8+): the frozen singleton
        /// must still be able to serialize a real typed payload, proving that
        /// the static-ctor warm-up (typed <c>JsonSerializer.Serialize</c>
        /// calls against each MTConnect top-level response surrogate — see
        /// <c>JsonFunctions.WarmReachableGraph</c>) ran BEFORE
        /// <c>MakeReadOnly(populateMissingResolver: false)</c>. If a future
        /// refactor reordered the two calls — or removed the warm-up
        /// entirely — the frozen, resolver-less options would throw
        /// <see cref="System.NotSupportedException"/> on the first real-payload
        /// Serialize. The invariant is documented in the JsonFunctions static
        /// ctor comment ("Order matters: the warm-up must run BEFORE
        /// MakeReadOnly"); this test names it as a regression pin.
        /// <para/>
        /// Calls <see cref="JsonSerializer.Serialize{TValue}(TValue, JsonSerializerOptions)"/>
        /// directly rather than via <see cref="JsonFunctions.Convert"/> because
        /// Convert's catch-all silently swallows serialization exceptions into
        /// a null return — routing through it would degrade a diagnostic
        /// "NotSupportedException at Serialize" into an opaque "Expected: not null".
        /// </summary>
        [Test]
        public void Frozen_singleton_can_serialize_a_real_payload_proving_warm_up_ran_before_MakeReadOnly()
        {
            var payload = new SamplePayload();

            string compact = string.Empty;
            string indented = string.Empty;
            Assert.DoesNotThrow(
                () => compact = JsonSerializer.Serialize(payload, JsonFunctions.DefaultOptions),
                "Frozen DefaultOptions must have a populated TypeInfoResolver — a static-ctor reorder that runs MakeReadOnly BEFORE the WarmReachableGraph typed-Serialize calls would surface here as NotSupportedException.");
            Assert.DoesNotThrow(
                () => indented = JsonSerializer.Serialize(payload, JsonFunctions.IndentOptions),
                "Frozen IndentOptions must have a populated TypeInfoResolver — same warm-up-before-freeze invariant as DefaultOptions.");

            Assert.That(compact, Does.Contain("\"Name\""),
                "The warmed-and-frozen singleton must still emit real property data — a resolver-less frozen options would either throw or emit empty output.");
            Assert.That(indented, Does.Contain("\"Name\""));
        }

        /// <summary>
        /// Structural warm-up-coverage pin (net8+) — <c>WarmReachableGraph</c>'s
        /// IL must contain a <c>newobj</c> instruction constructing
        /// <see cref="MTConnect.Errors.ErrorResponseDocument"/>, the fourth
        /// top-level response envelope written directly by
        /// <c>JsonResponseDocumentFormatter.Format(IErrorResponseDocument, ...)</c>
        /// without a Json* surrogate wrapper.
        /// <para/>
        /// A runtime <c>Assert.DoesNotThrow(() =&gt; Serialize(new ErrorResponseDocument(), frozen))</c>
        /// check is tautological here: once the TypeInfoResolver is set (which
        /// any preceding warm-up call does), STJ's <see cref="System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver"/>
        /// lazily populates <see cref="System.Text.Json.Serialization.Metadata.JsonTypeInfo"/>
        /// on frozen options for arbitrary types — the frozen state only locks
        /// the options' configuration surface (Converters, DefaultIgnoreCondition,
        /// etc.), not the internal metadata cache. Verified empirically:
        /// mutating out the <c>ErrorResponseDocument</c> warm-up line and running
        /// the "can it serialize" pin still passes (see cycle-5 test-coverage-audit
        /// mutation test 2026-08-21). The performance invariant the warm-up
        /// enforces — pay the LCG DynamicMethod emit for ErrorResponseDocument
        /// at assembly load, not on the first /probe error / parse-failure
        /// request — is therefore only observable at the source-of-truth level:
        /// the IL of <c>WarmReachableGraph</c>.
        /// </summary>
        [Test]
        public void WarmReachableGraph_IL_contains_newobj_for_ErrorResponseDocument_ctor()
        {
            AssertWarmReachableGraphNewsUp(typeof(MTConnect.Errors.ErrorResponseDocument));
        }

        /// <summary>
        /// Structural warm-up-coverage pin (net8+) — companion assertion that
        /// each of the three Json* top-level response surrogates is also
        /// news-up-ed by <c>WarmReachableGraph</c>. Mirrors the ErrorResponseDocument
        /// pin above; the same tautology argument applies to per-type
        /// "can it serialize" runtime checks.
        /// </summary>
        [Test]
        public void WarmReachableGraph_IL_contains_newobj_for_every_top_level_response_surrogate()
        {
            AssertWarmReachableGraphNewsUp(typeof(MTConnect.Streams.Json.JsonStreamsDocument));
            AssertWarmReachableGraphNewsUp(typeof(MTConnect.Assets.Json.JsonAssetsDocument));
            AssertWarmReachableGraphNewsUp(typeof(MTConnect.Devices.Json.JsonDevicesDocument));
        }

        /// <summary>
        /// Structural warm-up-coverage pin (net8+) — companion assertion for
        /// the F-IMP-C5-001 fix that populated the Error envelope with a
        /// concrete <see cref="MTConnect.Headers.MTConnectErrorHeader"/> +
        /// <see cref="MTConnect.Errors.Error"/> + <see cref="System.Version"/>.
        /// STJ resolves accessors for interface-typed properties
        /// (<c>IMTConnectErrorHeader Header</c>, <c>IEnumerable&lt;IError&gt; Errors</c>)
        /// only when the value is non-null; a revert to a naked
        /// <c>new ErrorResponseDocument()</c> would silently pass the
        /// ErrorResponseDocument-only IL pin above while re-opening the
        /// exact LCG-emit-on-first-error class F-IMP-C5-001 closed —
        /// MTConnectErrorHeader (9 properties), Error (2), and
        /// System.Version (6) would each pay their first cold accessor
        /// emit on the first real <c>/probe</c> error or parse failure.
        /// The three assertions below are the source-of-truth evidence
        /// that the populated warm-up shape survives.
        /// <para/>
        /// The walker (<see cref="AssertWarmReachableGraphNewsUp"/>)
        /// accepts either a <c>newobj</c> producing an instance of the
        /// expected type OR a <c>ldsfld</c> loading a static field of
        /// that type — semantically equivalent for warm-up, because STJ
        /// walks the runtime type of whatever value the caller passes
        /// into <see cref="System.Text.Json.JsonSerializer.Serialize{TValue}(TValue,System.Text.Json.JsonSerializerOptions)"/>
        /// regardless of how the instance was produced. The current
        /// warm-up uses <c>MTConnectVersions.Version25</c> (a static
        /// field of type <see cref="System.Version"/>) rather than a
        /// magic <c>new System.Version(2, 5)</c> literal (F-CR-C6-001);
        /// dropping either producer would fail the pin.
        /// <para/>
        /// System.Version is not news-up-ed OR ldsfld-loaded by the
        /// three surrogate envelopes' Header defaulting (they use their
        /// own concrete <c>MTConnectStreamsHeader</c> /
        /// <c>MTConnectDevicesHeader</c> / <c>MTConnectAssetsHeader</c>
        /// and initialize the Version property only after construction),
        /// so the walker match on <c>System.Version</c> uniquely
        /// fingerprints the Error-envelope initializer.
        /// </summary>
        [Test]
        public void WarmReachableGraph_IL_contains_newobj_for_concrete_Error_envelope_fields()
        {
            AssertWarmReachableGraphNewsUp(typeof(MTConnect.Headers.MTConnectErrorHeader));
            AssertWarmReachableGraphNewsUp(typeof(MTConnect.Errors.Error));
            AssertWarmReachableGraphNewsUp(typeof(System.Version));
        }

        // IL walker: proves JsonFunctions.WarmReachableGraph feeds a concrete
        // instance of <paramref name="expected"/> to STJ by locating EITHER a
        // `newobj <ctor-of-expected>` (fresh allocation) OR a
        // `ldsfld <static-field-of-type-expected>` (canonical-constant load)
        // in the method body. Runs on the compiled test assembly's view of
        // MTConnect.NET-JSON, so a source change that removes both producers
        // is caught deterministically at test time.
        //
        // Scans for the 5-byte patterns <newobj:0x73> <4-byte-token> and
        // <ldsfld:0x7E> <4-byte-token>. Full opcode-length decoding is
        // unnecessary: a spurious match would need a non-target opcode at
        // byte i whose following 4 bytes coincidentally decode to a valid
        // MemberRef token whose ResolveMethod/ResolveField returns a target
        // of the expected type — inside a ~40-byte method body, practically
        // impossible.
        //
        // Both producers are semantically equivalent for warm-up: STJ walks
        // the runtime type of the value passed into Serialize, and cannot
        // tell whether the instance came from a fresh newobj or from a
        // static field load (e.g. MTConnectVersions.Version25 for
        // System.Version).
        private static void AssertWarmReachableGraphNewsUp(System.Type expected)
        {
            var method = typeof(JsonFunctions).GetMethod(
                "WarmReachableGraph",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null,
                "JsonFunctions.WarmReachableGraph private static method must exist.");
            var body = method!.GetMethodBody();
            Assert.That(body, Is.Not.Null,
                "WarmReachableGraph must have a method body (not abstract / p-invoke).");
            var il = body!.GetILAsByteArray();
            Assert.That(il, Is.Not.Null);
            var module = method.Module;

            for (int i = 0; i + 4 < il!.Length; i++)
            {
                if (il[i] == 0x73)
                {
                    // newobj <ctor>
                    int token = System.BitConverter.ToInt32(il, i + 1);
                    System.Reflection.MethodBase? resolved;
                    try { resolved = module.ResolveMethod(token); }
                    catch { continue; }
                    if (resolved is System.Reflection.ConstructorInfo ctor
                        && ctor.DeclaringType == expected)
                    {
                        return;
                    }
                }
                else if (il[i] == 0x7E)
                {
                    // ldsfld <static-field>
                    int token = System.BitConverter.ToInt32(il, i + 1);
                    System.Reflection.FieldInfo? field;
                    try { field = module.ResolveField(token); }
                    catch { continue; }
                    if (field != null && field.FieldType == expected)
                    {
                        return;
                    }
                }
            }

            Assert.Fail(
                $"WarmReachableGraph must produce a concrete instance of {expected.FullName} " +
                "(via `new` or via a static-field load) so STJ walks its accessor graph at assembly load. " +
                "Without it, the first request that hits this envelope pays a cold LCG DynamicMethod emit " +
                "against the frozen singleton — which is the +3.2-3.8 MB/h RSS leak-in-miniature the warm-up exists to prevent.");
        }
#endif
    }
}
