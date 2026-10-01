// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

namespace MTConnect.AgentModule.MqttRelay.Tests
{
    /// <summary>
    /// Pins the SslProtocols-resolution contract for the MQTT relay
    /// module. The resolver turns the user-supplied
    /// <see cref="MqttRelayModuleConfiguration.SslProtocols"/> list
    /// into a bitwise-OR'd <see cref="SslProtocols"/> value the MQTT
    /// client stack accepts, and validates the input so a
    /// misconfiguration surfaces at module load rather than as a
    /// silent downgrade at connect time.
    /// </summary>
    [TestFixture]
    public class MqttRelayTlsProtocolResolverTests
    {
        /// <summary>Pins the default: a fresh MqttRelayModuleConfiguration ships with Tls12 (and Tls13 on frameworks that expose it) and the resolver accepts that default without error.</summary>
        [Test]
        public void Default_configuration_resolves_to_a_nonempty_bitmask_that_includes_Tls12()
        {
            var configuration = new MqttRelayModuleConfiguration();

            var resolved = MqttRelayTlsProtocolResolver.Resolve(configuration.SslProtocols);

            Assert.That(resolved, Is.Not.EqualTo(SslProtocols.None));
            Assert.That((resolved & SslProtocols.Tls12), Is.EqualTo(SslProtocols.Tls12),
                "The default protocol set must include Tls12.");
        }

#if NET48 || NET5_0_OR_GREATER
        /// <summary>Pins the modern-runtime default: on target frameworks where SslProtocols.Tls13 is defined, the shipped default includes Tls13 too.</summary>
        [Test]
        public void Default_configuration_includes_Tls13_on_modern_runtimes()
        {
            var configuration = new MqttRelayModuleConfiguration();

            var resolved = MqttRelayTlsProtocolResolver.Resolve(configuration.SslProtocols);

            Assert.That((resolved & SslProtocols.Tls13), Is.EqualTo(SslProtocols.Tls13),
                "On this framework the default protocol set must include Tls13.");
        }
#endif

        /// <summary>Pins explicit Tls12-only opt-in: user narrows the default to 1.2, resolver returns Tls12 alone.</summary>
        [Test]
        public void Explicit_Tls12_only_resolves_to_Tls12_bitmask()
        {
            var resolved = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls12" });

            Assert.That(resolved, Is.EqualTo(SslProtocols.Tls12));
        }

#if NET48 || NET5_0_OR_GREATER
        /// <summary>Pins explicit Tls13-only opt-in: user opts out of 1.2, resolver returns Tls13 alone.</summary>
        [Test]
        public void Explicit_Tls13_only_resolves_to_Tls13_bitmask()
        {
            var resolved = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls13" });

            Assert.That(resolved, Is.EqualTo(SslProtocols.Tls13));
        }

        /// <summary>Pins multi-protocol OR: user lists both 1.2 and 1.3, resolver returns the bitwise OR.</summary>
        [Test]
        public void Explicit_Tls12_plus_Tls13_resolves_to_bitwise_OR()
        {
            var resolved = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls12", "Tls13" });

            Assert.That(resolved, Is.EqualTo(SslProtocols.Tls12 | SslProtocols.Tls13));
        }
#endif

