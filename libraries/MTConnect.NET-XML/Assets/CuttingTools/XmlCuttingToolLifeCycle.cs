// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// Hand-written part of the XML serialization surrogate for a
    /// <c>CuttingToolLifeCycle</c>. Carries the life-cycle properties that are
    /// not model-generated and converts the whole element to and from the
    /// strongly-typed <see cref="ICuttingToolLifeCycle"/> model.
    /// </summary>
    public partial class XmlCuttingToolLifeCycle
    {
        /// <summary>
        /// The status of the cutting tool, such as <c>NEW</c> or <c>USED</c>.
        /// </summary>
        [XmlElement("CutterStatus")]
        public List<XmlCutterStatus> CutterStatus { get; set; }

        /// <summary>
        /// The number of times the tool has been reconditioned.
        /// </summary>
        [XmlElement("ReconditionCount")]
        public XmlReconditionCount ReconditionCount { get; set; }

        /// <summary>
        /// The accumulated and remaining life of the tool.
        /// </summary>
        [XmlElement("ToolLife")]
        public List<XmlToolLife> ToolLife { get; set; }

        /// <summary>
        /// Where the tool currently resides in a tool-handling system.
        /// </summary>
        [XmlElement("Location")]
        public XmlLocation Location { get; set; }

        /// <summary>
        /// The identifier of the tool group the program refers to the tool by.
        /// </summary>
        [XmlElement("ProgramToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// The number the program refers to the tool by.
        /// </summary>
        [XmlElement("ProgramToolNumber")]
        public string ProgramToolNumber { get; set; }

        /// <summary>
        /// The connection code identifying the machine-side interface of the
        /// tool.
        /// </summary>
        [XmlElement("ConnectionCodeMachineSide")]
        public string ConnectionCodeMachineSide { get; set; }

        /// <summary>
        /// The spindle speed limits the tool is expected to operate within.
        /// </summary>
        [XmlElement("ProcessSpindleSpeed")]
        public XmlProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        /// <summary>
        /// The feed rate limits the tool is expected to operate within.
        /// </summary>
        [XmlElement("ProcessFeedRate")]
        public XmlProcessFeedRate ProcessFeedRate { get; set; }

        /// <summary>
        /// The individual cutting items (edges) the tool comprises.
        /// </summary>
        [XmlElement("CuttingItems")]
        public XmlCuttingItems CuttingItems { get; set; }


        /// <summary>
        /// Converts this surrogate into the strongly-typed
        /// <see cref="ICuttingToolLifeCycle"/>, projecting each nested
        /// collection into its model representation.
        /// </summary>
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

        /// <summary>
        /// Writes the given <see cref="ICuttingToolLifeCycle"/> to
        /// <paramref name="writer"/> as a <c>CuttingToolLifeCycle</c> element,
        /// emitting only the properties and collections that are present.
        /// </summary>
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