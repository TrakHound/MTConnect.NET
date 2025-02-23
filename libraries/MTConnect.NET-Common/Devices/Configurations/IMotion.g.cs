// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Movement of the component relative to a coordinate system.
    /// </summary>
    public interface IMotion
    {
        /// <summary>
        /// Describes if this component is actuated directly or indirectly as a result of other motion.
        /// </summary>
        MTConnect.Devices.Configurations.MotionActuationType Actuation { get; }
        
        /// <summary>
        /// Axis along or around which the Component moves relative to a coordinate system.
        /// </summary>
        MTConnect.UnitVector3D Axis { get; }
        
        /// <summary>
        /// Coordinate system within which the kinematic motion occurs.
        /// </summary>
        string CoordinateSystemIdRef { get; }
        
        /// <summary>
        /// Textual description for Motion.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Unique identifier for this element.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Coordinates of the origin position of a coordinate system.
        /// </summary>
        MTConnect.UnitVector3D Origin { get; }
        
        /// <summary>
        /// Id.The kinematic chain connects all components using the parent relations. All motion is connected to the motion of the parent. The first node in the chain will not have a parent.
        /// </summary>
        string ParentIdRef { get; }
        
        /// <summary>
        /// Process of transforming to the origin position of the coordinate system from a parent coordinate system using Translation and Rotation.
        /// </summary>
        MTConnect.Devices.Configurations.ITransformation Transformation { get; }
        
        /// <summary>
        /// Type of motion.
        /// </summary>
        MTConnect.Devices.Configurations.MotionType Type { get; }
    }
}