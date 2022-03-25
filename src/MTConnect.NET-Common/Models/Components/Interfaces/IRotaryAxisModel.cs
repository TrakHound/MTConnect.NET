// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.Compositions;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;
using System.Collections.Generic;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// A Rotary axis represents rotation about a fixed axis.
    /// </summary>
    public interface IRotaryAxisModel : IAxisModel
    {
        /// <summary>
        /// The measurement of angular position.
        /// </summary>
        AngleModel Angle { get; set; }

        /// <summary>
        /// The measurement of the rate of change of angular position.
        /// </summary>
        AngularVelocityValue AngularVelocity { get; set; }
        IDataItemModel AngularVelocityDataItem { get; }

        /// <summary>
        /// The positive rate of change of angular velocity
        /// </summary>
        AngularAccelerationModel AngularAcceleration { get; set; }

        /// <summary>
        /// Negative rate of change of angular velocity
        /// </summary>
        AngularDecelerationModel AngularDeceleration { get; set; }

        /// <summary>
        /// The direction of motion.
        /// </summary>
        RotaryDirection RotaryDirection { get; set; }
        IDataItemModel RotaryDirectionDataItem { get; }

        /// <summary>
        /// The measurement of the rotational speed of a rotary axis.
        /// </summary>
        RotaryVelocityModel RotaryVelocity { get; set; }

        /// <summary>
        /// The value of a command issued to adjust the programmed velocity for a Rotary type axis.
        /// </summary>
        RotaryVelocityOverrideValue RotaryVelocityOverride { get; set; }

        /// <summary>
        /// The current operating mode for a Rotary type axis.
        /// </summary>
        RotaryMode RotaryMode { get; set; }

        /// <summary>
        /// A mechanism that holds a part, stock material, or any other item in place.
        /// </summary>
        ChuckModel Chuck { get; set; }

        /// <summary>
        /// A Linear axis represents prismatic motion along a fixed axis.
        /// </summary>
        IEnumerable<ILinearAxisModel> LinearAxes { get; }

        /// <summary>
        /// A Rotary axis represents rotation about a fixed axis.
        /// </summary>
        IEnumerable<IRotaryAxisModel> RotaryAxes { get; }

        ///// <summary>
        ///// A mechanism that holds a part, stock material, or any other item in place.
        ///// </summary>
        //ChuckModel AddChuck(string name);
    }
}
