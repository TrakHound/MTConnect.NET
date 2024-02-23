// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class CompositionDescriptions
    {
        /// <summary>
        /// Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.
        /// </summary>
        public const string Configuration = "Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.";
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        public const string Description = "Descriptive content.";
        
        /// <summary>
        /// Unique identifier for the Composition element.
        /// </summary>
        public const string Id = "Unique identifier for the Composition element.";
        
        /// <summary>
        /// Name of the Composition element.
        /// </summary>
        public const string Name = "Name of the Composition element.";
        
        /// <summary>
        /// Type of Composition.
        /// </summary>
        public const string Type = "Type of Composition.";
        
        /// <summary>
        /// Universally unique identifier for the Composition.
        /// </summary>
        public const string Uuid = "Universally unique identifier for the Composition.";
        
        /// <summary>
        /// Logical or physical entity that provides a capability.
        /// </summary>
        public const string Components = "Logical or physical entity that provides a capability.";
        
        /// <summary>
        /// Functional part of a piece of equipment contained within a Component.
        /// </summary>
        public const string Compositions = "Functional part of a piece of equipment contained within a Component.";
        
        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        public const string CoordinateSystemIdRef = "Specifies the CoordinateSystem for this Component and its children.";
        
        /// <summary>
        /// Common name associated with Component.
        /// </summary>
        public const string NativeName = "Common name associated with Component.";
        
        /// <summary>
        /// Pointer to information that is associated with another entity defined elsewhere in the MTConnectDevices entity for a piece of equipment.
        /// </summary>
        public const string References = "Pointer to information that is associated with another entity defined elsewhere in the MTConnectDevices entity for a piece of equipment.";
        
        /// <summary>
        /// Interval in milliseconds between the completion of the reading of the data associated with the Component until the beginning of the next sampling of that data.This information may be used by client software applications to understand how often information from a Component is expected to be refreshed.The refresh rate for data from all child Component entities will be thesampleInterval provided for the child Component.
        /// </summary>
        public const string SampleInterval = "Interval in milliseconds between the completion of the reading of the data associated with the Component until the beginning of the next sampling of that data.This information may be used by client software applications to understand how often information from a Component is expected to be refreshed.The refresh rate for data from all child Component entities will be thesampleInterval provided for the child Component.";
        
        /// <summary>
        /// SampleInterval.
        /// </summary>
        public const string SampleRate = "SampleInterval.";
    }
}