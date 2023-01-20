// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Source identifies the physical part of a device where the data represented by the DataItem is originally measured.
    /// </summary>
    public class Source : ISource
    {
        /// <summary>
        /// The id attribute of the Component that represents the physical part of a device where teh data represented by the DataItem is actually measured.
        /// </summary>
        public string ComponentId { get; set; }

        /// <summary>
        /// The id attribute of the DataItem that represents the originally measured value of the data referenced by this DataItem.
        /// </summary>
        public string DataItemId { get; set; }

        /// <summary>
        /// The identifier attribute of the Composition element that represents the physical part of a piece of equipment where the data represented by the DataItem element originated.
        /// </summary>
        public string CompositionId { get; set; }

        /// <summary>
        /// Identifier of the source entity.
        /// </summary>
        public string Value { get; set; }
    }
}