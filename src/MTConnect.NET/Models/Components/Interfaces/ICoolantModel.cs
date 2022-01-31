// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.Compositions;
using MTConnect.Streams.Events;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Coolant is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids that remove heat from a piece of equipment.
    /// </summary>
    public interface ICoolantModel : ISystemModel
    {
        /// <summary>
        /// The indication of the status of the source of energy for a Structural Element to allow it to perform
        /// its intended function or the state of an enabling signal providing permission for the Structural Element to perform its functions.
        /// </summary>
        PowerState PowerState { get; set; }

        /// <summary>
        /// A viscous liquid.
        /// </summary>
        IOilModel Oil { get; set; }

        /// <summary>
        /// An apparatus raising, driving, exhausting, or compressing fluids or gases by means of a piston, plunger, or set of rotating vanes.
        /// </summary>
        IPumpModel Pump { get; set; }

        /// <summary>
        /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        IMotorModel Motor { get; set; }

        /// <summary>
        /// A receptacle or container for holding material.
        /// </summary>
        ITankModel Tank { get; set; }
    }
}
