// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Programmatic code being executed.**DEPRECATED** in *Version 1.1*.
    /// </summary>
    public class CodeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CODE";
        public const string NameId = "code";
             
             
        public new const string DescriptionText = "Programmatic code being executed.**DEPRECATED** in *Version 1.1*.";
        
        public override string TypeDescription => DescriptionText;
        public override System.Version MaximumVersion => MTConnectVersions.Version11;
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public CodeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public CodeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}