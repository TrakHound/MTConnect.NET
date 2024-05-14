// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    public partial class XmlCuttingItem
    {
        [XmlArray("Measurements")]
        [XmlArrayItem(BodyDiameterMaxMeasurement.TypeId, typeof(XmlBodyDiameterMaxMeasurement))]
        [XmlArrayItem(BodyLengthMaxMeasurement.TypeId, typeof(XmlBodyLengthMaxMeasurement))]
        [XmlArrayItem(ChamferFlatLengthMeasurement.TypeId, typeof(XmlChamferFlatLengthMeasurement))]
        [XmlArrayItem(ChamferWidthMeasurement.TypeId, typeof(XmlChamferWidthMeasurement))]
        [XmlArrayItem(CornerRadiusMeasurement.TypeId, typeof(XmlCornerRadiusMeasurement))]
        [XmlArrayItem(CuttingDiameterMaxMeasurement.TypeId, typeof(XmlCuttingDiameterMaxMeasurement))]
        [XmlArrayItem(CuttingDiameterMeasurement.TypeId, typeof(XmlCuttingDiameterMeasurement))]
        [XmlArrayItem(CuttingEdgeLengthMeasurement.TypeId, typeof(XmlCuttingEdgeLengthMeasurement))]
        [XmlArrayItem(CuttingHeightMeasurement.TypeId, typeof(XmlCuttingHeightMeasurement))]
        [XmlArrayItem(CuttingReferencePointMeasurement.TypeId, typeof(XmlCuttingReferencePointMeasurement))]
        [XmlArrayItem(DepthOfCutMaxMeasurement.TypeId, typeof(XmlDepthOfCutMaxMeasurement))]
        [XmlArrayItem(DriveAngleMeasurement.TypeId, typeof(XmlDriveAngleMeasurement))]
        [XmlArrayItem(FlangeDiameterMaxMeasurement.TypeId, typeof(XmlFlangeDiameterMaxMeasurement))]
        [XmlArrayItem(FlangeDiameterMeasurement.TypeId, typeof(XmlFlangeDiameterMeasurement))]
        [XmlArrayItem(FunctionalLengthMeasurement.TypeId, typeof(XmlFunctionalLengthMeasurement))]
        [XmlArrayItem(FunctionalWidthMeasurement.TypeId, typeof(XmlFunctionalWidthMeasurement))]
        [XmlArrayItem(IncribedCircleDiameterMeasurement.TypeId, typeof(XmlIncribedCircleDiameterMeasurement))]
        [XmlArrayItem(InsertWidthMeasurement.TypeId, typeof(XmlInsertWidthMeasurement))]
        [XmlArrayItem(OverallToolLengthMeasurement.TypeId, typeof(XmlOverallToolLengthMeasurement))]
        [XmlArrayItem(PointAngleMeasurement.TypeId, typeof(XmlPointAngleMeasurement))]
        [XmlArrayItem(ProtrudingLengthMeasurement.TypeId, typeof(XmlProtrudingLengthMeasurement))]
        [XmlArrayItem(ShankDiameterMeasurement.TypeId, typeof(XmlShankDiameterMeasurement))]
        [XmlArrayItem(ShankHeightMeasurement.TypeId, typeof(XmlShankHeightMeasurement))]
        [XmlArrayItem(ShankLengthMeasurement.TypeId, typeof(XmlShankLengthMeasurement))]
        [XmlArrayItem(StepDiameterLengthMeasurement.TypeId, typeof(XmlStepDiameterLengthMeasurement))]
        [XmlArrayItem(StepIncludedAngleMeasurement.TypeId, typeof(XmlStepIncludedAngleMeasurement))]
        [XmlArrayItem(ToolCuttingEdgeAngleMeasurement.TypeId, typeof(XmlToolCuttingEdgeAngleMeasurement))]
        [XmlArrayItem(ToolLeadAngleMeasurement.TypeId, typeof(XmlToolLeadAngleMeasurement))]
        [XmlArrayItem(ToolOrientationMeasurement.TypeId, typeof(XmlToolOrientationMeasurement))]
        [XmlArrayItem(UsableLengthMaxMeasurement.TypeId, typeof(XmlUsableLengthMaxMeasurement))]
        [XmlArrayItem(WeightMeasurement.TypeId, typeof(XmlWeightMeasurement))]
        [XmlArrayItem(WiperEdgeLengthMeasurement.TypeId, typeof(XmlWiperEdgeLengthMeasurement))]
        public List<XmlMeasurement> Measurements { get; set; }
    }
}