// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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