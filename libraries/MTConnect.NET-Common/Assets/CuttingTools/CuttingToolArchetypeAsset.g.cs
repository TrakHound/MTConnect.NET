// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = EAID_8D60D3F8_AB1A_490b_923B_B978248EF331

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Asset that describes the static cutting tool geometries and nominal values as one would expect from a tool catalog.
    /// </summary>
    public partial class CuttingToolArchetypeAsset : Asset, ICuttingToolArchetypeAsset
    {
        public new const string DescriptionText = "Asset that describes the static cutting tool geometries and nominal values as one would expect from a tool catalog.";


        /// <summary>
        /// Detailed structure of the cutting tool which is static during its lifecycle. ISO 13399.
        /// </summary>
        public MTConnect.Assets.CuttingTools.ICuttingToolDefinition CuttingToolDefinition { get; set; }
        
        /// <summary>
        /// Data regarding the application or use of the tool.This data is provided by various pieces of equipment (i.e. machine tool, presetter) and statistical process control applications. Life cycle data will not remain static, but will change periodically when a tool is used or measured.
        /// </summary>
        public MTConnect.Assets.CuttingTools.ICuttingToolLifeCycle CuttingToolLifeCycle { get; set; }
        
        /// <summary>
        /// Unique identifier for this assembly.
        /// </summary>
        public string SerialNumber { get; set; }
        
        /// <summary>
        /// Identifier for a class of cutting tools.
        /// </summary>
        public string ToolId { get; set; }
    }
}