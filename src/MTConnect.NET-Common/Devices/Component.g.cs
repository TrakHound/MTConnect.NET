// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_8548C620_467A_4f50_9A22_58D84B7E8779

namespace MTConnect.Devices
{
    /// <summary>
    /// Logical or physical entity that provides a capability.
    /// </summary>
    public partial class Component : IComponent
    {
        public const string DescriptionText = "Logical or physical entity that provides a capability.";


        /// <summary>
        /// Logical or physical entity that provides a capability.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.IComponent> Components { get; set; }
        
        /// <summary>
        /// Functional part of a piece of equipment contained within a Component.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.IComposition> Compositions { get; set; }
        
        /// <summary>
        /// Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.
        /// </summary>
        public MTConnect.Devices.Configurations.IConfiguration Configuration { get; set; }
        
        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        public MTConnect.Devices.IDescription Description { get; set; }
        
        /// <summary>
        /// Unique identifier for the Component.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Name of the Component.When provided, name **MUST** be unique for all child Component entities of a parent Component.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Common name associated with Component.
        /// </summary>
        public string NativeName { get; set; }
        
        /// <summary>
        /// Pointer to information that is associated with another entity defined elsewhere in the MTConnectDevices entity for a piece of equipment.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.References.IReference> References { get; set; }
        
        /// <summary>
        /// Interval in milliseconds between the completion of the reading of the data associated with the Component until the beginning of the next sampling of that data.This information may be used by client software applications to understand how often information from a Component is expected to be refreshed.The refresh rate for data from all child Component entities will be thesame as for the parent Component element unless specifically overridden by another sampleInterval provided for the childComponent.
        /// </summary>
        public double SampleInterval { get; set; }
        
        /// <summary>
        /// **DEPRECATED** in *MTConnect Version 1.2*. Replaced by sampleInterval,Component.
        /// </summary>
        public double SampleRate { get; set; }
        
        /// <summary>
        /// Universally unique identifier for the Component.
        /// </summary>
        public string Uuid { get; set; }
    }
}