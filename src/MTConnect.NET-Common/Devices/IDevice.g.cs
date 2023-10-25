// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Component composed of a piece of equipment that produces observation about itself.
    /// </summary>
    public partial interface IDevice : IComponent
    {
        /// <summary>
        /// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
        /// </summary>
        string Hash { get; }
        
        /// <summary>
        /// MTConnect version of the Device Information Model used to configure the information to be published for a piece of equipment in an MTConnect Response Document.
        /// </summary>
        string MTConnectVersion { get; }
        
        /// <summary>
        /// Name of an element or a piece of equipment.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Universally unique identifier for the element.
        /// </summary>
        string Uuid { get; }
        
        /// <summary>
        /// Logical or physical entity that provides a capability.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IComponent> Components { get; }
        
        /// <summary>
        /// Functional part of a piece of equipment contained within a Component.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IComposition> Compositions { get; }
        
        /// <summary>
        /// Technical information about an entity describing its physical layout, functional characteristics, and relationships with other entities.
        /// </summary>
        MTConnect.Devices.Configurations.IConfiguration Configuration { get; }
        
        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        string CoordinateSystemIdRef { get; }
        
        /// <summary>
        /// Descriptive content.
        /// </summary>
        MTConnect.Devices.IDescription Description { get; }
        
        /// <summary>
        /// Unique identifier for the Component.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Common name associated with Component.
        /// </summary>
        string NativeName { get; }
        
        /// <summary>
        /// Pointer to information that is associated with another entity defined elsewhere in the MTConnectDevices entity for a piece of equipment.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.References.IReference> References { get; }
        
        /// <summary>
        /// Interval in milliseconds between the completion of the reading of the data associated with the Component until the beginning of the next sampling of that data.This information may be used by client software applications to understand how often information from a Component is expected to be refreshed.The refresh rate for data from all child Component entities will be thesame as for the parent Component element unless specifically overridden by another sampleInterval provided for the childComponent.
        /// </summary>
        double SampleInterval { get; }
        
        /// <summary>
        /// **deprecated** in *MTConnect Version 1.2*. Replaced by sampleInterval,Component.
        /// </summary>
        double SampleRate { get; }
    }
}