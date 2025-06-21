// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Asset that describes the static cutting tool geometries and nominal values as one would expect from a tool catalog.
    /// </summary>
    public partial interface ICuttingToolArchetypeAsset : IAsset
    {
        /// <summary>
        /// Detailed structure of the cutting tool which is static during its lifecycle. ISO 13399.
        /// </summary>
        MTConnect.Assets.CuttingTools.ICuttingToolDefinition CuttingToolDefinition { get; }
        
        /// <summary>
        /// Data regarding the application or use of the tool.This data is provided by various pieces of equipment (i.e. machine tool, presetter) and statistical process control applications. Life cycle data will not remain static, but will change periodically when a tool is used or measured.
        /// </summary>
        MTConnect.Assets.CuttingTools.ICuttingToolLifeCycle CuttingToolLifeCycle { get; }
        
        /// <summary>
        /// Unique identifier for this assembly.
        /// </summary>
        string SerialNumber { get; }
        
        /// <summary>
        /// Identifier for a class of cutting tools.
        /// </summary>
        string ToolId { get; }
    }
}