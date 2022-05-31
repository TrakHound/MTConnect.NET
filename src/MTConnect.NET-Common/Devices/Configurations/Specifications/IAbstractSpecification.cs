// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Configurations.Specifications
{
    /// <summary>
    /// Specification elements define information describing the design characteristics for a piece of equipment.
    /// </summary>
    public interface IAbstractSpecification
    {
        /// <summary>
        /// The unique identifier for this Specification.The id attribute MUST be unique within the MTConnectDevices document.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The name provides additional meaning and differentiates between Specifications.
        /// </summary>
        string Name { get; }

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
        /// A reference to the id attribute of the DataItem associated with this element
        /// </summary>
        string DataItemIdRef { get; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// </summary>
        string Units { get; }

        /// <summary>
        /// A reference to the id attribute of the Composition associated with this element.       
        /// </summary>
        string CompositionIdRef { get; }

        /// <summary>
        /// References the CoordinateSystem for geometric Specification elements.
        /// </summary>
        string CoordinateIdRef { get; }

        /// <summary>
        /// A reference to the creator of the Specification.
        /// </summary>
        Originator Originator { get; }
    }
}
