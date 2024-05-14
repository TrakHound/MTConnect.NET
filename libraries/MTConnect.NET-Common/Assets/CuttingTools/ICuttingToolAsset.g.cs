// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Asset that physically removes the material from the workpiece by shear deformation.
    /// </summary>
    public partial interface ICuttingToolAsset : IAsset
    {
        /// <summary>
        /// AssetId and/or the URL of the data source of CuttingToolArchetype.
        /// </summary>
        MTConnect.Assets.CuttingTools.ICuttingToolArchetypeReference CuttingToolArchetypeReference { get; }
        
        /// <summary>
        /// Detailed structure of the cutting tool which is static during its lifecycle. ISO 13399.
        /// </summary>
        MTConnect.Assets.CuttingTools.ICuttingToolDefinition CuttingToolDefinition { get; }
        
        /// <summary>
        /// Data regarding the application or use of the tool.This data is provided by various pieces of equipment (i.e. machine tool, presetter) and statistical process control applications. Life cycle data will not remain static, but will change periodically when a tool is used or measured.
        /// </summary>
        MTConnect.Assets.CuttingTools.ICuttingToolLifeCycle CuttingToolLifeCycle { get; }
        
        /// <summary>
        /// Manufacturers of the cutting tool.This will reference the tool item and adaptive items specifically. The cutting itemsmanufacturers’ will be a property of CuttingItem.> Note: In XML, the representation **MUST** be a comma(,) delimited list of manufacturer names. See CuttingTool Schema Diagrams.
        /// </summary>
        System.Collections.Generic.IEnumerable<string> Manufacturers { get; }
        
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