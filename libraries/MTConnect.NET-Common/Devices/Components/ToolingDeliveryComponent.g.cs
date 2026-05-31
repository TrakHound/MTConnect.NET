// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572382021_741508_42300

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliary that manages, positions, stores, and delivers tooling within a piece of equipment.
    /// </summary>
    public class ToolingDeliveryComponent : Component
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Component.
        /// </summary>
        public const string TypeId = "ToolingDelivery";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Component.
        /// </summary>
        public const string NameId = "toolingDelivery";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Auxiliary that manages, positions, stores, and delivers tooling within a piece of equipment.";

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
        public ToolingDeliveryComponent()
        {
            Type = TypeId;
        }
    }
}
