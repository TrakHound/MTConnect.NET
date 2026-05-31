// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381993_131470_42246

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that provides movement and distribution of pressurized liquid throughout the piece of equipment.
    /// </summary>
    public class HydraulicComponent : Component
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Component.
        /// </summary>
        public const string TypeId = "Hydraulic";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Component.
        /// </summary>
        public const string NameId = "hydraulic";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "System that provides movement and distribution of pressurized liquid throughout the piece of equipment.";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Component.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public HydraulicComponent()
        {
            Type = TypeId;
        }
    }
}
