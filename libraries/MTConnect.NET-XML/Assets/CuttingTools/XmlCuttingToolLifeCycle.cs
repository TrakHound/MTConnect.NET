// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public partial class XmlCuttingToolLifeCycle
    {
        [XmlElement("CutterStatus")]
        public List<XmlCutterStatus> CutterStatus { get; set; }

        [XmlElement("ReconditionCount")]
        public XmlReconditionCount ReconditionCount { get; set; }

        [XmlElement("ToolLife")]
        public List<XmlToolLife> ToolLife { get; set; }

        [XmlElement("Location")]
        public XmlLocation Location { get; set; }

        [XmlElement("ProgramToolGroup")]
        public string ProgramToolGroup { get; set; }

        [XmlElement("ProgramToolNumber")]
        public string ProgramToolNumber { get; set; }

        [XmlElement("ConnectionCodeMachineSide")]
        public string ConnectionCodeMachineSide { get; set; }

        [XmlElement("ProcessSpindleSpeed")]
        public XmlProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        [XmlElement("ProcessFeedRate")]
        public XmlProcessFeedRate ProcessFeedRate { get; set; }

        [XmlElement("CuttingItems")]
        public XmlCuttingItems CuttingItems { get; set; }


        public ICuttingToolLifeCycle ToCuttingToolLifeCycle()
        {
            var cuttingToolLifeCycle = new CuttingToolLifeCycle();

            // CutterStatus
            if (!CutterStatus.IsNullOrEmpty())
            {
                var statuses = new List<CutterStatusType>();
                foreach (var cutterStatus in CutterStatus)
                {
                    statuses.Add(cutterStatus.Status.ConvertEnum<CutterStatusType>());
                }
                cuttingToolLifeCycle.CutterStatus = statuses;
            }

            if (ReconditionCount != null) cuttingToolLifeCycle.ReconditionCount = ReconditionCount.ToReconditionCount();
            if (Location != null) cuttingToolLifeCycle.Location = Location.ToLocation();
            cuttingToolLifeCycle.ProgramToolGroup = ProgramToolGroup;
            cuttingToolLifeCycle.ProgramToolNumber = ProgramToolNumber;
            if (ProcessSpindleSpeed != null) cuttingToolLifeCycle.ProcessSpindleSpeed = ProcessSpindleSpeed.ToProcessSpindleSpeed();
            if (ProcessFeedRate != null) cuttingToolLifeCycle.ProcessFeedRate = ProcessFeedRate.ToProcessFeedRate();
            cuttingToolLifeCycle.ConnectionCodeMachineSide = ConnectionCodeMachineSide;

            // ToolLife
            if (!ToolLife.IsNullOrEmpty())
            {
                var toolifes = new List<IToolLife>();
                foreach (var toolife in ToolLife)
                {
                    toolifes.Add(toolife.ToToolLife());
                }
                cuttingToolLifeCycle.ToolLife = toolifes;
            }

            // Measurements
            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<IToolingMeasurement>();
                foreach (var measurement in Measurements)
                {
                    measurements.Add(measurement.ToMeasurement());
                }
                cuttingToolLifeCycle.Measurements = measurements;
            }

            if (CuttingItems != null)
            {
                var cuttingItems = new List<ICuttingItem>();
                foreach (var cuttingItem in CuttingItems.CuttingItems)
                {
                    cuttingItems.Add(cuttingItem.ToCuttingItem());
                }
                cuttingToolLifeCycle.CuttingItems = cuttingItems;
            }

            return cuttingToolLifeCycle;
        }

        public static void WriteXml(XmlWriter writer, ICuttingToolLifeCycle cuttingToolLifeCycle)
        {
            if (cuttingToolLifeCycle != null)
            {
                writer.WriteStartElement("CuttingToolLifeCycle");

                // Write CutterStatus
                if (!cuttingToolLifeCycle.CutterStatus.IsNullOrEmpty())
                {
                    writer.WriteStartElement("CutterStatus");
                    foreach (var cutterStatus in cuttingToolLifeCycle.CutterStatus)
                    {
                        writer.WriteStartElement("Status");
                        writer.WriteString(cutterStatus.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                // Write ProgramToolNumber
                if (!string.IsNullOrEmpty(cuttingToolLifeCycle.ProgramToolNumber))
                {
                    writer.WriteStartElement("ProgramToolNumber");
                    writer.WriteString(cuttingToolLifeCycle.ProgramToolNumber);
                    writer.WriteEndElement();
                }

                // Write ProgramToolGroup
                if (!string.IsNullOrEmpty(cuttingToolLifeCycle.ProgramToolGroup))
                {
                    writer.WriteStartElement("ProgramToolGroup");
                    writer.WriteString(cuttingToolLifeCycle.ProgramToolGroup);
                    writer.WriteEndElement();
                }

                // Write ReconditionCount
                if (cuttingToolLifeCycle.ReconditionCount != null)
                {
                    XmlReconditionCount.WriteXml(writer, cuttingToolLifeCycle.ReconditionCount);
                }

                // Write Location
                if (cuttingToolLifeCycle.Location != null)
                {
                    XmlLocation.WriteXml(writer, cuttingToolLifeCycle.Location);
                }

                // Write ProcessSpindleSpeed
                if (cuttingToolLifeCycle.ProcessSpindleSpeed != null)
                {
                    XmlProcessSpindleSpeed.WriteXml(writer, cuttingToolLifeCycle.ProcessSpindleSpeed);
                }

                // Write ProcessFeedRate
                if (cuttingToolLifeCycle.ProcessFeedRate != null)
                {
                    XmlProcessFeedRate.WriteXml(writer, cuttingToolLifeCycle.ProcessFeedRate);
                }

                // Write ConnectionCodeMachineSide
                if (!string.IsNullOrEmpty(cuttingToolLifeCycle.ConnectionCodeMachineSide))
                {
                    writer.WriteStartElement("ConnectionCodeMachineSide");
                    writer.WriteString(cuttingToolLifeCycle.ConnectionCodeMachineSide);
                    writer.WriteEndElement();
                }

                // Write ItemLife
                if (cuttingToolLifeCycle.ToolLife != null)
                {
                    XmlToolLife.WriteXml(writer, cuttingToolLifeCycle.ToolLife);
                }

                // Write Measurements
                if (cuttingToolLifeCycle.Measurements != null)
                {
                    XmlMeasurement.WriteXml(writer, cuttingToolLifeCycle.Measurements);
                }

                // Write CuttingItems
                if (!cuttingToolLifeCycle.CuttingItems.IsNullOrEmpty())
                {
                    XmlCuttingItem.WriteXml(writer, cuttingToolLifeCycle.CuttingItems);
                }

                writer.WriteEndElement();
            }
        }
    }
}