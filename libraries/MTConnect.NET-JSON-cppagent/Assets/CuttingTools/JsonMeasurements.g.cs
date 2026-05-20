// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.CuttingTools.Measurements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.CuttingTools
{
    /// <summary>
    /// cppagent-style JSON representation of a cutting tool's measurements. Each
    /// MTConnect measurement type is exposed as its own typed collection so the
    /// serialized object shape matches the C++ reference agent's output.
    /// </summary>
    public class JsonMeasurements
    {
        /// <summary>
        /// The <c>BodyDiameterMax</c> measurements declared on the cutting tool.
        /// Largest diameter of the body of a tool item.
        /// </summary>
        [JsonPropertyName("BodyDiameterMax")]
        public List<JsonMeasurement> BodyDiameterMaxMeasurements { get; set; }


        /// <summary>
        /// The <c>BodyLengthMax</c> measurements declared on the cutting tool.
        /// Distance measured along the X axis from that point of the item closest to the workpiece, including the cutting item for a tool item but excluding a protruding locking mechanism for an adaptive item, to either the front of the flange on a flanged body or the beginning of the connection interface feature on the machine side for cylindrical or prismatic shanks.
        /// </summary>
        [JsonPropertyName("BodyLengthMax")]
        public List<JsonMeasurement> BodyLengthMaxMeasurements { get; set; }


        /// <summary>
        /// The <c>ChamferFlatLength</c> measurements declared on the cutting tool.
        /// Flat length of a chamfer.
        /// </summary>
        [JsonPropertyName("ChamferFlatLength")]
        public List<JsonMeasurement> ChamferFlatLengthMeasurements { get; set; }


        /// <summary>
        /// The <c>ChamferWidth</c> measurements declared on the cutting tool.
        /// Width of the chamfer.
        /// </summary>
        [JsonPropertyName("ChamferWidth")]
        public List<JsonMeasurement> ChamferWidthMeasurements { get; set; }


        /// <summary>
        /// The <c>CornerRadius</c> measurements declared on the cutting tool.
        /// Nominal radius of a rounded corner measured in the X Y-plane.
        /// </summary>
        [JsonPropertyName("CornerRadius")]
        public List<JsonMeasurement> CornerRadiusMeasurements { get; set; }


        /// <summary>
        /// The <c>CuttingDiameterMax</c> measurements declared on the cutting tool.
        /// Maximum diameter of a circle on which the defined point Pk of each of the master inserts is located on a tool item. The normal of the machined peripheral surface points towards the axis of the cutting tool.
        /// </summary>
        [JsonPropertyName("CuttingDiameterMax")]
        public List<JsonMeasurement> CuttingDiameterMaxMeasurements { get; set; }


        /// <summary>
        /// The <c>CuttingDiameter</c> measurements declared on the cutting tool.
        /// Diameter of a circle on which the defined point Pk located on this cutting tool. The normal of the machined peripheral surface points towards the axis of the cutting tool.
        /// </summary>
        [JsonPropertyName("CuttingDiameter")]
        public List<JsonMeasurement> CuttingDiameterMeasurements { get; set; }


        /// <summary>
        /// The <c>CuttingEdgeLength</c> measurements declared on the cutting tool.
        /// Theoretical length of the cutting edge of a cutting item over sharp corners.
        /// </summary>
        [JsonPropertyName("CuttingEdgeLength")]
        public List<JsonMeasurement> CuttingEdgeLengthMeasurements { get; set; }


        /// <summary>
        /// The <c>CuttingHeight</c> measurements declared on the cutting tool.
        /// Distance from the basal plane of the tool item to the cutting point.
        /// </summary>
        [JsonPropertyName("CuttingHeight")]
        public List<JsonMeasurement> CuttingHeightMeasurements { get; set; }


        /// <summary>
        /// The <c>CuttingReferencePoint</c> measurements declared on the cutting tool.
        /// Theoretical sharp point of the cutting tool from which the major functional dimensions are taken.
        /// </summary>
        [JsonPropertyName("CuttingReferencePoint")]
        public List<JsonMeasurement> CuttingReferencePointMeasurements { get; set; }


        /// <summary>
        /// The <c>DepthOfCutMax</c> measurements declared on the cutting tool.
        /// Maximum engagement of the cutting edge or edges with the workpiece measured perpendicular to the feed motion.
        /// </summary>
        [JsonPropertyName("DepthOfCutMax")]
        public List<JsonMeasurement> DepthOfCutMaxMeasurements { get; set; }


        /// <summary>
        /// The <c>DriveAngle</c> measurements declared on the cutting tool.
        /// Angle between the driving mechanism locator on a tool item and the main cutting edge.
        /// </summary>
        [JsonPropertyName("DriveAngle")]
        public List<JsonMeasurement> DriveAngleMeasurements { get; set; }


        /// <summary>
        /// The <c>FlangeDiameterMax</c> measurements declared on the cutting tool.
        /// Dimension between two parallel tangents on the outside edge of a flange.
        /// </summary>
        [JsonPropertyName("FlangeDiameterMax")]
        public List<JsonMeasurement> FlangeDiameterMaxMeasurements { get; set; }


        /// <summary>
        /// The <c>FlangeDiameter</c> measurements declared on the cutting tool.
        /// Dimension between two parallel tangents on the outside edge of a flange.
        /// </summary>
        [JsonPropertyName("FlangeDiameter")]
        public List<JsonMeasurement> FlangeDiameterMeasurements { get; set; }


        /// <summary>
        /// The <c>FunctionalLength</c> measurements declared on the cutting tool.
        /// Distance from the gauge plane or from the end of the shank to the furthest point on the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool.The CuttingTool functional length will be the length of the entire tool, not a single cutting item. Each CuttingItem can have an independent FunctionalLength represented in its measurements.
        /// </summary>
        [JsonPropertyName("FunctionalLength")]
        public List<JsonMeasurement> FunctionalLengthMeasurements { get; set; }


        /// <summary>
        /// The <c>FunctionalWidth</c> measurements declared on the cutting tool.
        /// Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.
        /// </summary>
        [JsonPropertyName("FunctionalWidth")]
        public List<JsonMeasurement> FunctionalWidthMeasurements { get; set; }


        /// <summary>
        /// The <c>IncribedCircleDiameter</c> measurements declared on the cutting tool.
        /// Diameter of a circle to which all edges of a equilateral and round regular insert are tangential.
        /// </summary>
        [JsonPropertyName("IncribedCircleDiameter")]
        public List<JsonMeasurement> IncribedCircleDiameterMeasurements { get; set; }


        /// <summary>
        /// The <c>InsertWidth</c> measurements declared on the cutting tool.
        /// W1 is used for the insert width when an inscribed circle diameter is not practical.
        /// </summary>
        [JsonPropertyName("InsertWidth")]
        public List<JsonMeasurement> InsertWidthMeasurements { get; set; }


        /// <summary>
        /// The <c>OverallToolLength</c> measurements declared on the cutting tool.
        /// Largest length dimension of the cutting tool including the master insert where applicable.
        /// </summary>
        [JsonPropertyName("OverallToolLength")]
        public List<JsonMeasurement> OverallToolLengthMeasurements { get; set; }


        /// <summary>
        /// The <c>PointAngle</c> measurements declared on the cutting tool.
        /// Angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.
        /// </summary>
        [JsonPropertyName("PointAngle")]
        public List<JsonMeasurement> PointAngleMeasurements { get; set; }


        /// <summary>
        /// The <c>ProtrudingLength</c> measurements declared on the cutting tool.
        /// Dimension from the yz-plane to the furthest point of the tool item or adaptive item measured in the -X direction.
        /// </summary>
        [JsonPropertyName("ProtrudingLength")]
        public List<JsonMeasurement> ProtrudingLengthMeasurements { get; set; }


        /// <summary>
        /// The <c>ShankDiameter</c> measurements declared on the cutting tool.
        /// Dimension of the diameter of a cylindrical portion of a tool item or an adaptive item that can participate in a connection.
        /// </summary>
        [JsonPropertyName("ShankDiameter")]
        public List<JsonMeasurement> ShankDiameterMeasurements { get; set; }


        /// <summary>
        /// The <c>ShankHeight</c> measurements declared on the cutting tool.
        /// Dimension of the height of the shank.
        /// </summary>
        [JsonPropertyName("ShankHeight")]
        public List<JsonMeasurement> ShankHeightMeasurements { get; set; }


        /// <summary>
        /// The <c>ShankLength</c> measurements declared on the cutting tool.
        /// Dimension of the length of the shank.
        /// </summary>
        [JsonPropertyName("ShankLength")]
        public List<JsonMeasurement> ShankLengthMeasurements { get; set; }


        /// <summary>
        /// The <c>StepDiameterLength</c> measurements declared on the cutting tool.
        /// Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.
        /// </summary>
        [JsonPropertyName("StepDiameterLength")]
        public List<JsonMeasurement> StepDiameterLengthMeasurements { get; set; }


        /// <summary>
        /// The <c>StepIncludedAngle</c> measurements declared on the cutting tool.
        /// Angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.
        /// </summary>
        [JsonPropertyName("StepIncludedAngle")]
        public List<JsonMeasurement> StepIncludedAngleMeasurements { get; set; }


        /// <summary>
        /// The <c>ToolCuttingEdgeAngle</c> measurements declared on the cutting tool.
        /// Angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.
        /// </summary>
        [JsonPropertyName("ToolCuttingEdgeAngle")]
        public List<JsonMeasurement> ToolCuttingEdgeAngleMeasurements { get; set; }


        /// <summary>
        /// The <c>ToolLeadAngle</c> measurements declared on the cutting tool.
        /// Angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.
        /// </summary>
        [JsonPropertyName("ToolLeadAngle")]
        public List<JsonMeasurement> ToolLeadAngleMeasurements { get; set; }


        /// <summary>
        /// The <c>ToolOrientation</c> measurements declared on the cutting tool.
        /// Angle of the tool with respect to the workpiece for a given process. The value is application specific.
        /// </summary>
        [JsonPropertyName("ToolOrientation")]
        public List<JsonMeasurement> ToolOrientationMeasurements { get; set; }


        /// <summary>
        /// The <c>UsableLengthMax</c> measurements declared on the cutting tool.
        /// Maximum length of a cutting tool that can be used in a particular cutting operation including the non-cutting portions of the tool.
        /// </summary>
        [JsonPropertyName("UsableLengthMax")]
        public List<JsonMeasurement> UsableLengthMaxMeasurements { get; set; }


        /// <summary>
        /// The <c>Weight</c> measurements declared on the cutting tool.
        /// Total weight of the cutting tool in grams. The force exerted by the mass of the cutting tool.
        /// </summary>
        [JsonPropertyName("Weight")]
        public List<JsonMeasurement> WeightMeasurements { get; set; }


        /// <summary>
        /// The <c>WiperEdgeLength</c> measurements declared on the cutting tool.
        /// Measure of the length of a wiper edge of a cutting item.
        /// </summary>
        [JsonPropertyName("WiperEdgeLength")]
        public List<JsonMeasurement> WiperEdgeLengthMeasurements { get; set; }



        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonMeasurements() { }

        /// <summary>
        /// Initializes a new instance from a flat sequence of <paramref name="measurements"/>,
        /// bucketing each one into the typed collection that matches its MTConnect type.
        /// </summary>
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


        /// <summary>
        /// Flattens every typed measurement collection back into a single
        /// <see cref="IToolingMeasurement"/> sequence, restoring each item's MTConnect type.
        /// </summary>
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
