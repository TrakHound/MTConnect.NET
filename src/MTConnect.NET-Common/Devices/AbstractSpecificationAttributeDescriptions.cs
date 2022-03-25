// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices
{
    public static class AbstractSpecificationAttributeDescriptions
    {
        /// <summary>
        /// The unique identifier for this Specification.The id attribute MUST be unique within the MTConnectDevices document.
        /// </summary>
        public const string Id = "The unique identifier for this Specification.The id attribute MUST be unique within the MTConnectDevices document.";

        /// <summary>
        /// The name provides additional meaning and differentiates between Specifications.
        /// </summary>
        public const string Name = "The name provides additional meaning and differentiates between Specifications.";

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

        /// <summary>
        /// A reference to the id attribute of the DataItem associated with this element
        /// </summary>
        public const string DataItemIdRef = "A reference to the id attribute of the DataItem associated with this element";

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// </summary>
        public const string Units = "Units MUST be present for all DataItem elements in the SAMPLE category.";

        /// <summary>
        /// A reference to the id attribute of the Composition associated with this element.
        /// </summary>
        public const string CompositionIdRef = "A reference to the id attribute of the Composition associated with this element.";

        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        public const string CoordinateIdRef = "References the CoordinateSystem for geometric Specification elements.";

        /// <summary>
        /// A reference to the creator of the Specification.
        /// </summary>
        public const string Originator = "A reference to the creator of the Specification.";
    }
}
