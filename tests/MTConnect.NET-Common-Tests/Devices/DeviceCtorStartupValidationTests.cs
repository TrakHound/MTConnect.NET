// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Devices;
using NUnit.Framework;

namespace MTConnect.Tests.Common.DeviceCtorDefaults
{
    /// <summary>
    /// Pins the startup-time invariant that <see cref="MTConnectAgent.AddDevice"/>
    /// rejects any Device whose <see cref="IContainer.Uuid"/> is null or empty.
    ///
    /// Background: the parameterless <see cref="Device"/> ctor leaves
    /// <c>Id</c>, <c>Name</c>, and <c>Uuid</c> all null so the ctor does
    /// not silently inject placeholders into wire output. That contract
    /// is pinned by <see cref="DeviceCtorDefaultsTests"/>. The XSD,
    /// however, makes the <c>uuid</c> attribute on <c>Device</c>
    /// <c>use='required'</c> with <c>UuidType</c> documented as the
    /// element's identity 'for it's entire life'. Allowing a null-Uuid
    /// device through <c>AddDevice</c> would defer the error from
    /// startup-time configuration to wire-emission XSD validation,
    /// surfacing far away from the operator config that caused it.
    ///
    /// This fixture pins the early-failure invariant: registering a
    /// Device with no Uuid must raise <see cref="MTConnectAgent.InvalidDeviceAdded"/>
    /// (carrying a <see cref="ValidationResult"/> whose <see cref="ValidationResult.Code"/>
    /// is <c>DeviceUuidMissing</c> and whose <see cref="ValidationResult.Message"/>
    /// names the offending registration index and Device type) and the
    /// Device must not land in the agent's buffer. The event matches the
    /// existing Invalid*Added family on MTConnectAgent so subscribers
    /// can route Device-level rejections through the same handler
    /// pattern as Component / DataItem / Observation / Asset rejections.
    /// </summary>
    [TestFixture]
    [Category("DeviceComponentDefaultsRemoved")]
    public class DeviceCtorStartupValidationTests
    {
        [Test]
        public void AddDevice_with_null_uuid_raises_invalid_device_added_with_clear_message()
        {
            // The agent itself must register without complaint - the
            // built-in Agent meta-device gets a stable Uuid through the
            // MTConnectAgent ctor parameter, not through Device's
            // parameterless ctor.
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            IDevice? capturedDevice = null;
            ValidationResult capturedResult = default;
            var raisedCount = 0;
            agent.InvalidDeviceAdded += (device, result) =>
            {
                capturedDevice = device;
                capturedResult = result;
                raisedCount++;
            };

            // A bare new Device() leaves Id, Name, and Uuid all null -
            // the very shape a config-driven builder produces when the
            // operator's config omits the `uuid=` predicate.
            var device = new Device();

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Null,
                "AddDevice must return null when the Device fails Uuid validation, so callers can branch on the registration outcome without subscribing to the event.");
            Assert.That(raisedCount, Is.EqualTo(1),
                "InvalidDeviceAdded must fire exactly once for a single failed AddDevice call.");
            Assert.That(capturedDevice, Is.SameAs(device),
                "Event payload must carry the offending Device reference so subscribers can identify it.");
            Assert.That(capturedResult.IsValid, Is.False);
            Assert.That(capturedResult.Code, Is.EqualTo("DeviceUuidMissing"),
                "ValidationResult.Code must carry the machine-readable discriminator so subscribers can branch without parsing Message.");
            Assert.That(capturedResult.Message, Does.Contain("Uuid"),
                "ValidationResult.Message must name the missing field (Uuid) so the operator can diagnose the config gap.");
            Assert.That(capturedResult.Message, Does.Contain("Device").IgnoreCase,
                "ValidationResult.Message must name the offending Device's type so the operator can locate it in their config.");
        }

        [Test]
        public void AddDevice_with_empty_uuid_raises_invalid_device_added_with_clear_message()
        {
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            ValidationResult capturedResult = default;
            var raisedCount = 0;
            agent.InvalidDeviceAdded += (_, result) =>
            {
                capturedResult = result;
                raisedCount++;
            };

            // Empty-string Uuid is just as invalid as null - the XSD
            // requires the attribute to be present, and a zero-length
            // value would still fail downstream identity lookups.
            var device = new Device { Uuid = string.Empty, Id = "d0", Name = "EmptyUuidDevice" };

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Null);
            Assert.That(raisedCount, Is.EqualTo(1));
            Assert.That(capturedResult.IsValid, Is.False);
            Assert.That(capturedResult.Code, Is.EqualTo("DeviceUuidMissing"));
            Assert.That(capturedResult.Message, Does.Contain("Uuid"));
        }

        [Test]
        public void AddDevice_with_non_null_uuid_does_not_raise_invalid_device_added()
        {
            // Sanity floor: the happy path must remain green so the
            // validation doesn't accidentally reject well-formed devices
            // and doesn't fire the event for valid registrations.
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            var raisedCount = 0;
            agent.InvalidDeviceAdded += (_, _) => raisedCount++;

            var device = new Device
            {
                Id = "d1",
                Name = "VMC-3Axis",
                Uuid = "stable-uuid-d1",
            };

            var added = agent.AddDevice(device);

            Assert.That(added, Is.Not.Null,
                "AddDevice must return the normalized Device on the happy path.");
            Assert.That(raisedCount, Is.Zero,
                "InvalidDeviceAdded must NOT fire when the Device's Uuid is well-formed.");
        }

        [Test]
        public void AddDevice_event_message_names_registration_index()
        {
            // Register one well-formed device first so the second
            // (offending) device sits at registration index 1. The
            // message must include the index so an operator with many
            // devices can locate the offending one in their config.
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            ValidationResult capturedResult = default;
            agent.InvalidDeviceAdded += (_, result) => capturedResult = result;

            agent.AddDevice(new Device { Id = "d0", Name = "Good", Uuid = "good-uuid" });
            var added = agent.AddDevice(new Device());

            Assert.That(added, Is.Null);
            Assert.That(capturedResult.IsValid, Is.False);
            // The offending Device is the second AddDevice call from a
            // freshly-constructed agent, so its registration index is 1.
            Assert.That(capturedResult.Message, Does.Contain("1"),
                "ValidationResult.Message must include the offending Device's registration index (1) so the operator can locate it.");
        }

        [Test]
        public void ValidateDevice_returns_invalid_for_null_uuid_without_mutating_buffer()
        {
            // Callers may want to pre-flight a Device through the same
            // validation rule AddDevice applies, without actually
            // registering it. ValidateDevice is the public surface for
            // that and must not touch the agent's device buffer.
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            var deviceCountBefore = System.Linq.Enumerable.Count(agent.GetDevices());

            var result = agent.ValidateDevice(new Device());

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Code, Is.EqualTo("DeviceUuidMissing"));
            Assert.That(System.Linq.Enumerable.Count(agent.GetDevices()), Is.EqualTo(deviceCountBefore),
                "ValidateDevice must not register the Device, regardless of the validation outcome.");
        }

        [Test]
        public void ValidateDevice_returns_valid_for_well_formed_device()
        {
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            var result = agent.ValidateDevice(new Device { Id = "d1", Name = "VMC-3Axis", Uuid = "stable-uuid-d1" });

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Code, Is.Null.Or.Empty);
            Assert.That(result.Message, Is.Null.Or.Empty);
        }

        [Test]
        public void ValidateDevice_returns_invalid_for_null_device()
        {
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            var result = agent.ValidateDevice(null);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Code, Is.EqualTo("DeviceNull"));
        }
    }
}
