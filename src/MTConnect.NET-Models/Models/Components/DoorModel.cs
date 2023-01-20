// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Door is a Component that represents the information for a mechanical mechanism or closure that can cover, for example, a physical access portal into a piece of equipment.
    /// The closure can be opened or closed to allow or restrict access to other parts of the equipment.
    /// </summary>
    public class DoorModel : ComponentModel, IDoorModel
    {
        /// <summary>
        /// The operational state of a DOOR type component or composition element.
        /// </summary>
        public DoorState State
        {
            get => DataItemManager.GetDataItemValue<DoorState>(DataItem.CreateId(Id, DoorStateDataItem.NameId));
            set => DataItemManager.AddDataItem(new DoorStateDataItem(Id), value);
        }


        public DoorModel() 
        {
            Type = DoorComponent.TypeId;
        }

        public DoorModel(string componentId)
        {
            Id = componentId;
            Type = DoorComponent.TypeId;
        }
    }
}
