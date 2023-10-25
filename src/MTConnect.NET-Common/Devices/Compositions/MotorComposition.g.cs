// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
    /// </summary>
    public class MotorCompositionComposition : Composition 
    {
        public const string TypeId = "MOTOR";
        public const string NameId = "motorComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public MotorCompositionComposition()  { Type = TypeId; }
    }
}