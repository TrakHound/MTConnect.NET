// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Samples;
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
            get => GetSampleValue<TemperatureValue>(Devices.DataItems.Samples.TemperatureDataItem.NameId, Devices.DataItems.Samples.TemperatureDataItem.GetSubTypeId(Devices.DataItems.Samples.TemperatureDataItem.SubTypes.ACTUAL));
            set => AddDataItem(new TemperatureDataItem(Id, Devices.DataItems.Samples.TemperatureDataItem.SubTypes.ACTUAL), value);
        }
        public IDataItemModel TemperatureDataItem => GetDataItem(Devices.DataItems.Samples.TemperatureDataItem.NameId, Devices.DataItems.Samples.TemperatureDataItem.GetSubTypeId(Devices.DataItems.Samples.TemperatureDataItem.SubTypes.ACTUAL));


        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        public AccumulatedTimeValue AccumulatedTime
        {
            get => (AccumulatedTimeValue)GetSampleValue(Devices.DataItem.CreateId(Id, Devices.DataItems.Samples.AccumulatedTimeDataItem.NameId));
            set => AddDataItem(new AccumulatedTimeDataItem(Id), value);
        }
        public IDataItemModel AccumulatedTimeDataItem => GetDataItem(Devices.DataItems.Samples.AccumulatedTimeDataItem.NameId);


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