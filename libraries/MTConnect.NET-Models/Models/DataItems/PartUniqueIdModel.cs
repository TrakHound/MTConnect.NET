// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// Identifier given to a distinguishable, individual part.
    /// </summary>
    public class PartUniqueIdModel
    {
        /// <summary>
        /// Material that is used to produce parts.
        /// </summary>
        public string RawMaterial { get; set; }
        public IDataItemModel RawMaterialDataItem { get; set; }

        /// <summary>
        /// A serial number that uniquely identifies a specific part.
        /// </summary>
        public string SerialNumber { get; set; }
        public IDataItemModel SerialNumberDataItem { get; set; }

        /// <summary>
        /// The globally unique identifier as specified in ISO 11578 or RFC 4122.
        /// </summary>
        public string Uuid { get; set; }
        public IDataItemModel UuidDataItem { get; set; }
    }
}