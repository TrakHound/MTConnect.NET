// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.Events;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanical mechanism or closure that can cover a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.
    /// </summary>
    public class DoorModel : CompositionModel, IDoorModel
    {
        /// <summary>
        /// The operational state of a DOOR type component or composition element.
        /// </summary>
        public DoorState DoorState
        {
            get => GetDataItemValue<DoorState>(Devices.Events.DoorStateDataItem.NameId);
            set => AddDataItem(new DoorStateDataItem(Id), value);
        }
        public IDataItemModel DoorStateDataItem => GetDataItem(Devices.Events.DoorStateDataItem.NameId);

        /// <summary>
        /// The state or operating mode of a Lock.
        /// </summary>
        public LockState LockState
        {
            get => GetDataItemValue<LockState>(Devices.Events.LockStateDataItem.NameId);
            set => AddDataItem(new LockStateDataItem(Id), value);
        }
        public IDataItemModel LockStateDataItem => GetDataItem(Devices.Events.LockStateDataItem.NameId);


        public DoorModel() 
        {
            Type = DoorComposition.TypeId;
        }

        public DoorModel(string compositionId)
        {
            Id = compositionId;
            Type = DoorComposition.TypeId;
        }
    }
}
