// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using MTConnect.Assets.CuttingTools.Measurements;

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// A CuttingItem is the portion of the tool that physically removes the material from the workpiece by shear deformation.
    /// </summary>
    public class CuttingItem
    {
        /// <summary>
        /// The number or numbers representing the individual cutting item or items on the tool.
        /// </summary>
        [XmlAttribute("indices")]
        [JsonPropertyName("indices")]
        public int Indices { get; set; }

        /// <summary>
        /// The manufacturer identifier of this cutting item
        /// </summary>
        [XmlAttribute("itemId")]
        [JsonPropertyName("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// The manufacturers of the cutting item
        /// </summary>
        [XmlAttribute("manufacturers")]
        [JsonPropertyName("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// The material composition for this cutting item
        /// </summary>
        [XmlAttribute("grade")]
        [JsonPropertyName("grade")]
        public string Grade { get; set; }

        /// <summary>
        /// A free-form description of the cutting item.
        /// </summary>
        [XmlElement("Description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }


        /// <summary>
        /// A free form description of the location on the cutting tool.
        /// </summary>
        [XmlElement("Locus")]
        [JsonPropertyName("locus")]
        public string Locus { get; set; }

        /// <summary>
        /// The life of this cutting item.
        /// </summary>
        [XmlElement("ItemLife")]
        [JsonPropertyName("itemLife")]
        public List<ItemLife> ItemLife { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool ItemLifeSpecified => !ItemLife.IsNullOrEmpty();

        /// <summary>
        /// The status of the this cutting item. 
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
        /// The tool group the part program assigned this item.      
        /// </summary>
        [XmlElement("ProgramToolGroup")]
        [JsonPropertyName("programToolGroup")]
        public string ProgramToolGroup { get; set; }

        /// <summary>
        /// A collection of measurements relating to this cutting item.
        /// </summary>
        [XmlArray("Measurements")]
        [XmlArrayItem(ChamferFlatLengthMeasurement.TypeId, typeof(ChamferFlatLengthMeasurement))]
        [XmlArrayItem(ChamferWidthMeasurement.TypeId, typeof(ChamferWidthMeasurement))]
        [XmlArrayItem(CornerRadiusMeasurement.TypeId, typeof(CornerRadiusMeasurement))]
        [XmlArrayItem(CuttingDiameterMeasurement.TypeId, typeof(CuttingDiameterMeasurement))]
        [XmlArrayItem(CuttingEdgeLengthMeasurement.TypeId, typeof(CuttingEdgeLengthMeasurement))]
        [XmlArrayItem(CuttingHeightMeasurement.TypeId, typeof(CuttingHeightMeasurement))]
        [XmlArrayItem(CuttingReferencePointMeasurement.TypeId, typeof(CuttingReferencePointMeasurement))]
        [XmlArrayItem(DriveAngleMeasurement.TypeId, typeof(DriveAngleMeasurement))]
        [XmlArrayItem(FlangeDiameterMeasurement.TypeId, typeof(FlangeDiameterMeasurement))]
        [XmlArrayItem(FunctionalLengthMeasurement.TypeId, typeof(FunctionalLengthMeasurement))]
        [XmlArrayItem(FunctionalWidthMeasurement.TypeId, typeof(FunctionalWidthMeasurement))]
        [XmlArrayItem(InscribedCircleDiameterMeasurement.TypeId, typeof(InscribedCircleDiameterMeasurement))]
        [XmlArrayItem(InsertWidthMeasurement.TypeId, typeof(InsertWidthMeasurement))]
        [XmlArrayItem(PointAngleMeasurement.TypeId, typeof(PointAngleMeasurement))]
        [XmlArrayItem(StepDiameterLengthMeasurement.TypeId, typeof(StepDiameterLengthMeasurement))]
        [XmlArrayItem(StepIncludedAngleMeasurement.TypeId, typeof(StepIncludedAngleMeasurement))]
        [XmlArrayItem(ToolCuttingEdgeAngleMeasurement.TypeId, typeof(ToolCuttingEdgeAngleMeasurement))]
        [XmlArrayItem(ToolLeadAngleMeasurement.TypeId, typeof(ToolLeadAngleMeasurement))]
        [XmlArrayItem(ToolOrientationMeasurement.TypeId, typeof(ToolOrientationMeasurement))]
        [XmlArrayItem(WeightMeasurement.TypeId, typeof(WeightMeasurement))]
        [XmlArrayItem(WiperEdgeLengthMeasurement.TypeId, typeof(WiperEdgeLengthMeasurement))]
        [JsonPropertyName("measurements")]
        public List<Measurement> Measurements { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool MeasurementsSpecified => !Measurements.IsNullOrEmpty();


        public CuttingItem()
        {
            CutterStatus = new List<CutterStatus>();
            Measurements = new List<Measurement>();
        }


        public CuttingItem Process()
        {
            var cuttingItem = new CuttingItem();
            cuttingItem.Indices = Indices;
            cuttingItem.ItemId = ItemId;
            cuttingItem.Manufacturers = Manufacturers;
            cuttingItem.Grade = Grade;
            cuttingItem.Description = Description;
            cuttingItem.Locus = Locus;
            cuttingItem.ItemLife = ItemLife;
            cuttingItem.CutterStatus = CutterStatus;
            cuttingItem.ProgramToolGroup = ProgramToolGroup;

            if (!Measurements.IsNullOrEmpty())
            {
                var measurements = new List<Measurement>();
                foreach (var measurement in Measurements)
                {
                    var typeMeasurement = Measurement.Create(measurement.Type, measurement);
                    if (typeMeasurement != null) measurements.Add(typeMeasurement);
                }
                cuttingItem.Measurements = measurements;
            }

            return cuttingItem;
        }
    }
}
