// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace MTConnect.Assets.Pallets
{
    [XmlRoot("Pallet")]
    public class PalletAsset : Asset<PalletAsset>
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
