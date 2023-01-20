// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// A three space linear translation relative to a coordinate system.
    /// </summary>
    public class TranslationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TRANSLATION";
        public const string NameId = "translation";
        public new const string DescriptionText = "A three space linear translation relative to a coordinate system.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        public TranslationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public TranslationDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}