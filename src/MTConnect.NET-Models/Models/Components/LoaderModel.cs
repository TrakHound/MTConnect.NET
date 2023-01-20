// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Loader is an Auxiliary comprised of all the parts involved in moving and distributing materials, parts, tooling, and other items to or from a piece of equipment.
    /// </summary>
    public class LoaderModel : AuxiliaryModel
    {
        public LoaderModel()
        {
            Type = LoaderComponent.TypeId;
        }

        public LoaderModel(string componentId)
        {
            Id = componentId;
            Type = LoaderComponent.TypeId;
        }
    }
}
