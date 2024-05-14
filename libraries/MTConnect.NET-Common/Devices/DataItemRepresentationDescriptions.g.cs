// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class DataItemRepresentationDescriptions
    {
        /// <summary>
        /// Reported value(s) are represented as a set of key-value pair.Each reported value in the data set **MUST** have a unique key.
        /// </summary>
        public const string DATA_SET = "Reported value(s) are represented as a set of key-value pair.Each reported value in the data set **MUST** have a unique key.";
        
        /// <summary>
        /// Discrete.
        /// </summary>
        public const string DISCRETE = "Discrete.";
        
        /// <summary>
        /// Two dimensional set of key-value pair where the Entry represents a row, and the value is a set of key-value pair Cell elements. A table follows the same behavior as the data set for change tracking, clearing, and history. When an Entry changes, all Cell elements update as a single unit following the behavior of a data set.> Note: It is best to use Variable if the Cell entities represent multiple semantic types.Each Entry in the table **MUST** have a unique key. Each Cell of each Entry in the table **MUST** have a unique key.See Representation in Observation Information Model, for a description of Entry and Cell elements.
        /// </summary>
        public const string TABLE = "Two dimensional set of key-value pair where the Entry represents a row, and the value is a set of key-value pair Cell elements. A table follows the same behavior as the data set for change tracking, clearing, and history. When an Entry changes, all Cell elements update as a single unit following the behavior of a data set.> Note: It is best to use Variable if the Cell entities represent multiple semantic types.Each Entry in the table **MUST** have a unique key. Each Cell of each Entry in the table **MUST** have a unique key.See Representation in Observation Information Model, for a description of Entry and Cell elements.";
        
        /// <summary>
        /// Series of sampled data.The data is reported for a specified number of samples and each sample is reported with a fixed period.
        /// </summary>
        public const string TIME_SERIES = "Series of sampled data.The data is reported for a specified number of samples and each sample is reported with a fixed period.";
        
        /// <summary>
        /// Measured value of the sample data.representation **MUST** be determined to be `VALUE`.
        /// </summary>
        public const string VALUE = "Measured value of the sample data.representation **MUST** be determined to be `VALUE`.";


        public static string Get(DataItemRepresentation value)
        {
            switch (value)
            {
                case DataItemRepresentation.DATA_SET: return "Reported value(s) are represented as a set of key-value pair.Each reported value in the data set **MUST** have a unique key.";
                case DataItemRepresentation.DISCRETE: return "Discrete.";
                case DataItemRepresentation.TABLE: return "Two dimensional set of key-value pair where the Entry represents a row, and the value is a set of key-value pair Cell elements. A table follows the same behavior as the data set for change tracking, clearing, and history. When an Entry changes, all Cell elements update as a single unit following the behavior of a data set.> Note: It is best to use Variable if the Cell entities represent multiple semantic types.Each Entry in the table **MUST** have a unique key. Each Cell of each Entry in the table **MUST** have a unique key.See Representation in Observation Information Model, for a description of Entry and Cell elements.";
                case DataItemRepresentation.TIME_SERIES: return "Series of sampled data.The data is reported for a specified number of samples and each sample is reported with a fixed period.";
                case DataItemRepresentation.VALUE: return "Measured value of the sample data.representation **MUST** be determined to be `VALUE`.";
            }

            return null;
        }
    }
}