// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Method used to compute standard uncertainty.
    /// </summary>
    public class UncertaintyTypeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "UNCERTAINTY_TYPE";
        public const string NameId = "uncertaintyType";
             
        public new const string DescriptionText = "Method used to compute standard uncertainty.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22;       


        public UncertaintyTypeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public UncertaintyTypeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}