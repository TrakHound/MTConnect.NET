// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Current mode of the Controller component.
    /// </summary>
    public class ControllerModeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CONTROLLER_MODE";
        public const string NameId = "mode";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Current mode of the Controller component.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10;       


        public ControllerModeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public ControllerModeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}