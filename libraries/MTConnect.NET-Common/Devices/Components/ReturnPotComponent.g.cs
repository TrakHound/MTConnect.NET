// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605552257200_872757_2664

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that is a Pot for a tool that has been removed from spindle or Turret and awaiting for return to a ToolMagazine.
    /// </summary>
    public class ReturnPotComponent : Component
    {
        public const string TypeId = "ReturnPot";
        public const string NameId = "returnPot";
        public new const string DescriptionText = "Leaf Component that is a Pot for a tool that has been removed from spindle or Turret and awaiting for return to a ToolMagazine.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ReturnPotComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}