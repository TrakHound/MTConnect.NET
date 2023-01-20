// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// Specification elements define information describing the design characteristics for a piece of equipment.
    /// </summary>
    public class AbstractSpecification : IAbstractSpecification
    {
        /// <summary>
        /// The unique identifier for this Specification.The id attribute MUST be unique within the MTConnectDevices document.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name provides additional meaning and differentiates between Specifications.
        /// </summary>
        public string Name { get; set; }

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
        /// A reference to the id attribute of the DataItem associated with this element
        /// </summary>
        public string DataItemIdRef { get; set; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// A reference to the id attribute of the Composition associated with this element.       
        /// </summary>
        public string CompositionIdRef { get; set; }

        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        public string CoordinateIdRef { get; set; }

        /// <summary>
        /// A reference to the creator of the Specification.
        /// </summary>
        public Originator Originator { get; set; }
    }
}
