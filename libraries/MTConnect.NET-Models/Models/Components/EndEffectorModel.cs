// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// EndEffector is a System that represents the information for those functions that form the last link segment of a piece of equipment.
    /// It is the part of a piece of equipment that interacts with the manufacturing process.
    /// </summary>
    public class EndEffectorModel : SystemModel, IEndEffectorModel
    {
        public EndEffectorModel()
        {
            Type = EndEffectorComponent.TypeId;
        }

        public EndEffectorModel(string componentId)
        {
            Id = componentId;
            Type = EndEffectorComponent.TypeId;
        }
    }
}