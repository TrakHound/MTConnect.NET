// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools
{
    public class CuttingItemCollection
    {
        /// <summary>
        /// The number of Cutting Item.
        /// </summary>
        [XmlAttribute("count")]
        [JsonPropertyName("count")]
        public int Count { get; set; }


        [XmlElement("CuttingItem")]
        [JsonPropertyName("cuttingItem")]
        public List<CuttingItem> CuttingItems { get; set; }


        public CuttingItemCollection()
        {
            CuttingItems = new List<CuttingItem>();
        }

        public void Add(CuttingItem Item)
        {
            CuttingItems.Add(Item);
            Count = CuttingItems.Count;
        }
    }
}
