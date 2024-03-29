// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// When the representation is TABLE, the CellDefinition provides the Description and the units associated each Cell by key.
    /// </summary>
    public class CellDefinition : ICellDefinition
    {
        /// <summary>
        /// The unique identification of the Entry in the Definition.The description applies to all Entry observations having this key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The DataItem type that defines the meaning of the key.
        /// </summary>
        public string KeyType { get; set; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        /// The Description of the EntryDefinition.
        /// </summary>
        public string Description { get; set; }
    }
}