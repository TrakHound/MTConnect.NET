// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Identifier of a material used or consumed in the manufacturing process.
    /// </summary>
    public class MaterialDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MATERIAL";
        public const string NameId = "material";
             
             
        public new const string DescriptionText = "Identifier of a material used or consumed in the manufacturing process.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14;       


        public MaterialDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public MaterialDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}