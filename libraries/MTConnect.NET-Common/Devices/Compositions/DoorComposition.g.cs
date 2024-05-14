// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738868_947585_44698

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanical mechanism or closure that covers a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.
    /// </summary>
    public class DoorComposition : Composition 
    {
        public const string TypeId = "DOOR";
        public const string NameId = "doorComposition";
        public new const string DescriptionText = "Composition composed of a mechanical mechanism or closure that covers a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public DoorComposition()  { Type = TypeId; }
    }
}