// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that holds a part, stock material, or any other item in place.
    /// </summary>
    public class ChuckModel : CompositionModel, IChuckModel
    {
        /// <summary>
        /// An indication of the operating state of a mechanism that holds a part or stock material during a manufacturing process. 
        /// It may also represent a mechanism that holds any other mechanism in place within a piece of equipment.
        /// </summary>
        public ChuckState ChuckState
        {
            get => GetDataItemValue<ChuckState>(DataItem.CreateId(Id, ChuckStateDataItem.NameId));
            set => AddDataItem(new ChuckStateDataItem(Id), value);
        }

        /// <summary>
        /// An indication of the state of an interlock function or control logic state intended to prevent the associated CHUCK component from being operated.
        /// </summary>
        public ChuckInterlock ChuckInterlock
        {
            get => GetDataItemValue<ChuckInterlock>(DataItem.CreateId(Id, ChuckInterlockDataItem.NameId));
            set => AddDataItem(new ChuckInterlockDataItem(Id), value);
        }


        public ChuckModel() 
        {
            Type = ChuckComposition.TypeId;
        }

        public ChuckModel(string compositionId)
        {
            Id = compositionId;
            Type = ChuckComposition.TypeId;
        }
    }
}
