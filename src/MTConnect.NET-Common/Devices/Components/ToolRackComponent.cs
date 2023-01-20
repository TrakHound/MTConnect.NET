// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public ToolRackComponent()  { Type = TypeId; }
    }
}