        /// <summary>Pins case-insensitive parse: user writes lowercase / mixed-case, resolver still matches the enum member.</summary>
        [Test]
        public void Protocol_names_parse_case_insensitively()
        {
            var resolvedLower = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "tls12" });
            var resolvedMixed = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "TLS12" });

            Assert.That(resolvedLower, Is.EqualTo(SslProtocols.Tls12));
            Assert.That(resolvedMixed, Is.EqualTo(SslProtocols.Tls12));
        }

        /// <summary>Pins validation: null list is a configuration error, not a silent 'no TLS' or 'default TLS' fallback.</summary>
        [Test]
        public void Null_list_throws_configuration_error()
        {
            Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(null));
        }

        /// <summary>Pins validation: opt-out of all versions is a configuration error.</summary>
        [Test]
        public void Empty_list_throws_configuration_error()
        {
            Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string>()));
        }

        /// <summary>Pins validation: unknown protocol name surfaces a clear error with the name that failed to parse.</summary>
        [Test]
        public void Unknown_protocol_name_throws_configuration_error()
        {
            var ex = Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls99" }));

            Assert.That(ex.Message, Does.Contain("Tls99"));
        }

        /// <summary>Pins validation: a blank / whitespace entry is a configuration error.</summary>
        [Test]
        public void Blank_entry_throws_configuration_error()
        {
            Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "   " }));
        }

        /// <summary>Pins validation: 'None' is not a permitted opt-out; the surface accepts only concrete protocol names.</summary>
        [Test]
        public void None_entry_throws_configuration_error()
        {
            Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "None" }));
        }

        /// <summary>Pins validation: numeric input is rejected. Users must supply enum names so an accidental integer shift cannot silently rebind a version.</summary>
        [Test]
        public void Numeric_entry_throws_configuration_error()
        {
            Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "3072" }));
        }

        /// <summary>Pins validation: a null entry (distinct from a blank / whitespace entry) is a configuration error.</summary>
        [Test]
        public void Null_entry_inside_list_throws_configuration_error()
        {
            Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { null }));
        }

        /// <summary>Pins the trim contract: leading / trailing whitespace around a valid protocol name is trimmed and the name parses.</summary>
        [Test]
        public void Whitespace_around_valid_name_is_trimmed_before_parse()
        {
            var resolved = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "  Tls12  " });

            Assert.That(resolved, Is.EqualTo(SslProtocols.Tls12));
        }

        /// <summary>Pins bitwise-OR de-duplication: duplicate entries resolve to the same single bit; the user cannot accidentally shift bits by listing an entry twice.</summary>
        [Test]
        public void Duplicate_entries_bitwise_OR_to_the_single_bit()
        {
            var resolved = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls12", "Tls12" });

            Assert.That(resolved, Is.EqualTo(SslProtocols.Tls12));
        }

        /// <summary>Pins validation: a single entry containing comma-separated names is rejected. The surface is one-name-per-list-entry; a comma is a typing error.</summary>
        [Test]
        public void Comma_separated_single_entry_throws_configuration_error()
        {
            var ex = Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls12,Tls13" }));

            Assert.That(ex.Message, Does.Contain("Tls12,Tls13"));
        }

