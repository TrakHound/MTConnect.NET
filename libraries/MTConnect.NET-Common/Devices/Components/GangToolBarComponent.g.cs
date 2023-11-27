// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605551885706_266977_2498

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// ToolingDelivery composed of a tool mounting mechanism that holds any number of tools.
    /// </summary>
    public class GangToolBarComponent : Component
    {
        public const string TypeId = "GangToolBar";
        public const string NameId = "gangToolBarComponent";
        public new const string DescriptionText = "ToolingDelivery composed of a tool mounting mechanism that holds any number of tools.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public GangToolBarComponent() { Type = TypeId; }
    }
}