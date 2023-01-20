// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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