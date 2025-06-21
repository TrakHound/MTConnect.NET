// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class CuttingToolArchetypeAssetDescriptions
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
        /// Unique identifier for this assembly.
        /// </summary>
        public const string SerialNumber = "Unique identifier for this assembly.";
        
        /// <summary>
        /// Identifier for a class of cutting tools.
        /// </summary>
        public const string ToolId = "Identifier for a class of cutting tools.";
    }
}