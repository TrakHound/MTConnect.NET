// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using MTConnect.Assets.CuttingTools.Measurements;

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
        [JsonPropertyName("cutterStatus")]
        public List<CutterStatus> CutterStatus { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool CutterStatusSpecified => !CutterStatus.IsNullOrEmpty();

        /// <summary>
        /// The number of times this cutter has been reconditioned.
        /// </summary>
        [XmlElement("ReconditionCount")]
        [JsonPropertyName("reconditionCount")]
        public ReconditionCount ReconditionCount { get; set; }

        /// <summary>
        /// The cutting tool life as related to this assembly
        /// </summary>
        [XmlElement("ToolLife")]
        [JsonPropertyName("toolLife")]
        public ToolLife ToolLife { get; set; }

        /// <summary>
        /// The location this tool now resides in.
        /// </summary>
        [XmlElement("Location")]
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        /// <summary>
        /// The tool group this tool is assigned in the part program.
        /// </summary>
        [XmlElement("ProgramToolGroup")]
        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// The number of the tool as referenced in the part program.
        /// </summary>
        [XmlElement("ProgramToolNumber")]
        [JsonPropertyName("programToolNumber")]
        public string ProgramToolNumber { get; set; }

        /// <summary>
        /// The constrained process spindle speed for this tool
        /// </summary>
        [XmlElement("ProcessSpindleSpeed")]
        [JsonPropertyName("processSpindleSpeed")]
        public ProcessSpindleSpeed ProcessSpindleSpeed { get; set; }

        /// <summary>
        /// The constrained process feed rate for this tool in mm/s.
        /// </summary>
        [XmlElement("ProcessFeedrate")]
        [JsonPropertyName("processFeedrate")]
        public ProcessFeedrate ProcessFeedrate { get; set; }

        /// <summary>
        /// Identifier for the capability to connect any component of the cutting tool together, except assembly items, on the machine side. Code: CCMS
        /// </summary>
        [XmlElement("ConnectionCodeMachineSide")]
        [JsonPropertyName("connectionCodeMachineSide")]
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
        [JsonPropertyName("measurements")]
        public List<Measurement> Measurements { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool MeasurementsSpecified => !Measurements.IsNullOrEmpty();

        /// <summary>
        /// An optional set of individual cutting items.
        /// </summary>
        [XmlElement("CuttingItems")]
        [JsonPropertyName("cuttingItems")]
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


        public CuttingToolLifeCycle Process()
        {
            var lifeCycle = new CuttingToolLifeCycle();
            lifeCycle.CutterStatus = CutterStatus;
            lifeCycle.ReconditionCount = ReconditionCount;
            lifeCycle.ToolLife = ToolLife;
            lifeCycle.Location = Location;
            lifeCycle.ProgramToolGroup = ProgramToolGroup;
            lifeCycle.ProgramToolNumber = ProgramToolNumber;
            lifeCycle.ProcessSpindleSpeed = ProcessSpindleSpeed;
            lifeCycle.ProcessFeedrate = ProcessFeedrate;
            lifeCycle.ConnectionCodeMachineSide = ConnectionCodeMachineSide;
            lifeCycle.CuttingItems = CuttingItems;

            // Process Cutting Items
            if (CuttingItems != null && !CuttingItems.CuttingItems.IsNullOrEmpty() && CuttingItems.Count > 0)
            {
                var cuttingItems = new CuttingItemCollection();
                foreach (var cuttingItem in CuttingItems.CuttingItems)
                {
                    var processedCuttingItem = cuttingItem.Process();
                    if (processedCuttingItem != null) cuttingItems.Add(processedCuttingItem);
                }
                lifeCycle.CuttingItems = cuttingItems;
            }

            // Process Measurements
            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<Measurement>();
                foreach (var measurement in Measurements)
                {
                    var typeMeasurement = Measurement.Create(measurement.Type, measurement);
                    if (typeMeasurement != null) measurements.Add(typeMeasurement);
                }
                lifeCycle.Measurements = measurements;
            }

            return lifeCycle;
        }
    }
}