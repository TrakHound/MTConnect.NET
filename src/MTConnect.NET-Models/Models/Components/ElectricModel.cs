// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Electric is a System that represents the information for the main power supply for device piece of equipment and the distribution of that power throughout the equipment.
    /// The electric system will provide all the data with regard to electric current, voltage, frequency, etc. that applies to the piece of equipment as a functional unit.
    /// </summary>
    public class ElectricModel : SystemModel, IElectricModel
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
        /// The measurement of the electrical potential between two points in an electrical circuit in which the current periodically reverses direction.
        /// </summary>
        public VoltageACValue VoltageAC
        {
            get => (VoltageACValue)DataItemManager.GetSampleValue(DataItem.CreateId(Id, VoltageACDataItem.NameId));
            set => DataItemManager.AddDataItem(new VoltageACDataItem(Id), value);
        }

        /// <summary>
        /// The measurement of power flowing through or dissipated by an electrical circuit or piece of equipment.
        /// </summary>
        public WattageValue Wattage
        {
            get => (WattageValue)DataItemManager.GetSampleValue(DataItem.CreateId(Id, WattageDataItem.NameId));
            set => DataItemManager.AddDataItem(new WattageDataItem(Id), value);
        }

        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        public TemperatureValue Temperature
        {
            get => (TemperatureValue)DataItemManager.GetSampleValue(DataItem.CreateId(Id, TemperatureDataItem.NameId));
            set => DataItemManager.AddDataItem(new TemperatureDataItem(Id), value);
        }


        public ElectricModel()
        {
            Type = ElectricComponent.TypeId;
        }

        public ElectricModel(string componentId)
        {
            Id = componentId;
            Type = ElectricComponent.TypeId;
        }
    }
}
