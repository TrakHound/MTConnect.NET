// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Pallet;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.Pallet
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>Pallet</c> asset (2.5+). Mirrors the
    /// on-the-wire element and converts to and from the strongly-typed <see cref="PalletAsset"/>.
    /// </summary>
    [XmlRoot("Pallet")]
    public class XmlPalletAsset : XmlPhysicalAsset
    {
        /// <summary>
        /// The type of pallet (the <c>Type</c> element; <see cref="PalletAsset.PalletType"/> in the model).
        /// </summary>
        [XmlElement("Type")]
        public string PalletType { get; set; }

        /// <summary>
        /// The identifier of the pallet.
        /// </summary>
        [XmlElement("PalletId")]
        public string PalletId { get; set; }

        /// <summary>
        /// The number of the pallet, as the raw element text.
        /// </summary>
        [XmlElement("PalletNumber")]
        public string PalletNumber { get; set; }

        /// <summary>
        /// The actuation type of the pallet's clamping mechanism.
        /// </summary>
        [XmlElement("ClampingMethod")]
        public string ClampingMethod { get; set; }

        /// <summary>
        /// The actuation type of the pallet's mounting mechanism.
        /// </summary>
        [XmlElement("MountingMethod")]
        public string MountingMethod { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed <see cref="PalletAsset"/>.
        /// </summary>
        public override IAsset ToAsset()
        {
            var asset = new PalletAsset();
            CopyPhysicalProperties(asset);

            asset.PalletType = PalletType;
            asset.PalletId = PalletId;
            asset.PalletNumber = ParseInteger(PalletNumber);
            asset.ClampingMethod = ClampingMethod;
            asset.MountingMethod = MountingMethod;

            return asset;
        }

        /// <summary>
        /// Writes the given <see cref="IPalletAsset"/> to <paramref name="writer"/> as a <c>Pallet</c>
        /// element, omitting values that are not set (and, for a 2.5 document, everything but the
        /// <c>Measurements</c>, which is all 2.5 defines).
        /// </summary>
        public static new void WriteXml(XmlWriter writer, IAsset asset)
        {
            if (asset != null)
            {
                var pallet = (IPalletAsset)asset;

                writer.WriteStartElement("Pallet");

                WriteCommonXml(writer, asset);

                if (WritesFullContent)
                {
                    WriteElement(writer, "Type", pallet.PalletType);
                    WriteElement(writer, "PalletId", pallet.PalletId);
                    if (pallet.PalletNumber != 0) WriteElement(writer, "PalletNumber", pallet.PalletNumber.ToString(CultureInfo.InvariantCulture));
                    WriteElement(writer, "ClampingMethod", pallet.ClampingMethod);
                    WriteElement(writer, "MountingMethod", pallet.MountingMethod);
                }

                WritePhysicalElementsXml(writer, pallet);

                WriteCommonElementsXml(writer, asset);

                writer.WriteEndElement();
            }
        }
    }
}
