// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    public class Transformation
    {
        /// <summary>
        /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector.      
        /// </summary>
        [XmlElement("Translation")]
        [JsonPropertyName("translation")]
        public string Translation { get; set; }

        /// <summary>
        /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector.
        /// </summary>
        [XmlElement("Rotation")]
        [JsonPropertyName("rotation")]
        public string Rotation { get; set; }
    }
}
