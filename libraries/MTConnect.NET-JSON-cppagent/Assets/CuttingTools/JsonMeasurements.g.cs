// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    public class JsonMeasurements
    {
        [JsonPropertyName("BodyDiameterMax")]
        public List<JsonMeasurement> BodyDiameterMaxMeasurements { get; set; }


        [JsonPropertyName("BodyLengthMax")]
        public List<JsonMeasurement> BodyLengthMaxMeasurements { get; set; }


        [JsonPropertyName("ChamferFlatLength")]
        public List<JsonMeasurement> ChamferFlatLengthMeasurements { get; set; }


        [JsonPropertyName("ChamferWidth")]
        public List<JsonMeasurement> ChamferWidthMeasurements { get; set; }


        [JsonPropertyName("CornerRadius")]
        public List<JsonMeasurement> CornerRadiusMeasurements { get; set; }


        [JsonPropertyName("CuttingDiameterMax")]
        public List<JsonMeasurement> CuttingDiameterMaxMeasurements { get; set; }


        [JsonPropertyName("CuttingDiameter")]
        public List<JsonMeasurement> CuttingDiameterMeasurements { get; set; }


        [JsonPropertyName("CuttingEdgeLength")]
        public List<JsonMeasurement> CuttingEdgeLengthMeasurements { get; set; }


        [JsonPropertyName("CuttingHeight")]
        public List<JsonMeasurement> CuttingHeightMeasurements { get; set; }


        [JsonPropertyName("CuttingReferencePoint")]
        public List<JsonMeasurement> CuttingReferencePointMeasurements { get; set; }


        [JsonPropertyName("DepthOfCutMax")]
        public List<JsonMeasurement> DepthOfCutMaxMeasurements { get; set; }


        [JsonPropertyName("DriveAngle")]
        public List<JsonMeasurement> DriveAngleMeasurements { get; set; }


        [JsonPropertyName("FlangeDiameterMax")]
        public List<JsonMeasurement> FlangeDiameterMaxMeasurements { get; set; }


        [JsonPropertyName("FlangeDiameter")]
        public List<JsonMeasurement> FlangeDiameterMeasurements { get; set; }


        [JsonPropertyName("FunctionalLength")]
        public List<JsonMeasurement> FunctionalLengthMeasurements { get; set; }


        [JsonPropertyName("FunctionalWidth")]
        public List<JsonMeasurement> FunctionalWidthMeasurements { get; set; }


        [JsonPropertyName("IncribedCircleDiameter")]
        public List<JsonMeasurement> IncribedCircleDiameterMeasurements { get; set; }


        [JsonPropertyName("InsertWidth")]
        public List<JsonMeasurement> InsertWidthMeasurements { get; set; }


        [JsonPropertyName("OverallToolLength")]
        public List<JsonMeasurement> OverallToolLengthMeasurements { get; set; }


        [JsonPropertyName("PointAngle")]
        public List<JsonMeasurement> PointAngleMeasurements { get; set; }


        [JsonPropertyName("ProtrudingLength")]
        public List<JsonMeasurement> ProtrudingLengthMeasurements { get; set; }


        [JsonPropertyName("ShankDiameter")]
        public List<JsonMeasurement> ShankDiameterMeasurements { get; set; }


        [JsonPropertyName("ShankHeight")]
        public List<JsonMeasurement> ShankHeightMeasurements { get; set; }


        [JsonPropertyName("ShankLength")]
        public List<JsonMeasurement> ShankLengthMeasurements { get; set; }


        [JsonPropertyName("StepDiameterLength")]
        public List<JsonMeasurement> StepDiameterLengthMeasurements { get; set; }


        [JsonPropertyName("StepIncludedAngle")]
        public List<JsonMeasurement> StepIncludedAngleMeasurements { get; set; }


        [JsonPropertyName("ToolCuttingEdgeAngle")]
        public List<JsonMeasurement> ToolCuttingEdgeAngleMeasurements { get; set; }


        [JsonPropertyName("ToolLeadAngle")]
        public List<JsonMeasurement> ToolLeadAngleMeasurements { get; set; }


        [JsonPropertyName("ToolOrientation")]
        public List<JsonMeasurement> ToolOrientationMeasurements { get; set; }


        [JsonPropertyName("UsableLengthMax")]
        public List<JsonMeasurement> UsableLengthMaxMeasurements { get; set; }


        [JsonPropertyName("Weight")]
        public List<JsonMeasurement> WeightMeasurements { get; set; }


        [JsonPropertyName("WiperEdgeLength")]
        public List<JsonMeasurement> WiperEdgeLengthMeasurements { get; set; }



        public JsonMeasurements() { }

        public JsonMeasurements(IEnumerable<IToolingMeasurement> measurements)
        {
            if (!measurements.IsNullOrEmpty())
            {
                foreach (var measurement in measurements)
                {
                    switch (measurement.Type)
                    {
                        case BodyDiameterMaxMeasurement.TypeId:
                            if (BodyDiameterMaxMeasurements == null) BodyDiameterMaxMeasurements = new List<JsonMeasurement>();
                            BodyDiameterMaxMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case BodyLengthMaxMeasurement.TypeId:
                            if (BodyLengthMaxMeasurements == null) BodyLengthMaxMeasurements = new List<JsonMeasurement>();
                            BodyLengthMaxMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ChamferFlatLengthMeasurement.TypeId:
                            if (ChamferFlatLengthMeasurements == null) ChamferFlatLengthMeasurements = new List<JsonMeasurement>();
                            ChamferFlatLengthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ChamferWidthMeasurement.TypeId:
                            if (ChamferWidthMeasurements == null) ChamferWidthMeasurements = new List<JsonMeasurement>();
                            ChamferWidthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case CornerRadiusMeasurement.TypeId:
                            if (CornerRadiusMeasurements == null) CornerRadiusMeasurements = new List<JsonMeasurement>();
                            CornerRadiusMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case CuttingDiameterMaxMeasurement.TypeId:
                            if (CuttingDiameterMaxMeasurements == null) CuttingDiameterMaxMeasurements = new List<JsonMeasurement>();
                            CuttingDiameterMaxMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case CuttingDiameterMeasurement.TypeId:
                            if (CuttingDiameterMeasurements == null) CuttingDiameterMeasurements = new List<JsonMeasurement>();
                            CuttingDiameterMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case CuttingEdgeLengthMeasurement.TypeId:
                            if (CuttingEdgeLengthMeasurements == null) CuttingEdgeLengthMeasurements = new List<JsonMeasurement>();
                            CuttingEdgeLengthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case CuttingHeightMeasurement.TypeId:
                            if (CuttingHeightMeasurements == null) CuttingHeightMeasurements = new List<JsonMeasurement>();
                            CuttingHeightMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case CuttingReferencePointMeasurement.TypeId:
                            if (CuttingReferencePointMeasurements == null) CuttingReferencePointMeasurements = new List<JsonMeasurement>();
                            CuttingReferencePointMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case DepthOfCutMaxMeasurement.TypeId:
                            if (DepthOfCutMaxMeasurements == null) DepthOfCutMaxMeasurements = new List<JsonMeasurement>();
                            DepthOfCutMaxMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case DriveAngleMeasurement.TypeId:
                            if (DriveAngleMeasurements == null) DriveAngleMeasurements = new List<JsonMeasurement>();
                            DriveAngleMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case FlangeDiameterMaxMeasurement.TypeId:
                            if (FlangeDiameterMaxMeasurements == null) FlangeDiameterMaxMeasurements = new List<JsonMeasurement>();
                            FlangeDiameterMaxMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case FlangeDiameterMeasurement.TypeId:
                            if (FlangeDiameterMeasurements == null) FlangeDiameterMeasurements = new List<JsonMeasurement>();
                            FlangeDiameterMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case FunctionalLengthMeasurement.TypeId:
                            if (FunctionalLengthMeasurements == null) FunctionalLengthMeasurements = new List<JsonMeasurement>();
                            FunctionalLengthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case FunctionalWidthMeasurement.TypeId:
                            if (FunctionalWidthMeasurements == null) FunctionalWidthMeasurements = new List<JsonMeasurement>();
                            FunctionalWidthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case IncribedCircleDiameterMeasurement.TypeId:
                            if (IncribedCircleDiameterMeasurements == null) IncribedCircleDiameterMeasurements = new List<JsonMeasurement>();
                            IncribedCircleDiameterMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case InsertWidthMeasurement.TypeId:
                            if (InsertWidthMeasurements == null) InsertWidthMeasurements = new List<JsonMeasurement>();
                            InsertWidthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case OverallToolLengthMeasurement.TypeId:
                            if (OverallToolLengthMeasurements == null) OverallToolLengthMeasurements = new List<JsonMeasurement>();
                            OverallToolLengthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case PointAngleMeasurement.TypeId:
                            if (PointAngleMeasurements == null) PointAngleMeasurements = new List<JsonMeasurement>();
                            PointAngleMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ProtrudingLengthMeasurement.TypeId:
                            if (ProtrudingLengthMeasurements == null) ProtrudingLengthMeasurements = new List<JsonMeasurement>();
                            ProtrudingLengthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ShankDiameterMeasurement.TypeId:
                            if (ShankDiameterMeasurements == null) ShankDiameterMeasurements = new List<JsonMeasurement>();
                            ShankDiameterMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ShankHeightMeasurement.TypeId:
                            if (ShankHeightMeasurements == null) ShankHeightMeasurements = new List<JsonMeasurement>();
                            ShankHeightMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ShankLengthMeasurement.TypeId:
                            if (ShankLengthMeasurements == null) ShankLengthMeasurements = new List<JsonMeasurement>();
                            ShankLengthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case StepDiameterLengthMeasurement.TypeId:
                            if (StepDiameterLengthMeasurements == null) StepDiameterLengthMeasurements = new List<JsonMeasurement>();
                            StepDiameterLengthMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case StepIncludedAngleMeasurement.TypeId:
                            if (StepIncludedAngleMeasurements == null) StepIncludedAngleMeasurements = new List<JsonMeasurement>();
                            StepIncludedAngleMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ToolCuttingEdgeAngleMeasurement.TypeId:
                            if (ToolCuttingEdgeAngleMeasurements == null) ToolCuttingEdgeAngleMeasurements = new List<JsonMeasurement>();
                            ToolCuttingEdgeAngleMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ToolLeadAngleMeasurement.TypeId:
                            if (ToolLeadAngleMeasurements == null) ToolLeadAngleMeasurements = new List<JsonMeasurement>();
                            ToolLeadAngleMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case ToolOrientationMeasurement.TypeId:
                            if (ToolOrientationMeasurements == null) ToolOrientationMeasurements = new List<JsonMeasurement>();
                            ToolOrientationMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case UsableLengthMaxMeasurement.TypeId:
                            if (UsableLengthMaxMeasurements == null) UsableLengthMaxMeasurements = new List<JsonMeasurement>();
                            UsableLengthMaxMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case WeightMeasurement.TypeId:
                            if (WeightMeasurements == null) WeightMeasurements = new List<JsonMeasurement>();
                            WeightMeasurements.Add(new JsonMeasurement(measurement));
                            break;


                        case WiperEdgeLengthMeasurement.TypeId:
                            if (WiperEdgeLengthMeasurements == null) WiperEdgeLengthMeasurements = new List<JsonMeasurement>();
                            WiperEdgeLengthMeasurements.Add(new JsonMeasurement(measurement));
                            break;

                    
                    }            
                }
            }     
        }


        public IEnumerable<IToolingMeasurement> ToMeasurements()
        {
            var measurements = new List<IToolingMeasurement>();
        

            if (!BodyDiameterMaxMeasurements.IsNullOrEmpty()) foreach (var measurement in BodyDiameterMaxMeasurements) measurements.Add(measurement.ToMeasurement(BodyDiameterMaxMeasurement.TypeId));
            if (!BodyLengthMaxMeasurements.IsNullOrEmpty()) foreach (var measurement in BodyLengthMaxMeasurements) measurements.Add(measurement.ToMeasurement(BodyLengthMaxMeasurement.TypeId));
            if (!ChamferFlatLengthMeasurements.IsNullOrEmpty()) foreach (var measurement in ChamferFlatLengthMeasurements) measurements.Add(measurement.ToMeasurement(ChamferFlatLengthMeasurement.TypeId));
            if (!ChamferWidthMeasurements.IsNullOrEmpty()) foreach (var measurement in ChamferWidthMeasurements) measurements.Add(measurement.ToMeasurement(ChamferWidthMeasurement.TypeId));
            if (!CornerRadiusMeasurements.IsNullOrEmpty()) foreach (var measurement in CornerRadiusMeasurements) measurements.Add(measurement.ToMeasurement(CornerRadiusMeasurement.TypeId));
            if (!CuttingDiameterMaxMeasurements.IsNullOrEmpty()) foreach (var measurement in CuttingDiameterMaxMeasurements) measurements.Add(measurement.ToMeasurement(CuttingDiameterMaxMeasurement.TypeId));
            if (!CuttingDiameterMeasurements.IsNullOrEmpty()) foreach (var measurement in CuttingDiameterMeasurements) measurements.Add(measurement.ToMeasurement(CuttingDiameterMeasurement.TypeId));
            if (!CuttingEdgeLengthMeasurements.IsNullOrEmpty()) foreach (var measurement in CuttingEdgeLengthMeasurements) measurements.Add(measurement.ToMeasurement(CuttingEdgeLengthMeasurement.TypeId));
            if (!CuttingHeightMeasurements.IsNullOrEmpty()) foreach (var measurement in CuttingHeightMeasurements) measurements.Add(measurement.ToMeasurement(CuttingHeightMeasurement.TypeId));
            if (!CuttingReferencePointMeasurements.IsNullOrEmpty()) foreach (var measurement in CuttingReferencePointMeasurements) measurements.Add(measurement.ToMeasurement(CuttingReferencePointMeasurement.TypeId));
            if (!DepthOfCutMaxMeasurements.IsNullOrEmpty()) foreach (var measurement in DepthOfCutMaxMeasurements) measurements.Add(measurement.ToMeasurement(DepthOfCutMaxMeasurement.TypeId));
            if (!DriveAngleMeasurements.IsNullOrEmpty()) foreach (var measurement in DriveAngleMeasurements) measurements.Add(measurement.ToMeasurement(DriveAngleMeasurement.TypeId));
            if (!FlangeDiameterMaxMeasurements.IsNullOrEmpty()) foreach (var measurement in FlangeDiameterMaxMeasurements) measurements.Add(measurement.ToMeasurement(FlangeDiameterMaxMeasurement.TypeId));
            if (!FlangeDiameterMeasurements.IsNullOrEmpty()) foreach (var measurement in FlangeDiameterMeasurements) measurements.Add(measurement.ToMeasurement(FlangeDiameterMeasurement.TypeId));
            if (!FunctionalLengthMeasurements.IsNullOrEmpty()) foreach (var measurement in FunctionalLengthMeasurements) measurements.Add(measurement.ToMeasurement(FunctionalLengthMeasurement.TypeId));
            if (!FunctionalWidthMeasurements.IsNullOrEmpty()) foreach (var measurement in FunctionalWidthMeasurements) measurements.Add(measurement.ToMeasurement(FunctionalWidthMeasurement.TypeId));
            if (!IncribedCircleDiameterMeasurements.IsNullOrEmpty()) foreach (var measurement in IncribedCircleDiameterMeasurements) measurements.Add(measurement.ToMeasurement(IncribedCircleDiameterMeasurement.TypeId));
            if (!InsertWidthMeasurements.IsNullOrEmpty()) foreach (var measurement in InsertWidthMeasurements) measurements.Add(measurement.ToMeasurement(InsertWidthMeasurement.TypeId));
            if (!OverallToolLengthMeasurements.IsNullOrEmpty()) foreach (var measurement in OverallToolLengthMeasurements) measurements.Add(measurement.ToMeasurement(OverallToolLengthMeasurement.TypeId));
            if (!PointAngleMeasurements.IsNullOrEmpty()) foreach (var measurement in PointAngleMeasurements) measurements.Add(measurement.ToMeasurement(PointAngleMeasurement.TypeId));
            if (!ProtrudingLengthMeasurements.IsNullOrEmpty()) foreach (var measurement in ProtrudingLengthMeasurements) measurements.Add(measurement.ToMeasurement(ProtrudingLengthMeasurement.TypeId));
            if (!ShankDiameterMeasurements.IsNullOrEmpty()) foreach (var measurement in ShankDiameterMeasurements) measurements.Add(measurement.ToMeasurement(ShankDiameterMeasurement.TypeId));
            if (!ShankHeightMeasurements.IsNullOrEmpty()) foreach (var measurement in ShankHeightMeasurements) measurements.Add(measurement.ToMeasurement(ShankHeightMeasurement.TypeId));
            if (!ShankLengthMeasurements.IsNullOrEmpty()) foreach (var measurement in ShankLengthMeasurements) measurements.Add(measurement.ToMeasurement(ShankLengthMeasurement.TypeId));
            if (!StepDiameterLengthMeasurements.IsNullOrEmpty()) foreach (var measurement in StepDiameterLengthMeasurements) measurements.Add(measurement.ToMeasurement(StepDiameterLengthMeasurement.TypeId));
            if (!StepIncludedAngleMeasurements.IsNullOrEmpty()) foreach (var measurement in StepIncludedAngleMeasurements) measurements.Add(measurement.ToMeasurement(StepIncludedAngleMeasurement.TypeId));
            if (!ToolCuttingEdgeAngleMeasurements.IsNullOrEmpty()) foreach (var measurement in ToolCuttingEdgeAngleMeasurements) measurements.Add(measurement.ToMeasurement(ToolCuttingEdgeAngleMeasurement.TypeId));
            if (!ToolLeadAngleMeasurements.IsNullOrEmpty()) foreach (var measurement in ToolLeadAngleMeasurements) measurements.Add(measurement.ToMeasurement(ToolLeadAngleMeasurement.TypeId));
            if (!ToolOrientationMeasurements.IsNullOrEmpty()) foreach (var measurement in ToolOrientationMeasurements) measurements.Add(measurement.ToMeasurement(ToolOrientationMeasurement.TypeId));
            if (!UsableLengthMaxMeasurements.IsNullOrEmpty()) foreach (var measurement in UsableLengthMaxMeasurements) measurements.Add(measurement.ToMeasurement(UsableLengthMaxMeasurement.TypeId));
            if (!WeightMeasurements.IsNullOrEmpty()) foreach (var measurement in WeightMeasurements) measurements.Add(measurement.ToMeasurement(WeightMeasurement.TypeId));
            if (!WiperEdgeLengthMeasurements.IsNullOrEmpty()) foreach (var measurement in WiperEdgeLengthMeasurements) measurements.Add(measurement.ToMeasurement(WiperEdgeLengthMeasurement.TypeId));            

            return measurements;
        }
    }
}