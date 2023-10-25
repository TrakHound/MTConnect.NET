// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier for the current workholding or part clamp in use by a piece of equipment.
    /// </summary>
    public class WorkholdingIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WORKHOLDING_ID";
        public const string NameId = "";
             
        public new const string DescriptionText = "Identifier for the current workholding or part clamp in use by a piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public WorkholdingIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public WorkholdingIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}