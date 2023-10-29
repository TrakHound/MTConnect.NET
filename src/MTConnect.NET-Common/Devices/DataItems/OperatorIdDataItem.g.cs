// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of the person currently responsible for operating the piece of equipment.
    /// </summary>
    public class OperatorIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "OPERATOR_ID";
        public const string NameId = "operatorId";
             
        public new const string DescriptionText = "Identifier of the person currently responsible for operating the piece of equipment.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public OperatorIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public OperatorIdDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}