// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// ToolingDelivery is an Auxiliary that represents the information for a unit involved in managing, positioning, storing, and delivering tooling within a piece of equipment.
    /// </summary>
    public class ToolingDeliveryComponent : Component 
    {
        public const string TypeId = "ToolingDelivery";
        public const string NameId = "tooling";
        public new const string DescriptionText = "ToolingDelivery is an Auxiliary that represents the information for a unit involved in managing, positioning, storing, and delivering tooling within a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public ToolingDeliveryComponent()  { Type = TypeId; }
    }
}