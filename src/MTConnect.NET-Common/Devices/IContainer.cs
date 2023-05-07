// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.References;
using System;
using System.Collections.Generic;

namespace MTConnect.Devices
{
    public interface IContainer : IMTConnectEntity
    {
        /// <summary>
        /// The unique identifier for this Component in the document.
        /// An id MUST be unique across all the id attributes in the document.
        /// An XML ID-type.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The type of component
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The name of the Component.
        /// Name is an optional attribute.
        /// If provided, Name MUST be unique within a type of Component or subComponent.
        /// It is recommended that duplicate names SHOULD NOT occur within a Device.
        /// An NMTOKEN XML type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The name the device manufacturer assigned to the Component.
        /// If the native name is not provided it MUST be the Name.
        /// </summary>
        string NativeName { get; }

        /// <summary>
        /// The interval in milliseconds between the completion of the reading of one sample of data from a component until the beginning of the next sampling of that data.
        /// This is the number of milliseconds between data captures. 
        /// If the sample interval is smaller than one millisecond, the number can be represented as a floating point number.
        /// For example, an interval of 100 microseconds would be 0.1.
        /// </summary>
        double SampleInterval { get; }

        /// <summary>
        /// DEPRECATED IN REL. 1.2 (REPLACED BY sampleInterval)
        /// </summary>
        double SampleRate { get; }

        /// <summary>
        /// A unique identifier that will only refer to this Component.
        /// For example, this can be the manufacturer's code or the serial number.
        /// The uuid should be alphanumeric and not exceeding 255 characters.
        /// An NMTOKEN XML type.
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// Specifies the CoordinateSystem for this Component and its children.
        /// </summary>
        string CoordinateSystemIdRef { get; }

        /// <summary>
        /// An element that can contain any descriptive content. 
        /// This can contain information about the Component and manufacturer specific details.
        /// </summary>
        IDescription Description { get; }

        /// <summary>
        /// An XML element that contains technical information about a piece of equipment describing its physical layout or functional characteristics.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// A container for the DataItem elements associated with this Component element.
        /// </summary>
        IEnumerable<IDataItem> DataItems { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        IEnumerable<IReference> References { get; }


        /// <summary>
        /// The Container (Component or Device) that this DataItem is directly associated with
        /// </summary>
        IContainer Parent { get; set; }


        /// <summary>
        /// A MD5 Hash of the Component that can be used to compare Component objects
        /// </summary>
        string ChangeId { get; }


        /// <summary>
        /// The text description that describes what the Component Type represents
        /// </summary>
        string TypeDescription { get; }


        /// <summary>
        /// The full path of IDs that describes the location of the Component in the Device
        /// </summary>
        string IdPath { get; }

        /// <summary>
        /// The list of IDs (in order) that describes the location of the Component in the Device
        /// </summary>
        string[] IdPaths { get; }

        /// <summary>
        /// The full path of Types that describes the location of the Component in the Device
        /// </summary>
        string TypePath { get; }

        /// <summary>
        /// The list of Types (in order) that describes the location of the Component in the Device
        /// </summary>
        string[] TypePaths { get; }


        /// <summary>
        /// The maximum MTConnect Version that this Component Type is valid 
        /// (if set, this indicates that the Type has been Deprecated in the MTConnect Standard version specified)
        /// </summary>
        Version MaximumVersion { get; }

        /// <summary>
        /// The minimum MTConnect Version that this Component Type is valid 
        /// </summary>
        Version MinimumVersion { get; }
    }
}