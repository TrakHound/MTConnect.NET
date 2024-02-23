// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572382017_874684_42291

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that responds to a physical stimulus and transmits a resulting impulse or value from a sensing unit.
    /// </summary>
    public class SensorComponent : Component
    {
        public const string TypeId = "Sensor";
        public const string NameId = "sensor";
        public new const string DescriptionText = "Component that responds to a physical stimulus and transmits a resulting impulse or value from a sensing unit.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12; 


        public SensorComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}