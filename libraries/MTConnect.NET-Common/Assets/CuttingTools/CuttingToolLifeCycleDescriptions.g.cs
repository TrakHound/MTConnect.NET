// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class CuttingToolLifeCycleDescriptions
    {
        /// <summary>
        /// Identifier for the capability to connect any component of the cutting tool together, except Assembly Items, on the machine side. Code: `CCMS`
        /// </summary>
        public const string ConnectionCodeMachineSide = "Identifier for the capability to connect any component of the cutting tool together, except Assembly Items, on the machine side. Code: `CCMS`";
        
        /// <summary>
        /// Status of the cutting tool.
        /// </summary>
        public const string CutterStatus = "Status of the cutting tool.";
        
        /// <summary>
        /// Part of of the tool that physically removes the material from the workpiece by shear deformation.
        /// </summary>
        public const string CuttingItems = "Part of of the tool that physically removes the material from the workpiece by shear deformation.";
        
        /// <summary>
        /// Location of the pot or spindle the cutting tool currently resides in.If negativeOverlap or positiveOverlap is provided, the tool reserves additional locations on either side, otherwise if they are not given, no additional locations are required for this tool.If the pot occupies the first or last location, a rollover to the beginning or the end of the indexable values may occur. For example, if there are 64 pots and the tool is in pot 64 with a positiveOverlap of 1, the first pot **MAY** be occupied as well.
        /// </summary>
        public const string Location = "Location of the pot or spindle the cutting tool currently resides in.If negativeOverlap or positiveOverlap is provided, the tool reserves additional locations on either side, otherwise if they are not given, no additional locations are required for this tool.If the pot occupies the first or last location, a rollover to the beginning or the end of the indexable values may occur. For example, if there are 64 pots and the tool is in pot 64 with a positiveOverlap of 1, the first pot **MAY** be occupied as well.";
        
        /// <summary>
        /// Constrained scalar value associated with a cutting tool.
        /// </summary>
        public const string Measurements = "Constrained scalar value associated with a cutting tool.";
        
        /// <summary>
        /// Constrained process feed rate for the tool in mm/s.The value **MAY** contain the nominal process target feed rate if available. If ProcessFeedRate is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.
        /// </summary>
        public const string ProcessFeedRate = "Constrained process feed rate for the tool in mm/s.The value **MAY** contain the nominal process target feed rate if available. If ProcessFeedRate is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.";
        
        /// <summary>
        /// Constrained process spindle speed for the tool in revolutions/minute.The value **MAY** contain the nominal process target spindle speed if available. If ProcessSpindleSpeed is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.
        /// </summary>
        public const string ProcessSpindleSpeed = "Constrained process spindle speed for the tool in revolutions/minute.The value **MAY** contain the nominal process target spindle speed if available. If ProcessSpindleSpeed is provided, at least one value of maximum, nominal, or minimum **MUST** be specified.";
        
        /// <summary>
        /// Tool group this tool is assigned in the part program.
        /// </summary>
        public const string ProgramToolGroup = "Tool group this tool is assigned in the part program.";
        
        /// <summary>
        /// Number of the tool as referenced in the part program.
        /// </summary>
        public const string ProgramToolNumber = "Number of the tool as referenced in the part program.";
        
        /// <summary>
        /// Number of times the cutter has been reconditioned.
        /// </summary>
        public const string ReconditionCount = "Number of times the cutter has been reconditioned.";
        
        /// <summary>
        /// Cutting tool life as related to the assembly.
        /// </summary>
        public const string ToolLife = "Cutting tool life as related to the assembly.";
    }
}