// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605552258190_552410_2704

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that is a Pot for a tool that is awaiting transfer from a ToolMagazine to spindle or Turret.
    /// </summary>
    public class TransferPotComponent : Component
    {
        public const string TypeId = "TransferPot";
        public const string NameId = "transferPot";
        public new const string DescriptionText = "Leaf Component that is a Pot for a tool that is awaiting transfer from a ToolMagazine to spindle or Turret.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public TransferPotComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}