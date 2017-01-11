// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.MTConnectAssets.CuttingTools
{
    public class CuttingToolLifeCycle
    {
        #region "Required"

        /// <summary>
        /// The status of the this assembly. 
        /// Can be one more of the following values: NEW, AVAILABLE, UNAVAILABLE, ALLOCATED, UNALLOCATED, MEASURED, RECONDITIONED, NOT_REGISTERED, USED, EXPIRED, BROKEN, or UNKNOWN.
        /// </summary>
        [XmlArray("CutterStatus")]
        [XmlArrayItem("Status", typeof(CutterStatus))]
        public List<CutterStatus> CutterStatus { get; set; }

        #endregion

        #region "Optional"

        /// <summary>
        /// The number of times this cutter has been reconditioned.
        /// </summary>
        [XmlElement("ReconditionCount")]
        public ReconditionCount ReconditionCount { get; set; }

        /// <summary>
        /// The cutting tool life as related to this assembly
        /// </summary>
        [XmlElement("ToolLife")]
        public ToolLife ToolLife { get; set; }

        /// <summary>
        /// The location this tool now resides in.
        /// </summary>
        [XmlElement("Location")]
        public Location Location { get; set; }

        /// <summary>
        /// The tool group this tool is assigned in the part program.
        /// </summary>
        [XmlElement("ProgramToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// The number of the tool as referenced in the part program.
        /// </summary>
        [XmlElement("ProgramToolNumber")]
        public string ProgramToolNumber { get; set; }

        /// <summary>
        /// The constrained process spindle speed for this tool
        /// </summary>
        [XmlElement("ProcessSpindleSpeed")]
        public ProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        /// <summary>
        /// The constrained process feed rate for this tool in mm/s.
        /// </summary>
        [XmlElement("ProcessFeedrate")]
        public ProcessFeedrate ProcessFeedrate { get; set; }

        /// <summary>
        /// Identifier for the capability to connect any component of the cutting tool together, except assembly items, on the machine side. Code: CCMS
        /// </summary>
        [XmlElement("ConnectionCodeMachineSide")]
        public string ConnectionCodeMachineSide { get; set; }

        /// <summary>
        /// A collection of measurements for the tool assembly.
        /// </summary>
        [XmlArray("Measurements")]
        [XmlArrayItem("Measurement", typeof(Measurement))]
        public List<Measurement> Measurements { get; set; }

        /// <summary>
        /// An optional set of individual cutting items.
        /// </summary>
        [XmlArray("CuttingItems")]
        [XmlArrayItem("CuttingItem", typeof(CuttingItem))]
        public List<CuttingItem> CuttingItems { get; set; }

        #endregion
    }
}
