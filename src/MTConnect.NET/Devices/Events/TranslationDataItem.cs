// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
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
