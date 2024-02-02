// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_37B8CFD0_D728_4841_9A07_B6CF819EC895

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Detailed structure of the cutting tool which is static during its lifecycle. ISO 13399.
    /// </summary>
    public class CuttingToolDefinition : ICuttingToolDefinition
    {
        public const string DescriptionText = "Detailed structure of the cutting tool which is static during its lifecycle. ISO 13399.";


        /// <summary>
        /// Identifies the expected representation of the enclosed data.
        /// </summary>
        public MTConnect.Assets.CuttingTools.FormatType Format { get; set; }
        
        /// <summary>
        /// Text of the CuttingToolDefinition in format defined by format.
        /// </summary>
        public string Value { get; set; }
    }
}