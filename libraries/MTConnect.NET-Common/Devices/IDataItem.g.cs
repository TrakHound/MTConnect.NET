// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    /// <summary>
    /// Information reported about a piece of equipment.
    /// </summary>
    public partial interface IDataItem
    {
        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// </summary>
        MTConnect.Devices.DataItemCategory Category { get; }
        
        /// <summary>
        /// Identifier attribute of the Composition that the reported data is most closely associated.
        /// </summary>
        string CompositionId { get; }
        
        /// <summary>
        /// Organize a set of expected values that can be reported for a DataItem.
        /// </summary>
        MTConnect.Devices.IConstraints Constraints { get; }
        
        /// <summary>
        /// For measured values relative to a coordinate system like Position, the coordinate system used may be reported.**DEPRECATED** in *Version 2.0*. Replaced by coordinateSystemIdRef.
        /// </summary>
        MTConnect.Devices.DataItemCoordinateSystem CoordinateSystem { get; }
        
        /// <summary>
        /// Associated CoordinateSystem context for the DataItem.
        /// </summary>
        string CoordinateSystemIdRef { get; }
        
        /// <summary>
        /// Defines the meaning of Entry and Cell elements associated with the DataItem when the representation is either `DATA` or `TABLE`.
        /// </summary>
        MTConnect.Devices.IDataItemDefinition Definition { get; }
        
        /// <summary>
        /// Indication signifying whether each value reported for the Observation is significant and whether duplicate values are to be suppressed.If a value is not defined for discrete, the default value **MUST** be `false`.
        /// </summary>
        bool Discrete { get; }
        
        /// <summary>
        /// Provides a means to control when an agent records updated information for a DataItem.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IFilter> Filters { get; }
        
        /// <summary>
        /// Unique identifier for this data item.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Starting value for a DataItem as well as the value to be set for the DataItem after a reset event.
        /// </summary>
        string InitialValue { get; }
        
        /// <summary>
        /// Name of the data item.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Used to convert the reported value to represent the original measured value.
        /// </summary>
        int NativeScale { get; }
        
        /// <summary>
        /// Native units of measurement for the reported value of the data item.
        /// </summary>
        string NativeUnits { get; }
        
        /// <summary>
        /// Association between a DataItem and another entity.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Devices.IAbstractDataItemRelationship> Relationships { get; }
        
        /// <summary>
        /// Description of a means to interpret data consisting of multiple data points or samples reported as a single value.  If representation is not specified, it **MUST** be determined to be `VALUE`.
        /// </summary>
        MTConnect.Devices.DataItemRepresentation Representation { get; }
        
        /// <summary>
        /// Type of event that may cause a reset to occur.
        /// </summary>
        MTConnect.Devices.DataItemResetTrigger? ResetTrigger { get; }
        
        /// <summary>
        /// Rate at which successive samples of a data item are recorded by a piece of equipment.
        /// </summary>
        double SampleRate { get; }
        
        /// <summary>
        /// Number of significant digits in the reported value.
        /// </summary>
        int? SignificantDigits { get; }
        
        /// <summary>
        /// Identifies the Component, DataItem, or Composition from which a measured value originates.
        /// </summary>
        MTConnect.Devices.ISource Source { get; }
        
        /// <summary>
        /// Type of statistical calculation performed on a series of data samples to provide the reported data value.
        /// </summary>
        MTConnect.Devices.DataItemStatistic? Statistic { get; }
        
        /// <summary>
        /// Sub-categorization of the data item type.
        /// </summary>
        string SubType { get; }
        
        /// <summary>
        /// Type of data being measured. See DataItem Types.
        /// </summary>
        string Type { get; }
        
        /// <summary>
        /// Unit of measurement for the reported value of the data item.
        /// </summary>
        string Units { get; }
    }
}