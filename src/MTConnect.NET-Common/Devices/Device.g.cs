// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1620240839406_285612_1596

namespace MTConnect.Devices
{
    /// <summary>
    /// Component composed of a piece of equipment that produces observation about itself.
    /// </summary>
    public partial class Device : IDevice
    {
        public const string DescriptionText = "Component composed of a piece of equipment that produces observation about itself.";


        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        public string Hash { get; set; }
        
        /// <summary>
        /// MTConnect version of the Device Information Model used to configure the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        public System.Version MTConnectVersion { get; set; }
        
        /// <summary>
        /// Name of an element or a piece of equipment.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Universally unique identifier for the element.
        /// </summary>
        public string Uuid { get; set; }
        
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
        /// **deprecated** in *MTConnect Version 1.2*. Replaced by sampleInterval,Component.
        /// </summary>
        public double SampleRate { get; set; }
    }
}