// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanical mechanism or closure that covers a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.
    /// </summary>
    public class DoorCompositionComposition : Composition 
    {
        public const string TypeId = "DOOR";
        public const string NameId = "doorComposition";
        public new const string DescriptionText = "Composition composed of a mechanical mechanism or closure that covers a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public DoorCompositionComposition()  { Type = TypeId; }
    }
}