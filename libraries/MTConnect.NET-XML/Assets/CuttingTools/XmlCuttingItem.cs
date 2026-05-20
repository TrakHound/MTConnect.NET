// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// Hand-written part of the XML serialization surrogate for a cutting tool
    /// <c>CuttingItem</c> (an individual edge of the tool). Carries the
    /// properties that are not model-generated and converts to and from the
    /// strongly-typed <see cref="ICuttingItem"/> model.
    /// </summary>
    public partial class XmlCuttingItem
    {
        /// <summary>
        /// The range of indices on the tool the cutting item occupies, as the
        /// raw attribute text.
        /// </summary>
        [XmlAttribute("indices")]
        public string Indices { get; set; }

        /// <summary>
        /// The identifier of the cutting item within the tool.
        /// </summary>
        [XmlAttribute("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// The manufacturers of the cutting item, as a comma-separated list.
        /// </summary>
        [XmlAttribute("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// The material grade of the cutting item.
        /// </summary>
        [XmlAttribute("grade")]
        public string Grade { get; set; }

        /// <summary>
        /// The free-form description of the cutting item.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }

        /// <summary>
        /// The position of the cutting item on the tool.
        /// </summary>
        [XmlElement("Locus")]
        public string Locus { get; set; }

        /// <summary>
        /// The identifier of the tool group the program refers to the item by.
        /// </summary>
        [XmlElement("ProgramToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// The status of the cutting item, such as <c>NEW</c> or <c>USED</c>.
        /// </summary>
        [XmlElement("CutterStatus")]
        public List<XmlCutterStatus> CutterStatus { get; set; }

        /// <summary>
        /// The accumulated and remaining life of the cutting item.
        /// </summary>
        [XmlElement("ItemLife")]
        public List<XmlItemLife> ItemLife { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="ICuttingItem"/>, splitting the comma-separated
        /// manufacturers and projecting each nested collection into its model
        /// representation.
        /// </summary>
        public ICuttingItem ToCuttingItem()
        {
            var cuttingItem = new CuttingItem();
            cuttingItem.ItemId = ItemId;
            cuttingItem.Grade = Grade;
            cuttingItem.Description = Description;
            cuttingItem.Locus = Locus;
            cuttingItem.ProgramToolGroup = ProgramToolGroup;
            cuttingItem.Indices = Indices;

            // Manufacturers
            if (!string.IsNullOrEmpty(Manufacturers))
            {
                cuttingItem.Manufacturers = Manufacturers.Split(',');
            }

            // CutterStatus
            if (!CutterStatus.IsNullOrEmpty())
            {
                var statuses = new List<CutterStatusType>();
                foreach (var cutterStatus in CutterStatus)
                {
                    statuses.Add(cutterStatus.Status.ConvertEnum<CutterStatusType>());
                }
                cuttingItem.CutterStatus = statuses;
            }

            // ItemLife
            if (!ItemLife.IsNullOrEmpty())
            {
                var itemLifes = new List<IItemLife>();
                foreach (var itemLife in ItemLife)
                {
                    itemLifes.Add(itemLife.ToItemLife());
                }
                cuttingItem.ItemLife = itemLifes;
            }

            // Measurements
            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<IToolingMeasurement>();
                foreach (var measurement in Measurements)
                {
                    measurements.Add(measurement.ToMeasurement());
                }
                cuttingItem.Measurements = measurements;
            }

            return cuttingItem;
        }

        /// <summary>
        /// Writes the given cutting items to <paramref name="writer"/> wrapped
        /// in a <c>CuttingItems</c> element, one <c>CuttingItem</c> element per
        /// item, omitting optional values that are not set.
        /// </summary>
        public static void WriteXml(XmlWriter writer, IEnumerable<ICuttingItem> cuttingItems)
        {
            if (!cuttingItems.IsNullOrEmpty())
            {
                writer.WriteStartElement("CuttingItems");

                foreach (var cuttingItem in cuttingItems)
                {
                    writer.WriteStartElement("CuttingItem");
                    if (!string.IsNullOrEmpty(cuttingItem.ItemId)) writer.WriteAttributeString("itemId", cuttingItem.ItemId);
                    if (!string.IsNullOrEmpty(cuttingItem.Indices)) writer.WriteAttributeString("indices", cuttingItem.Indices);
                    if (!string.IsNullOrEmpty(cuttingItem.Grade)) writer.WriteAttributeString("grade", cuttingItem.Grade);

                    // Write Description
                    if (!string.IsNullOrEmpty(cuttingItem.Description))
                    {
                        writer.WriteStartElement("Description");
                        writer.WriteString(cuttingItem.Description);
                        writer.WriteEndElement();
                    }

                    // Write Locus
                    if (!string.IsNullOrEmpty(cuttingItem.Locus))
                    {
                        writer.WriteStartElement("Locus");
                        writer.WriteString(cuttingItem.Locus);
                        writer.WriteEndElement();
                    }

                    // Write Manufacturers
                    if (!cuttingItem.Manufacturers.IsNullOrEmpty())
                    {
                        var manufacturers = string.Join(",", cuttingItem.Manufacturers);
                        if (!string.IsNullOrEmpty(manufacturers))
                        {
                            writer.WriteAttributeString("manufacturers", manufacturers);
                        }
                    }

                    // Write ProgramToolGroup
                    if (!string.IsNullOrEmpty(cuttingItem.ProgramToolGroup))
                    {
                        writer.WriteStartElement("ProgramToolGroup");
                        writer.WriteString(cuttingItem.ProgramToolGroup);
                        writer.WriteEndElement();
                    }

                    // Write CutterStatus
                    if (!cuttingItem.CutterStatus.IsNullOrEmpty())
                    {
                        writer.WriteStartElement("CutterStatus");
                        foreach (var cutterStatus in cuttingItem.CutterStatus)
                        {
                            writer.WriteStartElement("Status");
                            writer.WriteString(cutterStatus.ToString());
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                    // Write ItemLife
                    if (cuttingItem.ItemLife != null)
                    {
                        XmlItemLife.WriteXml(writer, cuttingItem.ItemLife);
                    }

                    // Write Measurements
                    if (cuttingItem.Measurements != null)
                    {
                        XmlMeasurement.WriteXml(writer, cuttingItem.Measurements);
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}