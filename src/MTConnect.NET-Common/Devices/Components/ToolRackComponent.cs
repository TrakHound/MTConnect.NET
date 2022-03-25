// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// ToolRack is a ToolingDelivery that represents a linear or matrixed tool storage mechanism that holds any number of tools. Tools are located in STATIONs.
    /// </summary>
    public class ToolRackComponent : Component 
    {
        public const string TypeId = "ToolRack";
        public const string NameId = "toolrack";
        public new const string DescriptionText = "ToolRack is a ToolingDelivery that represents a linear or matrixed tool storage mechanism that holds any number of tools. Tools are located in STATIONs.";

        public override string TypeDescription => DescriptionText;


        public ToolRackComponent()  { Type = TypeId; }
    }
}
