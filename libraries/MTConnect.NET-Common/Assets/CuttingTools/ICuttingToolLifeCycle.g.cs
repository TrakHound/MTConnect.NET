// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Data regarding the application or use of the tool.This data is provided by various pieces of equipment (i.e. machine tool, presetter) and statistical process control applications. Life cycle data will not remain static, but will change periodically when a tool is used or measured.
    /// </summary>
    public partial interface ICuttingToolLifeCycle
    {
        /// <summary>
        /// Identifier for the capability to connect any component of the cutting tool together, except Assembly Items, on the machine side. Code: `CCMS`
        /// </summary>
        string ConnectionCodeMachineSide { get; }
        
        /// <summary>
        /// Status of the cutting tool.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.CuttingTools.CutterStatusType> CutterStatus { get; }
        
        /// <summary>
        /// Part of of the tool that physically removes the material from the workpiece by shear deformation.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.CuttingTools.ICuttingItem> CuttingItems { get; }
        
        /// <summary>
        /// Location of the pot or spindle the cutting tool currently resides in.If negativeOverlap or positiveOverlap is provided, the tool reserves additional locations on either side, otherwise if they are not given, no additional locations are required for this tool.If the pot occupies the first or last location, a rollover to the beginning or the end of the indexable values may occur. For example, if there are 64 pots and the tool is in pot 64 with a positiveOverlap of 1, the first pot **MAY** be occupied as well.
        /// </summary>
        MTConnect.Assets.CuttingTools.ILocation Location { get; }
        
        /// <summary>
        /// Constrained scalar value associated with a cutting tool.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.CuttingTools.IMeasurement> Measurements { get; }
        
        /// <summary>
        /// Constrained process feed rate for the tool in mm/s.The value **MAY** contain the nominal process target feed rate if available. If ProcessFeedRate is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.
        /// </summary>
        MTConnect.Assets.CuttingTools.IProcessFeedRate ProcessFeedRate { get; }
        
        /// <summary>
        /// Constrained process spindle speed for the tool in revolutions/minute.The value **MAY** contain the nominal process target spindle speed if available. If ProcessSpindleSpeed is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.
        /// </summary>
        MTConnect.Assets.CuttingTools.IProcessSpindleSpeed ProcessSpindleSpeed { get; }
        
        /// <summary>
        /// Tool group this tool is assigned in the part program.
        /// </summary>
        string ProgramToolGroup { get; }
        
        /// <summary>
        /// Number of the tool as referenced in the part program.
        /// </summary>
        string ProgramToolNumber { get; }
        
        /// <summary>
        /// Number of times the cutter has been reconditioned.
        /// </summary>
        MTConnect.Assets.CuttingTools.IReconditionCount ReconditionCount { get; }
        
        /// <summary>
        /// Cutting tool life as related to the assembly.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.CuttingTools.IToolLife> ToolLife { get; }
    }
}