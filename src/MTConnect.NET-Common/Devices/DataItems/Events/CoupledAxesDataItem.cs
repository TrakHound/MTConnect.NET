// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems.Events
{
    /// <summary>
    /// Refers to the set of associated axes.
    /// </summary>
    public class CoupledAxesDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "COUPLED_AXES";
        public const string NameId = "coupledAxes";
        public new const string DescriptionText = "Refers to the set of associated axes.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version12;


        public CoupledAxesDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public CoupledAxesDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}