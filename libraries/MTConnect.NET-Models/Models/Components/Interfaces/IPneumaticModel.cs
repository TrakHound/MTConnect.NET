// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Hydraulic is a System that represents the information for a system comprised of all the parts involved in moving and distributing pressurized liquid throughout the piece of equipment.
    /// </summary>
    public interface IPneumaticModel : ISystemModel
    {
        /// <summary>
        /// The indication of the status of the source of energy for a Structural Element to allow it to perform
        /// its intended function or the state of an enabling signal providing permission for the Structural Element to perform its functions.
        /// </summary>
        PowerState PowerState { get; set; }

        /// <summary>
        /// The force per unit area measured relative to atmospheric pressure.
        /// </summary>
        PressureValue Pressure { get; set; }
        IDataItemModel PressureDataItem { get; }
    }
}