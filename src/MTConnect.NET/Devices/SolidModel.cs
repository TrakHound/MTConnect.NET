// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// A SolidModel is a Configuration that references a file with the three-dimensional geometry of the Component or Composition.
    /// The geometry MAY have a transformation and a scale to position the Component with respect to the other Components.
    /// A geometry file can contain a set of assembled items, in this case, the SolidModel reference the id of the assembly model file and the specific item within that file.
    /// </summary>
    public class SolidModel
    {
        /// <summary>
        /// The unique identifier for this entity within the MTConnectDevices document.     
        /// </summary>
        [XmlAttribute("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The associated model file if an item reference is used.    
        /// </summary>
        [XmlAttribute("solidModelIdRef")]
        [JsonPropertyName("solidModelIdRef")]
        public string SolidModelIdRef { get; set; }

        /// <summary>
        /// The URL giving the location of the Solid Model.If not present, the model referenced in the solidModelIdRef is used.       
        /// </summary>
        [XmlAttribute("href")]
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// The reference to the item within the model within the related geometry.A solidModelIdRef MUST be given.
        /// </summary>
        [XmlAttribute("itemRef")]
        [JsonPropertyName("itemRef")]
        public string ItemRef { get; set; }

        /// <summary>
        /// The format of the referenced document.
        /// </summary>
        [XmlAttribute("mediaType")]
        [JsonPropertyName("mediaType")]
        public SolidModelMediaType MediaType { get; set; }

        /// <summary>
        /// A reference to the coordinate system for this SolidModel.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The translation of the origin to the position and orientation.
        /// </summary>
        [XmlElement("Transformation")]
        [JsonPropertyName("transformation")]
        public Transformation Transformation { get; set; }

        /// <summary>
        /// The SolidModel Scale is either a single multiplier applied to all three dimensions or a three space multiplier given in the X, Y, and Z dimensions in the coordinate system used for the SolidModel.
        /// </summary>
        [XmlAttribute("Scale")]
        [JsonPropertyName("scale")]
        public double Scale { get; set; }
    }
}
