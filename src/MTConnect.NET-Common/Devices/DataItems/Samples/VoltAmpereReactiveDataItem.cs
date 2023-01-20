// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.DataItems.Samples
{
    /// <summary>
    /// The measurement of reactive power in an AC electrical circuit(commonly referred to as VAR).
    /// </summary>
    public class VoltAmpereReactiveDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLT_AMPERE_REACTIVE";
        public const string NameId = "voltAmpReact";
        public const string DefaultUnits = Devices.Units.VOLT_AMPERE_REACTIVE;
        public new const string DescriptionText = "The measurement of reactive power in an AC electrical circuit(commonly referred to as VAR).";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version12;


        public VoltAmpereReactiveDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public VoltAmpereReactiveDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = DefaultUnits;
        }
    }
}
