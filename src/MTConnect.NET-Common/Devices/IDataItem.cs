// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.DataItems;
using MTConnect.Observations.Input;
using System;
using System.Collections.Generic;

namespace MTConnect.Devices
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by Elements representing various types of DataItem XML Elements.
    /// There can be mulitple types of DataItem XML Elements in the document.
    /// </summary>
    public interface IDataItem
    {
        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// Each category of information will provide similar characteristics in its representation.
        /// The available options are SAMPLE, EVENT, or CONDITION.
        /// </summary>
        DataItemCategory Category { get; }

        /// <summary>
        /// The unique identifier for this DataItem.
        /// The id attribute MUST be unique across the entire document including the ids for components.
        /// An XML ID-type.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The coordinate system being used.
        /// The available values for coordinateSystem are WORK and MACHINE.
        /// </summary>
        DataItemCoordinateSystem CoordinateSystem { get; }

        /// <summary>
        /// The associated CoordinateSystem context for the DataItem.
        /// </summary>
        string CoordinateSystemIdRef { get; }

        /// <summary>
        /// The name of the DataItem. A name is provided as an additional human readable identifier for this DataItem in addtion to the id.
        /// It is not required and will be implementation dependent.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The multiplier for the native units.
        /// The received data MAY be divided by this value before conversion.
        /// If provided, the value MUST be numeric.
        /// </summary>
        double NativeScale { get; }

        /// <summary>
        /// The native units used by the Component.
        /// These units will be converted before they are delivered to the application.
        /// </summary>
        string NativeUnits { get; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        string SubType { get; }

        /// <summary>
        /// Data calculated specific to a DataItem.
        /// Examples of statistic are AVERAGE, MINIMUM, MAXIMUM, ROOT_MEAN_SQUARE, RANGE, MEDIAN, MODE and STANDARD_DEVIATION.
        /// </summary>
        DataItemStatistic Statistic { get; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        string Units { get; }

        /// <summary>
        /// The reate at which successive samples of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a floating point number.
        /// For example, a rate 1 per 10 seconds would be 0.1.
        /// </summary>
        double SampleRate { get; }

        /// <summary>
        /// An indication signifying whether each value reported for the Data Entity is significant and whether duplicate values are to be suppressed.
        /// </summary>
        bool Discrete { get; }

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        DataItemRepresentation Representation { get; }

        /// <summary>
        /// The number of significant digits in the reported value.
        /// This is used by applications to dtermine accuracy of values.
        /// This SHOULD be specified for all numeric values.
        /// </summary>
        int SignificantDigits { get; }

        /// <summary>
        /// The identifier attribute of the Composition element that the reported data is most closely associated.
        /// </summary>
        string CompositionId { get; }

        /// <summary>
        /// Source is an XML element that indentifies the Component, Subcomponent, or DataItem representing the part of the device from which a measured value originates.
        /// </summary>
        ISource Source { get; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        IConstraints Constraints { get; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        IEnumerable<IFilter> Filters { get; }

        /// <summary>
        /// InitialValue is an optional XML element that defines the starting value for a data item as well as the value to be set for the data item after a reset event.
        /// </summary>
        string InitialValue { get; }

        /// <summary>
        /// ResetTrigger is an XML element that describes the reset action that causes a reset to occur.
        /// </summary>
        DataItemResetTrigger ResetTrigger { get; }

        /// <summary>
        /// The Definition provides additional descriptive information for any DataItem representations.
        /// When the representation is either DATA_SET or TABLE, it gives the specific meaning of a key and MAY provide a Description, type, and units for semantic interpretation of data.
        /// </summary>
        IDataItemDefinition Definition { get; }

        /// <summary>
        /// Relationships organizes DataItemRelationship and SpecificationRelationship.
        /// </summary>
        IEnumerable<IRelationship> Relationships { get; }


        /// <summary>
        /// The Device that this DataItem is associated with
        /// </summary>
        IDevice Device { get; }

        /// <summary>
        /// The Container (Component or Device) that this DataItem is directly associated with
        /// </summary>
        IContainer Container { get; }


        /// <summary>
        /// A MD5 Hash of the DataItem that can be used to compare DataItem objects
        /// </summary>
        string ChangeId { get; }


        /// <summary>
        /// The text description that describes what the DataItem Type represents
        /// </summary>
        string TypeDescription { get; }

        /// <summary>
        /// The text description that describes what the DataItem SubType represents
        /// </summary>
        string SubTypeDescription { get; }


        /// <summary>
        /// The full path of IDs that describes the location of the DataItem in the Device
        /// </summary>
        string IdPath { get; }

        /// <summary>
        /// The list of IDs (in order) that describes the location of the DataItem in the Device
        /// </summary>
        string[] IdPaths { get; }

        /// <summary>
        /// The full path of Types that describes the location of the DataItem in the Device
        /// </summary>
        string TypePath { get; }

        /// <summary>
        /// The list of Types (in order) that describes the location of the DataItem in the Device
        /// </summary>
        string[] TypePaths { get; }


        /// <summary>
        /// The maximum MTConnect Version that this DataItem Type is valid 
        /// (if set, this indicates that the Type has been Deprecated in the MTConnect Standard version specified)
        /// </summary>
        Version MaximumVersion { get; }

        /// <summary>
        /// The minimum MTConnect Version that this DataItem Type is valid 
        /// </summary>
        Version MinimumVersion { get; }


        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        ValidationResult IsValid(Version mtconnectVersion, IObservationInput observation);
    }
}
