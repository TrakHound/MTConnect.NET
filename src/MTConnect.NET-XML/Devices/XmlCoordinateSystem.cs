// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.CoordinateSystems;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// Configuration contains technical information about a component describing its physical layout,
    /// functional characteristics, and relationships with other components within a piece of equipment.
    /// </summary>
    [XmlRoot("CoordinateSystem")]
    public class XmlCoordinateSystem
    {
        /// <summary>
        /// The unique identifier for this element.    
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the coordinate system.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The manufacturer’s name or users name for the coordinate system.
        /// </summary>
        [XmlAttribute("nativeName")]
        public string NativeName { get; set; }

        /// <summary>
        /// A pointer to the id attribute of the parent CoordinateSystem.
        /// </summary>
        [XmlAttribute("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The type of coordinate system.
        /// </summary>
        [XmlAttribute("type")]
        public CoordinateSystemType Type { get; set; }

        /// <summary>
        /// The coordinates of the origin position of a coordinate system.
        /// </summary>
        [XmlElement("Origin")]
        public string Origin { get; set; }

        /// <summary>
        /// The process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        [XmlElement("Transformation")]
        public XmlTransformation Transformation { get; set; }

        /// <summary>
        /// The natural language description of the CoordinateSystem.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }


        public XmlCoordinateSystem() { }

        public XmlCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            if (coordinateSystem != null)
            {
                Id = coordinateSystem.Id;
                Name = coordinateSystem.Name;   
                NativeName = coordinateSystem.NativeName;
                ParentIdRef = coordinateSystem.ParentIdRef;
                Type = coordinateSystem.Type;
                Origin = coordinateSystem.Origin;
                if (coordinateSystem.Transformation != null) Transformation = new XmlTransformation(coordinateSystem.Transformation);
                Description = coordinateSystem.Description;
            }
        }

        public CoordinateSystem ToCoordinateSystem()
        {
            var coordinateSystem = new CoordinateSystem();
            coordinateSystem.Id = Id;
            coordinateSystem.Name = Name;
            coordinateSystem.NativeName = NativeName;
            coordinateSystem.ParentIdRef = ParentIdRef;
            coordinateSystem.Type = Type;
            coordinateSystem.Origin = Origin;
            if (Transformation != null) coordinateSystem.Transformation = Transformation.ToTransformation();
            coordinateSystem.Description = Description;
            return coordinateSystem;
        }
    }
}
