// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication of the reason that Execution is reporting a value of `WAIT`.
    /// </summary>
    public class WaitStateDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "WAIT_STATE";
        public const string NameId = "waitState";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Indication of the reason that Execution is reporting a value of `WAIT`.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15;       


        public WaitStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public WaitStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}