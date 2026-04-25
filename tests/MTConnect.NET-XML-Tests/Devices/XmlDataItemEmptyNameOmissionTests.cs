// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.Xml;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Devices
{
    /// <summary>
    /// Parity pin: the XML formatter is the reference shape for the JSON-cppagent
    /// fix on the `name` attribute (see https://github.com/TrakHound/MTConnect.NET/issues/138).
    /// This fixture confirms `XmlDataItem.WriteXml` already omits the `name`
    /// attribute when the source `IDataItem.Name` is null or empty, and emits it
    /// otherwise.
    ///
    /// Source authority:
    /// - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.5.xsd —
    ///   `DataItem/@name` is `use="optional"`. An optional attribute with no
    ///   value must be omitted from the wire, not emitted as `name=""`.
    /// </summary>
    [TestFixture]
    public class XmlDataItemEmptyNameOmissionTests
    {
        [Test]
        public void Xml_formatter_omits_name_attribute_when_source_Name_is_null()
        {
            var source = new DataItem
            {
                Id = "item-1",
                Type = "TEMPERATURE",
                Category = DataItemCategory.SAMPLE,
                Name = null
            };

            var doc = XDocument.Parse(WriteXml(source));
            Assert.That(doc.Root, Is.Not.Null);
            Assert.That(doc.Root!.Attribute("name"), Is.Null,
                "XmlDataItem must omit the 'name' attribute when source Name is null.");
        }

        [Test]
        public void Xml_formatter_omits_name_attribute_when_source_Name_is_empty()
        {
            var source = new DataItem
            {
                Id = "item-2",
                Type = "TEMPERATURE",
                Category = DataItemCategory.SAMPLE,
                Name = string.Empty
            };

            var doc = XDocument.Parse(WriteXml(source));
            Assert.That(doc.Root, Is.Not.Null);
            Assert.That(doc.Root!.Attribute("name"), Is.Null,
                "XmlDataItem must omit the 'name' attribute when source Name is empty.");
        }

        [Test]
        public void Xml_formatter_emits_name_attribute_when_source_Name_is_set()
        {
            var source = new DataItem
            {
                Id = "item-3",
                Type = "TEMPERATURE",
                Category = DataItemCategory.SAMPLE,
                Name = "temp"
            };

            var doc = XDocument.Parse(WriteXml(source));
            Assert.That(doc.Root, Is.Not.Null);
            Assert.That(doc.Root!.Attribute("name")?.Value, Is.EqualTo("temp"));
        }

        private static string WriteXml(IDataItem dataItem)
        {
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                XmlDataItem.WriteXml(writer, dataItem);
            }
            return sb.ToString();
        }
    }
}
