// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// ToolMagazine is a ToolingDelivery that represents a tool storage mechanism that holds any number of tools.Tools are located in POTs.
    /// POTs are moved into position to transfer tools into or out of the ToolMagazine by an AutomaticToolChanger.
    /// </summary>
    public class ToolMagazineComponent : Component 
    {
        public const string TypeId = "ToolMagazine";
        public const string NameId = "toolmag";
        public new const string DescriptionText = "ToolMagazine is a ToolingDelivery that represents a tool storage mechanism that holds any number of tools.Tools are located in POTs. POTs are moved into position to transfer tools into or out of the ToolMagazine by an AutomaticToolChanger.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public ToolMagazineComponent()  { Type = TypeId; }
    }
}