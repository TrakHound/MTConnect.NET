// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Identifier for a fixture.
    /// </summary>
    public class FixtureIdDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "FIXTURE_ID";
        public const string NameId = "fixtureId";
        public new const string DescriptionText = "Identifier for a fixture.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version20;


        public FixtureIdDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public FixtureIdDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
