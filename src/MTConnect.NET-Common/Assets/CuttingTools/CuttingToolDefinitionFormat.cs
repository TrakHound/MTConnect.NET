// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Identifies the expected representation of the enclosed data.
    /// </summary>
    public enum CuttingToolDefinitionFormat
    {
        /// <summary>
        /// The default value for the definition. The content will be an XML document.
        /// </summary>
        XML,

        /// <summary>
        /// The document will confirm to the ISO 10303 Part 21 standard.
        /// </summary>
        EXPRESS,

        /// <summary>
        /// The document will be a text representation of the tool data.
        /// </summary>
        TEXT,

        /// <summary>
        /// The document will be provided in an undefined format.
        /// </summary>
        UNDEFINED
    }
}