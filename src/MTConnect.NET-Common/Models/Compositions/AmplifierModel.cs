// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.Samples;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// An electronic component or circuit for amplifying power, electric current, or voltage.
    /// </summary>
    public class AmplifierModel : CompositionModel, IAmplifierModel
    {
        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        public TemperatureValue Temperature
        {
            get => GetSampleValue<TemperatureValue>(Devices.Samples.TemperatureDataItem.NameId, Devices.Samples.TemperatureDataItem.GetSubTypeId(Devices.Samples.TemperatureDataItem.SubTypes.ACTUAL));
            set => AddDataItem(new TemperatureDataItem(Id, Devices.Samples.TemperatureDataItem.SubTypes.ACTUAL), value);
        }
        public IDataItemModel TemperatureDataItem => GetDataItem(Devices.Samples.TemperatureDataItem.NameId, Devices.Samples.TemperatureDataItem.GetSubTypeId(Devices.Samples.TemperatureDataItem.SubTypes.ACTUAL));


        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        public AccumulatedTimeValue AccumulatedTime
        {
            get => (AccumulatedTimeValue)GetSampleValue(Devices.DataItem.CreateId(Id, Devices.Samples.AccumulatedTimeDataItem.NameId));
            set => AddDataItem(new AccumulatedTimeDataItem(Id), value);
        }
        public IDataItemModel AccumulatedTimeDataItem => GetDataItem(Devices.Samples.AccumulatedTimeDataItem.NameId);


        public AmplifierModel() 
        {
            Type = AmplifierComposition.TypeId;
        }

        public AmplifierModel(string compositionId)
        {
            Id = compositionId;
            Type = AmplifierComposition.TypeId;
        }
    }
}
