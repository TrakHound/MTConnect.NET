// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Events
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
