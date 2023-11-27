// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// GangToolBar is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools.
    /// Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by linearly positioning the GangToolBar.
    /// </summary>
    public class GangToolBarModel : ToolingDeliveryModel
    {
        public GangToolBarModel()
        {
            Type = GangToolbarComponent.TypeId;
        }

        public GangToolBarModel(string componentId)
        {
            Id = componentId;
            Type = GangToolbarComponent.TypeId;
        }
    }
}