// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Environmental is an Auxiliary that represents the information for a unit or function involved in monitoring, managing, or conditioning the environment around or within a piece of equipment.
    /// </summary>
    public interface IEnvironmentalModel : IAuxiliaryModel
    {
        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        TemperatureValue Temperature { get; set; }
        IDataItemModel TemperatureDataItem { get; }

        /// <summary>
        /// The amount of water vapor expressed in grams per cubic meter.
        /// </summary>
        HumidityAbsoluteValue HumidityAbsolute { get; set; }
        IDataItemModel HumidityAbsoluteDataItem { get; }

        /// <summary>
        /// The amount of water vapor present expressed as a percent to reach saturation at the same temperature.
        /// </summary>
        HumidityRelativeValue HumidityRelative { get; set; }
        IDataItemModel HumidityRelativeDataItem { get; }

        /// <summary>
        /// The ratio of the water vapor present over the total weight of the water vapor and air present expressed as a percent.
        /// </summary>
        HumiditySpecificValue HumiditySpecific { get; set; }
        IDataItemModel HumiditySpecificDataItem { get; }
    }
}