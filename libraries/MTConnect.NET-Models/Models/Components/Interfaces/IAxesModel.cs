// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Axis is an abstract Component that represents linear or rotational motion for a piece of equipment.
    /// </summary>
    public interface IAxesModel : IComponentModel
    {
        /// <summary>
        /// A Linear axis represents prismatic motion along a fixed axis.
        /// </summary>
        IEnumerable<ILinearAxisModel> LinearAxes { get; }

        /// <summary>
        /// A Rotary axis represents rotation about a fixed axis.
        /// </summary>
        IEnumerable<IRotaryAxisModel> RotaryAxes { get; }


        /// <summary>
        /// Gets the Linear Component with the "X" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. X1, X2)</param>
        ILinearAxisModel GetXAxis(string suffix = null);

        /// <summary>
        /// Gets the Linear Component with the "Y" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. Y1, Y2)</param>
        ILinearAxisModel GetYAxis(string suffix = null);

        /// <summary>
        /// Gets the Linear Component with the "Z" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. Z1, Z2)</param>
        ILinearAxisModel GetZAxis(string suffix = null);

        /// <summary>
        /// Gets the Roatry Component with the "A" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. A1, A2)</param>
        IRotaryAxisModel GetAAxis(string suffix = null);

        /// <summary>
        /// Gets the Roatry Component with the "B" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. B1, B2)</param>
        IRotaryAxisModel GetBAxis(string suffix = null);

        /// <summary>
        /// Gets the Roatry Component with the "C" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. C1, C2)</param>
        IRotaryAxisModel GetCAxis(string suffix = null);
    }
}