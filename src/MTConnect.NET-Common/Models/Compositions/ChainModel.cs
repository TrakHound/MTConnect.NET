// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.Samples;
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
            get => (AccumulatedTimeValue)GetSampleValue(DataItem.CreateId(Id, Devices.Samples.AccumulatedTimeDataItem.NameId));
            set => AddDataItem(new AccumulatedTimeDataItem(Id), value);
        }
        public IDataItemModel AccumulatedTimeDataItem => GetDataItem(Devices.Samples.AccumulatedTimeDataItem.NameId);


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
