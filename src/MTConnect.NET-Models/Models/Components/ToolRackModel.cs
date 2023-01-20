// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// ToolRack is a ToolingDelivery that represents a linear or matrixed tool storage mechanism that holds any number of tools. Tools are located in STATIONs.
    /// </summary>
    public class ToolRackModel : ToolingDeliveryModel
    {
        public ToolRackModel()
        {
            Type = ToolRackComponent.TypeId;
        }

        public ToolRackModel(string componentId)
        {
            Id = componentId;
            Type = ToolRackComponent.TypeId;
        }
    }
}