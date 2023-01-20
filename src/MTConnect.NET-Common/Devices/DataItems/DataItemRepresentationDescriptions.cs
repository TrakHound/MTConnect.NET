// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.DataItems
{
    public static class DataItemRepresentationDescriptions
    {
        /// <summary>
        /// The measured value of the sample data.
        /// </summary>
        public const string VALUE = "The measured value of the sample data.";

        /// <summary>
        /// The reported value(s) are represented as a set of key-value pairs.
        /// </summary>
        public const string DATA_SET = "The reported value(s) are represented as a set of key-value pairs.";

        /// <summary>
        /// DEPRECATED as a representation in MTConnect Version. 1.5. Replaced by the discrete attribute for a Data Entity
        /// </summary>
        public const string DISCRETE = "DEPRECATED as a representation in MTConnect Version. 1.5. Replaced by the discrete attribute for a Data Entity";

        /// <summary>
        /// A series of sampled data.
        /// </summary>
        public const string TIME_SERIES = "A series of sampled data.";

        /// <summary>
        /// A Table is a two dimensional set of key-value pairs where the Entry represents a row,
        /// and the value is a set of key-value pair Cell elements.
        /// </summary>
        public const string TABLE = "A Table is a two dimensional set of key-value pairs where the Entry represents a row, and the value is a set of key-value pair Cell elements.";


        public static string Get(DataItemRepresentation dataItemRepresentation)
        {
            switch (dataItemRepresentation)
            {
                case DataItemRepresentation.VALUE: return VALUE;
                case DataItemRepresentation.DATA_SET: return DATA_SET;
                case DataItemRepresentation.DISCRETE: return DISCRETE;
                case DataItemRepresentation.TIME_SERIES: return TIME_SERIES;
                case DataItemRepresentation.TABLE: return TABLE;
            }

            return "";
        }
    }
}
