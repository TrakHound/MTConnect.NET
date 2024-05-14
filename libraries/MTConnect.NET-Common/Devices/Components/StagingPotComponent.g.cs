// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605552257626_405215_2680

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that is a Pot for a tool that is awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.
    /// </summary>
    public class StagingPotComponent : Component
    {
        public const string TypeId = "StagingPot";
        public const string NameId = "stagingPot";
        public new const string DescriptionText = "Leaf Component that is a Pot for a tool that is awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public StagingPotComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}