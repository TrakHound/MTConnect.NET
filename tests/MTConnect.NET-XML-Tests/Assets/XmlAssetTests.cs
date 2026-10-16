// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MTConnect.Assets;
using MTConnect.Assets.ComponentConfigurationParameters;
using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Files;
using MTConnect.Assets.QIF;
using MTConnect.Assets.RawMaterials;
using MTConnect.Assets.Xml;
using MTConnect.Assets.Xml.CuttingTools;
using MTConnect.Formatters.Xml;
using MTConnect.Headers;
using NUnit.Framework;

namespace MTConnect.Tests.XML.Assets
{
    /// <summary>
    /// Pins <see cref="XmlAsset"/>: reading with namespaces, byte-order marks and prefixes intact,
    /// error reporting, the common attributes and Description element, per-version Description
    /// placement, round trips for each asset type, and that one unwritable asset cannot corrupt an
    /// MTConnectAssets document.
    /// </summary>
    [TestFixture]
    [Category("XmlAsset")]
    public class XmlAssetTests
    {
        private const string AssetsNamespace = "urn:mtconnect.org:MTConnectAssets:2.5";
        private const string QifNamespace = "http://qifstandards.org/xsd/qif3";

        private static readonly DateTime Timestamp = new DateTime(2024, 5, 6, 7, 8, 9, DateTimeKind.Utc);


        private static byte[] Bytes(string xml) => Encoding.UTF8.GetBytes(xml);

        private static string ToXmlString(IAsset asset)
        {
            using (var stream = XmlAsset.ToXml(asset))
            {
                Assert.That(stream, Is.Not.Null, "ToXml returned no stream");
                return new StreamReader(stream, Encoding.UTF8).ReadToEnd();
            }
        }

        private static T RoundTrip<T>(T asset) where T : class, IAsset
        {
            var xml = ToXmlString(asset);
            var read = XmlAsset.FromXml(asset.Type, Bytes(xml));
            Assert.That(read, Is.InstanceOf<T>(), $"Round trip failed for:\n{xml}");
            return (T)read;
        }

        private static CuttingToolAsset CreateCuttingTool(string assetId = "T1")
        {
            var asset = new CuttingToolAsset();
            asset.AssetId = assetId;
            asset.DeviceUuid = "dev";
            asset.Timestamp = Timestamp;
            asset.ToolId = "tool-" + assetId;
            asset.SerialNumber = "SN-" + assetId;
            return asset;
        }

        private static FileAsset CreateFile(string name = "part.nc")
        {
            var asset = new FileAsset();
            asset.AssetId = "F1";
            asset.DeviceUuid = "dev";
            asset.Timestamp = Timestamp;
            asset.Name = name;
            asset.MediaType = "text/plain";
            asset.Size = 10;
            asset.VersionId = "1";
            asset.CreationTime = Timestamp;
            asset.Location = new FileLocation { Href = "http://example.com/part.nc" };
            return asset;
        }

        private static XElement[] WriteAssetsDocument(Version version, params IAsset[] assets)
        {
            var document = new AssetsResponseDocument();
            document.Header = new MTConnectAssetsHeader { CreationTime = Timestamp, Sender = "test", Version = version.ToString() };
            document.Version = version;
            document.Assets = assets;

            using (var stream = XmlAssetsResponseDocument.ToXmlStream(document))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var root = XDocument.Load(stream).Root;
                return root.Elements().First(o => o.Name.LocalName == "Assets").Elements().ToArray();
            }
        }


        [Test]
        public void FromXml_reads_an_asset_carrying_a_prefixed_vendor_namespace()
        {
            // Removing the xmlns declarations from the text left "x:" undeclared, so the asset was dropped
            var xml = $@"<CuttingTool xmlns=""{AssetsNamespace}"" xmlns:x=""urn:example:vendor"" assetId=""T1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z"" toolId=""tool"" serialNumber=""SN""><x:Extra>1</x:Extra></CuttingTool>";

            var asset = XmlAsset.FromXml("CuttingTool", Bytes(xml));

            Assert.That(asset, Is.InstanceOf<CuttingToolAsset>());
            Assert.That(asset.AssetId, Is.EqualTo("T1"));
        }

