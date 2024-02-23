// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605551866030_769452_2402

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// ToolingDelivery composed of a tool storage mechanism that holds any number of tools.
    /// </summary>
    public class ToolMagazineComponent : Component
    {
        public const string TypeId = "ToolMagazine";
        public const string NameId = "toolMagazine";
        public new const string DescriptionText = "ToolingDelivery composed of a tool storage mechanism that holds any number of tools.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ToolMagazineComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}