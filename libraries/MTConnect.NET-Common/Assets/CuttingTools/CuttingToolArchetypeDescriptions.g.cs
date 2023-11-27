// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class CuttingToolArchetypeDescriptions
    {
        /// <summary>
        /// Detailed structure of the cutting tool which is static during its lifecycle. ISO 13399.
        /// </summary>
        public const string CuttingToolDefinition = "Detailed structure of the cutting tool which is static during its lifecycle. ISO 13399.";
        
        /// <summary>
        /// Data regarding the application or use of the tool.This data is provided by various pieces of equipment (i.e. machine tool, presetter) and statistical process control applications. Life cycle data will not remain static, but will change periodically when a tool is used or measured.
        /// </summary>
        public const string CuttingToolLifeCycle = "Data regarding the application or use of the tool.This data is provided by various pieces of equipment (i.e. machine tool, presetter) and statistical process control applications. Life cycle data will not remain static, but will change periodically when a tool is used or measured.";
        
        /// <summary>
        /// Manufacturers of the cutting tool.This will reference the tool item and adaptive items specifically. The cutting itemsmanufacturers’ will be a property of CuttingItem.> Note: In XML, the representation will be a comma(,) delimited list of manufacturer names. See CuttingTool Schema Diagrams.
        /// </summary>
        public const string Manufacturers = "Manufacturers of the cutting tool.This will reference the tool item and adaptive items specifically. The cutting itemsmanufacturers’ will be a property of CuttingItem.> Note: In XML, the representation will be a comma(,) delimited list of manufacturer names. See CuttingTool Schema Diagrams.";
        
        /// <summary>
        /// Unique identifier for this assembly.
        /// </summary>
        public const string SerialNumber = "Unique identifier for this assembly.";
        
        /// <summary>
        /// Identifier for a class of cutting tools.
        /// </summary>
        public const string ToolId = "Identifier for a class of cutting tools.";
    }
}