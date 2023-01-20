// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Models.DataItems
{
    /// <summary>
    /// Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.
    /// </summary>
    public class PartKindIdModel
    {
        /// <summary>
        /// An identifier given to a group of parts having similarities in geometry, manufacturing process, and/or functions.
        /// </summary>
        public string PartFamily { get; set; }
        public IDataItemModel PartFamilyDataItem { get; set; }

        /// <summary>
        /// A word or set of words by which a part is known, addressed, or referred to.
        /// </summary>
        public string PartName { get; set; }
        public IDataItemModel PartNameDataItem { get; set; }

        /// <summary>
        /// Identifier of a particular part design or model.
        /// </summary>
        public string PartNumber { get; set; }
        public IDataItemModel PartNumberDataItem { get; set; }

        /// <summary>
        /// The globally unique identifier as specified in ISO 11578 or RFC 4122.
        /// </summary>
        public string Uuid { get; set; }
        public IDataItemModel UuidDataItem { get; set; }
    }
}
