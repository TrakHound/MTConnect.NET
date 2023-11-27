// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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