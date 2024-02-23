// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = EAID_1F084FBF_2AC7_41f6_8485_C356E6D7A9C1

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Movement of the component relative to a coordinate system.
    /// </summary>
    public class Motion : IMotion
    {
        public const string DescriptionText = "Movement of the component relative to a coordinate system.";


        /// <summary>
        /// Describes if this component is actuated directly or indirectly as a result of other motion.
        /// </summary>
        public MTConnect.Devices.Configurations.MotionActuationType Actuation { get; set; }
        
        /// <summary>
        /// Axis along or around which the Component moves relative to a coordinate system.
        /// </summary>
        public MTConnect.UnitVector3D Axis { get; set; }
        
        /// <summary>
        /// Coordinate system within which the kinematic motion occurs.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        public MTConnect.Devices.IDescription Description { get; set; }
        
        /// <summary>
        /// Unique identifier for this element.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Coordinates of the origin position of a coordinate system.
        /// </summary>
        public MTConnect.UnitVector3D Origin { get; set; }
        
        /// <summary>
        /// Id.The kinematic chain connects all components using the parent relations. All motion is connected to the motion of the parent. The first node in the chain will not have a parent.
        /// </summary>
        public string ParentIdRef { get; set; }
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        public MTConnect.Devices.Configurations.ITransformation Transformation { get; set; }
        
        /// <summary>
        /// Type of motion.
        /// </summary>
        public MTConnect.Devices.Configurations.MotionType Type { get; set; }
    }
}