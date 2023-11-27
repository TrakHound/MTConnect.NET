// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// The time and date code associated with a material or other physical item.
    /// </summary>
    public class DateCodeModel
    {
        /// <summary>
        /// The time and date code relating to the production of a material or other physical item.
        /// </summary>
        public DateTime Manufacture { get; set; }
        public IDataItemModel ManufactureDataItem { get; set; }

        /// <summary>
        /// The time and date code relating to the expiration or end of useful life for a material or other physical item.
        /// </summary>
        public DateTime Expiration { get; set; }
        public IDataItemModel ExpirationDataItem { get; set; }

        /// <summary>
        /// The time and date code relating the first use of a material or other physical item.
        /// </summary>
        public DateTime FirstUse { get; set; }
        public IDataItemModel FirstUseDataItem { get; set; }
    }
}