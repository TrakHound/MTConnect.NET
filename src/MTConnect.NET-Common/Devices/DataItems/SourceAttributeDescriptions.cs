// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices
{
    public static class SourceAttributeDescriptions
    {
        /// <summary>
        /// The id attribute of the Component that represents the physical part of a device where teh data represented by the DataItem is actually measured.
        /// </summary>
        public const string ComponentId = "The id attribute of the Component that represents the physical part of a device where teh data represented by the DataItem is actually measured.";

        /// <summary>
        /// The id attribute of the DataItem that represents the originally measured value of the data referenced by this DataItem.
        /// </summary>
        public const string DataItemId = "The id attribute of the DataItem that represents the originally measured value of the data referenced by this DataItem.";

        /// <summary>
        /// The identifier attribute of the Composition element that represents the physical part of a piece of equipment where the data represented by the DataItem element originated.
        /// </summary>
        public const string CompositionId = "The identifier attribute of the Composition element that represents the physical part of a piece of equipment where the data represented by the DataItem element originated.";


        public const string Value = "";
    }
}