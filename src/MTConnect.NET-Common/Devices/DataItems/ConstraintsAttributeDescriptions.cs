// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    public static class ConstraintsAttributeDescriptions
    {
        /// <summary>
        /// If data reported for a DataItem is a range of numeric values, then the value reported MAY be bounded with an upper limit defined by this constraint.
        /// </summary>
        public const string Maximum = "If data reported for a DataItem is a range of numeric values, then the value reported MAY be bounded with an upper limit defined by this constraint.";

        /// <summary>
        /// If the data reported for a DataItem is a range of numeric values, the value reported MAY be bounded with a lower limit defined by this constraint.
        /// </summary>
        public const string Minimum = "If the data reported for a DataItem is a range of numeric values, the value reported MAY be bounded with a lower limit defined by this constraint.";

        /// <summary>
        /// The target or expected value for this data item.
        /// </summary>
        public const string Nominal = "The target or expected value for this data item.";

        /// <summary>
        /// A Data Element that defines a valid value for the data provided for a DataItem.
        /// </summary>
        public const string Values = "A Data Element that defines a valid value for the data provided for a DataItem.";
    }
}