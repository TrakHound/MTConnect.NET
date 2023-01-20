// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    public static class DataItemAttributeDescriptions
    {
        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// Each category of information will provide similar characteristics in its representation.
        /// The available options are SAMPLE, EVENT, or CONDITION.
        /// </summary>
        public const string Category = "Specifies the kind of information provided by a data item. Each category of information will provide similar characteristics in its representation. The available options are SAMPLE, EVENT, or CONDITION.";

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        public const string Representation = "Data consisting of multiple data points or samples or a file presented as a single DataItem. Each representation will have a unique format defined for each representation.";

        /// <summary>
        /// The unique identifier for this DataItem.
        /// The id attribute MUST be unique across the entire document including the ids for components.
        /// An XML ID-type.
        /// </summary>
        public const string Id = "The unique identifier for this DataItem.";

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        public const string Type = "The type of data being measured.";

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        public const string SubType = "A sub-categorization of the data item type.";

        /// <summary>
        /// The name of the DataItem. A name is provided as an additional human readable identifier for this DataItem in addtion to the id.
        /// It is not required and will be implementation dependent.
        /// </summary>
        public const string Name = "The name of the DataItem. A name is provided as an additional human readable identifier for this DataItem in addtion to the id.";

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        public const string Units = "Units MUST be present for all DataItem elements in the SAMPLE category.";

        /// <summary>
        /// The rate at which successive samples of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a floating point number.
        /// For example, a rate 1 per 10 seconds would be 0.1.
        /// </summary>
        public const string SampleRate = "The rate at which successive samples of a DataItem are recorded.";

        /// <summary>
        /// An indication signifying whether each value reported for the Data Entity is significant and whether duplicate values are to be suppressed.
        /// </summary>
        public const string Discrete = "An indication signifying whether each value reported for the Data Entity is significant and whether duplicate values are to be suppressed.";

        /// <summary>
        /// The multiplier for the native units.
        /// The received data MAY be divided by this value before conversion.
        /// If provided, the value MUST be numeric.
        /// </summary>
        public const string NativeScale = "The multiplier for the native units.";
       
        /// <summary>
        /// The native units used by the Component.
        /// These units will be converted before they are delivered to the application.
        /// </summary>
        public const string NativeUnits = "The native units used by the Component.";

        /// <summary>
        /// The coordinate system being used.
        /// The available values for coordinateSystem are WORK and MACHINE.
        /// </summary>
        public const string CoordinateSystem = "The coordinate system being used.";

        /// <summary>
        /// The associated CoordinateSystem context for the DataItem.
        /// </summary>
        public const string CoordinateSystemIdRef = "The associated CoordinateSystem context for the DataItem.";

        /// <summary>
        /// Data calculated specific to a DataItem.
        /// Examples of statistic are AVERAGE, MINIMUM, MAXIMUM, ROOT_MEAN_SQUARE, RANGE, MEDIAN, MODE and STANDARD_DEVIATION.
        /// </summary>
        public const string Statistic = "Data calculated specific to a DataItem.";

        /// <summary>
        /// The number of significant digits in the reported value.
        /// This is used by applications to determine accuracy of values.
        /// This SHOULD be specified for all numeric values.
        /// </summary>
        public const string SignificantDigits = "The number of significant digits in the reported value. This is used by applications to determine accuracy of values.";

        /// <summary>
        /// The identifier attribute of the Composition element that the reported data is most closely associated.
        /// </summary>
        public const string CompositionId = "The identifier attribute of the Composition element that the reported data is most closely associated.";

        /// <summary>
        /// InitialValue is an optional XML element that defines the starting value for a data item as well as the value to be set for the data item after a reset event.
        /// </summary>
        public const string InitialValue = "InitialValue is an optional XML element that defines the starting value for a data item as well as the value to be set for the data item after a reset event.";

        /// <summary>
        /// ResetTrigger is an XML element that describes the reset action that causes a reset to occur.
        /// </summary>
        public const string ResetTrigger = "ResetTrigger is an XML element that describes the reset action that causes a reset to occur.";
    }
}