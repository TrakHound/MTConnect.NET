// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1607345082819_606856_1629

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organize Auxiliary types.
    /// </summary>
    public class AuxiliariesComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Auxiliaries";
        public const string NameId = "auxiliariesComponent";
        public new const string DescriptionText = "Component that organize Auxiliary types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public AuxiliariesComponent() { Type = TypeId; }
    }
}