// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    public static class MeasurementCodeDescriptions
    {
        /// <summary>
        /// Maximum engagement of the cutting edge or edges with the workpiece measured perpendicular to the feed motion.
        /// </summary>
        public const string APMX = "Maximum engagement of the cutting edge or edges with the workpiece measured perpendicular to the feed motion.";
        
        /// <summary>
        /// Flat length of a chamfer.
        /// </summary>
        public const string BCH = "Flat length of a chamfer.";
        
        /// <summary>
        /// Largest diameter of the body of a tool item.
        /// </summary>
        public const string BDX = "Largest diameter of the body of a tool item.";
        
        /// <summary>
        /// Measure of the length of a wiper edge of a cutting item.
        /// </summary>
        public const string BS = "Measure of the length of a wiper edge of a cutting item.";
        
        /// <summary>
        /// Width of the chamfer.
        /// </summary>
        public const string CHW = "Width of the chamfer.";
        
        /// <summary>
        /// Theoretical sharp point of the cutting tool from which the major functional dimensions are taken.
        /// </summary>
        public const string CRP = "Theoretical sharp point of the cutting tool from which the major functional dimensions are taken.";
        
        /// <summary>
        /// Maximum diameter of a circle on which the defined point Pk of each of the master inserts is located on a tool item. The normal of the machined peripheral surface points towards the axis of the cutting tool.
        /// </summary>
        public const string DC = "Maximum diameter of a circle on which the defined point Pk of each of the master inserts is located on a tool item. The normal of the machined peripheral surface points towards the axis of the cutting tool.";
        
        /// <summary>
        /// Diameter of a circle on which the defined point Pk located on this cutting tool.The normal of the machined peripheral surface points towards the axis of the cutting tool.
        /// </summary>
        public const string DCx = "Diameter of a circle on which the defined point Pk located on this cutting tool.The normal of the machined peripheral surface points towards the axis of the cutting tool.";
        
        /// <summary>
        /// Dimension between two parallel tangents on the outside edge of a flange.
        /// </summary>
        public const string DF = "Dimension between two parallel tangents on the outside edge of a flange.";
        
        /// <summary>
        /// Dimension of the diameter of a cylindrical portion of a tool item or an adaptive item that can participate in a connection.
        /// </summary>
        public const string DMM = "Dimension of the diameter of a cylindrical portion of a tool item or an adaptive item that can participate in a connection.";
        
        /// <summary>
        /// Angle between the driving mechanism locator on a tool item and the main cutting edge.
        /// </summary>
        public const string DRVA = "Angle between the driving mechanism locator on a tool item and the main cutting edge.";
        
        /// <summary>
        /// Dimension of the height of the shank.
        /// </summary>
        public const string H = "Dimension of the height of the shank.";
        
        /// <summary>
        /// Distance from the basal plane of the tool item to the cutting point.
        /// </summary>
        public const string HF = "Distance from the basal plane of the tool item to the cutting point.";
        
        /// <summary>
        /// Diameter of a circle to which all edges of a equilateral and round regular insert are tangential.
        /// </summary>
        public const string IC = "Diameter of a circle to which all edges of a equilateral and round regular insert are tangential.";
        
        /// <summary>
        /// Angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.
        /// </summary>
        public const string KAPR = "Angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.";
        
        /// <summary>
        /// Theoretical length of the cutting edge of a cutting item over sharp corners.
        /// </summary>
        public const string L = "Theoretical length of the cutting edge of a cutting item over sharp corners.";
        
        /// <summary>
        /// Distance measured along the X axis from that point of the item closest to the workpiece, including the cutting item for a tool item but excluding a protruding locking mechanism for an adaptive item, to either the front of the flange on a flanged body or the beginning of the connection interface feature on the machine side for cylindrical or prismatic shanks.
        /// </summary>
        public const string LBX = "Distance measured along the X axis from that point of the item closest to the workpiece, including the cutting item for a tool item but excluding a protruding locking mechanism for an adaptive item, to either the front of the flange on a flanged body or the beginning of the connection interface feature on the machine side for cylindrical or prismatic shanks.";
        
        /// <summary>
        /// Distance from the gauge plane or from the end of the shank to the furthest point on the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. The CuttingTool functional length will be the length of the entire tool, not a single cutting item. Each CuttingItem can have an independent FunctionalLength represented in its measurements.
        /// </summary>
        public const string LF = "Distance from the gauge plane or from the end of the shank to the furthest point on the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. The CuttingTool functional length will be the length of the entire tool, not a single cutting item. Each CuttingItem can have an independent FunctionalLength represented in its measurements.";
        
        /// <summary>
        /// Distance from the gauge plane or from the end of the shank of the cutting tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. This measurement will be with reference to the cutting tool and **MUST NOT** exist without a cutting tool.
        /// </summary>
        public const string LFx = "Distance from the gauge plane or from the end of the shank of the cutting tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. This measurement will be with reference to the cutting tool and **MUST NOT** exist without a cutting tool.";
        
        /// <summary>
        /// Dimension from the yz-plane to the furthest point of the tool item or adaptive item measured in the -X direction.
        /// </summary>
        public const string LPR = "Dimension from the yz-plane to the furthest point of the tool item or adaptive item measured in the -X direction.";
        
        /// <summary>
        /// Dimension of the length of the shank.
        /// </summary>
        public const string LS = "Dimension of the length of the shank.";
        
        /// <summary>
        /// Maximum length of a cutting tool that can be used in a particular cutting operation including the non-cutting portions of the tool.
        /// </summary>
        public const string LUX = "Maximum length of a cutting tool that can be used in a particular cutting operation including the non-cutting portions of the tool.";
        
        /// <summary>
        /// Angle of the tool with respect to the workpiece for a given process. The value is application specific.
        /// </summary>
        public const string N_A = "Angle of the tool with respect to the workpiece for a given process. The value is application specific.";
        
        /// <summary>
        /// Largest length dimension of the cutting tool including the master insert where applicable.
        /// </summary>
        public const string OAL = "Largest length dimension of the cutting tool including the master insert where applicable.";
        
        /// <summary>
        /// Angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.
        /// </summary>
        public const string PSIR = "Angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.";
        
        /// <summary>
        /// Nominal radius of a rounded corner measured in the X Y-plane.
        /// </summary>
        public const string RE = "Nominal radius of a rounded corner measured in the X Y-plane.";
        
        /// <summary>
        /// Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.
        /// </summary>
        public const string SDLx = "Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.";
        
        /// <summary>
        /// Angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.
        /// </summary>
        public const string SIG = "Angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.";
        
        /// <summary>
        /// Angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.
        /// </summary>
        public const string STAx = "Angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.";
        
        /// <summary>
        /// Insert width when an inscribed circle diameter is not practical.
        /// </summary>
        public const string W1 = "Insert width when an inscribed circle diameter is not practical.";
        
        /// <summary>
        /// Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.
        /// </summary>
        public const string WF = "Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.";
        
        /// <summary>
        /// Total weight of the cutting tool in grams. The force exerted by the mass of the cutting tool.
        /// </summary>
        public const string WT = "Total weight of the cutting tool in grams. The force exerted by the mass of the cutting tool.";


        public static string Get(string value)
        {
            switch (value)
            {
                case MeasurementCode.APMX: return "Maximum engagement of the cutting edge or edges with the workpiece measured perpendicular to the feed motion.";
                case MeasurementCode.BCH: return "Flat length of a chamfer.";
                case MeasurementCode.BDX: return "Largest diameter of the body of a tool item.";
                case MeasurementCode.BS: return "Measure of the length of a wiper edge of a cutting item.";
                case MeasurementCode.CHW: return "Width of the chamfer.";
                case MeasurementCode.CRP: return "Theoretical sharp point of the cutting tool from which the major functional dimensions are taken.";
                case MeasurementCode.DC: return "Maximum diameter of a circle on which the defined point Pk of each of the master inserts is located on a tool item. The normal of the machined peripheral surface points towards the axis of the cutting tool.";
                case MeasurementCode.DCx: return "Diameter of a circle on which the defined point Pk located on this cutting tool.The normal of the machined peripheral surface points towards the axis of the cutting tool.";
                case MeasurementCode.DF: return "Dimension between two parallel tangents on the outside edge of a flange.";
                case MeasurementCode.DMM: return "Dimension of the diameter of a cylindrical portion of a tool item or an adaptive item that can participate in a connection.";
                case MeasurementCode.DRVA: return "Angle between the driving mechanism locator on a tool item and the main cutting edge.";
                case MeasurementCode.H: return "Dimension of the height of the shank.";
                case MeasurementCode.HF: return "Distance from the basal plane of the tool item to the cutting point.";
                case MeasurementCode.IC: return "Diameter of a circle to which all edges of a equilateral and round regular insert are tangential.";
                case MeasurementCode.KAPR: return "Angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.";
                case MeasurementCode.L: return "Theoretical length of the cutting edge of a cutting item over sharp corners.";
                case MeasurementCode.LBX: return "Distance measured along the X axis from that point of the item closest to the workpiece, including the cutting item for a tool item but excluding a protruding locking mechanism for an adaptive item, to either the front of the flange on a flanged body or the beginning of the connection interface feature on the machine side for cylindrical or prismatic shanks.";
                case MeasurementCode.LF: return "Distance from the gauge plane or from the end of the shank to the furthest point on the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. The CuttingTool functional length will be the length of the entire tool, not a single cutting item. Each CuttingItem can have an independent FunctionalLength represented in its measurements.";
                case MeasurementCode.LFx: return "Distance from the gauge plane or from the end of the shank of the cutting tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. This measurement will be with reference to the cutting tool and **MUST NOT** exist without a cutting tool.";
                case MeasurementCode.LPR: return "Dimension from the yz-plane to the furthest point of the tool item or adaptive item measured in the -X direction.";
                case MeasurementCode.LS: return "Dimension of the length of the shank.";
                case MeasurementCode.LUX: return "Maximum length of a cutting tool that can be used in a particular cutting operation including the non-cutting portions of the tool.";
                case MeasurementCode.N_A: return "Angle of the tool with respect to the workpiece for a given process. The value is application specific.";
                case MeasurementCode.OAL: return "Largest length dimension of the cutting tool including the master insert where applicable.";
                case MeasurementCode.PSIR: return "Angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.";
                case MeasurementCode.RE: return "Nominal radius of a rounded corner measured in the X Y-plane.";
                case MeasurementCode.SDLx: return "Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.";
                case MeasurementCode.SIG: return "Angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.";
                case MeasurementCode.STAx: return "Angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.";
                case MeasurementCode.W1: return "Insert width when an inscribed circle diameter is not practical.";
                case MeasurementCode.WF: return "Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.";
                case MeasurementCode.WT: return "Total weight of the cutting tool in grams. The force exerted by the mass of the cutting tool.";
            }

            return null;
        }
    }
}