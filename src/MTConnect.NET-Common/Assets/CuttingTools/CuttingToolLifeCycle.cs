// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using MTConnect.Assets.CuttingTools.Measurements;
using MTConnect.Assets.CuttingTools.Measurements.Assembly;

namespace MTConnect.Assets.CuttingTools
{
    public class CuttingToolLifeCycle
    {
        /// <summary>
        /// The status of the this assembly. 
        /// Can be one more of the following values: NEW, AVAILABLE, UNAVAILABLE, ALLOCATED, UNALLOCATED, MEASURED, RECONDITIONED, NOT_REGISTERED, USED, EXPIRED, BROKEN, or UNKNOWN.
        /// </summary>
        [XmlArray("CutterStatus")]
        [XmlArrayItem("Status", typeof(CutterStatus))]
        public List<CutterStatus> CutterStatus { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool CutterStatusSpecified => !CutterStatus.IsNullOrEmpty();

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
        [XmlArrayItem(BodyDiameterMaxMeasurement.TypeId, typeof(BodyDiameterMaxMeasurement))]
        [XmlArrayItem(BodyLengthMaxMeasurement.TypeId, typeof(BodyLengthMaxMeasurement))]
        [XmlArrayItem(DepthOfCutMaxMeasurement.TypeId, typeof(DepthOfCutMaxMeasurement))]
        [XmlArrayItem(CuttingDiameterMaxMeasurement.TypeId, typeof(CuttingDiameterMaxMeasurement))]
        [XmlArrayItem(FlangeDiameterMaxMeasurement.TypeId, typeof(FlangeDiameterMaxMeasurement))]
        [XmlArrayItem(OverallToolLengthMeasurement.TypeId, typeof(OverallToolLengthMeasurement))]
        [XmlArrayItem(ShankDiameterMeasurement.TypeId, typeof(ShankDiameterMeasurement))]
        [XmlArrayItem(ShankHeightMeasurement.TypeId, typeof(ShankHeightMeasurement))]
        [XmlArrayItem(ShankLengthMeasurement.TypeId, typeof(ShankLengthMeasurement))]
        [XmlArrayItem(UsableLengthMaxMeasurement.TypeId, typeof(UsableLengthMaxMeasurement))]
        [XmlArrayItem(ProtrudingLengthMeasurement.TypeId, typeof(ProtrudingLengthMeasurement))]
        [XmlArrayItem(WeightMeasurement.TypeId, typeof(WeightMeasurement))]
        [XmlArrayItem(FunctionalLengthMeasurement.TypeId, typeof(FunctionalLengthMeasurement))]
        public List<Measurement> Measurements { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool MeasurementsSpecified => !Measurements.IsNullOrEmpty();

        /// <summary>
        /// An optional set of individual cutting items.
        /// </summary>
        [XmlElement("CuttingItems")]
        public CuttingItemCollection CuttingItems { get; set; }

        //[XmlIgnore]
        //[JsonIgnore]
        //public bool CuttingItemsSpecified => !CuttingItems.CuttingItems.IsNullOrEmpty();

        [XmlIgnore]
        [JsonIgnore]
        public bool IsAvailable => CutterStatus.Contains(CuttingTools.CutterStatus.AVAILABLE);

        [XmlIgnore]
        [JsonIgnore]
        public bool IsAllocated => CutterStatus.Contains(CuttingTools.CutterStatus.ALLOCATED);

        [XmlIgnore]
        [JsonIgnore]
        public bool IsBroken => CutterStatus.Contains(CuttingTools.CutterStatus.BROKEN);

        [XmlIgnore]
        [JsonIgnore]
        public bool IsExpired => CutterStatus.Contains(CuttingTools.CutterStatus.EXPIRED);

        [XmlIgnore]
        [JsonIgnore]
        public bool IsMeasured => CutterStatus.Contains(CuttingTools.CutterStatus.MEASURED);

        [XmlIgnore]
        [JsonIgnore]
        public bool IsNew => CutterStatus.Contains(CuttingTools.CutterStatus.NEW);

        [XmlIgnore]
        [JsonIgnore]
        public bool IsUsed => CutterStatus.Contains(CuttingTools.CutterStatus.USED);


        public CuttingToolLifeCycle()
        {
            CutterStatus = new List<CutterStatus>();
            Measurements = new List<Measurement>();
            CuttingItems = new CuttingItemCollection();
        }
    }
}
