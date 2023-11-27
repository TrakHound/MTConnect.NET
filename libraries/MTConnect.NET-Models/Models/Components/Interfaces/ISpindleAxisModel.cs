// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.Compositions;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    public interface ISpindleAxisModel : IAxisModel
    {
        /// <summary>
        /// The measurement of the rotational speed of a rotary axis.
        /// </summary>
        RotaryVelocityModel RotaryVelocity { get; set; }

        /// <summary>
        /// The value of a command issued to adjust the programmed velocity for a Rotary type axis.
        /// </summary>
        RotaryVelocityOverrideValue RotaryVelocityOverride { get; set; }

        /// <summary>
        /// The direction of motion.
        /// </summary>
        RotaryDirection Direction { get; set; }

        /// <summary>
        /// The measurement of angular position.
        /// </summary>
        AngleModel Angle { get; set; }

        /// <summary>
        /// The current operating mode for a Rotary type axis.
        /// </summary>
        RotaryMode Mode { get; set; }

        /// <summary>
        /// A mechanism that holds a part, stock material, or any other item in place.
        /// </summary>
        ChuckModel Chuck { get; set; }


        ChuckModel AddChuck(string name);
    }
}