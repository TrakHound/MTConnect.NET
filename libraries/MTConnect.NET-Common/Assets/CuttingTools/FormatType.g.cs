// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public enum FormatType
    {
        /// <summary>
        /// Document will confirm to the ISO 10303 Part 21 standard.
        /// </summary>
        EXPRESS,
        
        /// <summary>
        /// Document will be a text representation of the tool data.
        /// </summary>
        TEXT,
        
        /// <summary>
        /// Document will be provided in an undefined format.
        /// </summary>
        UNDEFINED,
        
        /// <summary>
        /// Default value for the definition. The content will be an XML document.
        /// </summary>
        XML
    }
}