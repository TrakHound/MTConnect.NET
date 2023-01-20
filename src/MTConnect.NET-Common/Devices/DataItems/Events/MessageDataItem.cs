// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Any text string of information to be transferred from a piece of equipment to a client software application.
    /// </summary>
    public class MessageDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "MESSAGE";
        public const string NameId = "msg";
        public new const string DescriptionText = "Any text string of information to be transferred from a piece of equipment to a client software application.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public MessageDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public MessageDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
