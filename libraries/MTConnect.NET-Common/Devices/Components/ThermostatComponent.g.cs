// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_4_45f01b9_1643678703742_369144_1539

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component composed of a sensor or an instrument that measures temperature.Thermostat was **DEPRECATED** in *MTConnect Version 1.2* and was replaced by Temperature.
    /// </summary>
    public class ThermostatComponent : Component
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Component.
        /// </summary>
        public const string TypeId = "Thermostat";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Component.
        /// </summary>
        public const string NameId = "thermostat";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Component composed of a sensor or an instrument that measures temperature.Thermostat was **DEPRECATED** in *MTConnect Version 1.2* and was replaced by Temperature.";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The maximum MTConnect Version that this Component is valid for; set when the type has been deprecated.
        /// </summary>
        public override System.Version MaximumVersion => MTConnectVersions.Version12;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Component.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version10;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public ThermostatComponent()
        {
            Type = TypeId;
        }
    }
}
