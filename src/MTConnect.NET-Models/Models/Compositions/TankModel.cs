// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Conditions;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Observations.Samples.Values;

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
            get => GetSampleValue<CapacityFluidValue>(Devices.DataItems.Samples.CapacityFluidDataItem.NameId);
            set => AddDataItem(new CapacityFluidDataItem(Id), value);
        }
        public IDataItemModel CapacityFluidDataItem => GetDataItem(Devices.DataItems.Samples.CapacityFluidDataItem.NameId);

        /// <summary>
        /// The measurement of the amount of a substance remaining compared to the planned maximum amount of that substance.
        /// </summary>
        public FillLevelValue FillLevel
        {
            get => GetSampleValue<FillLevelValue>(Devices.DataItems.Samples.FillLevelDataItem.NameId);
            set => AddDataItem(new FillLevelDataItem(Id), value);
        }
        public IDataItemModel FillLevelDataItem => GetDataItem(Devices.DataItems.Samples.FillLevelDataItem.NameId);

        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        public Observations.IConditionObservation SystemCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.SystemCondition.NameId);
            set => AddCondition(new SystemCondition(Id), value);
        }
        public IDataItemModel SystemConditionDataItem => GetDataItem(Devices.DataItems.Conditions.SystemCondition.NameId);

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        public Observations.IConditionObservation HardwareCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.HardwareCondition.NameId);
            set => AddCondition(new HardwareCondition(Id), value);
        }
        public IDataItemModel HardwareConditionDataItem => GetDataItem(Devices.DataItems.Conditions.HardwareCondition.NameId);

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        public Observations.IConditionObservation CommunicationsCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.CommunicationsCondition.NameId);
            set => AddCondition(new CommunicationsCondition(Id), value);
        }
        public IDataItemModel CommunicationsConditionDataItem => GetDataItem(Devices.DataItems.Conditions.CommunicationsCondition.NameId);


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
