// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Protective is a System that represents the information for those functions that detect or prevent harm or damage to equipment or personnel. 
    /// Protective does not include the information relating to the Enclosure system.
    /// </summary>
    public class ProtectiveModel : SystemModel, IProtectiveModel
    {
        public ProtectiveModel()
        {
            Type = ProtectiveComponent.TypeId;
        }

        public ProtectiveModel(string componentId)
        {
            Id = componentId;
            Type = ProtectiveComponent.TypeId;
        }
    }
}
