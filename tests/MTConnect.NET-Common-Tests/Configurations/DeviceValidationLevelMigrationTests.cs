// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using MTConnect.Agents;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Configurations
{
    /// <summary>
    /// Pins the load-time migration bridge that mirrors <see cref="AgentConfiguration.InputValidationLevel"/>
    /// onto <see cref="AgentConfiguration.DeviceValidationLevel"/> when the source configuration omits the
    /// <c>deviceValidationLevel</c> key.
    ///
    /// Motivation: pre-split, a single <see cref="InputValidationLevel"/> knob gated both Observation/Asset
    /// validation and Device-tree validation. The split introduced by PR #218 leaves consumers who only set
    /// <c>inputValidationLevel</c> silently downgraded on the Device-tree side. The migration bridge
    /// preserves the pre-split expectation.
    ///
    /// Also pins the two setter guards from the same PR: an out-of-range integer on either enum property
    /// raises <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [TestFixture]
    [Category("DeviceValidationLevel")]
    public class DeviceValidationLevelMigrationTests
    {
        // ---------------------------------------------------------------
        // JSON load path: implicit InputValidationLevel mirror
        // ---------------------------------------------------------------

        /// <summary>Pins that a JSON config with only <c>inputValidationLevel: 3</c> (Strict) loads with <see cref="DeviceValidationLevel.Strict"/>.</summary>
        [Test]
        public void ReadJson_InputValidationLevel_Strict_Only_Mirrors_Onto_DeviceValidationLevel()
        {
            // The shipped JsonSerializerOptions treat enums as their integer ordinals — no
            // JsonStringEnumConverter registered — so the fixture wires the ordinal directly.
            var path = WriteTempJson("{\"inputValidationLevel\":3}");
            try
            {
                var config = AgentConfiguration.ReadJson(path);

                Assert.That(config, Is.Not.Null, "the loader must not have swallowed the config");
                Assert.That(config!.InputValidationLevel, Is.EqualTo(InputValidationLevel.Strict));
                Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Strict),
                    "when the JSON omits deviceValidationLevel, the loader must mirror InputValidationLevel " +
                    "so pre-split consumers keep getting Strict Device-tree validation");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>Pins that an explicit <c>deviceValidationLevel</c> beats the mirror even when both are present in JSON.</summary>
        [Test]
        public void ReadJson_Explicit_DeviceValidationLevel_Beats_Mirror()
        {
            // Deliberately set the two knobs to different arms so the mirror
            // path would corrupt DeviceValidationLevel if it fired anyway.
            var path = WriteTempJson(
                "{\"inputValidationLevel\":3,\"deviceValidationLevel\":0}");
            try
            {
                var config = AgentConfiguration.ReadJson(path);

                Assert.That(config, Is.Not.Null);
                Assert.That(config!.InputValidationLevel, Is.EqualTo(InputValidationLevel.Strict));
                Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Ignore),
                    "an explicit deviceValidationLevel key must NOT be silently overwritten by the mirror");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>Pins that a JSON config with neither knob set still loads with both defaulted to Warning.</summary>
        [Test]
        public void ReadJson_Empty_Object_Loads_With_Warning_Defaults()
        {
            var path = WriteTempJson("{}");
            try
            {
                var config = AgentConfiguration.ReadJson(path);

                Assert.That(config, Is.Not.Null);
                Assert.That(config!.InputValidationLevel, Is.EqualTo(InputValidationLevel.Warning));
                Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Warning));
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>Pins that the mirror covers every enum arm, not just Strict.</summary>
        [TestCase(InputValidationLevel.Ignore, DeviceValidationLevel.Ignore)]
        [TestCase(InputValidationLevel.Warning, DeviceValidationLevel.Warning)]
        [TestCase(InputValidationLevel.Remove, DeviceValidationLevel.Remove)]
        [TestCase(InputValidationLevel.Strict, DeviceValidationLevel.Strict)]
        public void ReadJson_InputValidationLevel_Only_Mirrors_All_Arms(
            InputValidationLevel input,
            DeviceValidationLevel expectedMirrored)
        {
            var path = WriteTempJson($"{{\"inputValidationLevel\":{(int)input}}}");
            try
            {
                var config = AgentConfiguration.ReadJson(path);

                Assert.That(config, Is.Not.Null);
                Assert.That(config!.DeviceValidationLevel, Is.EqualTo(expectedMirrored));
            }
            finally
            {
                File.Delete(path);
            }
        }

        // ---------------------------------------------------------------
        // Programmatic Normalize()
        // ---------------------------------------------------------------

        /// <summary>Pins that a caller who builds an <see cref="AgentConfiguration"/> in code and calls <see cref="AgentConfiguration.Normalize"/> latches the mirror into the backing field.</summary>
        /// <remarks>
        /// Dime cycle-2 finding M3-C2 hardened the getter to self-mirror in the null case, so the
        /// pre-Normalize read already returns <see cref="DeviceValidationLevel.Remove"/> in this
        /// scenario. Normalize's role is to latch that mirror into the backing field so
        /// post-Normalize serialisation carries the concrete value (not null); the sticky-suppression
        /// semantics still fall out of the null-check inside <see cref="AgentConfiguration.Normalize"/>.
        /// </remarks>
        [Test]
        public void Normalize_Mirrors_InputValidationLevel_When_DeviceValidationLevel_Not_Explicit()
        {
            var config = new AgentConfiguration();
            config.InputValidationLevel = InputValidationLevel.Remove;

            // Under the M3-C2 self-mirroring getter, the pre-Normalize read already reports the
            // mirror — programmatic callers no longer see the bare Warning default just because
            // they forgot to call Normalize().
            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Remove),
                "getter self-mirrors from _inputValidationLevel while _deviceValidationLevel is null — dime M3-C2");

            config.Normalize();

            // Post-Normalize the mirror is latched into the backing field; the getter returns the
            // same value via the explicit branch now instead of the null-mirror branch.
            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Remove));
        }

        /// <summary>Pins that an explicit assignment to <see cref="AgentConfiguration.DeviceValidationLevel"/> disables the mirror on subsequent <see cref="AgentConfiguration.Normalize"/> calls.</summary>
        [Test]
        public void Normalize_Skips_Mirror_When_DeviceValidationLevel_Explicitly_Set()
        {
            var config = new AgentConfiguration();
            config.InputValidationLevel = InputValidationLevel.Strict;
            config.DeviceValidationLevel = DeviceValidationLevel.Ignore;

            config.Normalize();

            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Ignore),
                "an explicit DeviceValidationLevel assignment must sticky-suppress the mirror");
        }

        // ---------------------------------------------------------------
        // Setter validation (F-SEC-003)
        // ---------------------------------------------------------------

        /// <summary>Pins that an out-of-range integer cast to <see cref="DeviceValidationLevel"/> is rejected at the setter.</summary>
        [Test]
        public void DeviceValidationLevel_Setter_Rejects_Undefined_Enum_Value()
        {
            var config = new AgentConfiguration();
            Assert.Throws<ArgumentOutOfRangeException>(
                () => config.DeviceValidationLevel = (DeviceValidationLevel)42);
        }

        /// <summary>Pins that an out-of-range integer cast to <see cref="InputValidationLevel"/> is rejected at the setter.</summary>
        [Test]
        public void InputValidationLevel_Setter_Rejects_Undefined_Enum_Value()
        {
            var config = new AgentConfiguration();
            Assert.Throws<ArgumentOutOfRangeException>(
                () => config.InputValidationLevel = (InputValidationLevel)99);
        }

        /// <summary>Pins that every defined enum arm is accepted (no false positives from the guard).</summary>
        [TestCase(DeviceValidationLevel.Ignore)]
        [TestCase(DeviceValidationLevel.Warning)]
        [TestCase(DeviceValidationLevel.Remove)]
        [TestCase(DeviceValidationLevel.Strict)]
        public void DeviceValidationLevel_Setter_Accepts_Every_Defined_Arm(DeviceValidationLevel arm)
        {
            var config = new AgentConfiguration();
            Assert.DoesNotThrow(() => config.DeviceValidationLevel = arm);
            Assert.That(config.DeviceValidationLevel, Is.EqualTo(arm));
        }

        /// <summary>
        /// Positive-arm coverage FLOOR for <see cref="InputValidationLevel"/>.
        /// The sibling assertion for <c>DeviceValidationLevel</c> above ensured no
        /// false-positive rejections; the parallel <c>InputValidationLevel</c>
        /// setter uses the same <c>Enum.IsDefined</c> guard (AgentConfiguration.cs:176)
        /// and must be pinned identically so a regression that widens or narrows
        /// the accepted arm set on the input axis fails loudly.
        /// </summary>
        [TestCase(InputValidationLevel.Ignore)]
        [TestCase(InputValidationLevel.Warning)]
        [TestCase(InputValidationLevel.Remove)]
        [TestCase(InputValidationLevel.Strict)]
        public void InputValidationLevel_Setter_Accepts_Every_Defined_Arm(InputValidationLevel arm)
        {
            var config = new AgentConfiguration();
            Assert.DoesNotThrow(() => config.InputValidationLevel = arm);
            Assert.That(config.InputValidationLevel, Is.EqualTo(arm));
        }

        /// <summary>
        /// Boundary coverage FLOOR for the <see cref="DeviceValidationLevel"/> setter guard
        /// (AgentConfiguration.cs:151). The setter uses <see cref="Enum.IsDefined(Type, object)"/>
        /// which rejects EVERY ordinal that is not a defined arm — the surviving valid arms
        /// are 0..3. Pin the four boundary shapes the FLOOR names — negative one, first
        /// invalid over-max ordinal, and the two integer extremes — so a regression that
        /// swaps <c>IsDefined</c> for a permissive range check (for example <c>value &lt;= Strict</c>,
        /// which would accept -1) fails on the first case rather than sneaking past the
        /// existing single (42) coverage.
        /// </summary>
        [TestCase(-1)]
        [TestCase(4)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void DeviceValidationLevel_Setter_Rejects_Out_Of_Range_Boundary(int ordinal)
        {
            var config = new AgentConfiguration();
            Assert.Throws<ArgumentOutOfRangeException>(
                () => config.DeviceValidationLevel = (DeviceValidationLevel)ordinal,
                $"DeviceValidationLevel setter must reject ordinal {ordinal} — Enum.IsDefined guard is strict against every value outside 0..3.");
        }

        /// <summary>
        /// Boundary coverage FLOOR for the <see cref="InputValidationLevel"/> setter guard
        /// (AgentConfiguration.cs:176). Same four shapes as the DeviceValidationLevel
        /// boundary — the guard is textually identical and must stay strict on both axes.
        /// </summary>
        [TestCase(-1)]
        [TestCase(4)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void InputValidationLevel_Setter_Rejects_Out_Of_Range_Boundary(int ordinal)
        {
            var config = new AgentConfiguration();
            Assert.Throws<ArgumentOutOfRangeException>(
                () => config.InputValidationLevel = (InputValidationLevel)ordinal,
                $"InputValidationLevel setter must reject ordinal {ordinal} — Enum.IsDefined guard is strict against every value outside 0..3.");
        }

        /// <summary>
        /// Pins the setter exception shape — <c>paramName</c> is <c>"value"</c> and the
        /// <c>ActualValue</c> carries the offending enum ordinal so callers logging the
        /// exception get the diagnostic. A regression that throws a bare
        /// <c>ArgumentException</c> (dropping the paramName / actual-value tuple) would
        /// still satisfy the coarser <c>Assert.Throws&lt;ArgumentOutOfRangeException&gt;</c>
        /// gates above only because ArgumentOutOfRangeException derives from
        /// ArgumentException — but the message shape carrying the two documented pieces of
        /// diagnostic is a contract callers depend on. This test pins that shape.
        /// </summary>
        [Test]
        public void DeviceValidationLevel_Setter_Exception_Carries_ParamName_And_ActualValue()
        {
            var config = new AgentConfiguration();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => config.DeviceValidationLevel = (DeviceValidationLevel)99);

            Assert.That(ex!.ParamName, Is.EqualTo("value"),
                "paramName is a documented invariant — subscribers log it verbatim.");
            Assert.That(ex.ActualValue, Is.EqualTo((DeviceValidationLevel)99),
                "ActualValue carries the offending ordinal so the diagnostic names the caller's mistake.");
            Assert.That(ex.Message, Does.Contain("DeviceValidationLevel"),
                "Message must name the enum so subscribers can distinguish DVL vs IVL failures.");
        }

        /// <summary>Same exception-shape pin as above, but for <see cref="InputValidationLevel"/>.</summary>
        [Test]
        public void InputValidationLevel_Setter_Exception_Carries_ParamName_And_ActualValue()
        {
            var config = new AgentConfiguration();
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => config.InputValidationLevel = (InputValidationLevel)99);

            Assert.That(ex!.ParamName, Is.EqualTo("value"));
            Assert.That(ex.ActualValue, Is.EqualTo((InputValidationLevel)99));
            Assert.That(ex.Message, Does.Contain("InputValidationLevel"));
        }

        // ---------------------------------------------------------------
        // Direct Normalize() — every arm, not just Remove
        // ---------------------------------------------------------------

        /// <summary>
        /// Pins the programmatic-<see cref="AgentConfiguration.Normalize"/> mirror across
        /// every <see cref="InputValidationLevel"/> arm — not just <c>Remove</c> as the
        /// original single test covered. The <c>ReadJson</c>-driven parametrised test
        /// above exercises the mirror THROUGH the loader; this test exercises the mirror
        /// DIRECTLY so a regression that skips the mirror on a specific arm (for example
        /// an off-by-one enum-cast bug producing wrong ordinals for arms 0 or 3) fails
        /// on the pinned arm rather than being masked by the ordinal happening to align.
        /// </summary>
        [TestCase(InputValidationLevel.Ignore, DeviceValidationLevel.Ignore)]
        [TestCase(InputValidationLevel.Warning, DeviceValidationLevel.Warning)]
        [TestCase(InputValidationLevel.Remove, DeviceValidationLevel.Remove)]
        [TestCase(InputValidationLevel.Strict, DeviceValidationLevel.Strict)]
        public void Normalize_Mirrors_Every_InputValidationLevel_Arm(
            InputValidationLevel input,
            DeviceValidationLevel expectedMirrored)
        {
            var config = new AgentConfiguration();
            config.InputValidationLevel = input;

            config.Normalize();

            Assert.That(config.DeviceValidationLevel, Is.EqualTo(expectedMirrored),
                $"Normalize must mirror InputValidationLevel.{input} onto DeviceValidationLevel.{expectedMirrored} because DeviceValidationLevel was never assigned explicitly.");
        }

        /// <summary>
        /// Pins Normalize idempotency: calling <see cref="AgentConfiguration.Normalize"/>
        /// a second time is a stable no-op — the DeviceValidationLevel value is the
        /// mirrored value, not the ctor default. A regression that reset the
        /// nullable backing field to null inside Normalize (making the mirror
        /// re-fire) would silently overwrite a subsequent explicit assignment;
        /// pinning idempotency catches that class of bug.
        /// </summary>
        [Test]
        public void Normalize_Is_Idempotent_On_Repeat_Calls()
        {
            var config = new AgentConfiguration();
            config.InputValidationLevel = InputValidationLevel.Strict;

            config.Normalize();
            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Strict),
                "first Normalize mirrors the input axis onto the device axis.");

            config.Normalize();
            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Strict),
                "second Normalize must be a stable no-op — the mirrored value must not regress to the ctor default.");
        }

        /// <summary>
        /// Pins that setting <see cref="AgentConfiguration.InputValidationLevel"/>
        /// AFTER an explicit <see cref="AgentConfiguration.DeviceValidationLevel"/>
        /// assignment does NOT re-arm the mirror. The DVL setter populates the
        /// nullable backing field to a non-null value and the IVL setter never
        /// touches it — a caller who explicitly set DVL then later set IVL must
        /// not have DVL silently overwritten on the next Normalize call.
        /// </summary>
        [Test]
        public void Normalize_Explicit_DeviceValidationLevel_Then_Later_InputValidationLevel_Assignment_Does_Not_Rearm_Mirror()
        {
            var config = new AgentConfiguration();
            config.DeviceValidationLevel = DeviceValidationLevel.Ignore;
            // The DVL setter populated the nullable backing field to non-null. Later
            // IVL assignment must not reset it to null.
            config.InputValidationLevel = InputValidationLevel.Strict;

            config.Normalize();

            Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Ignore),
                "later InputValidationLevel assignment must not re-arm the mirror — the explicit-DVL latch is sticky.");
        }

        /// <summary>
        /// Pins sticky suppression across every <see cref="DeviceValidationLevel"/> arm —
        /// the existing single-arm sticky-suppression test only covered
        /// <c>Strict → Ignore</c>. A regression that resets the nullable backing
        /// field to null only on a specific arm (for example on the ctor-default
        /// arm) would slip past the single existing test.
        /// </summary>
        [TestCase(DeviceValidationLevel.Ignore)]
        [TestCase(DeviceValidationLevel.Warning)]
        [TestCase(DeviceValidationLevel.Remove)]
        [TestCase(DeviceValidationLevel.Strict)]
        public void Normalize_Sticky_Suppression_Holds_For_Every_Explicit_Arm(DeviceValidationLevel explicitArm)
        {
            var config = new AgentConfiguration();
            // Pick an IVL arm that is DIFFERENT from the explicit DVL arm so the mirror
            // path would corrupt DVL if the latch failed. Both enums share ordinals so
            // the cast is meaningful.
            config.InputValidationLevel = explicitArm == DeviceValidationLevel.Strict
                ? InputValidationLevel.Ignore
                : InputValidationLevel.Strict;
            config.DeviceValidationLevel = explicitArm;

            config.Normalize();

            Assert.That(config.DeviceValidationLevel, Is.EqualTo(explicitArm),
                $"the explicit-DVL latch must stick for arm {explicitArm} regardless of the divergent IVL value.");
        }

        // ---------------------------------------------------------------
        // YAML load path — parallel to ReadJson mirror
        // ---------------------------------------------------------------

        /// <summary>
        /// Pins the YAML load path invokes <see cref="AgentConfiguration.Normalize"/> —
        /// the docstring on <c>Normalize</c> lists <c>ReadYaml</c> as an invocation site
        /// (AgentConfiguration.cs:240) but the existing fixture only exercises the JSON
        /// path. A regression that dropped the <c>configuration.Normalize()</c> call from
        /// <c>ReadYaml</c> (AgentConfiguration.cs:439) would silently downgrade YAML-loading
        /// consumers to the ctor-default DeviceValidationLevel on every arm change of
        /// InputValidationLevel — a diagnostic-silent behavior break.
        /// </summary>
        [Test]
        public void ReadYaml_InputValidationLevel_Only_Mirrors_Onto_DeviceValidationLevel()
        {
            var path = WriteTempYaml("inputValidationLevel: 3\n");
            try
            {
                var config = AgentConfiguration.ReadYaml(path);

                Assert.That(config, Is.Not.Null, "the YAML loader must not have swallowed the config.");
                Assert.That(config!.InputValidationLevel, Is.EqualTo(InputValidationLevel.Strict));
                Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Strict),
                    "the YAML load path must invoke Normalize so pre-split consumers keep the mirrored DeviceValidationLevel.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>Pins that YAML explicit <c>deviceValidationLevel</c> beats the mirror — sister of the JSON test.</summary>
        [Test]
        public void ReadYaml_Explicit_DeviceValidationLevel_Beats_Mirror()
        {
            var path = WriteTempYaml(
                "inputValidationLevel: 3\ndeviceValidationLevel: 0\n");
            try
            {
                var config = AgentConfiguration.ReadYaml(path);

                Assert.That(config, Is.Not.Null);
                Assert.That(config!.InputValidationLevel, Is.EqualTo(InputValidationLevel.Strict));
                Assert.That(config.DeviceValidationLevel, Is.EqualTo(DeviceValidationLevel.Ignore),
                    "an explicit deviceValidationLevel key in YAML must NOT be silently overwritten by the mirror.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        // ---------------------------------------------------------------
        // Load-time enum-value validation — dime H2 + M4
        // ---------------------------------------------------------------

        /// <summary>
        /// Pins that a JSON config with an out-of-range integer for
        /// <c>inputValidationLevel</c> throws an <see cref="ArgumentException"/>
        /// whose message names the offending configuration path — the pre-fix
        /// <c>catch { }</c> silently returned null, hiding the actionable
        /// setter diagnostic from the operator. Dime cycle-1 finding H2
        /// (security-audit A09 + code-review F-CR-241-04) required the swallow
        /// be replaced; M4 required the path be attached.
        /// </summary>
        [Test]
        public void ReadJson_Invalid_Enum_Ordinal_Throws_ArgumentException_With_Path()
        {
            var path = WriteTempJson("{\"inputValidationLevel\":42}");
            try
            {
                var ex = Assert.Throws<ArgumentException>(() => AgentConfiguration.ReadJson(path));
                Assert.That(ex!.Message, Does.Contain(path),
                    "the wrapped exception must carry the configuration path so operators can trace the bad key back to its file.");
                Assert.That(ex.Message, Does.Contain("InputValidationLevel"),
                    "the wrapped exception must preserve the setter's actionable message naming the failing enum.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Sibling of the JSON test — the YAML load path must also surface the
        /// enum-out-of-range setter exception (unwrapping any deserializer-level
        /// wrapper — YamlDotNet nests AOORE inside its own container) with the
        /// configuration path attached.
        /// </summary>
        [Test]
        public void ReadYaml_Invalid_Enum_Ordinal_Throws_ArgumentException_With_Path()
        {
            var path = WriteTempYaml("inputValidationLevel: 42\n");
            try
            {
                var ex = Assert.Throws<ArgumentException>(() => AgentConfiguration.ReadYaml(path));
                Assert.That(ex!.Message, Does.Contain(path),
                    "the wrapped exception must carry the configuration path so operators can trace the bad key back to its file.");
                Assert.That(ex.Message, Does.Contain("InputValidationLevel"),
                    "the unwrapped setter message must survive so callers still see the actionable diagnostic.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Pins that a malformed-but-parseable-shape JSON (invalid syntax that
        /// makes JsonSerializer throw a non-enum exception) preserves the
        /// documented null-return contract — the H2 fix converts silent swallow
        /// into Trace.TraceError diagnostic but must NOT change the return
        /// contract for non-enum failures.
        /// </summary>
        [Test]
        public void ReadJson_Malformed_Json_Returns_Null_Preserving_Loader_Contract()
        {
            var path = WriteTempJson("{ this is not valid json ]");
            try
            {
                AgentConfiguration config = new AgentConfiguration();
                Assert.DoesNotThrow(() => config = AgentConfiguration.ReadJson(path),
                    "non-enum parse failures must not throw — the documented loader contract is null-on-failure.");
                Assert.That(config, Is.Null,
                    "the loader must return null for malformed input to preserve pre-fix caller contracts.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        // ---------------------------------------------------------------
        // UnwrapArgumentOutOfRange — private helper depth-bound pins
        // ---------------------------------------------------------------
        //
        // The H2 fix introduces a private static helper `UnwrapArgumentOutOfRange(Exception)`
        // on <see cref="AgentConfiguration"/> that walks the <see cref="Exception.InnerException"/>
        // chain looking for an <see cref="ArgumentOutOfRangeException"/> — deserializers
        // (YamlDotNet notably) nest the setter throw inside their own container. The walk
        // is depth-bounded at MaxUnwrapDepth = 16 to defend against a pathological deeply-
        // nested chain looping forever. The public YAML load path only produces a chain
        // of depth 2-3 so it cannot exercise the depth ceiling; these fixtures pin the
        // ceiling directly via reflection so a regression that removes the ceiling
        // (introducing an unbounded loop) OR bumps it to a different value fails loudly.

        private static ArgumentOutOfRangeException? InvokeUnwrap(Exception ex)
        {
            var method = typeof(AgentConfiguration).GetMethod(
                "UnwrapArgumentOutOfRange",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null,
                "UnwrapArgumentOutOfRange must exist as a static non-public helper on AgentConfiguration — the H2 fix contract.");
            return (ArgumentOutOfRangeException?)method!.Invoke(null, new object?[] { ex });
        }

        private static Exception BuildWrappedChain(Exception innermost, int wrapCount)
        {
            var current = innermost;
            for (var i = 0; i < wrapCount; i++)
            {
                current = new InvalidOperationException($"wrap-{i}", current);
            }
            return current;
        }

        /// <summary>
        /// Pins that <c>UnwrapArgumentOutOfRange</c> returns the AOORE when it sits
        /// exactly at the surface (depth 0). The direct <c>catch (ArgumentOutOfRangeException)</c>
        /// filter should hit before this helper runs in the loader, but the helper
        /// must still handle a depth-0 chain because <see cref="Exception.InnerException"/>
        /// walks include their own root.
        /// </summary>
        [Test]
        public void UnwrapArgumentOutOfRange_Returns_Root_When_Root_Is_AOORE()
        {
            var aoore = new ArgumentOutOfRangeException("value", "root");

            var result = InvokeUnwrap(aoore);

            Assert.That(result, Is.SameAs(aoore),
                "the helper must return the AOORE when it sits at depth 0 (root).");
        }

        /// <summary>
        /// Pins that <c>UnwrapArgumentOutOfRange</c> returns the AOORE when it sits
        /// at exactly the deepest depth the ceiling still visits — the loop iterates
        /// <c>i = 0..15</c>, checking depths 0..15 inclusive. AOORE at depth 15 is
        /// visited on iteration 15 and returned; AOORE at depth 16 is never visited
        /// (loop exits after iteration 15 before advancing to depth 16).
        /// </summary>
        [Test]
        public void UnwrapArgumentOutOfRange_Finds_AOORE_At_Ceiling_Depth_Fifteen()
        {
            // 15 wrappers around the AOORE puts the AOORE at depth 15 from the outer root.
            var aoore = new ArgumentOutOfRangeException("value", "deep-15");
            var chain = BuildWrappedChain(aoore, wrapCount: 15);

            var result = InvokeUnwrap(chain);

            Assert.That(result, Is.SameAs(aoore),
                "the helper must find the AOORE at depth 15 — the deepest depth the MaxUnwrapDepth = 16 loop still visits (i = 15).");
        }

        /// <summary>
        /// Pins that <c>UnwrapArgumentOutOfRange</c> returns null when the AOORE sits
        /// past the depth ceiling — the loop bails after 16 iterations without walking
        /// to depth 16 or beyond. A regression that removes the bound and loops until
        /// InnerException is null would find the AOORE at depth 20 and return it — that
        /// would satisfy the H2 unwrap contract but violate the depth-guard invariant
        /// against pathological chains. The FLOOR pins the exact ceiling.
        /// </summary>
        [Test]
        public void UnwrapArgumentOutOfRange_Returns_Null_When_AOORE_Sits_Past_Depth_Ceiling()
        {
            var aoore = new ArgumentOutOfRangeException("value", "deep-20");
            var chain = BuildWrappedChain(aoore, wrapCount: 20);

            var result = InvokeUnwrap(chain);

            Assert.That(result, Is.Null,
                "the helper must NOT walk past MaxUnwrapDepth = 16 — an AOORE at depth 20 must return null so a pathological deeply-nested chain never loops forever.");
        }

        /// <summary>
        /// Pins that <c>UnwrapArgumentOutOfRange</c> returns null on a chain that
        /// contains no AOORE at any depth. The loader relies on this null-return to
        /// distinguish an enum-out-of-range setter throw from a generic parser /
        /// IO failure — the former is rethrown as ArgumentException, the latter is
        /// traced and null-returned per the loader contract. A regression that
        /// returned the outermost exception on a no-match walk would misroute
        /// generic failures into the enum-error path.
        /// </summary>
        [Test]
        public void UnwrapArgumentOutOfRange_Returns_Null_On_Chain_Without_AOORE()
        {
            var innermost = new InvalidOperationException("innermost");
            var chain = BuildWrappedChain(innermost, wrapCount: 5);

            var result = InvokeUnwrap(chain);

            Assert.That(result, Is.Null,
                "the helper must return null when the InnerException chain contains no AOORE — the loader routes non-enum failures to the trace-and-return-null path.");
        }

        /// <summary>
        /// Pins that <c>UnwrapArgumentOutOfRange</c> returns null when handed a null
        /// exception. The current implementation guards via the loop condition
        /// (<c>current != null</c>) so the method is null-safe. A regression that
        /// dereferenced the parameter before the guard would NRE on this input.
        /// </summary>
        [Test]
        public void UnwrapArgumentOutOfRange_Returns_Null_On_Null_Input()
        {
            var result = InvokeUnwrap(null!);

            Assert.That(result, Is.Null,
                "the helper must be null-safe — the loop condition current != null guards the very first iteration.");
        }

        // ---------------------------------------------------------------
        // Trace-cap-hit diagnostic pin — dime L2-C2 (UnwrapArgumentOutOfRange)
        // ---------------------------------------------------------------
        //
        // The cycle-2 L2-C2 change added a `Trace.TraceWarning` line to
        // <c>UnwrapArgumentOutOfRange</c> that fires when the walk exits after
        // MaxUnwrapDepth iterations with a non-null current frame — the diagnostic
        // tells the operator the walk gave up before finding a wrapped AOORE that
        // may exist deeper in the chain. The existing depth-ceiling test
        // (<c>UnwrapArgumentOutOfRange_Returns_Null_When_AOORE_Sits_Past_Depth_Ceiling</c>)
        // pins the null-return contract but does NOT capture the trace output.
        // A regression that removes the TraceWarning line (silently degrading the
        // diagnostic) still passes the null-return test. This fixture attaches a
        // TraceListener to capture the warning and pins its shape.

        /// <summary>
        /// Pins that <c>UnwrapArgumentOutOfRange</c> emits a
        /// <see cref="Trace.TraceWarning(string)"/> when the walk exits at
        /// <c>MaxUnwrapDepth = 16</c> with a non-null current frame remaining —
        /// the diagnostic operators depend on to know a pathological wrapping
        /// chain was truncated. Dime cycle-2 finding L2-C2.
        /// </summary>
        [Test]
        public void UnwrapArgumentOutOfRange_Traces_Warning_When_MaxUnwrapDepth_Exceeded()
        {
            var listener = new CapturingTraceListener();
            Trace.Listeners.Add(listener);
            try
            {
                // 20 wrappers around an AOORE puts the AOORE at depth 20 — past
                // the MaxUnwrapDepth = 16 ceiling. The walk exits with `current`
                // non-null (depth 16 is an InvalidOperationException wrap, not
                // the AOORE), so the trace line fires.
                var aoore = new ArgumentOutOfRangeException("value", "deep-20");
                var chain = BuildWrappedChain(aoore, wrapCount: 20);

                var result = InvokeUnwrap(chain);

                Assert.That(result, Is.Null,
                    "precondition — the depth-cap-hit path returns null; the trace warning is what pins the operator-visible diagnostic.");
                Assert.That(listener.Warnings.Count, Is.EqualTo(1),
                    "the L2-C2 fix requires exactly one Trace.TraceWarning per cap-hit call — not zero (regression that dropped the trace) and not more (regression that placed it inside the loop).");
                var warning = listener.Warnings[0];
                Assert.That(warning, Does.Contain("UnwrapArgumentOutOfRange"),
                    "the warning must name the helper so operators can grep for the specific site.");
                Assert.That(warning, Does.Contain("MaxUnwrapDepth=16"),
                    "the warning must carry the exact ceiling value so a regression that changed the constant surfaces here rather than silently.");
            }
            finally
            {
                Trace.Listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Pins that <c>UnwrapArgumentOutOfRange</c> does NOT emit the cap-hit
        /// trace warning on a chain that terminates naturally before the ceiling
        /// (InnerException is null earlier). A regression that fired the warning
        /// unconditionally would spam operator logs on every non-enum failure.
        /// </summary>
        [Test]
        public void UnwrapArgumentOutOfRange_Does_Not_Trace_Warning_On_Chain_Shorter_Than_Ceiling()
        {
            var listener = new CapturingTraceListener();
            Trace.Listeners.Add(listener);
            try
            {
                // 5 wrappers around an InvalidOperationException — chain
                // terminates at depth 5 (InnerException becomes null),
                // no AOORE anywhere, but well within the MaxUnwrapDepth = 16
                // ceiling so the trace warning line MUST NOT fire.
                var innermost = new InvalidOperationException("innermost");
                var chain = BuildWrappedChain(innermost, wrapCount: 5);

                var result = InvokeUnwrap(chain);

                Assert.That(result, Is.Null,
                    "precondition — no AOORE anywhere in the chain so the helper returns null.");
                Assert.That(listener.Warnings.Count, Is.EqualTo(0),
                    "the trace warning must ONLY fire when the depth ceiling is reached; a chain that terminates naturally before the ceiling must not surface the diagnostic.");
            }
            finally
            {
                Trace.Listeners.Remove(listener);
            }
        }

        // ---------------------------------------------------------------
        // Loader triage across every overload — dime M2-C2
        // ---------------------------------------------------------------
        //
        // The M2-C2 refactor extracted the three-clause triage into one
        // <c>LoadWithTriage&lt;T&gt;</c> helper shared by all four public loader
        // entrypoints:
        //
        //   * ReadJson&lt;T&gt;(string) — the generic entrypoint (also the target of
        //     the ReadJson(string) shortcut, so the existing
        //     ReadJson_Invalid_Enum_Ordinal_Throws_ArgumentException_With_Path test
        //     exercises this path transitively).
        //   * ReadJson(Type, string) — the Type-taking overload.
        //   * ReadYaml&lt;T&gt;(string) — the generic entrypoint (also the target of
        //     the ReadYaml(string) shortcut, exercised transitively above).
        //   * ReadYaml(Type, string) — the Type-taking overload.
        //
        // The two Type-taking overloads were previously untested end-to-end.
        // A regression that swapped their <c>LoadWithTriage</c> body for the
        // pre-M2-C2 inline shape (dropping the middle wrapped-AOORE catch on
        // the JSON side, for example) would slip past the existing coverage.
        // These fixtures pin the shared-triage contract on every loader.

        /// <summary>
        /// Pins that the Type-taking <c>ReadJson(Type, path)</c> overload
        /// surfaces the invalid-enum diagnostic through the shared
        /// <c>LoadWithTriage</c> helper — the M2-C2 extraction requires
        /// every overload behave identically. A regression that reverted this
        /// overload to an inline catch (dropping the middle wrapped-AOORE clause)
        /// would return null instead of the actionable ArgumentException.
        /// </summary>
        [Test]
        public void ReadJson_Type_Overload_Invalid_Enum_Ordinal_Throws_ArgumentException_With_Path()
        {
            var path = WriteTempJson("{\"inputValidationLevel\":42}");
            try
            {
                var ex = Assert.Throws<ArgumentException>(
                    () => AgentConfiguration.ReadJson(typeof(AgentConfiguration), path));
                Assert.That(ex!.Message, Does.Contain(path),
                    "the Type-taking JSON overload must attach the configuration path — parity with the generic overload.");
                Assert.That(ex.Message, Does.Contain("InputValidationLevel"),
                    "the Type-taking JSON overload must preserve the setter's actionable message.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Sibling pin for the Type-taking <c>ReadYaml(Type, path)</c> overload.
        /// The YAML path exercises the wrapped-AOORE clause because YamlDotNet
        /// nests the AOORE inside its own container exception.
        /// </summary>
        [Test]
        public void ReadYaml_Type_Overload_Invalid_Enum_Ordinal_Throws_ArgumentException_With_Path()
        {
            var path = WriteTempYaml("inputValidationLevel: 42\n");
            try
            {
                var ex = Assert.Throws<ArgumentException>(
                    () => AgentConfiguration.ReadYaml(typeof(AgentConfiguration), path));
                Assert.That(ex!.Message, Does.Contain(path),
                    "the Type-taking YAML overload must attach the configuration path — parity with the generic overload.");
                Assert.That(ex.Message, Does.Contain("InputValidationLevel"),
                    "the Type-taking YAML overload must unwrap the deserializer wrapper and preserve the setter's actionable message.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Pins that the Type-taking <c>ReadJson(Type, path)</c> overload
        /// preserves the null-return loader contract on non-enum failures —
        /// symmetric with <see cref="ReadJson_Malformed_Json_Returns_Null_Preserving_Loader_Contract"/>
        /// for the generic overload.
        /// </summary>
        [Test]
        public void ReadJson_Type_Overload_Malformed_Json_Returns_Null_Preserving_Loader_Contract()
        {
            var path = WriteTempJson("{ this is not valid json ]");
            try
            {
                AgentConfiguration config = new AgentConfiguration();
                Assert.DoesNotThrow(() => config = AgentConfiguration.ReadJson(typeof(AgentConfiguration), path),
                    "non-enum parse failures must not throw — the documented loader contract is null-on-failure.");
                Assert.That(config, Is.Null,
                    "the Type-taking JSON overload must return null for malformed input — parity with the generic overload's contract.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Pins the shared <c>LoadWithTriage&lt;T&gt;</c> helper's
        /// <c>where T : AgentConfiguration</c> generic constraint — the M2-C2
        /// refactor relies on returning null on the generic fall-through
        /// (requires a reference-type constraint, which the AgentConfiguration
        /// base-type constraint implies), and cycle-3 F-SIMP-C3-001 tightened
        /// the constraint from the original <c>where T : class</c> so the helper
        /// itself can stamp <see cref="AgentConfiguration.Path"/> and invoke
        /// <see cref="AgentConfiguration.Normalize"/> on the loaded instance
        /// without duplicating that tail in every loader closure. A regression
        /// that widened the constraint back to <c>class</c> would compile-error
        /// inside the helper body on the Path/Normalize lines; a regression
        /// that dropped it entirely would compile-error on <c>return null</c>.
        /// Pinning both signals via reflection catches the change at test time
        /// rather than through a downstream build break.
        /// </summary>
        [Test]
        public void LoadWithTriage_Has_AgentConfiguration_Constraint_On_T_Parameter()
        {
            var method = typeof(AgentConfiguration).GetMethod(
                "LoadWithTriage",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null,
                "LoadWithTriage must exist as a static non-public generic helper on AgentConfiguration — the M2-C2 extraction contract.");
            Assert.That(method!.IsGenericMethodDefinition, Is.True,
                "LoadWithTriage must remain a generic method — collapsing to a non-generic returning AgentConfiguration would re-force each caller to cast, undoing the extraction.");
            var genericArgs = method.GetGenericArguments();
            Assert.That(genericArgs.Length, Is.EqualTo(1),
                "LoadWithTriage takes exactly one generic parameter T.");
            var constraints = genericArgs[0].GetGenericParameterConstraints();
            Assert.That(
                constraints,
                Has.Member(typeof(AgentConfiguration)),
                "T must carry the AgentConfiguration base-type constraint (`where T : AgentConfiguration`) so the helper can stamp Path and call Normalize on the loaded instance directly — dime F-SIMP-C3-001 shape contract; `where T : AgentConfiguration` implies the ReferenceTypeConstraint by construction so the `return null` in the generic fall-through still compiles.");
        }

        /// <summary>
        /// Pins that the generic-fall-through branch of <c>LoadWithTriage</c> emits
        /// a <see cref="Trace.TraceError(string)"/> line naming the configuration
        /// path, not just silently returning null. The pre-M2-C2 inline triage
        /// contained the same trace line, so this is a shape-preservation pin —
        /// a regression that dropped the trace on the shared helper would remove
        /// operator diagnostics for every loader at once (blast radius × 4 vs. × 1
        /// pre-extraction).
        /// </summary>
        [Test]
        public void LoadWithTriage_Generic_Fall_Through_Traces_Error_With_Path()
        {
            var listener = new CapturingTraceListener();
            Trace.Listeners.Add(listener);
            try
            {
                var path = WriteTempJson("{ this is not valid json ]");
                try
                {
                    var result = AgentConfiguration.ReadJson(path);

                    Assert.That(result, Is.Null,
                        "precondition — malformed JSON hits the generic fall-through and returns null.");
                    Assert.That(listener.Errors.Count, Is.GreaterThanOrEqualTo(1),
                        "the generic-fall-through must emit at least one Trace.TraceError so the operator sees the diagnostic — dime M2-C2 shape preservation.");
                    Assert.That(listener.Errors[0], Does.Contain(path),
                        "the error trace must name the configuration path so the operator can trace the failure back to its file.");
                    Assert.That(listener.Errors[0], Does.Contain("Config load failed"),
                        "the error trace must carry the documented 'Config load failed' prefix used by the extracted helper.");
                }
                finally
                {
                    File.Delete(path);
                }
            }
            finally
            {
                Trace.Listeners.Remove(listener);
            }
        }

        // ---------------------------------------------------------------
        // F-SEC-002 — MapInputToDeviceValidationLevel exhaustive switch,
        // default-arm throw on unmapped ordinal
        // ---------------------------------------------------------------

        /// <summary>
        /// Pins the default arm of the <c>MapInputToDeviceValidationLevel</c>
        /// switch expression that cycle-3 F-SEC-002 introduced. The four mapped
        /// arms (Ignore / Warning / Remove / Strict) are exercised indirectly by
        /// <c>Normalize_Mirrors_Every_InputValidationLevel_Arm</c> above — but
        /// the default arm, whose entire raison d'être is to fail loudly rather
        /// than silently coerce to <c>default(DeviceValidationLevel)</c>, has
        /// no test on the mapped-arm side. A regression that swapped
        /// <c>_ =&gt; throw new InvalidOperationException(...)</c> for
        /// <c>_ =&gt; default(DeviceValidationLevel)</c> — the exact footgun the
        /// refactor eliminated — would silently pass every existing test and
        /// re-introduce the runtime coercion the switch was written to prevent.
        ///
        /// The public setter <c>InputValidationLevel = ...</c> now guards via
        /// <c>ThrowIfUndefined</c>, so the default arm cannot be reached
        /// through the normal API surface — the private backing field must be
        /// poked. Reflection-invoking the private static helper directly with
        /// an unmapped ordinal is the minimal, precise pin.
        /// </summary>
        [Test]
        public void MapInputToDeviceValidationLevel_Default_Arm_Throws_InvalidOperationException_On_Unmapped_Ordinal()
        {
            var method = typeof(AgentConfiguration).GetMethod(
                "MapInputToDeviceValidationLevel",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null,
                "MapInputToDeviceValidationLevel must exist as a static non-public helper on AgentConfiguration — the F-SEC-002 refactor contract.");

            var unmapped = (InputValidationLevel)99;
            var ex = Assert.Throws<TargetInvocationException>(
                () => method!.Invoke(null, new object[] { unmapped }));
            Assert.That(ex!.InnerException, Is.InstanceOf<InvalidOperationException>(),
                "the default arm of the switch expression must throw InvalidOperationException — dime F-SEC-002 hard-fail contract; a regression to `_ => default(DeviceValidationLevel)` would silently coerce unmapped ordinals to Ignore, re-introducing the exact static-alias footgun the exhaustive switch eliminated.");
            Assert.That(ex.InnerException!.Message, Does.Contain("Unmapped InputValidationLevel"),
                "the InvalidOperationException message must name the enum type so the operator sees which mirror-mapping table is stale.");
            Assert.That(ex.InnerException.Message, Does.Contain("99"),
                "the InvalidOperationException message must carry the offending ordinal so a shipped mismatch can be diagnosed from the trace alone.");
        }

        /// <summary>
        /// Pins that each of the four <see cref="InputValidationLevel"/> arms
        /// maps to its ordinal-matching <see cref="DeviceValidationLevel"/> arm
        /// through the F-SEC-002 helper directly (not just transitively via
        /// <c>Normalize</c>). A regression that transposed two arms in the
        /// switch expression — e.g. mapped <c>InputValidationLevel.Remove</c>
        /// to <c>DeviceValidationLevel.Warning</c> — would leave the transitive
        /// tests passing when the enum ordinals happened to align on the
        /// permuted arms; the direct pin fails on the transposition.
        /// </summary>
        [TestCase(InputValidationLevel.Ignore, DeviceValidationLevel.Ignore)]
        [TestCase(InputValidationLevel.Warning, DeviceValidationLevel.Warning)]
        [TestCase(InputValidationLevel.Remove, DeviceValidationLevel.Remove)]
        [TestCase(InputValidationLevel.Strict, DeviceValidationLevel.Strict)]
        public void MapInputToDeviceValidationLevel_Every_Mapped_Arm_Returns_Ordinal_Mirror(
            InputValidationLevel input, DeviceValidationLevel expected)
        {
            var method = typeof(AgentConfiguration).GetMethod(
                "MapInputToDeviceValidationLevel",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null,
                "MapInputToDeviceValidationLevel must exist as a static non-public helper on AgentConfiguration — the F-SEC-002 refactor contract.");

            var result = (DeviceValidationLevel)method!.Invoke(null, new object[] { input })!;
            Assert.That(result, Is.EqualTo(expected),
                $"MapInputToDeviceValidationLevel({input}) must return {expected} — dime F-SEC-002 explicit-switch mapping contract; a transposed arm would flip the DVL mirror silently.");
        }

        // ---------------------------------------------------------------
        // F-SIMP-C3-001 — LoadWithTriage stamps configuration.Path on the
        // returned instance (hoisted from the four loader closures)
        // ---------------------------------------------------------------

        /// <summary>
        /// Pins that <see cref="AgentConfiguration.ReadJson(string)"/> stamps the
        /// loaded configuration's <see cref="AgentConfiguration.Path"/> property
        /// with the file the config was loaded from. The M2-C2 refactor extracted
        /// the triage wrapper and cycle-3 F-SIMP-C3-001 hoisted the
        /// <c>configuration.Path = configurationPath</c> assignment INTO the
        /// helper (out of the four closures). A regression that dropped the
        /// hoisted assignment would leave <c>Path</c> null after every load,
        /// silently breaking downstream file-relative resolutions (the Path
        /// property's docstring names it "the default target when the
        /// configuration is saved") — a diagnostic-silent behavior break.
        /// </summary>
        [Test]
        public void ReadJson_Stamps_Configuration_Path_On_Loaded_Instance()
        {
            var path = WriteTempJson("{\"inputValidationLevel\":1}");
            try
            {
                var config = AgentConfiguration.ReadJson(path);

                Assert.That(config, Is.Not.Null,
                    "precondition — the loader must not have swallowed a valid config.");
                Assert.That(config!.Path, Is.EqualTo(path),
                    "ReadJson must stamp AgentConfiguration.Path with the file the config was loaded from — dime F-SIMP-C3-001 hoisted-stamp contract; a regression that dropped `configuration.Path = configurationPath` from LoadWithTriage would silently leave downstream save/relative-resolve paths null.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Sibling of the JSON test — <see cref="AgentConfiguration.ReadYaml(string)"/>
        /// must also stamp <see cref="AgentConfiguration.Path"/>. The four loader
        /// entrypoints share <c>LoadWithTriage</c>, so a regression that dropped
        /// the hoisted stamp would fail on every loader in parallel; pinning both
        /// JSON and YAML surfaces catches an unlikely path-specific regression
        /// (e.g. a partial revert that only touched the YAML closure).
        /// </summary>
        [Test]
        public void ReadYaml_Stamps_Configuration_Path_On_Loaded_Instance()
        {
            var path = WriteTempYaml("inputValidationLevel: 1\n");
            try
            {
                var config = AgentConfiguration.ReadYaml(path);

                Assert.That(config, Is.Not.Null,
                    "precondition — the loader must not have swallowed a valid config.");
                Assert.That(config!.Path, Is.EqualTo(path),
                    "ReadYaml must stamp AgentConfiguration.Path with the file the config was loaded from — dime F-SIMP-C3-001 hoisted-stamp contract; sibling of the JSON pin.");
            }
            finally
            {
                File.Delete(path);
            }
        }

        // ---------------------------------------------------------------
        // Trace-listener capture harness
        // ---------------------------------------------------------------

        private sealed class CapturingTraceListener : TraceListener
        {
            public System.Collections.Generic.List<string> Warnings { get; } = new System.Collections.Generic.List<string>();
            public System.Collections.Generic.List<string> Errors { get; } = new System.Collections.Generic.List<string>();
            private readonly StringBuilder _lineBuffer = new StringBuilder();

            public override void Write(string? message) => _lineBuffer.Append(message);

            public override void WriteLine(string? message)
            {
                _lineBuffer.Append(message);
                // No routing hint — the raw TraceWarning / TraceError calls flow
                // through TraceEvent below with a matching event type. The
                // Write/WriteLine fallbacks are here so a listener attached to
                // a plain Trace.WriteLine still captures.
                _lineBuffer.Clear();
            }

            public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? message)
            {
                switch (eventType)
                {
                    case TraceEventType.Warning:
                        Warnings.Add(message ?? string.Empty);
                        break;
                    case TraceEventType.Error:
                    case TraceEventType.Critical:
                        Errors.Add(message ?? string.Empty);
                        break;
                }
            }

            public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? format, params object?[]? args)
            {
                var message = args != null && args.Length > 0 && format != null ? string.Format(format, args) : format;
                TraceEvent(eventCache, source, eventType, id, message);
            }
        }

        // ---------------------------------------------------------------
        // Fixture harness
        // ---------------------------------------------------------------

        private static string WriteTempJson(string json)
        {
            var path = Path.Combine(Path.GetTempPath(), $"agent-config-{Guid.NewGuid():N}.json");
            File.WriteAllText(path, json);
            return path;
        }

        private static string WriteTempYaml(string yaml)
        {
            var path = Path.Combine(Path.GetTempPath(), $"agent-config-{Guid.NewGuid():N}.yaml");
            File.WriteAllText(path, yaml);
            return path;
        }
    }
}
