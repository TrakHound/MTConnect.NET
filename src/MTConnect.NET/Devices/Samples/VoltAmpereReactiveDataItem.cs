// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Samples
{
    /// <summary>
    /// The measurement of reactive power in an AC electrical circuit(commonly referred to as VAR).
    /// </summary>
    public class VoltAmpereReactiveDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLT_AMPERE_REACTIVE";
        public const string NameId = "voltAmpReact";


        public VoltAmpereReactiveDataItem()
        {
            DataItemCategory = CategoryId;
            Type = TypeId;
            Units = Devices.Units.VOLT_AMPERE_REACTIVE;
        }

        public VoltAmpereReactiveDataItem(string parentId)
        {
            Id = CreateId(parentId, NameId);
            DataItemCategory = CategoryId;
            Type = TypeId;
            Name = NameId;
            Units = Devices.Units.VOLT_AMPERE_REACTIVE;
        }
    }
}
