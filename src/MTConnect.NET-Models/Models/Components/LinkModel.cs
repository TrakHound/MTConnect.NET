// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Link is a Structure providing a connection between Components.
    /// </summary>
    public class LinkModel : StructureModel
    {
        public LinkModel() 
        {
            Type = LinkComponent.TypeId;
        }

        public LinkModel(string componentId)
        {
            Id = componentId;
            Type = LinkComponent.TypeId;
        }
    }
}