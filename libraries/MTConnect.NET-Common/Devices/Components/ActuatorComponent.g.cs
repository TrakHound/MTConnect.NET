// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381968_750236_42201

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component composed of a physical apparatus that moves or controls a mechanism or system.
    /// </summary>
    public class ActuatorComponent : Component
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Component.
        /// </summary>
        public const string TypeId = "Actuator";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Component.
        /// </summary>
        public const string NameId = "actuator";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Component composed of a physical apparatus that moves or controls a mechanism or system.";

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
        public ActuatorComponent()
        {
            Type = TypeId;
        }
    }
}
