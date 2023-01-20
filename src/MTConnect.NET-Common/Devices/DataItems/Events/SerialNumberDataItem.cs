// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// The serial number associated with a Component, Asset, or Device.
    /// </summary>
    public class SerialNumberDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SERIAL_NUMBER";
        public const string NameId = "serialNumber";
        public new const string DescriptionText = "The serial number associated with a Component, Asset, or Device.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public SerialNumberDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public SerialNumberDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}