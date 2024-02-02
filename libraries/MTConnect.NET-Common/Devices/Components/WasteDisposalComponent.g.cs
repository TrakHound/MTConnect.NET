// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382023_920799_42303

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliary that removes manufacturing byproducts from a piece of equipment.
    /// </summary>
    public class WasteDisposalComponent : Component
    {
        public const string TypeId = "WasteDisposal";
        public const string NameId = "wasteDisposal";
        public new const string DescriptionText = "Auxiliary that removes manufacturing byproducts from a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public WasteDisposalComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}