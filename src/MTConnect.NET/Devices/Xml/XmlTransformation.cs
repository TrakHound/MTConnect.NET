// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Configuration contains technical information about a component describing its physical layout,
    /// functional characteristics, and relationships with other components within a piece of equipment.
    /// </summary>
    [XmlRoot("Transformation")]
    public class XmlTransformation
    {
        /// <summary>
        /// Translations along X, Y, and Z axes are expressed as x,y, and z respectively within a 3-dimensional vector.      
        /// </summary>
        [XmlElement("Translation")]
        public string Translation { get; set; }

        /// <summary>
        /// Rotations about X, Y, and Z axes are expressed in A, B, and C respectively within a 3-dimensional vector.
        /// </summary>
        [XmlElement("Rotation")]
        public string Rotation { get; set; }


        public XmlTransformation() { }

        public XmlTransformation(Transformation transformation)
        {
            if (transformation != null)
            {
                Translation = transformation.Translation;
                Rotation = transformation.Rotation;
            }
        }

        public Transformation ToTransformation()
        {
            var transformation = new Transformation();
            transformation.Translation = Translation;
            transformation.Rotation = Rotation;
            return transformation;
        }
    }
}
