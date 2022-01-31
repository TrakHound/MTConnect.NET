// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.Conditions;
using MTConnect.Devices.Samples;
using MTConnect.Streams.Samples;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A receptacle or container for holding material.
    /// </summary>
    public class TankModel : CompositionModel, ITankModel
    {
        /// <summary>
        /// The fluid capacity of an object or container.
        /// </summary>
        public CapacityFluidValue CapacityFluid
        {
            get => GetSampleValue<CapacityFluidValue>(Devices.Samples.CapacityFluidDataItem.NameId);
            set => AddDataItem(new CapacityFluidDataItem(Id), value);
        }
        public IDataItemModel CapacityFluidDataItem => GetDataItem(Devices.Samples.CapacityFluidDataItem.NameId);

        /// <summary>
        /// The measurement of the amount of a substance remaining compared to the planned maximum amount of that substance.
        /// </summary>
        public FillLevelValue FillLevel
        {
            get => GetSampleValue<FillLevelValue>(Devices.Samples.FillLevelDataItem.NameId);
            set => AddDataItem(new FillLevelDataItem(Id), value);
        }
        public IDataItemModel FillLevelDataItem => GetDataItem(Devices.Samples.FillLevelDataItem.NameId);

        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        public Streams.Condition SystemCondition
        {
            get => GetCondition(Devices.Conditions.SystemCondition.NameId);
            set => AddCondition(new SystemCondition(Id), value);
        }
        public IDataItemModel SystemConditionDataItem => GetDataItem(Devices.Conditions.SystemCondition.NameId);

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        public Streams.Condition HardwareCondition
        {
            get => GetCondition(Devices.Conditions.HardwareCondition.NameId);
            set => AddCondition(new HardwareCondition(Id), value);
        }
        public IDataItemModel HardwareConditionDataItem => GetDataItem(Devices.Conditions.HardwareCondition.NameId);

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        public Streams.Condition CommunicationsCondition
        {
            get => GetCondition(Devices.Conditions.CommunicationsCondition.NameId);
            set => AddCondition(new CommunicationsCondition(Id), value);
        }
        public IDataItemModel CommunicationsConditionDataItem => GetDataItem(Devices.Conditions.CommunicationsCondition.NameId);


        public TankModel() 
        {
            Type = TankComposition.TypeId;
        }

        public TankModel(string compositionId)
        {
            Id = compositionId;
            Type = TankComposition.TypeId;
        }
    }
}
