// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.CuttingTools.Measurements;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization wrapper for the <c>BodyDiameterMax</c> cutting-tool measurement.
    /// Largest diameter of the body of a tool item.
    /// </summary>
    public class XmlBodyDiameterMaxMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="BodyDiameterMaxMeasurement.TypeId"/> (<c>BodyDiameterMax</c>).
        /// </summary>
        public XmlBodyDiameterMaxMeasurement() { Type = BodyDiameterMaxMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>BodyLengthMax</c> cutting-tool measurement.
    /// Distance measured along the X axis from that point of the item closest to the workpiece, including the cutting item for a tool item but excluding a protruding locking mechanism for an adaptive item, to either the front of the flange on a flanged body or the beginning of the connection interface feature on the machine side for cylindrical or prismatic shanks.
    /// </summary>
    public class XmlBodyLengthMaxMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="BodyLengthMaxMeasurement.TypeId"/> (<c>BodyLengthMax</c>).
        /// </summary>
        public XmlBodyLengthMaxMeasurement() { Type = BodyLengthMaxMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ChamferFlatLength</c> cutting-tool measurement.
    /// Flat length of a chamfer.
    /// </summary>
    public class XmlChamferFlatLengthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ChamferFlatLengthMeasurement.TypeId"/> (<c>ChamferFlatLength</c>).
        /// </summary>
        public XmlChamferFlatLengthMeasurement() { Type = ChamferFlatLengthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ChamferWidth</c> cutting-tool measurement.
    /// Width of the chamfer.
    /// </summary>
    public class XmlChamferWidthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ChamferWidthMeasurement.TypeId"/> (<c>ChamferWidth</c>).
        /// </summary>
        public XmlChamferWidthMeasurement() { Type = ChamferWidthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>CornerRadius</c> cutting-tool measurement.
    /// Nominal radius of a rounded corner measured in the X Y-plane.
    /// </summary>
    public class XmlCornerRadiusMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="CornerRadiusMeasurement.TypeId"/> (<c>CornerRadius</c>).
        /// </summary>
        public XmlCornerRadiusMeasurement() { Type = CornerRadiusMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>CuttingDiameterMax</c> cutting-tool measurement.
    /// Maximum diameter of a circle on which the defined point Pk of each of the master inserts is located on a tool item. The normal of the machined peripheral surface points towards the axis of the cutting tool.
    /// </summary>
    public class XmlCuttingDiameterMaxMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="CuttingDiameterMaxMeasurement.TypeId"/> (<c>CuttingDiameterMax</c>).
        /// </summary>
        public XmlCuttingDiameterMaxMeasurement() { Type = CuttingDiameterMaxMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>CuttingDiameter</c> cutting-tool measurement.
    /// Diameter of a circle on which the defined point Pk located on this cutting tool. The normal of the machined peripheral surface points towards the axis of the cutting tool.
    /// </summary>
    public class XmlCuttingDiameterMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="CuttingDiameterMeasurement.TypeId"/> (<c>CuttingDiameter</c>).
        /// </summary>
        public XmlCuttingDiameterMeasurement() { Type = CuttingDiameterMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>CuttingEdgeLength</c> cutting-tool measurement.
    /// Theoretical length of the cutting edge of a cutting item over sharp corners.
    /// </summary>
    public class XmlCuttingEdgeLengthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="CuttingEdgeLengthMeasurement.TypeId"/> (<c>CuttingEdgeLength</c>).
        /// </summary>
        public XmlCuttingEdgeLengthMeasurement() { Type = CuttingEdgeLengthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>CuttingHeight</c> cutting-tool measurement.
    /// Distance from the basal plane of the tool item to the cutting point.
    /// </summary>
    public class XmlCuttingHeightMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="CuttingHeightMeasurement.TypeId"/> (<c>CuttingHeight</c>).
        /// </summary>
        public XmlCuttingHeightMeasurement() { Type = CuttingHeightMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>CuttingReferencePoint</c> cutting-tool measurement.
    /// Theoretical sharp point of the cutting tool from which the major functional dimensions are taken.
    /// </summary>
    public class XmlCuttingReferencePointMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="CuttingReferencePointMeasurement.TypeId"/> (<c>CuttingReferencePoint</c>).
        /// </summary>
        public XmlCuttingReferencePointMeasurement() { Type = CuttingReferencePointMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>DepthOfCutMax</c> cutting-tool measurement.
    /// Maximum engagement of the cutting edge or edges with the workpiece measured perpendicular to the feed motion.
    /// </summary>
    public class XmlDepthOfCutMaxMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="DepthOfCutMaxMeasurement.TypeId"/> (<c>DepthOfCutMax</c>).
        /// </summary>
        public XmlDepthOfCutMaxMeasurement() { Type = DepthOfCutMaxMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>DriveAngle</c> cutting-tool measurement.
    /// Angle between the driving mechanism locator on a tool item and the main cutting edge.
    /// </summary>
    public class XmlDriveAngleMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="DriveAngleMeasurement.TypeId"/> (<c>DriveAngle</c>).
        /// </summary>
        public XmlDriveAngleMeasurement() { Type = DriveAngleMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>FlangeDiameterMax</c> cutting-tool measurement.
    /// Dimension between two parallel tangents on the outside edge of a flange.
    /// </summary>
    public class XmlFlangeDiameterMaxMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="FlangeDiameterMaxMeasurement.TypeId"/> (<c>FlangeDiameterMax</c>).
        /// </summary>
        public XmlFlangeDiameterMaxMeasurement() { Type = FlangeDiameterMaxMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>FlangeDiameter</c> cutting-tool measurement.
    /// Dimension between two parallel tangents on the outside edge of a flange.
    /// </summary>
    public class XmlFlangeDiameterMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="FlangeDiameterMeasurement.TypeId"/> (<c>FlangeDiameter</c>).
        /// </summary>
        public XmlFlangeDiameterMeasurement() { Type = FlangeDiameterMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>FunctionalLength</c> cutting-tool measurement.
    /// Distance from the gauge plane or from the end of the shank to the furthest point on the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool.The CuttingTool functional length will be the length of the entire tool, not a single cutting item. Each CuttingItem can have an independent FunctionalLength represented in its measurements.
    /// </summary>
    public class XmlFunctionalLengthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="FunctionalLengthMeasurement.TypeId"/> (<c>FunctionalLength</c>).
        /// </summary>
        public XmlFunctionalLengthMeasurement() { Type = FunctionalLengthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>FunctionalWidth</c> cutting-tool measurement.
    /// Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.
    /// </summary>
    public class XmlFunctionalWidthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="FunctionalWidthMeasurement.TypeId"/> (<c>FunctionalWidth</c>).
        /// </summary>
        public XmlFunctionalWidthMeasurement() { Type = FunctionalWidthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>IncribedCircleDiameter</c> cutting-tool measurement.
    /// Diameter of a circle to which all edges of a equilateral and round regular insert are tangential.
    /// </summary>
    public class XmlIncribedCircleDiameterMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="IncribedCircleDiameterMeasurement.TypeId"/> (<c>IncribedCircleDiameter</c>).
        /// </summary>
        public XmlIncribedCircleDiameterMeasurement() { Type = IncribedCircleDiameterMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>InsertWidth</c> cutting-tool measurement.
    /// W1 is used for the insert width when an inscribed circle diameter is not practical.
    /// </summary>
    public class XmlInsertWidthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="InsertWidthMeasurement.TypeId"/> (<c>InsertWidth</c>).
        /// </summary>
        public XmlInsertWidthMeasurement() { Type = InsertWidthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>OverallToolLength</c> cutting-tool measurement.
    /// Largest length dimension of the cutting tool including the master insert where applicable.
    /// </summary>
    public class XmlOverallToolLengthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="OverallToolLengthMeasurement.TypeId"/> (<c>OverallToolLength</c>).
        /// </summary>
        public XmlOverallToolLengthMeasurement() { Type = OverallToolLengthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>PointAngle</c> cutting-tool measurement.
    /// Angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.
    /// </summary>
    public class XmlPointAngleMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="PointAngleMeasurement.TypeId"/> (<c>PointAngle</c>).
        /// </summary>
        public XmlPointAngleMeasurement() { Type = PointAngleMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ProtrudingLength</c> cutting-tool measurement.
    /// Dimension from the yz-plane to the furthest point of the tool item or adaptive item measured in the -X direction.
    /// </summary>
    public class XmlProtrudingLengthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ProtrudingLengthMeasurement.TypeId"/> (<c>ProtrudingLength</c>).
        /// </summary>
        public XmlProtrudingLengthMeasurement() { Type = ProtrudingLengthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ShankDiameter</c> cutting-tool measurement.
    /// Dimension of the diameter of a cylindrical portion of a tool item or an adaptive item that can participate in a connection.
    /// </summary>
    public class XmlShankDiameterMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ShankDiameterMeasurement.TypeId"/> (<c>ShankDiameter</c>).
        /// </summary>
        public XmlShankDiameterMeasurement() { Type = ShankDiameterMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ShankHeight</c> cutting-tool measurement.
    /// Dimension of the height of the shank.
    /// </summary>
    public class XmlShankHeightMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ShankHeightMeasurement.TypeId"/> (<c>ShankHeight</c>).
        /// </summary>
        public XmlShankHeightMeasurement() { Type = ShankHeightMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ShankLength</c> cutting-tool measurement.
    /// Dimension of the length of the shank.
    /// </summary>
    public class XmlShankLengthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ShankLengthMeasurement.TypeId"/> (<c>ShankLength</c>).
        /// </summary>
        public XmlShankLengthMeasurement() { Type = ShankLengthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>StepDiameterLength</c> cutting-tool measurement.
    /// Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.
    /// </summary>
    public class XmlStepDiameterLengthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="StepDiameterLengthMeasurement.TypeId"/> (<c>StepDiameterLength</c>).
        /// </summary>
        public XmlStepDiameterLengthMeasurement() { Type = StepDiameterLengthMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>StepIncludedAngle</c> cutting-tool measurement.
    /// Angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.
    /// </summary>
    public class XmlStepIncludedAngleMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="StepIncludedAngleMeasurement.TypeId"/> (<c>StepIncludedAngle</c>).
        /// </summary>
        public XmlStepIncludedAngleMeasurement() { Type = StepIncludedAngleMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ToolCuttingEdgeAngle</c> cutting-tool measurement.
    /// Angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.
    /// </summary>
    public class XmlToolCuttingEdgeAngleMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ToolCuttingEdgeAngleMeasurement.TypeId"/> (<c>ToolCuttingEdgeAngle</c>).
        /// </summary>
        public XmlToolCuttingEdgeAngleMeasurement() { Type = ToolCuttingEdgeAngleMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ToolLeadAngle</c> cutting-tool measurement.
    /// Angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.
    /// </summary>
    public class XmlToolLeadAngleMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ToolLeadAngleMeasurement.TypeId"/> (<c>ToolLeadAngle</c>).
        /// </summary>
        public XmlToolLeadAngleMeasurement() { Type = ToolLeadAngleMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>ToolOrientation</c> cutting-tool measurement.
    /// Angle of the tool with respect to the workpiece for a given process. The value is application specific.
    /// </summary>
    public class XmlToolOrientationMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="ToolOrientationMeasurement.TypeId"/> (<c>ToolOrientation</c>).
        /// </summary>
        public XmlToolOrientationMeasurement() { Type = ToolOrientationMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>UsableLengthMax</c> cutting-tool measurement.
    /// Maximum length of a cutting tool that can be used in a particular cutting operation including the non-cutting portions of the tool.
    /// </summary>
    public class XmlUsableLengthMaxMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="UsableLengthMaxMeasurement.TypeId"/> (<c>UsableLengthMax</c>).
        /// </summary>
        public XmlUsableLengthMaxMeasurement() { Type = UsableLengthMaxMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>Weight</c> cutting-tool measurement.
    /// Total weight of the cutting tool in grams. The force exerted by the mass of the cutting tool.
    /// </summary>
    public class XmlWeightMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="WeightMeasurement.TypeId"/> (<c>Weight</c>).
        /// </summary>
        public XmlWeightMeasurement() { Type = WeightMeasurement.TypeId; }
    }

    /// <summary>
    /// XML serialization wrapper for the <c>WiperEdgeLength</c> cutting-tool measurement.
    /// Measure of the length of a wiper edge of a cutting item.
    /// </summary>
    public class XmlWiperEdgeLengthMeasurement : XmlMeasurement
    {
        /// <summary>
        /// Initializes a new instance and fixes the measurement <c>type</c> to <see cref="WiperEdgeLengthMeasurement.TypeId"/> (<c>WiperEdgeLength</c>).
        /// </summary>
        public XmlWiperEdgeLengthMeasurement() { Type = WiperEdgeLengthMeasurement.TypeId; }
    }

}
