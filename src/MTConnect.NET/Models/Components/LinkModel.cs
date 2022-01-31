// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
