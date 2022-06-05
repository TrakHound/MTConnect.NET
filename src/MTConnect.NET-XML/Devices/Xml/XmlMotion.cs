// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Motion;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Configuration contains technical information about a component describing its physical layout,
    /// functional characteristics, and relationships with other components within a piece of equipment.
    /// </summary>
    [XmlRoot("Motion")]
    public class XmlMotion
    {
        /// <summary>
        /// The unique identifier for this element.    
        /// </summary>
        [XmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>
        /// A pointer to the id attribute of the parent CoordinateSystem.
        /// </summary>
        [XmlAttribute("parentIdRef")]
        public string ParentIdRef { get; set; }

        /// <summary>
        /// The coordinate system within which the kinematic motion occurs.
        /// </summary>
        [XmlAttribute("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// Describes the type of motion.    
        /// </summary>
        [XmlAttribute("type")]
        public MotionType Type { get; set; }

        /// <summary>
        /// Describes if this Component is actuated directly or indirectly as a result of other motion.
        /// </summary>
        [XmlAttribute("actuation")]
        public MotionActuationType Actuation { get; set; }

        /// <summary>
        /// The natural language description of the CoordinateSystem.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }

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
        /// Axis defines the axis along or around which the Component moves relative to a coordinate system.
        /// </summary>
        [XmlElement("Axis")]
        public string Axis { get; set; }


        public XmlMotion() { }

        public XmlMotion(Motion motion)
        {
            if (motion != null)
            {
                Id = motion.Id;
                ParentIdRef = motion.ParentIdRef;
                CoordinateSystemIdRef = motion.CoordinateSystemIdRef;
                Type = motion.Type;
                Actuation = motion.Actuation;
                Axis = motion.Axis;
                Origin = motion.Origin;
                if (motion.Transformation != null) Transformation = new XmlTransformation(motion.Transformation);
                Description = motion.Description;
            }
        }

        public Motion ToMotion()
        {
            var motion = new Motion();
            motion.Id = Id;
            motion.ParentIdRef = ParentIdRef;
            motion.CoordinateSystemIdRef = CoordinateSystemIdRef;
            motion.Type = Type;
            motion.Actuation = Actuation;
            motion.Axis = Axis;
            motion.Origin = Origin;
            if (Transformation != null) motion.Transformation = Transformation.ToTransformation();
            motion.Description = Description;
            return motion;
        }
    }
}
