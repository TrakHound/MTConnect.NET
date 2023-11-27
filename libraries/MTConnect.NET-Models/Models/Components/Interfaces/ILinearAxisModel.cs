// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;
using System.Collections.Generic;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// A Linear axis represents prismatic motion along a fixed axis.
    /// </summary>
    public interface ILinearAxisModel : IAxisModel
    {
        /// <summary>
        /// A measured or calculated position of a Component element as reported by a piece of equipment.
        /// </summary>
        PositionModel MachinePosition { get; set; }

        /// <summary>
        /// A measured or calculated position of a Component element as reported by a piece of equipment.
        /// </summary>
        PositionModel WorkPosition { get; set; }

        /// <summary>
        /// The measurement of the feedrate of a linear axis.
        /// </summary>
        AxisFeedrateModel AxisFeedrate { get; set; }

        /// <summary>
        /// The value of a signal or calculation issued to adjust the feedrate of an individual linear type axis.
        /// </summary>
        AxisFeedrateOverrideModel AxisFeedrateOverride { get; set; }

        /// <summary>
        /// The direction of motion.
        /// </summary>
        LinearDirection Direction { get; set; }

        /// <summary>
        /// A Linear axis represents prismatic motion along a fixed axis.
        /// </summary>
        IEnumerable<ILinearAxisModel> LinearAxes { get; }

        /// <summary>
        /// A Rotary axis represents rotation about a fixed axis.
        /// </summary>
        IEnumerable<IRotaryAxisModel> RotaryAxes { get; }
    }
}