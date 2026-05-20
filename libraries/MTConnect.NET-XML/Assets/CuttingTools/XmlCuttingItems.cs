// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml.CuttingTools
{
    /// <summary>
    /// XML serialization surrogate for the <c>CuttingItems</c> container of a
    /// CuttingTool life cycle. Holds the collection of individual cutting edges
    /// and the declared count of items for round-trip validation.
    /// </summary>
    public class XmlCuttingItems
    {
        /// <summary>
        /// The number of cutting items the tool declares, carried by the
        /// <c>count</c> attribute.
        /// </summary>
        [XmlAttribute("count")]
        public int Count { get; set; }

        /// <summary>
        /// The individual cutting edges, serialized as repeated
        /// <c>CuttingItem</c> elements.
        /// </summary>
        [XmlElement("CuttingItem")]
        public List<XmlCuttingItem> CuttingItems { get; set; }
    }
}