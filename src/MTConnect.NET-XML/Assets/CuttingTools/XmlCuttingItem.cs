// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.Json.CuttingTools;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public class XmlCuttingItem
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

        //[JsonPropertyName("itemLife")]
        //public IEnumerable<JsonItemLife> ItemLife { get; set; }

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

            // Indices
            if (!string.IsNullOrEmpty(Indices))
            {
                cuttingItem.Indices = Indices.Split(",");
            }

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

            //// ItemLife
            //if (!ItemLife.IsNullOrEmpty())
            //{
            //    var itemLifes = new List<IItemLife>();
            //    foreach (var itemLife in ItemLife)
            //    {
            //        itemLifes.Add(itemLife.ToItemLife());
            //    }
            //    cuttingItem.ItemLife = itemLifes;
            //}

            //// CutterStatus
            //if (!CutterStatus.IsNullOrEmpty())
            //{
            //    var statuses = new List<CutterStatusType>();
            //    foreach (var cutterStatus in CutterStatus)
            //    {
            //        statuses.Add(cutterStatus.ConvertEnum<CutterStatusType>());
            //    }
            //    cuttingItem.CutterStatus = statuses;
            //}

            //// Measurements
            //if (!Measurements.IsNullOrEmpty())
            //{
            //    var measurements = new List<IMeasurement>();
            //    foreach (var measurement in Measurements)
            //    {
            //        measurements.Add(measurement.ToMeasurement());
            //    }
            //    cuttingItem.Measurements = measurements;
            //}

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

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}