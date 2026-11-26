// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Devices
{
    /// <summary>
    /// <see cref="MTConnect.Devices.Device.RemoveDataItem(string)"/> must remove a DataItem wherever it sits in the
    /// Device: directly on the Device, on a Component, or on a Composition. It used to walk only
    /// the Components, so a DataItem on the Device itself could not be removed (issue #231).
    /// </summary>
    [TestFixture]
    public class DeviceRemoveDataItemTests
    {
        private const string DataItemId = "di-1";

        private static DataItem NewDataItem(string id = DataItemId) => new DataItem
        {
            Id = id,
            Name = id,
            Category = DataItemCategory.EVENT,
            Type = "UNKNOWN_EVENT_TYPE",
        };

        private static MTConnect.Devices.Device NewDevice() => new MTConnect.Devices.Device { Id = "d1", Name = "d1", Uuid = "d1-uuid" };

        /// <summary>A DataItem directly on the Device is removed.</summary>
        [Test]
        public void Removes_DataItem_On_Device()
        {
            var device = NewDevice();
            device.AddDataItem(NewDataItem());
            device.AddDataItem(NewDataItem("di-keep"));
            Assert.That(device.DataItems.Any(o => o.Id == DataItemId), Is.True, "precondition");

            device.RemoveDataItem(DataItemId);

            Assert.That(device.DataItems.Any(o => o.Id == DataItemId), Is.False);
            Assert.That(device.DataItems.Any(o => o.Id == "di-keep"), Is.True, "other DataItems must be kept");
        }

        /// <summary>A DataItem on a child Component is still removed.</summary>
        [Test]
        public void Removes_DataItem_On_Component()
        {
            var device = NewDevice();
            var component = new LinearComponent { Id = "c1", Name = "c1" };
            component.AddDataItem(NewDataItem());
            device.AddComponent(component);
            Assert.That(device.GetDataItems().Any(o => o.Id == DataItemId), Is.True, "precondition");

            device.RemoveDataItem(DataItemId);

            // GetDataItems() returns null once no DataItems are left
            Assert.That(device.GetDataItems()?.Any(o => o.Id == DataItemId), Is.Not.True);
        }

        /// <summary>A DataItem on a Composition is removed.</summary>
        [Test]
        public void Removes_DataItem_On_Composition()
        {
            var device = NewDevice();
            var composition = new ActuatorComposition { Id = "cmp1", Name = "cmp1" };
            composition.AddDataItem(NewDataItem());
            device.AddComposition(composition);
            Assert.That(device.GetDataItems().Any(o => o.Id == DataItemId), Is.True, "precondition");

            device.RemoveDataItem(DataItemId);

            // GetDataItems() returns null once no DataItems are left
            Assert.That(device.GetDataItems()?.Any(o => o.Id == DataItemId), Is.Not.True);
        }

        /// <summary>Removing an ID that is not present leaves the Device unchanged.</summary>
        [Test]
        public void Unknown_Id_Is_NoOp()
        {
            var device = NewDevice();
            device.AddDataItem(NewDataItem());

            device.RemoveDataItem("not-present");

            Assert.That(device.DataItems.Count(), Is.EqualTo(1));
        }
    }
}
