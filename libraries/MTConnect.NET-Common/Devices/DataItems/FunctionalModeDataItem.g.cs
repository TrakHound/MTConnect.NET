// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Current intended production status of the Component.
    /// </summary>
    public class FunctionalModeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "FUNCTIONAL_MODE";
        public const string NameId = "functionalMode";
        public const DataItemRepresentation DefaultRepresentation = DataItemRepresentation.VALUE;     
             
        public new const string DescriptionText = "Current intended production status of the Component.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public FunctionalModeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation;  
            
        }

        public FunctionalModeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Representation = DefaultRepresentation; 
            
        }
    }
}