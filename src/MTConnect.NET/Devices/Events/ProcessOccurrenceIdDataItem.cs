// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
{
    /// <summary>
    /// An identifier of a process being executed by the device.
    /// </summary>
    public class ProcessOccurrenceIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_OCCURRENCE_ID";
        public const string NameId = "processOccurrenceId";


        public ProcessOccurrenceIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ProcessOccurrenceIdDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
