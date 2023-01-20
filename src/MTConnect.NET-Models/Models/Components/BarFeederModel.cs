// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// BarFeeder is a Loader involved in delivering bar stock to a piece of equipment.
    /// </summary>
    public class BarFeederModel : LoaderModel
    {
        public BarFeederModel()
        {
            Type = BarFeederComponent.TypeId;
        }

        public BarFeederModel(string componentId)
        {
            Id = componentId;
            Type = BarFeederComponent.TypeId;
        }
    }
}