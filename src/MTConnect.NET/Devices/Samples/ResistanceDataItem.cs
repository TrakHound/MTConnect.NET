// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of the degree to which a substance opposes the passage of an electric current.
    /// </summary>
    public class ResistanceDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "RESISTANCE";
        public const string NameId = "resistance";


        public ResistanceDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            Units = Devices.Units.OHM;
        }

        public ResistanceDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = Devices.Units.OHM;
        }
    }
}