#if NET5_0_OR_GREATER
#pragma warning disable SYSLIB0039 // TLS 1.0 / 1.1 enum members are obsolete; the resolver validates strings, we merely pin the resulting bitmask.
        /// <summary>Pins the deprecated-protocol acceptance behavior. The resolver validates against defined enum members only; it does NOT refuse deprecated members (Tls, Tls11) if the user explicitly opts in. A follow-up may add a downgrade-warning log, but the current contract is 'user opts in, resolver honors' - this test pins that so a future change requires an explicit test edit.</summary>
        [Test]
        public void Deprecated_Tls10_accepted_when_defined_on_runtime()
        {
            // "Tls" is the SslProtocols member for TLS 1.0 on frameworks
            // that still define it. On a runtime where the member has
            // been removed the resolver throws; the assertion below
            // handles both shapes.
            var tls10Defined = Enum.GetNames(typeof(SslProtocols))
                .Any(n => string.Equals(n, "Tls", StringComparison.OrdinalIgnoreCase));

            if (!tls10Defined)
            {
                Assert.Throws<MqttRelayConfigurationException>(
                    () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls" }));
                return;
            }

            var resolved = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls" });

            Assert.That(resolved, Is.EqualTo(SslProtocols.Tls),
                "Pin: deprecated Tls1.0 is accepted by the resolver on runtimes that still define it. The resolver does NOT downgrade-guard; a follow-up may add a warning.");
        }

        /// <summary>Pins the deprecated-protocol acceptance behavior for Tls11 (see Tls10 test above for rationale).</summary>
        [Test]
        public void Deprecated_Tls11_accepted_when_defined_on_runtime()
        {
            var tls11Defined = Enum.GetNames(typeof(SslProtocols))
                .Any(n => string.Equals(n, "Tls11", StringComparison.OrdinalIgnoreCase));

            if (!tls11Defined)
            {
                Assert.Throws<MqttRelayConfigurationException>(
                    () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls11" }));
                return;
            }

            var resolved = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls11" });

            Assert.That(resolved, Is.EqualTo(SslProtocols.Tls11),
                "Pin: deprecated Tls1.1 is accepted by the resolver on runtimes that still define it.");
        }
#pragma warning restore SYSLIB0039
#endif

        /// <summary>Pins the enum-arm-exhaustiveness contract for Ssl3. The resolver refuses names that the running framework does not expose. On .NET 5+ SslProtocols.Ssl3 is [Obsolete] and may not be a defined member; either way the assertion holds because the failure mode surfaces via MqttRelayConfigurationException.</summary>
        [Test]
        public void Ssl3_behavior_pinned_against_runtime_definition()
        {
            var ssl3Defined = Enum.GetNames(typeof(SslProtocols))
                .Contains("Ssl3", StringComparer.OrdinalIgnoreCase);

            if (!ssl3Defined)
            {
                Assert.Throws<MqttRelayConfigurationException>(
                    () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Ssl3" }));
                return;
            }

            // If defined, the resolver accepts it (the current contract
            // is 'user opts in, resolver honors'). Pin that so a future
            // downgrade-guard requires an explicit test edit.
            var resolved = MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Ssl3" });
            Assert.That(resolved, Is.Not.EqualTo(SslProtocols.None),
                "Pin: Ssl3 opt-in resolves to a non-None bitmask on runtimes that expose it.");
        }

        /// <summary>Pins that the error message on an unknown protocol name enumerates the accepted names, letting the user self-correct without having to consult the source. Anchors on 'Tls12' because every target framework exposes it.</summary>
        [Test]
        public void Error_message_on_unknown_protocol_lists_valid_names_including_Tls12()
        {
            var ex = Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "Tls99" }));

            Assert.That(ex.Message, Does.Contain("Tls12"),
                "The unknown-protocol error must enumerate the accepted names so the user sees Tls12 in the diagnostic.");
        }

        /// <summary>Pins the two-arg MqttRelayConfigurationException ctor: message + inner exception both round-trip through the standard Exception surface.</summary>
        [Test]
        public void Configuration_exception_two_arg_ctor_preserves_message_and_inner_exception()
        {
            var inner = new InvalidOperationException("underlying");
            var ex = new MqttRelayConfigurationException("outer diagnostic", inner);

            Assert.That(ex.Message, Is.EqualTo("outer diagnostic"));
            Assert.That(ex.InnerException, Is.SameAs(inner));
        }

        /// <summary>Pins the single-arg MqttRelayConfigurationException ctor: message round-trips, inner exception stays null.</summary>
        [Test]
        public void Configuration_exception_single_arg_ctor_preserves_message_and_null_inner()
        {
            var ex = new MqttRelayConfigurationException("diagnostic only");

            Assert.That(ex.Message, Is.EqualTo("diagnostic only"));
            Assert.That(ex.InnerException, Is.Null);
        }

        /// <summary>Pins that an entry composed only of a tab character is treated as blank (not smuggled through the trim + parse as an empty name).</summary>
        [Test]
        public void Tab_only_entry_throws_configuration_error()
        {
            Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "\t" }));
        }

        /// <summary>Pins that the empty-string entry surfaces the same clear diagnostic as the blank-entry path, not a silent skip.</summary>
        [Test]
        public void Empty_string_entry_throws_configuration_error()
        {
            Assert.Throws<MqttRelayConfigurationException>(
                () => MqttRelayTlsProtocolResolver.Resolve(new List<string> { "" }));
        }
    }
}
