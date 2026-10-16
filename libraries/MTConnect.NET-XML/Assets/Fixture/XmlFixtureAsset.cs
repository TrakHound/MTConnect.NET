// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Fixture;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Fixture
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>Fixture</c> asset (2.5+). Mirrors the
    /// on-the-wire element and converts to and from the strongly-typed <see cref="FixtureAsset"/>.
    /// </summary>
    [XmlRoot("Fixture")]
    public class XmlFixtureAsset : XmlPhysicalAsset
    {
        /// <summary>
        /// The identifier of the fixture.
        /// </summary>
        [XmlElement("FixtureId")]
        public string FixtureId { get; set; }

        /// <summary>
        /// The number of the fixture, as the raw element text.
        /// </summary>
        [XmlElement("FixtureNumber")]
        public string FixtureNumber { get; set; }

        /// <summary>
        /// The actuation type of the fixture's clamping mechanism.
        /// </summary>
        [XmlElement("ClampingMethod")]
        public string ClampingMethod { get; set; }

        /// <summary>
        /// The actuation type of the fixture's mounting mechanism.
        /// </summary>
        [XmlElement("MountingMethod")]
        public string MountingMethod { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed <see cref="FixtureAsset"/>.
        /// </summary>
        public override IAsset ToAsset()
        {
            var asset = new FixtureAsset();
            CopyPhysicalProperties(asset);

            asset.FixtureId = FixtureId;
            asset.FixtureNumber = ParseInteger(FixtureNumber);
            asset.ClampingMethod = ClampingMethod;
            asset.MountingMethod = MountingMethod;

            return asset;
        }

        /// <summary>
        /// Writes the given <see cref="IFixtureAsset"/> to <paramref name="writer"/> as a <c>Fixture</c>
        /// element, omitting values that are not set (and, for a 2.5 document, everything but the
        /// <c>Measurements</c>, which is all 2.5 defines).
        /// </summary>
        public static new void WriteXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                var fixture = (IFixtureAsset)asset;

                writer.WriteStartElement("Fixture");

                WriteCommonXml(writer, asset);

                if (WritesFullContent)
                {
                    WriteElement(writer, "FixtureId", fixture.FixtureId);
                    if (fixture.FixtureNumber != 0) WriteElement(writer, "FixtureNumber", fixture.FixtureNumber.ToString(CultureInfo.InvariantCulture));
                    WriteElement(writer, "ClampingMethod", fixture.ClampingMethod);
                    WriteElement(writer, "MountingMethod", fixture.MountingMethod);
                }

                WritePhysicalElementsXml(writer, fixture);

                WriteCommonElementsXml(writer, asset);

                writer.WriteEndElement();
            }
        }
    }
}
