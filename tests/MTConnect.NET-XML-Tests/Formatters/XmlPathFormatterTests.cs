// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Formatters.Xml;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Formatters
{
    /// <summary>
    /// Pins that <see cref="XmlPathFormatter"/> evaluates a path against the device model it is given:
    /// its per-device XML cache was keyed on the device Id alone, so after a device model change (or for
    /// another Agent's device with the same Id) the path was evaluated against the stale model.
    /// </summary>
    [TestFixture]
    [Category("XmlPathFormatter")]
    public class XmlPathFormatterTests
    {
        private static IDevicesResponseDocument Document(params string[] dataItemIds)
        {
            var device = new Device
            {
                Id = "dev",
                Name = "dev",
                Uuid = "dev-uuid",
                DataItems = dataItemIds.Select(id => (IDataItem)new ProgramDataItem { Id = id }).ToList()
            };

            return new DevicesResponseDocument { Devices = new List<IDevice> { device } };
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: changed device model with same id is not served from cache.
        /// </summary>
        [Test]
        public void ChangedDeviceModel_WithSameId_IsNotServedFromCache()
        {
            var formatter = new XmlPathFormatter();

            var before = formatter.GetDataItemIds(Document("a", "b"), "//DataItem[@id=\"b\"]");
            var after = formatter.GetDataItemIds(Document("a", "c"), "//DataItem[@id=\"c\"]");

            Assert.That(before, Is.EqualTo(new[] { "b" }));
            Assert.That(after, Is.EqualTo(new[] { "c" }));
        }
    }
}
