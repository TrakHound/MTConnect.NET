// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605117029255_781563_1842

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that delivers compressed gas or fluid and controls the pressure and rate of pressure change to a desired target set-point.
    /// </summary>
    public class PressureComponent : Component
    {
        public const string TypeId = "Pressure";
        public const string NameId = "pressure";
        public new const string DescriptionText = "System that delivers compressed gas or fluid and controls the pressure and rate of pressure change to a desired target set-point.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public PressureComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}