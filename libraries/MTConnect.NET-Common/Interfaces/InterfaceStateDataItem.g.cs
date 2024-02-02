// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operational state of an Interface.
    /// </summary>
    public class InterfaceStateDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "INTERFACE_STATE";
        public const string NameId = "interfaceState";
             
        public new const string DescriptionText = "Operational state of an Interface.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public InterfaceStateDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public InterfaceStateDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}