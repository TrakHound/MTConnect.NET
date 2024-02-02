// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106471_971269_44432

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
    /// </summary>
    public class MotorComponent : Component
    {
        public const string TypeId = "Motor";
        public const string NameId = "motor";
        public new const string DescriptionText = "Leaf Component that converts electrical, pneumatic, or hydraulic energy into mechanical energy.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public MotorComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}