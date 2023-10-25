// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_002C94B7_1257_49be_8EAA_CE7FCD7AFF8A

namespace MTConnect.Devices
{
    /// <summary>
    /// Information reported about a piece of equipment.
    /// </summary>
    public partial class DataItem : IDataItem
    {
        public const string DescriptionText = "Information reported about a piece of equipment.";


        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// </summary>
        public MTConnect.Devices.DataItemCategory Category { get; set; }
        
        /// <summary>
        /// Identifier attribute of the Composition that the reported data is most closely associated.
        /// </summary>
        public string CompositionId { get; set; }
        
        /// <summary>
        /// Organize a set of expected values that can be reported for a DataItem.
        /// </summary>
        public MTConnect.Devices.IConstraints Constraints { get; set; }
        
        /// <summary>
        /// For measured values relative to a coordinate system like Position, the coordinate system used may be reported.**DEPRECATED** in *Version 2.0*. Replaced by coordinateSystemIdRef.
        /// </summary>
        public MTConnect.Devices.DataItemCoordinateSystem CoordinateSystem { get; set; }
        
        /// <summary>
        /// Associated CoordinateSystem context for the DataItem.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }
        
        /// <summary>
        /// Defines the meaning of Entry and Cell elements associated with the DataItem when the representation is either `DATA` or `TABLE`.
        /// </summary>
        public MTConnect.Devices.IDataItemDefinition Definition { get; set; }
        
        /// <summary>
        /// Indication signifying whether each value reported for the Observation is significant and whether duplicate values are to be suppressed.If a value is not defined for discrete, the default value **MUST** be `false`.
        /// </summary>
        public bool Discrete { get; set; }
        
        /// <summary>
        /// Provides a means to control when an agent records updated information for a DataItem.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Devices.IFilter> Filters { get; set; }
        
        /// <summary>
        /// Unique identifier for this data item.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Starting value for a DataItem as well as the value to be set for the DataItem after a reset event.
        /// </summary>
        public string InitialValue { get; set; }
        
        /// <summary>
        /// Name of the data item.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Used to convert the reported value to represent the original measured value.
        /// </summary>
        public int NativeScale { get; set; }
        
        /// <summary>
        /// Native units of measurement for the reported value of the data item.
        /// </summary>
        public string NativeUnits { get; set; }
        
        /// <summary>
        /// Association between a DataItem and another entity.
        /// </summary>
        public System.Collections.Generic.IEnumerable<AbstractDataItemRelationship> Relationships { get; set; }
        
        /// <summary>
        /// Description of a means to interpret data consisting of multiple data points or samples reported as a single value.  If representation is not specified, it **MUST** be determined to be `VALUE`.
        /// </summary>
        public MTConnect.Devices.DataItemRepresentation Representation { get; set; }
        
        /// <summary>
        /// Type of event that may cause a reset to occur.
        /// </summary>
        public MTConnect.Devices.ResetTrigger ResetTrigger { get; set; }
        
        /// <summary>
        /// Rate at which successive samples of a data item are recorded by a piece of equipment.
        /// </summary>
        public double SampleRate { get; set; }
        
        /// <summary>
        /// Number of significant digits in the reported value.
        /// </summary>
        public int SignificantDigits { get; set; }
        
        /// <summary>
        /// Identifies the Component, DataItem, or Composition from which a measured value originates.
        /// </summary>
        public MTConnect.Devices.ISource Source { get; set; }
        
        /// <summary>
        /// Type of statistical calculation performed on a series of data samples to provide the reported data value.
        /// </summary>
        public MTConnect.Devices.DataItemStatistic Statistic { get; set; }
        
        /// <summary>
        /// Sub-categorization of the data item type.
        /// </summary>
        public string SubType { get; set; }
        
        /// <summary>
        /// Type of data being measured. See DataItem Types.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Unit of measurement for the reported value of the data item.
        /// </summary>
        public string Units { get; set; }
    }
}