// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738875_244806_44720

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
    /// </summary>
    public class MotorComposition : Composition 
    {
        public const string TypeId = "MOTOR";
        public const string NameId = "motorComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public MotorComposition()  { Type = TypeId; }
    }
}