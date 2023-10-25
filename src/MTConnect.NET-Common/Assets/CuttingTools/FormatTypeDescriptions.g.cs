// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class FormatTypeDescriptions
    {
        /// <summary>
        /// Document will confirm to the ISO 10303 Part 21 standard.
        /// </summary>
        public const string EXPRESS = "Document will confirm to the ISO 10303 Part 21 standard.";
        
        /// <summary>
        /// Document will be a text representation of the tool data.
        /// </summary>
        public const string TEXT = "Document will be a text representation of the tool data.";
        
        /// <summary>
        /// Document will be provided in an undefined format.
        /// </summary>
        public const string UNDEFINED = "Document will be provided in an undefined format.";
        
        /// <summary>
        /// Default value for the definition. The content will be an XML document.
        /// </summary>
        public const string XML = "Default value for the definition. The content will be an XML document.";
    }
}