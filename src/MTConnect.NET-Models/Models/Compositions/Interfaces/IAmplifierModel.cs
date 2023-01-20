// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.DataItems.Samples;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// An electronic component or circuit for amplifying power, electric current, or voltage.
    /// </summary>
    public interface IAmplifierModel : ICompositionModel
    {
        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        TemperatureValue Temperature { get; set; }
        IDataItemModel TemperatureDataItem { get; }


        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        AccumulatedTimeValue AccumulatedTime { get; set; }
        IDataItemModel AccumulatedTimeDataItem { get; }
    }
}
