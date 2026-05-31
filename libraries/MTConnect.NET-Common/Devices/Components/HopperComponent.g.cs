// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106470_968785_44426

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a chamber or bin in which materials are stored temporarily, typically being filled through the top and dispensed through the bottom.
    /// </summary>
    public class HopperComponent : Component
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Component.
        /// </summary>
        public const string TypeId = "Hopper";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Component.
        /// </summary>
        public const string NameId = "hopper";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Leaf Component composed of a chamber or bin in which materials are stored temporarily, typically being filled through the top and dispensed through the bottom.";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Component.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public HopperComponent()
        {
            Type = TypeId;
        }
    }
}
