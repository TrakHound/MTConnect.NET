// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Events;
using MTConnect.Devices.Samples;
using MTConnect.Streams.Events;
using MTConnect.Streams.Samples;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Pneumatic is a System that uses compressed gasses to actuate components or do work within the piece of equipment.
    /// </summary>
    public class PneumaticModel : SystemModel, IPneumaticModel
    {
        /// <summary>
        /// The indication of the status of the source of energy for a Structural Element to allow it to perform
        /// its intended function or the state of an enabling signal providing permission for the Structural Element to perform its functions.
        /// </summary>
        public PowerState PowerState
        {
            get => GetDataItemValue<PowerState>(DataItem.CreateId(Id, PowerStateDataItem.NameId));
            set => AddDataItem(new PowerStateDataItem(Id), value);
        }

        /// <summary>
        /// The force per unit area measured relative to atmospheric pressure.
        /// </summary>
        public PressureValue Pressure
        {
            get => GetSampleValue<PressureValue>(Devices.Samples.PressureDataItem.NameId);
            set => AddDataItem(new PressureDataItem(Id), value);
        }
        public IDataItemModel PressureDataItem => GetDataItem(Devices.Samples.PressureDataItem.NameId);


        public PneumaticModel()
        {
            Type = PneumaticComponent.TypeId;
        }

        public PneumaticModel(string componentId)
        {
            Id = componentId;
            Type = PneumaticComponent.TypeId;
        }
    }
}
