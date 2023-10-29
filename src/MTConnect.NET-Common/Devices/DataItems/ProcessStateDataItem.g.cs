// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Particular condition of the process occurrence at a specific time.
    /// </summary>
    public class ProcessStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROCESS_STATE";
        public const string NameId = "processState";
             
        public new const string DescriptionText = "Particular condition of the process occurrence at a specific time.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version18;       


        public ProcessStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ProcessStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}