// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605552258019_616231_2696

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that is a Pot for a tool that has to be removed from a ToolMagazine or Turret to a location outside of the piece of equipment.
    /// </summary>
    public class RemovalPotComponent : Component
    {
        public const string TypeId = "RemovalPot";
        public const string NameId = "removalPot";
        public new const string DescriptionText = "Leaf Component that is a Pot for a tool that has to be removed from a ToolMagazine or Turret to a location outside of the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public RemovalPotComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}