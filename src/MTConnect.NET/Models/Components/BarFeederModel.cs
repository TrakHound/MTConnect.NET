// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

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
