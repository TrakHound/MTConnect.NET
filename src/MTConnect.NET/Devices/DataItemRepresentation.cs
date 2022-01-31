// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public enum DataItemRepresentation
    {
        /// <summary>
        /// The measured value of the sample data.
        /// </summary>
        VALUE,

        /// <summary>
        /// The reported value(s) are represented as a set of key-value pairs.
        /// </summary>
        DATA_SET,

        /// <summary>
        /// DEPRECATED as a representation in MTConnect Version. 1.5. Replaced by the discrete attribute for a Data Entity
        /// </summary>
        DISCRETE,

        /// <summary>
        /// A series of sampled data.
        /// </summary>
        TIME_SERIES,

        /// <summary>
        /// A Table is a two dimensional set of key-value pairs where the Entry represents a row,
        /// and the value is a set of key-value pair Cell elements. The Table follows the same behavior as the
        /// Data Set for change tracking, clearing, and history. When an Entry changes, all Cell elements update
        /// as a single unit following the behavior of a Data Set. 
        /// Note: It is best to use the VARIABLE DataItem type if the Cell elements represent multiple semantic types.
        /// </summary>
        TABLE
    }
}
