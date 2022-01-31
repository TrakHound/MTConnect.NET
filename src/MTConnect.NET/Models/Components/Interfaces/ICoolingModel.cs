// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.DataItems;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Cooling is a System used to to extract controlled amounts of heat to achieve a target temperature at a specified cooling rate.
    /// </summary>
    public interface ICoolingModel : ISystemModel
    {
        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        TemperatureModel Temperature { get; set; }
    }
}