        [Test]
        public void FromXml_keeps_the_namespace_of_an_embedded_QIF_document()
        {
            var xml = $@"<QIFDocumentWrapper xmlns=""{AssetsNamespace}"" assetId=""Q1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z"" qifDocumentType=""PLAN""><QIFDocument xmlns=""{QifNamespace}"" idMax=""1""><Header/></QIFDocument></QIFDocumentWrapper>";

            var asset = XmlAsset.FromXml("QIFDocumentWrapper", Bytes(xml)) as IQIFDocumentWrapperAsset;

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.QIFDocument, Does.Contain($"xmlns=\"{QifNamespace}\""));
            Assert.That(XElement.Parse(asset.QIFDocument).Name.NamespaceName, Is.EqualTo(QifNamespace));
        }

        [Test]
        public void FromXml_reads_input_with_a_byte_order_mark_and_leading_whitespace()
        {
            var xml = "\r\n  <?xml version=\"1.0\" encoding=\"utf-8\"?><CuttingTool assetId=\"T1\" deviceUuid=\"dev\" timestamp=\"2024-05-06T07:08:09Z\" toolId=\"tool\"/>";
            var bytes = Encoding.UTF8.GetPreamble().Concat(Bytes(xml)).ToArray();

            var asset = XmlAsset.FromXml("CuttingTool", bytes);

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.AssetId, Is.EqualTo("T1"));
        }

        [Test]
        public void FromXml_reads_input_without_an_MTConnect_namespace()
        {
            var asset = XmlAsset.FromXml("CuttingTool", Bytes(@"<CuttingTool assetId=""T1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z"" toolId=""tool""/>"));

            Assert.That(asset, Is.Not.Null);
        }

        [Test]
        public void TryFromXml_reports_why_reading_failed()
        {
            Assert.That(XmlAsset.TryFromXml("NoSuchType", Bytes("<NoSuchType/>"), out var unknown, out var unknownError), Is.False);
            Assert.That(unknown, Is.Null);
            Assert.That(unknownError, Does.Contain("NoSuchType"));

            Assert.That(XmlAsset.TryFromXml("CuttingTool", Bytes("<CuttingTool assetId="), out var malformed, out var malformedError), Is.False);
            Assert.That(malformed, Is.Null);
            Assert.That(malformedError, Is.Not.Empty);

            Assert.That(XmlAsset.TryFromXml("CuttingTool", Bytes("<File assetId=\"F1\"/>"), out _, out var wrongRootError), Is.False);
            Assert.That(wrongRootError, Does.Contain("File"));
        }

        [Test]
        public void CreateAsset_reports_the_read_error()
        {
            var result = new XmlEntityFormatter().CreateAsset("CuttingTool", new MemoryStream(Bytes("<CuttingTool assetId=")));

            Assert.That(result.Success, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        }

        [Test]
        public void FromXml_reads_the_Description_element_and_the_hash_attribute()
        {
            var xml = $@"<File xmlns=""{AssetsNamespace}"" assetId=""F1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z"" hash=""abc="" name=""a"" mediaType=""text/plain"" applicationCategory=""DEVICE"" applicationType=""DATA"" size=""1"" versionId=""1"" state=""PRODUCTION""><CreationTime>2024-05-06T07:08:09Z</CreationTime><Description>  Spindle &amp; probe  </Description></File>";

            var asset = XmlAsset.FromXml("File", Bytes(xml));

            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.Description, Is.EqualTo("Spindle & probe"));
            Assert.That(asset.Hash, Is.EqualTo("abc="));
        }

        [Test]
        public void ToXml_omits_unset_attributes_and_writes_timestamps_in_UTC()
        {
            var asset = CreateCuttingTool();
            asset.DeviceUuid = null;
            asset.Timestamp = Timestamp.ToLocalTime();

            var element = XElement.Parse(ToXmlString(asset));

            Assert.That(element.Attribute("deviceUuid"), Is.Null);
            Assert.That(DateTime.Parse(element.Attribute("timestamp").Value).ToUniversalTime(), Is.EqualTo(Timestamp));
            Assert.That(element.Attribute("timestamp").Value, Does.EndWith("Z"));
        }

        [Test]
        public void ToXml_returns_null_for_a_type_without_a_serializer()
        {
            var asset = new CuttingToolAsset { AssetId = "X", Type = "NoSuchType" };

            Assert.That(XmlAsset.ToXml(asset), Is.Null);
            Assert.That(new XmlEntityFormatter().Format(asset).Success, Is.False);
        }

        [Test]
        public void GetAssetType_maps_only_asset_surrogates()
        {
            Assert.That(XmlAsset.GetAssetType("CuttingTool"), Is.EqualTo(typeof(XmlCuttingToolAsset)));
            Assert.That(XmlAsset.GetAssetType("CuttingToolArchetype"), Is.EqualTo(typeof(XmlCuttingToolArchetypeAsset)));
            Assert.That(XmlAsset.GetAssetType("Unknown"), Is.Null);
            Assert.That(XmlAsset.GetAssetType("AbstractFile"), Is.Null);
            Assert.That(XmlAsset.GetAssetType(null), Is.Null);
        }


        [Test]
        public void CuttingTool_round_trips_with_description_and_hash()
        {
            var asset = CreateCuttingTool();
            asset.Description = "Finishing tool";
            asset.Hash = "h1";
            asset.Manufacturers = new[] { "Acme" };

            var read = RoundTrip(asset);

            Assert.That(read.AssetId, Is.EqualTo("T1"));
            Assert.That(read.ToolId, Is.EqualTo("tool-T1"));
            Assert.That(read.SerialNumber, Is.EqualTo("SN-T1"));
            Assert.That(read.Timestamp.ToUniversalTime(), Is.EqualTo(Timestamp));
            Assert.That(read.Description, Is.EqualTo("Finishing tool"));
            Assert.That(read.Hash, Is.EqualTo("h1"));
            Assert.That(read.Manufacturers, Is.EqualTo(new[] { "Acme" }));
        }

        [Test]
        public void CuttingToolArchetype_round_trips_as_an_archetype()
        {
            // Read back with Type "CuttingTool", the archetype was then given to the CuttingTool writer and dropped
            var asset = new CuttingToolArchetypeAsset { AssetId = "A1", DeviceUuid = "dev", Timestamp = Timestamp, ToolId = "tool" };
            Assert.That(asset.Type, Is.EqualTo("CuttingToolArchetype"));

            var read = RoundTrip(asset);

            Assert.That(read.Type, Is.EqualTo("CuttingToolArchetype"));
            Assert.That(read.ToolId, Is.EqualTo("tool"));
            Assert.That(XElement.Parse(ToXmlString(read)).Name.LocalName, Is.EqualTo("CuttingToolArchetype"));
        }

        [Test]
        public void File_round_trips()
        {
            var read = RoundTrip(CreateFile());

            Assert.That(read.Name, Is.EqualTo("part.nc"));
            Assert.That(read.Size, Is.EqualTo(10));
            Assert.That(read.VersionId, Is.EqualTo("1"));
            Assert.That(read.Location?.Href, Is.EqualTo("http://example.com/part.nc"));
        }

        [Test]
        public void RawMaterial_round_trips()
        {
            var asset = new RawMaterialAsset { AssetId = "R1", DeviceUuid = "dev", Timestamp = Timestamp, Name = "Bar stock", Form = Form.BAR, CurrentQuantity = 3 };

            var read = RoundTrip(asset);

            Assert.That(read.Name, Is.EqualTo("Bar stock"));
            Assert.That(read.Form, Is.EqualTo(Form.BAR));
            Assert.That(read.CurrentQuantity, Is.EqualTo(3));
        }

        [Test]
        public void QIFDocumentWrapper_round_trips_with_its_document_namespace()
        {
            var asset = new QIFDocumentWrapperAsset { AssetId = "Q1", DeviceUuid = "dev", Timestamp = Timestamp, QifDocumentType = QIFDocumentType.PLAN };
            asset.QIFDocument = $"<QIFDocument xmlns=\"{QifNamespace}\"><Header /></QIFDocument>";

            var read = RoundTrip(asset);

            Assert.That(read.QifDocumentType, Is.EqualTo(QIFDocumentType.PLAN));
            Assert.That(XElement.Parse(read.QIFDocument).Name.NamespaceName, Is.EqualTo(QifNamespace));
        }

        [Test]
        public void ComponentConfigurationParameters_round_trips()
        {
            var asset = new ComponentConfigurationParametersAsset { AssetId = "C1", DeviceUuid = "dev", Timestamp = Timestamp };
            asset.ParameterSets = new List<IParameterSet>
            {
                new ParameterSet { Name = "Set", Parameters = new List<IParameter> { new Parameter { Identifier = "p1", Name = "Speed", Value = "10" } } }
            };

            var read = RoundTrip(asset);

            var parameter = read.ParameterSets?.SingleOrDefault()?.Parameters?.SingleOrDefault();
            Assert.That(parameter?.Identifier, Is.EqualTo("p1"));
            Assert.That(parameter?.Value, Is.EqualTo("10"));
        }


        [Test]
        public void An_asset_that_fails_part_way_does_not_corrupt_the_assets_document()
        {
            // The control character fails the "name" attribute after the File element is open
            var broken = CreateFile("bad\u0001name");
            var good = CreateCuttingTool("T2");

            var assets = WriteAssetsDocument(MTConnectVersions.Version25, broken, good);

            Assert.That(assets.Select(o => o.Name.LocalName), Is.EqualTo(new[] { "CuttingTool" }));
            Assert.That(assets[0].Attribute("assetId")?.Value, Is.EqualTo("T2"));
        }

        [Test]
        public void Description_is_placed_for_the_document_version()
        {
            var tool = CreateCuttingTool();
            tool.Description = "Tool description";
            tool.CuttingToolDefinition = new CuttingToolDefinition { Format = FormatType.EXPRESS, Value = "data" };
            var file = CreateFile();
            file.Description = "File description";

            // Before 2.6: first element of a CuttingTool, and not defined for a File
            var v25 = WriteAssetsDocument(MTConnectVersions.Version25, tool, file);
            Assert.That(v25[0].Elements().First().Name.LocalName, Is.EqualTo("Description"));
            Assert.That(v25[1].Elements().Any(o => o.Name.LocalName == "Description"), Is.False);

            // 2.6 and later: last element of every asset
            var v27 = WriteAssetsDocument(MTConnectVersions.Version27, tool, file);
            Assert.That(v27[0].Elements().Last().Name.LocalName, Is.EqualTo("Description"));
            Assert.That(v27[0].Elements().Count(o => o.Name.LocalName == "Description"), Is.EqualTo(1));
            Assert.That(v27[1].Elements().Last().Value, Is.EqualTo("File description"));
        }

        [Test]
        public void Assets_document_reads_archetypes_descriptions_and_the_version()
        {
            var xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<MTConnectAssets xmlns=""{AssetsNamespace}"" xmlns:m=""{AssetsNamespace}"" xmlns:x=""urn:example:vendor"">
  <Header creationTime=""2024-05-06T07:08:09Z"" sender=""test"" instanceId=""1"" version=""2.5"" assetBufferSize=""10"" assetCount=""2"" />
  <Assets>
    <CuttingTool assetId=""T1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z"" toolId=""tool""><Description>Tool</Description><x:Extra/></CuttingTool>
    <CuttingToolArchetype assetId=""A1"" deviceUuid=""dev"" timestamp=""2024-05-06T07:08:09Z"" toolId=""tool"" />
  </Assets>
</MTConnectAssets>";

            var document = XmlAssetsResponseDocument.FromXml(Bytes(xml));

            Assert.That(document, Is.Not.Null);
            Assert.That(document.Version, Is.EqualTo(MTConnectVersions.Version25));
            Assert.That(document.Assets.Select(o => o.Type), Is.EqualTo(new[] { "CuttingTool", "CuttingToolArchetype" }));
            Assert.That(document.Assets.First().Description, Is.EqualTo("Tool"));
        }
    }
}
