// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public enum DataItemRepresentation
    {
        /// <summary>
        /// Reported value(s) are represented as a set of key-value pair.Each reported value in the data set **MUST** have a unique key.
        /// </summary>
        DATA_SET,
        
        /// <summary>
        /// **DEPRECATED** as a representation in *MTConnect Version 1.5*. Replaced by the discrete,DataItem attribute of a DataItem.
        /// </summary>
        DISCRETE,
        
        /// <summary>
        /// Two dimensional set of key-value pair where the Entry represents a row, and the value is a set of key-value pair Cell elements. A table follows the same behavior as the data set for change tracking, clearing, and history. When an Entry changes, all Cell elements update as a single unit following the behavior of a data set.> Note: It is best to use the Variable DataItem type if the Cell elements represent multiple semantic types.Each Entry in the table **MUST** have a unique key. Each Cell of each Entry in the table **MUST** have a unique key.See Representation in Observation Information Model, for a description of Entry and Cell elements.
        /// </summary>
        TABLE,
        
        /// <summary>
        /// Series of sampled data.The data is reported for a specified number of samples and each sample is reported with a fixed period.
        /// </summary>
        TIME_SERIES,
        
        /// <summary>
        /// Measured value of the sample data.If no representation,DataItem is specified for a data item, the representation,DataItem **MUST** be determined to be `VALUE`.
        /// </summary>
        VALUE
    }
}