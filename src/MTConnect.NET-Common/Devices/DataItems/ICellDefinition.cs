// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// When the representation is TABLE, the CellDefinition provides the Description and the units associated each Cell by key.
    /// </summary>
    public interface ICellDefinition
    {
        /// <summary>
        /// The unique identification of the Entry in the Definition.The description applies to all Entry observations having this key.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The DataItem type that defines the meaning of the key.
        /// </summary>
        string KeyType { get; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        string Units { get; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        string SubType { get; }

        /// <summary>
        /// The Description of the EntryDefinition.
        /// </summary>
        string Description { get; }
    }
}
