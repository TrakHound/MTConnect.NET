// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class CuttingToolDefinition
    {
        /// <summary>
        /// Identifies the expected representation of the enclosed data.
        /// </summary>
        [XmlAttribute("format")]
        public CuttingToolDefinitionFormat Format { get; set; }
    }
}
