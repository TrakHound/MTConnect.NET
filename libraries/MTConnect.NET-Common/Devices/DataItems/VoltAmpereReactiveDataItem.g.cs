// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Reactive power in an AC electrical circuit (commonly referred to as VAR).
    /// </summary>
    public class VoltAmpereReactiveDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "VOLT_AMPERE_REACTIVE";
        public const string NameId = "voltAmpereReactive";
             
        public const string DefaultUnits = Devices.Units.VOLT_AMPERE_REACTIVE;     
        public new const string DescriptionText = "Reactive power in an AC electrical circuit (commonly referred to as VAR).";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version12;       


        public VoltAmpereReactiveDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public VoltAmpereReactiveDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}