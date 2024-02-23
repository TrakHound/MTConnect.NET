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
        public const string TypeId = "Thermostat";
        public const string NameId = "thermostat";
        public new const string DescriptionText = "Component composed of a sensor or an instrument that measures temperature.Thermostat was **DEPRECATED** in *MTConnect Version 1.2* and was replaced by Temperature.";

        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version12;
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public ThermostatComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}