// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Lubrication is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids used to lubricate portions of the piece of equipment.
    /// </summary>
    public class LubricationModel : SystemModel, ILubricationModel
    {
        public LubricationModel()
        {
            Type = LubricationComponent.TypeId;
        }

        public LubricationModel(string componentId)
        {
            Id = componentId;
            Type = LubricationComponent.TypeId;
        }
    }
}