// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381982_394383_42225

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that manages a chemical mixture used in a manufacturing process being performed at that piece of equipment.
    /// </summary>
    public class DielectricComponent : Component
    {
        public const string TypeId = "Dielectric";
        public const string NameId = "dielectric";
        public new const string DescriptionText = "System that manages a chemical mixture used in a manufacturing process being performed at that piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public DielectricComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}