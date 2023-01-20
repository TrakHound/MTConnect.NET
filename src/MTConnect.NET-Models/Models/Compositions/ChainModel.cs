// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// An interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public class ChainModel : CompositionModel, IChainModel
    {
        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        public AccumulatedTimeValue AccumulatedTime
        {
            get => (AccumulatedTimeValue)GetSampleValue(DataItem.CreateId(Id, Devices.DataItems.Samples.AccumulatedTimeDataItem.NameId));
            set => AddDataItem(new AccumulatedTimeDataItem(Id), value);
        }
        public IDataItemModel AccumulatedTimeDataItem => GetDataItem(Devices.DataItems.Samples.AccumulatedTimeDataItem.NameId);


        public ChainModel() 
        {
            Type = ChainComposition.TypeId;
        }

        public ChainModel(string compositionId)
        {
            Id = compositionId;
            Type = ChainComposition.TypeId;
        }
    }
}