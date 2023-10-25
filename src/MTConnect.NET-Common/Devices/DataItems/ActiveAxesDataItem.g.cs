// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Set of axes currently associated with a Path or Controller.
    /// </summary>
    public class ActiveAxesDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "ACTIVE_AXES";
        public const string NameId = "";
             
        public new const string DescriptionText = "Set of axes currently associated with a Path or Controller.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public ActiveAxesDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ActiveAxesDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}