// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An indication of the operating condition of a mechanism represented by a Composition type element.
    /// </summary>
    public class CompositionStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "COMPOSITION_STATE";
        public const string NameId = "compState";

        public enum SubTypes
        {
            /// <summary>
            /// An indication of the operating state of a mechanism represented by a Composition type component.
            /// </summary>
            ACTION,

            /// <summary>
            /// An indication of the position of a mechanism that may move in a lateral direction. The mechanism is represented by a Composition type component.
            /// </summary>
            LATERAL,

            /// <summary>
            /// An indication of the open or closed state of a mechanism. The mechanism is represented by a Composition type component.
            /// </summary>
            MOTION,

            /// <summary>
            /// An indication of the activation state of a mechanism represented by a Composition type component.
            /// </summary>
            SWITCHED,

            /// <summary>
            /// An indication of the position of a mechanism that may move in a vertical direction. The mechanism is represented by a Composition type component.
            /// </summary>
            VERTICAL
        }


        public CompositionStateDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
        }

        public CompositionStateDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            DataItemCategory = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }


        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.ACTION: return "action";
                case SubTypes.LATERAL: return "lateral";
                case SubTypes.MOTION: return "motion";
                case SubTypes.SWITCHED: return "switched";
                case SubTypes.VERTICAL: return "vertical";
            }

            return null;
        }
    }
}
