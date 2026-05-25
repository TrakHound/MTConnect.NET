// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Devices;
using NUnit.Framework;
using System;

namespace MTConnect.Tests.Common.DeviceCtorDefaults
{
    /// <summary>
    /// Pins the startup-time invariant that <see cref="MTConnectAgent.AddDevice"/>
    /// rejects any device whose <see cref="IDevice.Uuid"/> is null or empty.
    ///
    /// Background: the parameterless <see cref="Device"/> ctor leaves
    /// <c>Id</c>, <c>Name</c>, and <c>Uuid</c> all null so the ctor does
    /// not silently inject placeholders into wire output. That contract
    /// is pinned by <see cref="DeviceCtorDefaultsTests"/>. The XSD,
    /// however, makes the <c>uuid</c> attribute on <c>Device</c>
    /// <c>use='required'</c> with <c>UuidType</c> documented as the
    /// element's identity "for it's entire life". Allowing a null-Uuid
    /// device through <c>AddDevice</c> would defer the error from
    /// startup-time configuration to wire-emission XSD validation,
    /// surfacing far away from the operator config that caused it.
    ///
    /// This fixture pins the early-failure invariant: registering a
    /// device with no Uuid must throw an <see cref="InvalidOperationException"/>
    /// whose message names the offending registration index and device
    /// type so the operator can diagnose the missing config quickly.
    /// </summary>
    [TestFixture]
    [Category("DeviceComponentDefaultsRemoved")]
    public class DeviceCtorStartupValidationTests
    {
        [Test]
        public void AddDevice_with_null_uuid_throws_with_clear_message()
        {
            // The agent itself must register without complaint — the
            // built-in Agent meta-device gets a stable Uuid through the
            // MTConnectAgent ctor parameter, not through Device's
            // parameterless ctor.
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            // A bare new Device() leaves Id, Name, and Uuid all null —
            // the very shape a config-driven builder produces when the
            // operator's config omits the `uuid=` predicate.
            var device = new Device();

            var ex = Assert.Throws<InvalidOperationException>(
                () => agent.AddDevice(device));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Uuid"),
                "Exception message must name the missing field (Uuid) so the operator can diagnose the config gap.");
            Assert.That(ex.Message, Does.Contain("Device").IgnoreCase,
                "Exception message must name the offending device's type so the operator can locate it in their config.");
        }

        [Test]
        public void AddDevice_with_empty_uuid_throws_with_clear_message()
        {
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            // Empty-string Uuid is just as invalid as null — the XSD
            // requires the attribute to be present, and a zero-length
            // value would still fail downstream identity lookups.
            var device = new Device { Uuid = string.Empty, Id = "d0", Name = "EmptyUuidDevice" };

            var ex = Assert.Throws<InvalidOperationException>(
                () => agent.AddDevice(device));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Uuid"));
        }

        [Test]
        public void AddDevice_with_non_null_uuid_does_not_throw()
        {
            // Sanity floor: the happy path must remain green so the
            // validation doesn't accidentally reject well-formed devices.
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            var device = new Device
            {
                Id = "d1",
                Name = "VMC-3Axis",
                Uuid = "stable-uuid-d1",
            };

            Assert.DoesNotThrow(() => agent.AddDevice(device));
        }

        [Test]
        public void AddDevice_exception_message_names_registration_index()
        {
            // Register one well-formed device first so the second
            // (offending) device sits at registration index 1. The
            // message must include the index so an operator with many
            // devices can locate the offending one in their config.
            using var agent = new MTConnectAgent(uuid: "test-agent", initializeAgentDevice: false);

            agent.AddDevice(new Device { Id = "d0", Name = "Good", Uuid = "good-uuid" });

            var ex = Assert.Throws<InvalidOperationException>(
                () => agent.AddDevice(new Device()));

            Assert.That(ex, Is.Not.Null);
            // The offending device is the second AddDevice call from a
            // freshly-constructed agent, so its registration index is 1.
            Assert.That(ex!.Message, Does.Contain("1"),
                "Exception message must include the offending device's registration index (1) so the operator can locate it.");
        }
    }
}
