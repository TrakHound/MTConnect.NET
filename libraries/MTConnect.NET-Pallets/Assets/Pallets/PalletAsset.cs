// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace MTConnect.Assets.Pallets
{
    [XmlRoot("Pallet")]
    public class PalletAsset : Asset
    {
        [XmlAttribute("palletId")]
        [JsonPropertyName("palletId")]
        public string PalletId { get; set; }

        [XmlArray("PalletPositions")]
        [XmlArrayItem("PalletPosition")]
        [JsonPropertyName("palletPositions")]
        public List<PalletPosition> PalletPositions { get; set; }


        public PalletAsset()
        {
            Type = "Pallet";
            PalletPositions = new List<PalletPosition>();
        }
    }
}