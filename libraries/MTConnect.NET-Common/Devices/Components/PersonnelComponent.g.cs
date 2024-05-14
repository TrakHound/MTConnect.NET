// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572382005_168835_42270

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Resource composed of an individual or individuals who either control, support, or otherwise interface with a piece of equipment.
    /// </summary>
    public class PersonnelComponent : Component
    {
        public const string TypeId = "Personnel";
        public const string NameId = "personnel";
        public new const string DescriptionText = "Resource composed of an individual or individuals who either control, support, or otherwise interface with a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public PersonnelComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}