// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class DataItemDescriptions
    {
        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// </summary>
        public const string Category = "Specifies the kind of information provided by a data item.";
        
        /// <summary>
        /// Identifier attribute of the Composition that the reported data is most closely associated.
        /// </summary>
        public const string CompositionId = "Identifier attribute of the Composition that the reported data is most closely associated.";
        
        /// <summary>
        /// Organize a set of expected values that can be reported for a DataItem.
        /// </summary>
        public const string Constraints = "Organize a set of expected values that can be reported for a DataItem.";
        
        /// <summary>
        /// For measured values relative to a coordinate system like Position, the coordinate system used may be reported.**DEPRECATED** in *Version 2.0*. Replaced by coordinateSystemIdRef.
        /// </summary>
        public const string CoordinateSystem = "For measured values relative to a coordinate system like Position, the coordinate system used may be reported.**DEPRECATED** in *Version 2.0*. Replaced by coordinateSystemIdRef.";
        
        /// <summary>
        /// Associated CoordinateSystem context for the DataItem.
        /// </summary>
        public const string CoordinateSystemIdRef = "Associated CoordinateSystem context for the DataItem.";
        
        /// <summary>
        /// Defines the meaning of Entry and Cell elements associated with the DataItem when the representation is either `DATA` or `TABLE`.
        /// </summary>
        public const string Definition = "Defines the meaning of Entry and Cell elements associated with the DataItem when the representation is either `DATA` or `TABLE`.";
        
        /// <summary>
        /// Indication signifying whether each value reported for the Observation is significant and whether duplicate values are to be suppressed.If a value is not defined for discrete, the default value **MUST** be `false`.
        /// </summary>
        public const string Discrete = "Indication signifying whether each value reported for the Observation is significant and whether duplicate values are to be suppressed.If a value is not defined for discrete, the default value **MUST** be `false`.";
        
        /// <summary>
        /// Provides a means to control when an agent records updated information for a DataItem.
        /// </summary>
        public const string Filters = "Provides a means to control when an agent records updated information for a DataItem.";
        
        /// <summary>
        /// Unique identifier for this data item.
        /// </summary>
        public const string Id = "Unique identifier for this data item.";
        
        /// <summary>
        /// Starting value for a DataItem as well as the value to be set for the DataItem after a reset event.
        /// </summary>
        public const string InitialValue = "Starting value for a DataItem as well as the value to be set for the DataItem after a reset event.";
        
        /// <summary>
        /// Name of the data item.
        /// </summary>
        public const string Name = "Name of the data item.";
        
        /// <summary>
        /// Used to convert the reported value to represent the original measured value.
        /// </summary>
        public const string NativeScale = "Used to convert the reported value to represent the original measured value.";
        
        /// <summary>
        /// Native units of measurement for the reported value of the data item.
        /// </summary>
        public const string NativeUnits = "Native units of measurement for the reported value of the data item.";
        
        /// <summary>
        /// Association between a DataItem and another entity.
        /// </summary>
        public const string Relationships = "Association between a DataItem and another entity.";
        
        /// <summary>
        /// Description of a means to interpret data consisting of multiple data points or samples reported as a single value.  If representation is not specified, it **MUST** be determined to be `VALUE`.
        /// </summary>
        public const string Representation = "Description of a means to interpret data consisting of multiple data points or samples reported as a single value.  If representation is not specified, it **MUST** be determined to be `VALUE`.";
        
        /// <summary>
        /// Type of event that may cause a reset to occur.
        /// </summary>
        public const string ResetTrigger = "Type of event that may cause a reset to occur.";
        
        /// <summary>
        /// Rate at which successive samples of a data item are recorded by a piece of equipment.
        /// </summary>
        public const string SampleRate = "Rate at which successive samples of a data item are recorded by a piece of equipment.";
        
        /// <summary>
        /// Number of significant digits in the reported value.
        /// </summary>
        public const string SignificantDigits = "Number of significant digits in the reported value.";
        
        /// <summary>
        /// Identifies the Component, DataItem, or Composition from which a measured value originates.
        /// </summary>
        public const string Source = "Identifies the Component, DataItem, or Composition from which a measured value originates.";
        
        /// <summary>
        /// Type of statistical calculation performed on a series of data samples to provide the reported data value.
        /// </summary>
        public const string Statistic = "Type of statistical calculation performed on a series of data samples to provide the reported data value.";
        
        /// <summary>
        /// Sub-categorization of the data item type.
        /// </summary>
        public const string SubType = "Sub-categorization of the data item type.";
        
        /// <summary>
        /// Type of data being measured. See DataItem Types.
        /// </summary>
        public const string Type = "Type of data being measured. See DataItem Types.";
        
        /// <summary>
        /// Unit of measurement for the reported value of the data item.
        /// </summary>
        public const string Units = "Unit of measurement for the reported value of the data item.";
    }
}