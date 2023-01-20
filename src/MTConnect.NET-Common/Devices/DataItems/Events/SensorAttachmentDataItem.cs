// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// A SensorAttachment is an Event defining an Attachment between a sensor and an entity.
    /// </summary>
    public class SensorAttachmentDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "SENSOR_ATTACHMENT";
        public const string NameId = "sensorAttachment";
        public new const string DescriptionText = "A SensorAttachment is an Event defining an Attachment between a sensor and an entity.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public SensorAttachmentDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public SensorAttachmentDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}