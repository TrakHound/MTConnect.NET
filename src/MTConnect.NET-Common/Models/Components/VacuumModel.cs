// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Vacuum is a System that evacuates gases and liquids from an enclosed and sealed space to a controlled negative pressure or a molecular density below the prevailing atmospheric level.
    /// </summary>
    public class VacuumModel : SystemModel, IVacuumModel
    {
        public VacuumModel()
        {
            Type = VacuumComponent.TypeId;
        }

        public VacuumModel(string componentId)
        {
            Id = componentId;
            Type = VacuumComponent.TypeId;
        }
    }
}
