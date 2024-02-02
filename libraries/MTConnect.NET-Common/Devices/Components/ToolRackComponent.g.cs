// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605551899241_174607_2546

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// ToolingDelivery composed of a linear or matrixed tool storage mechanism that holds any number of tools.
    /// </summary>
    public class ToolRackComponent : Component
    {
        public const string TypeId = "ToolRack";
        public const string NameId = "toolRack";
        public new const string DescriptionText = "ToolingDelivery composed of a linear or matrixed tool storage mechanism that holds any number of tools.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ToolRackComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}