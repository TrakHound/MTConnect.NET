// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools.Measurements
{
    public enum Code
    {
        /// <summary>
        /// Maximum engagement of the cutting edge or edges with the workpiece measured perpendicular to the feed motion.
        /// </summary>
        APMX,
        
        /// <summary>
        /// Flat length of a chamfer.
        /// </summary>
        BCH,
        
        /// <summary>
        /// Largest diameter of the body of a tool item.
        /// </summary>
        BDX,
        
        /// <summary>
        /// Measure of the length of a wiper edge of a cutting item.
        /// </summary>
        BS,
        
        /// <summary>
        /// Width of the chamfer.
        /// </summary>
        CHW,
        
        /// <summary>
        /// Theoretical sharp point of the cutting tool from which the major functional dimensions are taken.
        /// </summary>
        CRP,
        
        /// <summary>
        /// Maximum diameter of a circle on which the defined point Pk of each of the master inserts is located on a tool item. The normal of the machined peripheral surface points towards the axis of the cutting tool.
        /// </summary>
        DC,
        
        /// <summary>
        /// Diameter of a circle on which the defined point Pk located on this cutting tool.The normal of the machined peripheral surface points towards the axis of the cutting tool.
        /// </summary>
        D_CX,
        
        /// <summary>
        /// Dimension between two parallel tangents on the outside edge of a flange.
        /// </summary>
        DF,
        
        /// <summary>
        /// Dimension of the diameter of a cylindrical portion of a tool item or an adaptive item that can participate in a connection.
        /// </summary>
        DMM,
        
        /// <summary>
        /// Angle between the driving mechanism locator on a tool item and the main cutting edge.
        /// </summary>
        DRVA,
        
        /// <summary>
        /// Dimension of the height of the shank.
        /// </summary>
        H,
        
        /// <summary>
        /// Distance from the basal plane of the tool item to the cutting point.
        /// </summary>
        HF,
        
        /// <summary>
        /// Diameter of a circle to which all edges of a equilateral and round regular insert are tangential.
        /// </summary>
        IC,
        
        /// <summary>
        /// Angle between the tool cutting edge plane and the tool feed plane measured in a plane parallel the xy-plane.
        /// </summary>
        KAPR,
        
        /// <summary>
        /// Theoretical length of the cutting edge of a cutting item over sharp corners.
        /// </summary>
        L,
        
        /// <summary>
        /// Distance measured along the X axis from that point of the item closest to the workpiece, including the cutting item for a tool item but excluding a protruding locking mechanism for an adaptive item, to either the front of the flange on a flanged body or the beginning of the connection interface feature on the machine side for cylindrical or prismatic shanks.
        /// </summary>
        LBX,
        
        /// <summary>
        /// Distance from the gauge plane or from the end of the shank to the furthest point on the tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. The CuttingTool functional length will be the length of the entire tool, not a single cutting item. Each CuttingItem can have an independent FunctionalLength represented in its measurements.
        /// </summary>
        LF,
        
        /// <summary>
        /// Distance from the gauge plane or from the end of the shank of the cutting tool, if a gauge plane does not exist, to the cutting reference point determined by the main function of the tool. This measurement will be with reference to the cutting tool and **MUST NOT** exist without a cutting tool.
        /// </summary>
        L_FX,
        
        /// <summary>
        /// Dimension from the yz-plane to the furthest point of the tool item or adaptive item measured in the -X direction.
        /// </summary>
        LPR,
        
        /// <summary>
        /// Dimension of the length of the shank.
        /// </summary>
        LS,
        
        /// <summary>
        /// Maximum length of a cutting tool that can be used in a particular cutting operation including the non-cutting portions of the tool.
        /// </summary>
        LUX,
        
        /// <summary>
        /// Angle of the tool with respect to the workpiece for a given process. The value is application specific.
        /// </summary>
        N_A,
        
        /// <summary>
        /// Largest length dimension of the cutting tool including the master insert where applicable.
        /// </summary>
        OAL,
        
        /// <summary>
        /// Angle between the tool cutting edge plane and a plane perpendicular to the tool feed plane measured in a plane parallel the xy-plane.
        /// </summary>
        PSIR,
        
        /// <summary>
        /// Nominal radius of a rounded corner measured in the X Y-plane.
        /// </summary>
        RE,
        
        /// <summary>
        /// Length of a portion of a stepped tool that is related to a corresponding cutting diameter measured from the cutting reference point of that cutting diameter to the point on the next cutting edge at which the diameter starts to change.
        /// </summary>
        S_D_LX,
        
        /// <summary>
        /// Angle between the major cutting edge and the same cutting edge rotated by 180 degrees about the tool axis.
        /// </summary>
        SIG,
        
        /// <summary>
        /// Angle between a major edge on a step of a stepped tool and the same cutting edge rotated 180 degrees about its tool axis.
        /// </summary>
        S_T_AX,
        
        /// <summary>
        /// Insert width when an inscribed circle diameter is not practical.
        /// </summary>
        WONE,
        
        /// <summary>
        /// Distance between the cutting reference point and the rear backing surface of a turning tool or the axis of a boring bar.
        /// </summary>
        WF,
        
        /// <summary>
        /// Total weight of the cutting tool in grams. The force exerted by the mass of the cutting tool.
        /// </summary>
        WT
    }
}