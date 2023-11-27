// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;

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
            get => DataItemManager.GetDataItemValue<PowerState>(DataItem.CreateId(Id, PowerStateDataItem.NameId));
            set => DataItemManager.AddDataItem(new PowerStateDataItem(Id), value);
        }

        /// <summary>
        /// The force per unit area measured relative to atmospheric pressure.
        /// </summary>
        public PressureValue Pressure
        {
            get => DataItemManager.GetSampleValue<PressureValue>(Devices.DataItems.Samples.PressureDataItem.NameId);
            set => DataItemManager.AddDataItem(new PressureDataItem(Id), value);
        }
        public IDataItemModel PressureDataItem => DataItemManager.GetDataItem(Devices.DataItems.Samples.PressureDataItem.NameId);


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