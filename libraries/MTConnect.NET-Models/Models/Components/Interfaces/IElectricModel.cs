// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Electric is a System that represents the information for the main power supply for device piece of equipment and the distribution of that power throughout the equipment.
    /// The electric system will provide all the data with regard to electric current, voltage, frequency, etc. that applies to the piece of equipment as a functional unit.
    /// </summary>
    public interface IElectricModel : ISystemModel
    {
        /// <summary>
        /// The indication of the status of the source of energy for a Structural Element to allow it to perform
        /// its intended function or the state of an enabling signal providing permission for the Structural Element to perform its functions.
        /// </summary>
        PowerState PowerState { get; set; }

        /// <summary>
        /// The measurement of the electrical potential between two points in an electrical circuit in which the current periodically reverses direction.
        /// </summary>
        VoltageACValue VoltageAC { get; set; }

        /// <summary>
        /// The measurement of power flowing through or dissipated by an electrical circuit or piece of equipment.
        /// </summary>
        WattageValue Wattage { get; set; }

        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        TemperatureValue Temperature { get; set; }
    }
}