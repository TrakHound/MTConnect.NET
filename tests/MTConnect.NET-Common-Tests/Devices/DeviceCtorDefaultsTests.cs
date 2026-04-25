// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using NUnit.Framework;

namespace MTConnect.Tests.Common.DeviceCtorDefaults
{
    /// <summary>
    /// Pins the contract that the parameterless <see cref="Device"/>
    /// and <see cref="Agent"/> constructors leave identity and naming
    /// fields <c>null</c>. Stamping a placeholder identifier, the
    /// placeholder name <c>"dev"</c>, or a fresh random UUID per
    /// construction would silently inject those placeholders into wire
    /// output for any caller that does not set the fields explicitly.
    /// </summary>
    [TestFixture]
    [Category("DeviceComponentDefaultsRemoved")]
    public class DeviceCtorDefaultsTests
    {
        [Test]
        public void Default_constructor_leaves_Id_null()
        {
            var device = new Device();
            Assert.That(device.Id, Is.Null);
        }

        [Test]
        public void Default_constructor_leaves_Name_null()
        {
            var device = new Device();
            Assert.That(device.Name, Is.Null);
        }

        [Test]
        public void Default_constructor_leaves_Uuid_null()
        {
            var device = new Device();
            Assert.That(device.Uuid, Is.Null);
        }

        [Test]
        public void Object_initializer_still_sets_Id()
        {
            var device = new Device { Id = "device-1" };
            Assert.That(device.Id, Is.EqualTo("device-1"));
        }

        [Test]
        public void Object_initializer_still_sets_Name()
        {
            var device = new Device { Name = "spindle-A" };
            Assert.That(device.Name, Is.EqualTo("spindle-A"));
        }

        [Test]
        public void Object_initializer_still_sets_Uuid()
        {
            var device = new Device { Uuid = "F1" };
            Assert.That(device.Uuid, Is.EqualTo("F1"));
        }

        [Test]
        public void Sequential_default_constructors_produce_identical_null_identity()
        {
            var first = new Device();
            var second = new Device();

            Assert.That(first.Id, Is.Null);
            Assert.That(second.Id, Is.Null);
            Assert.That(first.Name, Is.Null);
            Assert.That(second.Name, Is.Null);
            Assert.That(first.Uuid, Is.Null);
            Assert.That(second.Uuid, Is.Null);
        }

        [Test]
        public void Default_constructor_still_sets_Type()
        {
            // Type is infrastructure, not identity — left alone by the campaign.
            var device = new Device();
            Assert.That(device.Type, Is.EqualTo(Device.TypeId));
        }

        [Test]
        public void Default_constructor_still_initializes_collections()
        {
            // Collections are infrastructure for later population — left alone.
            var device = new Device();
            Assert.That(device.DataItems, Is.Not.Null);
            Assert.That(device.Components, Is.Not.Null);
            Assert.That(device.Compositions, Is.Not.Null);
        }

        [Test]
        public void Agent_default_constructor_leaves_Uuid_null()
        {
            var agent = new Agent();
            Assert.That(agent.Uuid, Is.Null);
        }

        [Test]
        public void Agent_default_constructor_leaves_Id_null()
        {
            var agent = new Agent();
            Assert.That(agent.Id, Is.Null);
        }

        [Test]
        public void Agent_default_constructor_leaves_Name_null()
        {
            var agent = new Agent();
            Assert.That(agent.Name, Is.Null);
        }
    }
}
