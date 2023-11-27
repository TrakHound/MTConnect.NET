// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Dielectric is a System that represents the information for a system that manages a chemical mixture used in a manufacturing
    /// process being performed at that piece of equipment.For example, this could describe
    /// the dielectric system for an EDM process or the chemical bath used in a plating process.
    /// </summary>
    public class DielectricModel : SystemModel, IDielectricModel
    {
        public DielectricModel()
        {
            Type = DielectricComponent.TypeId;
        }

        public DielectricModel(string componentId)
        {
            Id = componentId;
            Type = DielectricComponent.TypeId;
        }
    }
}