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
        public const string TypeId = "Actuator";
        public const string NameId = "actuator";
        public new const string DescriptionText = "Component composed of a physical apparatus that moves or controls a mechanism or system.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public ActuatorComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}