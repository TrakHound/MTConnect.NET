// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106467_401181_44411

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that dispenses liquid or powered materials.
    /// </summary>
    public class ExtrusionUnitComponent : Component
    {
        public const string TypeId = "ExtrusionUnit";
        public const string NameId = "extrusionUnit";
        public new const string DescriptionText = "Leaf Component that dispenses liquid or powered materials.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public ExtrusionUnitComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}