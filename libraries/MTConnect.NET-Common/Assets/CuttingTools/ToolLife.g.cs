// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_37DECE45_091E_4f0c_AD72_EB481C0C1919

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Cutting tool life as related to the assembly.
    /// </summary>
    public partial class ToolLife : IToolLife
    {
        public const string DescriptionText = "Cutting tool life as related to the assembly.";


        /// <summary>
        /// Indicates if the tool life counts from zero to maximum or maximum to zero.
        /// </summary>
        public MTConnect.Assets.CuttingTools.CountDirectionType CountDirection { get; set; }
        
        /// <summary>
        /// Initial life of the tool when it is new.
        /// </summary>
        public double? Initial { get; set; }
        
        /// <summary>
        /// End of life limit for the tool.
        /// </summary>
        public double? Limit { get; set; }
        
        /// <summary>
        /// Type of tool life being accumulated.
        /// </summary>
        public MTConnect.Assets.CuttingTools.ToolLifeType Type { get; set; }
        
        /// <summary>
        /// Value of ToolLife.
        /// </summary>
        public double Value { get; set; }
        
        /// <summary>
        /// Point at which a tool life warning will be raised.
        /// </summary>
        public double? Warning { get; set; }
    }
}