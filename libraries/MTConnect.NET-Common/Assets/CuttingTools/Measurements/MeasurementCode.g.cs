// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    public static class MeasurementCode
    {
        /// <summary>
        /// Maximum engagement of the cutting edge or edges with the workpiece measured perpendicular to the feed motion.
        /// </summary>
        public const string APMX = "APMX";
        
        /// <summary>
        /// Flat length of a chamfer.
        /// </summary>
        public const string BCH = "BCH";
        
        /// <summary>
        /// Largest diameter of the body of a tool item.
        /// </summary>
        public const string BDX = "BDX";
        
        /// <summary>
        /// Measure of the length of a wiper edge of a cutting item.
        /// </summary>
        public const string BS = "BS";
        
        /// <summary>
        /// Width of the chamfer.
        /// </summary>
        public const string CHW = "CHW";
        
        /// <summary>
        /// Theoretical sharp point of the cutting tool from which the major functional dimensions are taken.
        /// </summary>
        public const string CRP = "CRP";
        
        /// <summary>
        /// Maximum diameter of a circle on which the defined point Pk of each of the master inserts is located on a tool item. The normal of the machined peripheral surface points towards the axis of the cutting tool.
        /// </summary>
        public const string DC = "DC";
        
        /// <summary>
        /// Diameter of a circle on which the defined point Pk located on this cutting tool.The normal of the machined peripheral surface points towards the axis of the cutting tool.
        /// </summary>
        public const string DCx = "DCx";
        
        /// <summary>
        /// Dimension between two parallel tangents on the outside edge of a flange.
        /// </summary>
        public const string DF = "DF";
        
        /// <summary>
        /// Dimension of the diameter of a cylindrical portion of a tool item or an adaptive item that can participate in a connection.
        /// </summary>
        public const string DMM = "DMM";
        
        /// <summary>
        /// Angle between the driving mechanism locator on a tool item and the main cutting edge.
        /// </summary>
        public const string DRVA = "DRVA";
        
        /// <summary>
        /// Dimension of the height of the shank.
        /// </summary>
        public const string H = "H";
        
        /// <summary>
        /// Distance from the basal plane of the tool item to the cutting point.
        /// </summary>
        public const string HF = "HF";
        
        /// <summary>
        /// Diameter of a circle to which all edges of a equilateral and round regular insert are tangential.
        /// </summary>
        public const string IC = "IC";
        
        /// <summary>
        /// Angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.
        /// </summary>
        public const string KAPR = "KAPR";
        
        /// <summary>
        /// Theoretical length of the cutting edge of a cutting item over sharp corners.
        /// </summary>
        public const string L = "L";
        
        /// <summary>
        /// Distance measured along the X axis from that point of the item closest to the workpiece, including the cutting item for a tool item but excluding a protruding locking mechanism for an adaptive item, to either the front of the flange on a flanged body or the beginning of the connection interface feature on the machine side for cylindrical or prismatic shanks.
        /// </summary>
        public const string LBX = "LBX";
        
        /// <summary>
        /// Distance from the gauge plane or from the end of the shank to the furthest point on the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. The CuttingTool functional length will be the length of the entire tool, not a single cutting item. Each CuttingItem can have an independent FunctionalLength represented in its measurements.
        /// </summary>
        public const string LF = "LF";
        
        /// <summary>
        /// Distance from the gauge plane or from the end of the shank of the cutting tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. This measurement will be with reference to the cutting tool and **MUST NOT** exist without a cutting tool.
        /// </summary>
        public const string LFx = "LFx";
        
        /// <summary>
        /// Dimension from the yz-plane to the furthest point of the tool item or adaptive item measured in the -X direction.
        /// </summary>
        public const string LPR = "LPR";
        
        /// <summary>
        /// Dimension of the length of the shank.
        /// </summary>
        public const string LS = "LS";
        
        /// <summary>
        /// Maximum length of a cutting tool that can be used in a particular cutting operation including the non-cutting portions of the tool.
        /// </summary>
        public const string LUX = "LUX";
        
        /// <summary>
        /// Angle of the tool with respect to the workpiece for a given process. The value is application specific.
        /// </summary>
        public const string N_A = "N/A";
        
        /// <summary>
        /// Largest length dimension of the cutting tool including the master insert where applicable.
        /// </summary>
        public const string OAL = "OAL";
        
        /// <summary>
        /// Angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.
        /// </summary>
        public const string PSIR = "PSIR";
        
        /// <summary>
        /// Nominal radius of a rounded corner measured in the X Y-plane.
        /// </summary>
        public const string RE = "RE";
        
        /// <summary>
        /// Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.
        /// </summary>
        public const string SDLx = "SDLx";
        
        /// <summary>
        /// Angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.
        /// </summary>
        public const string SIG = "SIG";
        
        /// <summary>
        /// Angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.
        /// </summary>
        public const string STAx = "STAx";
        
        /// <summary>
        /// Insert width when an inscribed circle diameter is not practical.
        /// </summary>
        public const string W1 = "W1";
        
        /// <summary>
        /// Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.
        /// </summary>
        public const string WF = "WF";
        
        /// <summary>
        /// Total weight of the cutting tool in grams. The force exerted by the mass of the cutting tool.
        /// </summary>
        public const string WT = "WT";
    }
}