// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using MTConnect.Agents;
using MTConnect.Configurations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the contract that <see cref="DeterministicAgentUuid.TryValidate"/>
    /// gates every source of the Agent meta-device UUID (operator-supplied
    /// override AND persisted <c>agent.information.json</c> state) so that
    /// malformed values — anything that does not parse as an RFC 4122 UUID —
    /// are rejected on the way in and the resolution falls through to the
    /// next path (persisted → derived).
    ///
    /// <para>
    /// Silently forwarding a non-UUID string violates MTConnect Part 1, which
    /// types the <c>uuid</c> attribute as the <c>UUID</c> DataType (RFC 4122)
    /// and mandates that value remain stable and unique for the agent's entire
    /// lifetime. The wire XSD currently types <c>uuid</c> only as
    /// <c>xs:string</c>, so schema validation would silently accept a
    /// malformed value; the cppagent reference implementation and every
    /// downstream consumer that trusts the DataType annotation for aggregation
    /// or historical keying reject the resulting wire content.
    /// </para>
    ///
    /// <para>
    /// Fixture drives <see cref="AgentUuidResolver.Resolve"/> directly — the
    /// same method <c>MTConnectAgentApplication.StartAgent</c> calls — so
    /// production and tests cannot silently diverge on branch order, guard
    /// semantics, or normalization output.
    /// </para>
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class AgentUuidValidationTests
    {
        private string _stateFilePath = null!;
        private string? _backupStateFile;

        /// <summary>Sets up the fixture before each test.</summary>
        [SetUp]
        public void SetUp()
        {
            _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MTConnectAgentInformation.Filename);
            _backupStateFile = null;

            // Sweep orphan .valbak.* files from a prior crashed test run so
            // successive TearDowns cannot restore stale state.
            var directory = Path.GetDirectoryName(_stateFilePath);
            if (directory != null && Directory.Exists(directory))
            {
                foreach (var stale in Directory.EnumerateFiles(directory, MTConnectAgentInformation.Filename + ".valbak.*"))
                {
                    try { File.Delete(stale); } catch { /* best-effort — do not fail the test */ }
                }
            }

            // Back up any pre-existing state file so we do not perturb other
            // tests or the developer environment.
            if (File.Exists(_stateFilePath))
            {
                _backupStateFile = _stateFilePath + ".valbak." + Guid.NewGuid().ToString("N");
                File.Move(_stateFilePath, _backupStateFile);
            }
        }

        /// <summary>Tears down the fixture after each test.</summary>
        [TearDown]
        public void TearDown()
        {
            // Remove any state file we left behind, then restore the backup.
            if (File.Exists(_stateFilePath))
            {
                File.Delete(_stateFilePath);
            }
            if (_backupStateFile != null && File.Exists(_backupStateFile))
            {
                File.Move(_backupStateFile, _stateFilePath);
                _backupStateFile = null;
            }
        }

        // ------------------------------------------------------------------
        // Unit tests — DeterministicAgentUuid.TryValidate low-level contract
        // ------------------------------------------------------------------

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> rejects
        /// <see langword="null"/>, empty, and whitespace-only inputs.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        [TestCase("\n")]
        public void TryValidate_null_or_whitespace_returns_false(string input)
        {
            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.False);
            Assert.That(normalized, Is.Null);
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> rejects strings
        /// that do not parse as RFC 4122 UUIDs.
        /// </summary>
        [TestCase("not-a-uuid")]
        [TestCase("fixture-stable-uuid-001")]
        [TestCase("agent_1234567890abcdef")]
        [TestCase("123")]
        [TestCase("6ba7b810-9dad-11d1-80b4-00c04fd430c8-extra")]
        public void TryValidate_unparseable_returns_false(string input)
        {
            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.False);
            Assert.That(normalized, Is.Null);
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> rejects the
        /// all-zero <see cref="Guid.Empty"/> value across every accepted
        /// input format — <see cref="Guid.TryParse(string, out Guid)"/> is
        /// happy to parse "00000000-…", but adopting it as an agent's meta
        /// UUID would collide every agent in a fleet on the same identifier,
        /// which the RFC 4122 "unique for the resource's entire lifetime"
        /// contract disallows.
        /// </summary>
        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("{00000000-0000-0000-0000-000000000000}")]
        [TestCase("(00000000-0000-0000-0000-000000000000)")]
        [TestCase("00000000000000000000000000000000")]
        public void TryValidate_all_zero_guid_returns_false(string input)
        {
            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.False,
                "Guid.Empty is a fleet-collision hazard — TryValidate must reject it on every accepted format.");
            Assert.That(normalized, Is.Null);
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> accepts the
        /// canonical hyphenated "D" form and returns it unchanged.
        /// </summary>
        [Test]
        public void TryValidate_canonical_hyphenated_form_returns_true_unchanged()
        {
            const string Canonical = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

            var ok = DeterministicAgentUuid.TryValidate(Canonical, out var normalized);

            Assert.That(ok, Is.True);
            Assert.That(normalized, Is.EqualTo(Canonical));
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> normalizes the
        /// braced "B", parenthesized "P", and bare-hex "N" forms to the
        /// canonical hyphenated "D" form so the wire representation stays
        /// stable regardless of the input format.
        /// </summary>
        [TestCase("{6ba7b810-9dad-11d1-80b4-00c04fd430c8}", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [TestCase("(6ba7b810-9dad-11d1-80b4-00c04fd430c8)", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [TestCase("6ba7b8109dad11d180b400c04fd430c8", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        public void TryValidate_non_canonical_format_normalizes_to_hyphenated(string input, string expected)
        {
            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.True);
            Assert.That(normalized, Is.EqualTo(expected));
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> also accepts the
        /// hex-braced "X" format (<c>{0xhh,0xhh,0xhh,{...}}</c>) that
        /// <see cref="Guid.TryParse(string, out Guid)"/> recognises, and
        /// normalises it to the canonical hyphenated "D" form. Closes the
        /// last Guid-format enum arm not covered by the sibling test.
        /// </summary>
        [Test]
        public void TryValidate_hex_braced_X_format_normalizes_to_hyphenated()
        {
            const string XForm = "{0x6ba7b810,0x9dad,0x11d1,{0x80,0xb4,0x00,0xc0,0x4f,0xd4,0x30,0xc8}}";
            const string Expected = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

            var ok = DeterministicAgentUuid.TryValidate(XForm, out var normalized);

            Assert.That(ok, Is.True);
            Assert.That(normalized, Is.EqualTo(Expected));
        }

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> accepts uppercase
        /// hex characters (case-insensitive per RFC 4122) and normalizes the
        /// output to lowercase so the wire representation is stable regardless
        /// of the operator's typing.
        /// </summary>
        [Test]
        public void TryValidate_uppercase_hex_is_normalized_to_lowercase()
        {
            const string Uppercased = "6BA7B810-9DAD-11D1-80B4-00C04FD430C8";
            const string Expected = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

            var ok = DeterministicAgentUuid.TryValidate(Uppercased, out var normalized);

            Assert.That(ok, Is.True);
            Assert.That(normalized, Is.EqualTo(Expected));
        }

        // ------------------------------------------------------------------
        // Integration tests — AgentUuidResolver.Resolve three-path algorithm
        // ------------------------------------------------------------------

        /// <summary>
        /// Boot-simulation regression — malformed
        /// <c>AgentApplicationConfiguration.AgentUuid</c> on a fresh boot
        /// (no <c>agent.information.json</c>) must fall through to the
        /// derived UUID rather than being silently stored verbatim.
        /// </summary>
        [Test]
        public void Malformed_AgentUuid_on_fresh_boot_falls_through_to_derived()
        {
            const string MalformedInput = "not-a-uuid";
            const string ServiceName = "test-agent-malformed-fresh";
            var hostname = Environment.MachineName;

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = MalformedInput,
                ServiceName = ServiceName,
            };

            var resolved = SimulateFreshBoot(configuration, hostname);

            var expectedDerived = DeterministicAgentUuid.Derive(ServiceName, hostname, port: 0);
            Assert.That(resolved, Is.EqualTo(expectedDerived),
                "Malformed operator override on a fresh boot must fall through to the derived UUID.");
            Assert.That(resolved, Is.Not.EqualTo(MalformedInput),
                "The malformed input must NOT be stored verbatim (Part-1 UuidType violation).");
        }

        /// <summary>
        /// Boot-simulation regression — malformed
        /// <c>AgentApplicationConfiguration.AgentUuid</c> when a valid
        /// <c>agent.information.json</c> exists must preserve the persisted
        /// UUID rather than being silently stored verbatim.
        /// </summary>
        [Test]
        public void Malformed_AgentUuid_with_valid_persisted_state_preserves_persisted()
        {
            const string MalformedInput = "not-a-uuid";
            const string PersistedUuid = "cfbff0d1-9375-5685-968a-48ce8b50a653";

            // Pre-write the state file with a valid UUID.
            var preexisting = new MTConnectAgentInformation(PersistedUuid);
            preexisting.Save();

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = MalformedInput,
                ServiceName = "test-agent-malformed-persisted",
            };

            var resolved = SimulateWarmBoot(configuration, Environment.MachineName);

            Assert.That(resolved, Is.EqualTo(PersistedUuid),
                "Malformed operator override with valid persisted state must keep the persisted UUID.");
            Assert.That(resolved, Is.Not.EqualTo(MalformedInput),
                "The malformed input must NOT overwrite the persisted UUID.");
        }

        /// <summary>
        /// Path-2 hardening — malformed persisted state (e.g. a pre-hardening
        /// agent version wrote a non-UUID string, or the file was hand-edited)
        /// must ALSO fall through to the derived UUID rather than flowing on
        /// to the wire. Prevents the exact XSD-validation failure the PR was
        /// written to prevent, closed on the persisted-state axis too.
        /// </summary>
        [Test]
        public void Malformed_persisted_state_with_no_override_falls_through_to_derived()
        {
            const string MalformedPersisted = "agent_1234567890abcdef";
            const string ServiceName = "test-agent-malformed-persisted-path2";
            var hostname = Environment.MachineName;

            var preexisting = new MTConnectAgentInformation(MalformedPersisted);
            preexisting.Save();

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = null,
                ServiceName = ServiceName,
            };

            var resolved = SimulateWarmBoot(configuration, hostname);

            var expectedDerived = DeterministicAgentUuid.Derive(ServiceName, hostname, port: 0);
            Assert.That(resolved, Is.EqualTo(expectedDerived),
                "Malformed persisted UUID with no override must fall through to derived — Path 2 must validate.");
            Assert.That(resolved, Is.Not.EqualTo(MalformedPersisted),
                "The malformed persisted value must NOT reach the wire.");
        }

        /// <summary>
        /// Boot-simulation regression — valid non-canonical
        /// <c>AgentApplicationConfiguration.AgentUuid</c> (e.g. braced form)
        /// is accepted and normalized to the canonical hyphenated form so the
        /// wire representation stays stable across boots.
        /// </summary>
        [Test]
        public void Valid_non_canonical_AgentUuid_is_normalized_to_hyphenated()
        {
            const string BracedInput = "{6ba7b810-9dad-11d1-80b4-00c04fd430c8}";
            const string ExpectedCanonical = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = BracedInput,
                ServiceName = "test-agent-canonicalization",
            };

            var resolved = SimulateFreshBoot(configuration, Environment.MachineName);

            Assert.That(resolved, Is.EqualTo(ExpectedCanonical),
                "Non-canonical valid UUID inputs must be normalized to the hyphenated D-form.");
        }

        /// <summary>
        /// Two-boot regression — first boot with no override and no state file
        /// derives + persists a UUID; second boot with no override reads the
        /// persisted UUID from <c>agent.information.json</c> and adopts it
        /// verbatim (Path 2). Bit-identical across the two boots.
        /// </summary>
        [Test]
        public void Persisted_UUID_is_adopted_on_second_boot_when_no_override()
        {
            const string ServiceName = "test-agent-persisted-second-boot";
            var hostname = Environment.MachineName;

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = null,
                ServiceName = ServiceName,
            };

            // Boot 1 — no override, no state file → derive + persist.
            var boot1Uuid = SimulateFreshBoot(configuration, hostname);
            Assert.That(File.Exists(_stateFilePath), Is.True,
                "Boot 1 must persist agent.information.json with the resolved UUID.");
            var persistedAfterBoot1 = MTConnectAgentInformation.Read();
            Assert.That(persistedAfterBoot1, Is.Not.Null);
            Assert.That(persistedAfterBoot1!.Uuid, Is.EqualTo(boot1Uuid),
                "Boot 1's on-disk UUID must equal the resolved value.");

            // Boot 2 — no override, but persisted state exists → adopt persisted.
            var boot2Uuid = SimulateWarmBoot(configuration, hostname);
            Assert.That(boot2Uuid, Is.EqualTo(boot1Uuid),
                "Boot 2 must adopt the persisted UUID bit-identically (Path 2).");
        }

        /// <summary>
        /// First-boot persistence regression — no override, no state file →
        /// Path 3 derives the UUID AND <see cref="MTConnectAgentInformation.Save"/>
        /// writes it to <c>agent.information.json</c> so subsequent boots hit
        /// Path 2. Guards against a regression where the resolve step returns
        /// the derived value but the persist step is dropped.
        /// </summary>
        [Test]
        public void First_boot_persists_derived_UUID_to_agent_information_json()
        {
            const string ServiceName = "test-agent-first-boot-persist";
            var hostname = Environment.MachineName;

            var configuration = new AgentApplicationConfiguration
            {
                AgentUuid = null,
                ServiceName = ServiceName,
            };

            var resolved = SimulateFreshBoot(configuration, hostname);

            Assert.That(File.Exists(_stateFilePath), Is.True,
                "agent.information.json must exist after the first boot.");
            var persisted = MTConnectAgentInformation.Read();
            Assert.That(persisted, Is.Not.Null);
            Assert.That(persisted!.Uuid, Is.EqualTo(resolved),
                "Persisted UUID on disk must equal the resolved value.");
            var expectedDerived = DeterministicAgentUuid.Derive(ServiceName, hostname, port: 0);
            Assert.That(persisted.Uuid, Is.EqualTo(expectedDerived),
                "Persisted UUID must equal the deterministic derivation for the given ServiceName.");
        }

        // ------------------------------------------------------------------
        // Warn-delegate contract — invocation count, message shape, both
        // arms of the "persisted" vs "derived" fallback-kind switch, and
        // the null-delegate no-op guard.
        // ------------------------------------------------------------------

        /// <summary>
        /// When Path 1 (operator override) wins, the warn delegate must NOT
        /// be invoked — the happy path is silent by design so a valid
        /// operator configuration does not pollute the log with warnings.
        /// </summary>
        [Test]
        public void Warn_delegate_not_invoked_when_operator_override_is_valid()
        {
            var messages = new List<string>();

            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
                persistedUuid: null,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(resolved, Is.EqualTo("6ba7b810-9dad-11d1-80b4-00c04fd430c8"));
            Assert.That(messages, Is.Empty,
                "Path 1 (valid override) must be silent — no warn.");
        }

        /// <summary>
        /// When Path 1 is null and Path 2 (persisted) wins, the warn delegate
        /// must NOT be invoked — an empty override + valid persisted is the
        /// normal warm-boot happy path.
        /// </summary>
        [Test]
        public void Warn_delegate_not_invoked_when_override_null_and_persisted_valid()
        {
            var messages = new List<string>();

            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: null,
                persistedUuid: "cfbff0d1-9375-5685-968a-48ce8b50a653",
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(resolved, Is.EqualTo("cfbff0d1-9375-5685-968a-48ce8b50a653"));
            Assert.That(messages, Is.Empty,
                "Path 2 (valid persisted, null override) must be silent — no warn.");
        }

        /// <summary>
        /// When Path 1 is null AND Path 2 is null (both truly empty — the
        /// fresh-boot case), the warn delegate must NOT be invoked — both
        /// early-return guards are gated on <c>!IsNullOrEmpty(...)</c>.
        /// </summary>
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(null, "")]
        [TestCase("", "")]
        public void Warn_delegate_not_invoked_when_both_override_and_persisted_are_empty(
            string? overrideValue, string? persistedValue)
        {
            var messages = new List<string>();

            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: overrideValue,
                persistedUuid: persistedValue,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(resolved, Is.EqualTo(DeterministicAgentUuid.Derive("test-agent", "test-host", 0)));
            Assert.That(messages, Is.Empty,
                "Both empty ≠ malformed; fresh boot must be silent.");
        }

        /// <summary>
        /// When Path 1 is rejected and Path 2 (persisted) is a valid UUID,
        /// the warn message must name "persisted" as the fallback kind — the
        /// operator learns which alternative source overrode their supplied
        /// value. Pins the "persisted" arm of the two-arm fallback-kind
        /// ternary in <see cref="AgentUuidResolver.Resolve"/>.
        /// </summary>
        [Test]
        public void Warn_message_names_persisted_when_override_bad_and_persisted_valid()
        {
            var messages = new List<string>();
            const string Malformed = "not-a-uuid";
            const string ValidPersisted = "cfbff0d1-9375-5685-968a-48ce8b50a653";

            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: Malformed,
                persistedUuid: ValidPersisted,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(resolved, Is.EqualTo(ValidPersisted));
            Assert.That(messages, Has.Count.EqualTo(1),
                "One warn — for the rejected operator override; the valid persisted UUID needs no warn.");
            Assert.That(messages[0], Does.Contain("AgentUuid override"));
            Assert.That(messages[0], Does.Contain($"length={Malformed.Length}"),
                "Warn reports length only — the raw value must never appear (secret-leakage guard).");
            Assert.That(messages[0], Does.Not.Contain(Malformed),
                "Warn must not echo the raw operator-supplied value.");
            Assert.That(messages[0], Does.Contain("falling back to persisted UUID"));
        }

        /// <summary>
        /// When Path 1 is rejected and Path 2 (persisted) is also rejected /
        /// null, the warn message for the operator override must name
        /// "derived" as the fallback kind. Pins the "derived" arm of the
        /// two-arm fallback-kind ternary in
        /// <see cref="AgentUuidResolver.Resolve"/>.
        /// </summary>
        [Test]
        public void Warn_message_names_derived_when_override_bad_and_persisted_absent()
        {
            var messages = new List<string>();
            const string Malformed = "not-a-uuid";

            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: Malformed,
                persistedUuid: null,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(resolved, Is.EqualTo(DeterministicAgentUuid.Derive("test-agent", "test-host", 0)));
            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0], Does.Contain("AgentUuid override"));
            Assert.That(messages[0], Does.Contain("falling back to derived UUID"));
        }

        /// <summary>
        /// When Path 1 (operator override) is rejected AND Path 2 (persisted
        /// state) is rejected — the failing-both case a mid-life container
        /// upgrade produces — the warn delegate must be invoked TWICE, once
        /// per rejected source. Guards against a regression that swallows
        /// the second warn under the first.
        /// </summary>
        [Test]
        public void Warn_delegate_invoked_twice_when_both_override_and_persisted_are_malformed()
        {
            var messages = new List<string>();
            const string BadOverride = "not-a-uuid";
            const string BadPersisted = "also-not-a-uuid";

            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: BadOverride,
                persistedUuid: BadPersisted,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(resolved, Is.EqualTo(DeterministicAgentUuid.Derive("test-agent", "test-host", 0)));
            Assert.That(messages, Has.Count.EqualTo(2),
                "Two rejected sources → two warns; the second must not be swallowed.");
            Assert.That(messages[0], Does.Contain("AgentUuid override"));
            Assert.That(messages[0], Does.Contain($"length={BadOverride.Length}"));
            Assert.That(messages[0], Does.Not.Contain(BadOverride),
                "Raw operator value must never appear in the warn message.");
            Assert.That(messages[0], Does.Contain("falling back to derived UUID"));
            Assert.That(messages[1], Does.Contain("Persisted AgentUuid"));
            Assert.That(messages[1], Does.Contain($"length={BadPersisted.Length}"));
            Assert.That(messages[1], Does.Not.Contain(BadPersisted),
                "Raw persisted-state value must never appear in the warn message.");
            Assert.That(messages[1], Does.Contain("falling back to derived UUID"));
        }

        /// <summary>
        /// When Path 1 (operator override) is absent and Path 2 (persisted
        /// state) is rejected, only the persisted-state warn is emitted —
        /// the operator did not supply anything to warn about.
        /// </summary>
        [Test]
        public void Warn_delegate_emits_persisted_warn_only_when_override_null_and_persisted_bad()
        {
            var messages = new List<string>();
            const string BadPersisted = "also-not-a-uuid";

            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: null,
                persistedUuid: BadPersisted,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(resolved, Is.EqualTo(DeterministicAgentUuid.Derive("test-agent", "test-host", 0)));
            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0], Does.Contain("Persisted AgentUuid"));
            Assert.That(messages[0], Does.Not.Contain("override"),
                "No override supplied → the override-warn must NOT be emitted.");
        }

        /// <summary>
        /// A null <c>warn</c> delegate is valid — the resolver documents it
        /// as "optional" and callers that do not want warnings must not
        /// crash. Guards against a null-conditional invocation regression.
        /// </summary>
        [Test]
        public void Null_warn_delegate_does_not_throw_on_either_rejection_path()
        {
            Assert.DoesNotThrow((Action)(() =>
            {
                _ = AgentUuidResolver.Resolve(
                    operatorSuppliedUuid: "not-a-uuid",
                    persistedUuid: "also-not-a-uuid",
                    agentName: "test-agent",
                    hostname: "test-host",
                    warn: null);
            }));
        }

        /// <summary>
        /// The <c>warn</c> parameter defaults to <see langword="null"/> —
        /// callers that omit it entirely must get the same no-throw
        /// behavior as an explicitly-null delegate.
        /// </summary>
        [Test]
        public void Default_warn_argument_omitted_does_not_throw()
        {
            Assert.DoesNotThrow((Action)(() =>
            {
                _ = AgentUuidResolver.Resolve(
                    operatorSuppliedUuid: "not-a-uuid",
                    persistedUuid: "also-not-a-uuid",
                    agentName: "test-agent",
                    hostname: "test-host");
            }));
        }

        // ------------------------------------------------------------------
        // Redaction — the warn message reports the LENGTH of the rejected
        // value and never echoes its raw content. This is the single guard
        // that closes both log-injection (embedded CR/LF/U+2028/ANSI escapes
        // cannot forge a new log line if the value never appears) and
        // secret-leakage (a mis-pasted API key, bearer token, or password
        // in the AgentUuid config slot cannot land in the log archive).
        // ------------------------------------------------------------------

        /// <summary>
        /// The Path 1 warn message must report the length of the rejected
        /// operator override, must not echo any character of the raw value,
        /// and must not contain any control character regardless of what the
        /// operator supplied — a single redaction guard that closes both
        /// log-injection (no CR/LF/U+2028/ANSI/control byte can appear if the
        /// value itself is never echoed) and secret leakage (a mis-pasted
        /// API key or bearer token in the AgentUuid slot cannot land in
        /// archives).
        /// </summary>
        [TestCase("bad-uuid\r\nFAKE-LOG-LINE-INJECTED")]
        [TestCase("bad\rvalue")]
        [TestCase("bad\nvalue")]
        [TestCase("bad\u2028value")] // Unicode LINE SEPARATOR
        [TestCase("bad\u2029value")] // Unicode PARAGRAPH SEPARATOR
        [TestCase("bad\u0085value")] // NEL
        [TestCase("bad\x1b[2Jvalue")] // ANSI CSI clear-screen
        [TestCase("bad\x00value")]    // NUL
        [TestCase("bad\tvalue")]      // TAB
        [TestCase("ghp_abcdef0123456789abcdef0123456789abcd")] // paste-in-wrong-field secret shape
        [TestCase("Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.payload.sig")]
        public void Warn_message_never_echoes_raw_operator_value_and_reports_length(string input)
        {
            var messages = new List<string>();

            _ = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: input,
                persistedUuid: null,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0], Does.Contain("AgentUuid override"));
            Assert.That(messages[0], Does.Contain($"length={input.Length}"),
                "Warn must report the input length so operators can spot a length-mismatch mispaste.");
            Assert.That(messages[0], Does.Not.Contain(input),
                "Warn must never echo the raw operator value — closes secret-leakage.");
            AssertLogSafe(messages[0]);
        }

        /// <summary>
        /// The Path 2 (persisted state) warn message mirrors the Path 1
        /// guarantees — reports length, never echoes the raw persisted
        /// value, and contains no control characters. The persisted-state
        /// file may carry any garbage a prior agent version wrote, so the
        /// same redaction discipline applies.
        /// </summary>
        [TestCase("garbage-persisted-value")]
        [TestCase("truncated-uuid-write\rfromacrashedboot")]
        [TestCase("ghk_secret_that_leaked_into_state_file_somehow")]
        public void Warn_message_for_persisted_state_never_echoes_raw_value_and_reports_length(string input)
        {
            var messages = new List<string>();

            _ = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: null,
                persistedUuid: input,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0], Does.Contain("Persisted AgentUuid"));
            Assert.That(messages[0], Does.Contain($"length={input.Length}"));
            Assert.That(messages[0], Does.Not.Contain(input),
                "Persisted-state warn must not echo the raw file content.");
            AssertLogSafe(messages[0]);
        }

        /// <summary>
        /// Log-injection guard shared between the operator-override and
        /// persisted-state warn assertions — every character in the message
        /// must be either printable (>= U+0020) or a plain TAB (U+0009).
        /// Rejects CR, LF, NEL, LINE / PARAGRAPH SEPARATOR, ANSI CSI, NUL
        /// and every other C0 control byte, guarding against a regression
        /// that funnels raw input back into the message.
        /// </summary>
        private static void AssertLogSafe(string message)
        {
            foreach (var c in message)
            {
                Assert.That(
                    c == '\t' || c >= ' ',
                    Is.True,
                    $"Warn message contains disallowed control character U+{((int)c):X4}.");
                Assert.That(c, Is.Not.EqualTo('\u0085'), "NEL leaked into warn.");
                Assert.That(c, Is.Not.EqualTo('\u2028'), "LINE SEPARATOR leaked into warn.");
                Assert.That(c, Is.Not.EqualTo('\u2029'), "PARAGRAPH SEPARATOR leaked into warn.");
            }
        }

        // ------------------------------------------------------------------
        // Path-3 hostname fallback — the agentName-is-null / empty case
        // routes through DeterministicAgentUuid.Derive's own fallback.
        // ------------------------------------------------------------------

        /// <summary>
        /// When Path 3 is reached and <c>agentName</c> is <see langword="null"/>,
        /// the hostname stands in as the seed component per
        /// <see cref="DeterministicAgentUuid.Derive"/>'s documented fallback.
        /// The output is stable, deterministic, and identical to
        /// <c>Derive(hostname, hostname, 0)</c>.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        public void Resolve_falls_back_to_hostname_when_agentName_is_null_or_empty(string? agentName)
        {
            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: null,
                persistedUuid: null,
                agentName: agentName,
                hostname: "canonical-host",
                warn: null);

            var expected = DeterministicAgentUuid.Derive(agentName, "canonical-host", 0);
            Assert.That(resolved, Is.EqualTo(expected));
            // Cross-check: Derive(null, host, 0) == Derive(host, host, 0).
            var expectedViaHost = DeterministicAgentUuid.Derive("canonical-host", "canonical-host", 0);
            Assert.That(resolved, Is.EqualTo(expectedViaHost),
                "agentName null/empty ⇒ Derive seeds with hostname; outputs must match.");
        }

        // ------------------------------------------------------------------
        // Persisted-path failure integration — the malformed persisted UUID
        // is not just a synthetic string parameter, it MUST round-trip
        // through the real MTConnectAgentInformation.Save/Read (JSON file
        // on disk) and still be rejected by the resolver. Guards against a
        // regression that only tests the in-memory path.
        // ------------------------------------------------------------------

        /// <summary>
        /// End-to-end integration: a malformed UUID string is written to
        /// <c>agent.information.json</c> via
        /// <see cref="MTConnectAgentInformation.Save"/>, read back via the
        /// real <see cref="MTConnectAgentInformation.Read"/>, and rejected
        /// by <see cref="AgentUuidResolver.Resolve"/> which falls through
        /// to the derived UUID. Confirms the failure path is exercised
        /// through the real serialiser, not just an in-memory string.
        /// </summary>
        [Test]
        public void Malformed_persisted_state_survives_JSON_round_trip_and_is_rejected()
        {
            const string MalformedPersisted = "definitely-not-a-uuid-42";
            const string ServiceName = "test-agent-json-round-trip";
            var hostname = Environment.MachineName;

            // Write malformed value to disk via the production serialiser.
            var toPersist = new MTConnectAgentInformation(MalformedPersisted);
            toPersist.Save();

            // Verify the JSON on disk actually contains the malformed value —
            // guards against a silent Save-side validator that never existed
            // but might be added later without failing the harness.
            var raw = File.ReadAllText(_stateFilePath);
            Assert.That(raw, Does.Contain(MalformedPersisted),
                "Precondition: the malformed value must actually be on disk.");

            // Read back via the production reader and hand its Uuid to Resolve.
            var reread = MTConnectAgentInformation.Read();
            Assert.That(reread, Is.Not.Null);
            Assert.That(reread!.Uuid, Is.EqualTo(MalformedPersisted),
                "Precondition: the reader must surface the malformed value verbatim.");

            var messages = new List<string>();
            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: null,
                persistedUuid: reread.Uuid,
                agentName: ServiceName,
                hostname: hostname,
                warn: messages.Add);

            var expectedDerived = DeterministicAgentUuid.Derive(ServiceName, hostname, port: 0);
            Assert.That(resolved, Is.EqualTo(expectedDerived));
            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0], Does.Contain("Persisted AgentUuid"));
        }

        // ------------------------------------------------------------------
        // TryValidate boundary characterisation — pins the delegated
        // Guid.TryParse contract on the input classes the resolver may
        // receive from operators or a hand-edited state file. Closes the
        // §1.0d-trigies-novodecies coverage FLOOR on boundary classes.
        // ------------------------------------------------------------------

        /// <summary>
        /// <see cref="DeterministicAgentUuid.TryValidate"/> trims leading and
        /// trailing whitespace before delegating to
        /// <see cref="Guid.TryParse(string, out Guid)"/>, so a copy-pasted
        /// value with stray whitespace is accepted and normalized to the
        /// canonical hyphenated "D" form. Pinning the trim locally (rather
        /// than relying on <c>Guid.TryParse</c>'s implicit trim behaviour)
        /// keeps the length-cap defense meaningful against padded input and
        /// isolates the resolver from any future runtime tightening.
        /// </summary>
        [TestCase(" 6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [TestCase("6ba7b810-9dad-11d1-80b4-00c04fd430c8 ")]
        [TestCase(" 6ba7b810-9dad-11d1-80b4-00c04fd430c8 ")]
        [TestCase("\t6ba7b810-9dad-11d1-80b4-00c04fd430c8\r\n")]
        public void TryValidate_leading_and_trailing_whitespace_is_trimmed_and_normalized(string input)
        {
            const string Expected = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.True,
                "Guid.TryParse trims leading/trailing whitespace — TryValidate must forward that acceptance.");
            Assert.That(normalized, Is.EqualTo(Expected),
                "The trimmed value must round-trip to the canonical D form.");
        }

        /// <summary>
        /// Mixed-case hex is accepted (RFC 4122 case-insensitive) and the
        /// output is normalized to lowercase so the wire representation is
        /// stable regardless of how the operator typed the value. Companion
        /// to <see cref="TryValidate_uppercase_hex_is_normalized_to_lowercase"/>.
        /// </summary>
        [Test]
        public void TryValidate_mixed_case_hex_is_accepted_and_normalized_to_lowercase()
        {
            const string MixedCase = "6Ba7B810-9dAd-11D1-80B4-00c04Fd430c8";
            const string Expected = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";

            var ok = DeterministicAgentUuid.TryValidate(MixedCase, out var normalized);

            Assert.That(ok, Is.True);
            Assert.That(normalized, Is.EqualTo(Expected));
        }

        /// <summary>
        /// Inputs with an embedded control character in the middle of the
        /// UUID text (NUL, CRLF, space) must be rejected — trimming only
        /// removes outer whitespace, so any interior control byte breaks
        /// the parse. Closes the "malformed-in-the-middle" boundary that
        /// the redaction guard already handles at the warn layer.
        /// </summary>
        [TestCase("6ba7b810-9dad-11d1-80b4-00c04fd430c8\0")]      // trailing NUL — Guid.TryParse rejects
        [TestCase("6ba7b810\09dad-11d1-80b4-00c04fd430c8")]       // embedded NUL
        [TestCase("6ba7b810\r\n9dad-11d1-80b4-00c04fd430c8")]     // embedded CRLF
        [TestCase("6ba7b810 9dad-11d1-80b4-00c04fd430c8")]        // embedded space
        [TestCase("6ba7b810\t9dad-11d1-80b4-00c04fd430c8")]       // embedded tab
        public void TryValidate_interior_control_or_whitespace_is_rejected(string input)
        {
            var ok = DeterministicAgentUuid.TryValidate(input, out var normalized);

            Assert.That(ok, Is.False,
                "Guid.TryParse only trims outer whitespace; interior control bytes must be rejected.");
            Assert.That(normalized, Is.Null);
        }

        /// <summary>
        /// Overlong inputs — a valid UUID prefix followed by trailing
        /// garbage — must be rejected. Closes the boundary class the
        /// existing <c>"…-extra"</c> case sketches but with a much longer
        /// tail more typical of a mis-pasted secret / bearer token.
        /// </summary>
        [Test]
        public void TryValidate_overlong_input_with_valid_prefix_is_rejected()
        {
            var overlong = "6ba7b810-9dad-11d1-80b4-00c04fd430c8" + new string('x', 200);

            var ok = DeterministicAgentUuid.TryValidate(overlong, out var normalized);

            Assert.That(ok, Is.False,
                "A valid UUID prefix + trailing garbage must not parse — full-string match required.");
            Assert.That(normalized, Is.Null);
        }

        /// <summary>
        /// Trailing CR/LF followed by extra text (the classic log-injection
        /// payload shape) must be rejected at the parse layer — the redaction
        /// guard closes the log-line-forgery risk downstream, but the parse
        /// layer must also refuse to canonicalise the value so it never
        /// reaches the wire.
        /// </summary>
        [Test]
        public void TryValidate_trailing_crlf_with_injected_text_is_rejected()
        {
            const string Injected = "6ba7b810-9dad-11d1-80b4-00c04fd430c8\r\nFAKE-LOG-INJECTED";

            var ok = DeterministicAgentUuid.TryValidate(Injected, out var normalized);

            Assert.That(ok, Is.False);
            Assert.That(normalized, Is.Null);
        }

        // ------------------------------------------------------------------
        // Resolve Path-1-wins-with-malformed-Path-2 — covers the branch
        // where a valid operator override short-circuits BEFORE the
        // persisted-state validation runs, so no warn is emitted for the
        // malformed persisted value. Closes the 3x3 override/persisted
        // matrix cell not covered by the existing warn-count tests.
        // ------------------------------------------------------------------

        /// <summary>
        /// When Path 1 (operator override) is valid AND Path 2 (persisted
        /// state) is malformed, the resolver returns the override
        /// immediately without touching the persisted-state validation —
        /// so the warn delegate MUST NOT fire for the malformed persisted
        /// value. Pins the early-return short-circuit at
        /// <see cref="AgentUuidResolver.Resolve"/> line 114.
        /// </summary>
        [Test]
        public void Resolve_valid_override_short_circuits_and_ignores_malformed_persisted_without_warn()
        {
            var messages = new List<string>();
            const string ValidOverride = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";
            const string MalformedPersist = "definitely-not-a-uuid";

            var resolved = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: ValidOverride,
                persistedUuid: MalformedPersist,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(resolved, Is.EqualTo(ValidOverride),
                "Valid override wins Path 1; persisted state is never consulted.");
            Assert.That(messages, Is.Empty,
                "Path 1 short-circuits before the persisted-state warn arm — malformed persisted must be silent.");
        }

        // ------------------------------------------------------------------
        // Length-cap defense — TryValidate rejects mega-payloads before
        // invoking Guid.TryParse so a pasted-in blob can never allocate
        // megabytes of parse state. 72 chars is the longest legal RFC 4122
        // textual form ("X" with braces around 11 hex segments) plus slack.
        // ------------------------------------------------------------------

        /// <summary>
        /// A 73-character input (one past the length cap) must be rejected
        /// even when it contains a syntactically valid UUID prefix — the
        /// length gate short-circuits before <c>Guid.TryParse</c> runs.
        /// </summary>
        [Test]
        public void TryValidate_input_one_past_length_cap_is_rejected()
        {
            var overCap = new string('a', 73);

            var ok = DeterministicAgentUuid.TryValidate(overCap, out var normalized);

            Assert.That(ok, Is.False,
                "73 characters exceeds the 72-char length cap — must be rejected.");
            Assert.That(normalized, Is.Null);
        }

        /// <summary>
        /// A mega-payload (10 KB) must be rejected instantly without
        /// invoking <c>Guid.TryParse</c> on the whole string; the length
        /// cap is the DoS defense that keeps the resolver O(1) on hostile
        /// input.
        /// </summary>
        [Test]
        public void TryValidate_mega_payload_is_rejected_before_parse()
        {
            var mega = new string('x', 10_000);

            var ok = DeterministicAgentUuid.TryValidate(mega, out var normalized);

            Assert.That(ok, Is.False);
            Assert.That(normalized, Is.Null);
        }

        // ------------------------------------------------------------------
        // Guid.Empty warn wording — the resolver's warn message must be
        // broad enough to cover BOTH parse failure AND the RFC 4122 nil
        // UUID (which parses fine but is rejected as a collision hazard).
        // ------------------------------------------------------------------

        /// <summary>
        /// When the operator supplies the all-zero nil UUID, the resolver
        /// must warn with the broad "not an acceptable RFC 4122 UUID"
        /// wording — not the narrower "unparseable" wording — because the
        /// nil UUID DOES parse per RFC 4122 §4.1.7. Operators debugging
        /// why their all-zero UUID was rejected should not look for a
        /// parse failure that never happened.
        /// </summary>
        [Test]
        public void Resolve_warn_on_nil_uuid_uses_broad_acceptable_wording()
        {
            var messages = new List<string>();
            const string Nil = "00000000-0000-0000-0000-000000000000";

            _ = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: Nil,
                persistedUuid: null,
                agentName: "test-agent",
                hostname: "test-host",
                warn: messages.Add);

            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0], Does.Contain("not an acceptable RFC 4122 UUID"),
                "Warn must use broad 'not acceptable' wording so operators recognise nil-UUID rejection.");
            Assert.That(messages[0], Does.Contain("nil UUID"),
                "Warn should mention the nil-UUID rejection cause explicitly for diagnostic clarity.");
            AssertLogSafe(messages[0]);
        }

        // ------------------------------------------------------------------
        // Derive defensive guard — refuses to derive from an empty seed so
        // a misconfigured caller cannot produce a fleet-wide collision UUID.
        // ------------------------------------------------------------------

        /// <summary>
        /// <see cref="DeterministicAgentUuid.Derive"/> throws
        /// <see cref="ArgumentException"/> when both <c>agentName</c> and
        /// <c>hostname</c> are null / empty — the derived seed would be
        /// "agent::0", hashing to a single UUID for every misconfigured
        /// agent and defeating the entire lifetime-unique guarantee.
        /// </summary>
        [TestCase(null, null)]
        [TestCase(null, "")]
        [TestCase("", null)]
        [TestCase("", "")]
        public void Derive_throws_when_both_agent_name_and_hostname_are_empty(string agentName, string hostname)
        {
            Assert.Throws<ArgumentException>(
                (Action)(() => DeterministicAgentUuid.Derive(agentName, hostname, port: 0)),
                "Both seed components empty must not silently derive a fleet-wide collision UUID.");
        }

        /// <summary>
        /// Non-empty <c>agentName</c> is sufficient — the null / empty
        /// <c>hostname</c> is tolerated because <c>Environment.MachineName</c>
        /// is never null in practice but the API surface accepts either.
        /// </summary>
        [TestCase("agent-name", null)]
        [TestCase("agent-name", "")]
        public void Derive_accepts_null_or_empty_hostname_when_agent_name_supplied(string agentName, string hostname)
        {
            var uuid = DeterministicAgentUuid.Derive(agentName, hostname, port: 0);

            Assert.That(Guid.TryParse(uuid, out _), Is.True,
                "Derive must succeed when agentName is non-empty even if hostname is null / empty.");
        }

        // ------------------------------------------------------------------
        // Boot-simulation helpers — thin wrappers around AgentUuidResolver.
        // ------------------------------------------------------------------

        /// <summary>
        /// Replays the fresh-boot UUID resolution slice of
        /// <c>MTConnectAgentApplication.StartAgent</c> — no
        /// <c>agent.information.json</c> on disk — via
        /// <see cref="AgentUuidResolver.Resolve"/> (the exact call production
        /// makes) followed by <see cref="MTConnectAgentInformation.Save"/> so
        /// on-disk state assertions can verify persistence.
        /// </summary>
        private string SimulateFreshBoot(
            AgentApplicationConfiguration configuration,
            string hostname)
        {
            var existing = MTConnectAgentInformation.Read();
            var freshlyConstructed = existing == null;
            var info = existing ?? new MTConnectAgentInformation();

            info.Uuid = AgentUuidResolver.Resolve(
                operatorSuppliedUuid: configuration.AgentUuid,
                persistedUuid: freshlyConstructed ? null : info.Uuid,
                agentName: configuration.ServiceName,
                hostname: hostname);

            info.Save();
            return info.Uuid;
        }

        /// <summary>
        /// Replays the warm-boot UUID resolution slice — pre-existing
        /// <c>agent.information.json</c> is read first — via
        /// <see cref="AgentUuidResolver.Resolve"/>. Semantically identical to
        /// <see cref="SimulateFreshBoot"/>; two named helpers make the
        /// per-test intent (fresh vs warm) unambiguous at the call site.
        /// </summary>
        private string SimulateWarmBoot(
            AgentApplicationConfiguration configuration,
            string hostname) => SimulateFreshBoot(configuration, hostname);
    }
}
