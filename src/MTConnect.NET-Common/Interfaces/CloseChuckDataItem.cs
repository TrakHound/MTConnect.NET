// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Operating state of the request to close a chuck.
    /// </summary>
    public class CloseChuckDataItem : InterfaceDataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "CLOSE_CHUCK";
        public const string NameId = "closeChuck";
        public new const string DescriptionText = "Operating state of the request to close a chuck.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public CloseChuckDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public CloseChuckDataItem(
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