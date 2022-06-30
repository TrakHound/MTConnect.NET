// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.References;
using System;
using System.Collections.Generic;

namespace MTConnect.Devices
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    public interface IComponent
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

        IEnumerable<IDataItem> DataItems { get; set; }

        IEnumerable<IComponent> Components { get; set; }

        /// <summary>
        /// A container for the Composition elements associated with this Component element.
        /// </summary>
        IEnumerable<IComposition> Compositions { get; set; }

        /// <summary>
        /// An XML container consisting of one or more types of Reference XML elements.
        /// </summary>
        IEnumerable<IReference> References { get; }

        /// <summary>
        /// A MD5 Hash of the Component that can be used to compare Component objects
        /// </summary>
        string ChangeId { get; }

        string TypeDescription { get; }

        bool IsOrganizer { get; }

        Version MaximumVersion { get; }

        Version MinimumVersion { get; }


        /// <summary>
        /// Return a list of All DataItems
        /// </summary>
        IEnumerable<IDataItem> GetDataItems();

        /// <summary>
        /// Return the DataItem matching either the ID, Name, or Source of the specified Key
        /// </summary>
        IDataItem GetDataItemByKey(string dataItemKey);

        /// <summary>
        /// Return a list of All Components
        /// </summary>
        IEnumerable<IComponent> GetComponents();
    }
}
