// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the service to close a door.
    /// </summary>
    public class CloseDoorDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CLOSE_DOOR";
        public const string NameId = "closeDoor";
        public new const string DescriptionText = "Operating state of the service to close a door.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public CloseDoorDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public CloseDoorDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
        }
    }
}