// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605552256946_709565_2656

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a tool storage location associated with a ToolMagazine or AutomaticToolChanger.
    /// </summary>
    public class PotComponent : Component
    {
        public const string TypeId = "Pot";
        public const string NameId = "pot";
        public new const string DescriptionText = "Leaf Component composed of a tool storage location associated with a ToolMagazine or AutomaticToolChanger.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public PotComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}