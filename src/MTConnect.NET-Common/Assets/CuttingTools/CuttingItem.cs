// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using MTConnect.Assets.CuttingTools.Measurements;
using MTConnect.Assets.CuttingTools.Measurements.CuttingItem;

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
        public int Indices { get; set; }

        /// <summary>
        /// The manufacturer identifier of this cutting item
        /// </summary>
        [XmlAttribute("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// The manufacturers of the cutting item
        /// </summary>
        [XmlAttribute("manufacturers")]
        public string Manufacturers { get; set; }

        /// <summary>
        /// The material composition for this cutting item
        /// </summary>
        [XmlAttribute("grade")]
        public string Grade { get; set; }

        /// <summary>
        /// A free-form description of the cutting item.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }


        /// <summary>
        /// A free form description of the location on the cutting tool.
        /// </summary>
        [XmlElement("Locus")]
        public string Locus { get; set; }

        /// <summary>
        /// The life of this cutting item.
        /// </summary>
        [XmlElement("ItemLife")]
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
        public List<CutterStatus> CutterStatus { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool CutterStatusSpecified => !CutterStatus.IsNullOrEmpty();

        /// <summary>
        /// The tool group the part program assigned this item.      
        /// </summary>
        [XmlElement("ProgramToolGroup")]
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
        public List<Measurement> Measurements { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool MeasurementsSpecified => !Measurements.IsNullOrEmpty();


        public CuttingItem()
        {
            CutterStatus = new List<CutterStatus>();
            Measurements = new List<Measurement>();
        }
    }
}
