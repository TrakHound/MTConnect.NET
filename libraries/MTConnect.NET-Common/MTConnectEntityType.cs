// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect
{
    /// <summary>
    /// The top-level kinds of entity in the MTConnect information model, used to classify an entity without inspecting its concrete type.
    /// </summary>
    public enum MTConnectEntityType
    {
        /// <summary>
        /// A piece of equipment described by the device model.
        /// </summary>
        Device,

        /// <summary>
        /// A functional or physical part of a device.
        /// </summary>
        Component,

        /// <summary>
        /// A lower-level building block contained within a component.
        /// </summary>
        Composition,

        /// <summary>
        /// A definition of an item of streaming data produced by a device.
        /// </summary>
        DataItem,

        /// <summary>
        /// A reported value of a DataItem at a point in time.
        /// </summary>
        Observation,

        /// <summary>
        /// A piece of equipment-related information that is not streaming data, such as a cutting tool or file.
        /// </summary>
        Asset
    }
}
