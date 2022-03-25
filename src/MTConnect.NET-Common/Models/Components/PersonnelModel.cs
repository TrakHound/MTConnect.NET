// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Personnel is a Resource that provides information about an individual or individuals who either control, support, or otherwise interface with a piece of equipment.
    /// </summary>
    public class PersonnelModel : ResourceModel
    {
        public const string TypeId = "Personnel";


        public PersonnelModel()
        {
            Type = TypeId;
        }

        public PersonnelModel(string componentId)
        {
            Id = componentId;
            Type = TypeId;
        }
    }
}
