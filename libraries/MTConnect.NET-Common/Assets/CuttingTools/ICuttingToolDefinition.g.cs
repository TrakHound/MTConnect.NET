// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Detailed structure of the cutting tool which is static during its lifecycle. ISO 13399.
    /// </summary>
    public interface ICuttingToolDefinition
    {
        /// <summary>
        /// Identifies the expected representation of the enclosed data.
        /// </summary>
        MTConnect.Assets.CuttingTools.FormatType Format { get; }
        
        /// <summary>
        /// Text of the CuttingToolDefinition in format defined by format.
        /// </summary>
        string Value { get; }
    }
}