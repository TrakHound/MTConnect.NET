// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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


        public ToolingDeliveryComponent()  { Type = TypeId; }
    }
}
