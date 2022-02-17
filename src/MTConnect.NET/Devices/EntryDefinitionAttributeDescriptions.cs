// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class EntryDefinitionAttributeDescriptions
    {
        /// <summary>
        /// The unique identification of the Entry in the Definition.The description applies to all Entry observations having this key.
        /// </summary>
        public const string Key = "The unique identification of the Entry in the Definition.The description applies to all Entry observations having this key.";

        /// <summary>
        /// The DataItem type that defines the meaning of the key.
        /// </summary>
        public const string KeyType = "The DataItem type that defines the meaning of the key.";

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        public const string Units = "Units MUST be present for all DataItem elements in the SAMPLE category. If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.";

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        public const string Type = "The type of data being measured. Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.";

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        public const string SubType = "A sub-categorization of the data item type. For example, the Sub-types of POSITION can be ACTUAL or COMMANDED. Not all types have subTypes and they can be optional.";
    }
}
