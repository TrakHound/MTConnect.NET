// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580378218487_830191_2313

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier for the current workholding or part clamp in use by a piece of equipment.**DEPRECATION WARNING**: Recommend using `FIXTURE_ID` instead.
    /// </summary>
    public class WorkholdingIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WORKHOLDING_ID";
        public const string NameId = "workholdingId";
             
             
        public new const string DescriptionText = "Identifier for the current workholding or part clamp in use by a piece of equipment.**DEPRECATION WARNING**: Recommend using `FIXTURE_ID` instead.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11;       


        public WorkholdingIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
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