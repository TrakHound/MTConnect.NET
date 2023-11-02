// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public partial class XmlCuttingItem
    {
        [XmlAttribute("indices")]
        public string Indices { get; set; }

        [XmlAttribute("itemId")]
        public string ItemId { get; set; }

        [XmlAttribute("manufacturers")]
        public string Manufacturers { get; set; }

        [XmlAttribute("grade")]
        public string Grade { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("locus")]
        public string Locus { get; set; }

        [XmlElement("CutterStatus")]
        public List<XmlCutterStatus> CutterStatus { get; set; }

        [XmlElement("ItemLife")]
        public List<XmlItemLife> ItemLife { get; set; }

        [XmlAttribute("programToolGroup")]
        public string ProgramToolGroup { get; set; }


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
                cuttingItem.Manufacturers = Manufacturers.Split(",");
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
                var measurements = new List<IMeasurement>();
                foreach (var measurement in Measurements)
                {
                    measurements.Add(measurement.ToMeasurement());
                }
                cuttingItem.Measurements = measurements;
            }

            return cuttingItem;
        }

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
                    if (!string.IsNullOrEmpty(cuttingItem.Description)) writer.WriteAttributeString("description", cuttingItem.Description);
                    if (!string.IsNullOrEmpty(cuttingItem.Locus)) writer.WriteAttributeString("locus", cuttingItem.Locus);

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