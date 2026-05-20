// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Shdr
{
    /// <summary>
    /// Classifies the kind of MTConnect observation carried by an inbound SHDR line. The SHDR
    /// parser inspects the matching DataItem and tags each line with one of these values so the
    /// agent can route it to the right observation type (single value, condition, message,
    /// data set, table, or time series).
    /// </summary>
    public enum ShdrObservationType
    {
        /// <summary>A single Sample or Event observation (one DataItem id, one value).</summary>
        DataItem,

        /// <summary>A Condition observation with severity, native code, native severity, qualifier, and message fields.</summary>
        Condition,

        /// <summary>A Message DataItem observation that carries a native code in addition to the text value.</summary>
        Message,

        /// <summary>A DataSet observation made up of key/value entries.</summary>
        DataSet,

        /// <summary>A Table observation made up of rows with cell key/value pairs.</summary>
        Table,

        /// <summary>A TimeSeries observation whose payload is a vector of floating-point samples.</summary>
        TimeSeries
    }
}
