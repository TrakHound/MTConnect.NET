// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605116368942_480454_1665

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that evacuates gases and liquids from an enclosed and sealed space to a controlled negative pressure or a molecular density below the prevailing atmospheric level.
    /// </summary>
    public class VacuumComponent : Component
    {
        public const string TypeId = "Vacuum";
        public const string NameId = "vacuum";
        public new const string DescriptionText = "System that evacuates gases and liquids from an enclosed and sealed space to a controlled negative pressure or a molecular density below the prevailing atmospheric level.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public VacuumComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}