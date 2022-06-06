// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Conditions;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Models.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Samples.Values;
using System;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A viscous liquid.
    /// </summary>
    public class OilModel : CompositionModel, IOilModel
    {
        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        public TemperatureValue Temperature
        {
            get => GetSampleValue<TemperatureValue>(Devices.DataItems.Samples.TemperatureDataItem.NameId, Devices.DataItems.Samples.TemperatureDataItem.GetSubTypeId(Devices.DataItems.Samples.TemperatureDataItem.SubTypes.ACTUAL));
            set => AddDataItem(new TemperatureDataItem(Id), value);
        }
        public IDataItemModel TemperatureDataItem => GetDataItem(Devices.DataItems.Samples.TemperatureDataItem.NameId, Devices.DataItems.Samples.TemperatureDataItem.GetSubTypeId(Devices.DataItems.Samples.TemperatureDataItem.SubTypes.ACTUAL));

        /// <summary>
        /// The force per unit area measured relative to atmospheric pressure.
        /// </summary>
        public PressureValue Pressure
        {
            get => GetSampleValue<PressureValue>(Devices.DataItems.Samples.PressureDataItem.NameId);
            set => AddDataItem(new PressureDataItem(Id), value);
        }
        public IDataItemModel PressureDataItem => GetDataItem(Devices.DataItems.Samples.PressureDataItem.NameId);

        /// <summary>
        /// The measurement of a fluids resistance to flow
        /// </summary>
        public ViscosityValue Viscosity
        {
            get => GetSampleValue<ViscosityValue>(Devices.DataItems.Samples.ViscosityDataItem.NameId);
            set => AddDataItem(new TemperatureDataItem(Id), value);
        }
        public IDataItemModel ViscosityDataItem => GetDataItem(Devices.DataItems.Samples.ViscosityDataItem.NameId);

        /// <summary>
        /// The measurement of the percentage of one component within a mixture of components
        /// </summary>
        public ConcentrationValue Concentration
        {
            get => GetSampleValue<ConcentrationValue>(Devices.DataItems.Samples.ConcentrationDataItem.NameId);
            set => AddDataItem(new ConcentrationDataItem(Id), value);
        }
        public IDataItemModel ConcentrationDataItem => GetDataItem(Devices.DataItems.Samples.ConcentrationDataItem.NameId);

        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        public AccumulatedTimeValue AccumulatedTime
        {
            get => GetSampleValue<AccumulatedTimeValue>(Devices.DataItems.Samples.AccumulatedTimeDataItem.NameId);
            set => AddDataItem(new AccumulatedTimeDataItem(Id), value);
        }
        public IDataItemModel AccumulatedTimeDataItem => GetDataItem(Devices.DataItems.Samples.AccumulatedTimeDataItem.NameId);

        /// <summary>
        /// The time and date code associated with a material or other physical item.
        /// </summary>
        public DateCodeModel DateCode
        {
            get => GetDateCode();
            set => SetDateCode(value);
        }

        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        public ConditionObservation SystemCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.SystemCondition.NameId);
            set => AddCondition(new SystemCondition(Id), value);
        }
        public IDataItemModel SystemConditionDataItem => GetDataItem(Devices.DataItems.Conditions.SystemCondition.NameId);

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        public ConditionObservation HardwareCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.HardwareCondition.NameId);
            set => AddCondition(new HardwareCondition(Id), value);
        }
        public IDataItemModel HardwareConditionDataItem => GetDataItem(Devices.DataItems.Conditions.HardwareCondition.NameId);

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        public ConditionObservation CommunicationsCondition
        {
            get => GetCondition(Devices.DataItems.Conditions.CommunicationsCondition.NameId);
            set => AddCondition(new CommunicationsCondition(Id), value);
        }
        public IDataItemModel CommunicationsConditionDataItem => GetDataItem(Devices.DataItems.Conditions.CommunicationsCondition.NameId);


        public OilModel() 
        {
            Type = OilComposition.TypeId;
        }

        public OilModel(string compositionId)
        {
            Id = compositionId;
            Type = OilComposition.TypeId;
        }


        protected DateCodeModel GetDateCode()
        {
            var x = new DateCodeModel();

            // Manufacture
            x.Manufacture = GetDataItemValue(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.MANUFACTURE)).ToDateTime();
            x.ManufactureDataItem = GetDataItem(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.MANUFACTURE));

            // Expiration
            x.Expiration = GetDataItemValue(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.EXPIRATION)).ToDateTime();
            x.ExpirationDataItem = GetDataItem(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.EXPIRATION));

            // First Use
            x.FirstUse = GetDataItemValue(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.FIRST_USE)).ToDateTime();
            x.FirstUseDataItem = GetDataItem(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.FIRST_USE));

            return x;
        }

        protected void SetDateCode(DateCodeModel model)
        {
            if (model != null)
            {
                if (model.Manufacture > DateTime.MinValue)
                {
                    // Manufacture
                    AddDataItem(new DateCodeDataItem(Id, DateCodeDataItem.SubTypes.MANUFACTURE), model.Manufacture.ToString("o"));
                }

                if (model.Expiration > DateTime.MinValue)
                {
                    // Expiration
                    AddDataItem(new DateCodeDataItem(Id, DateCodeDataItem.SubTypes.EXPIRATION), model.Expiration.ToString("o"));
                }

                if (model.FirstUse > DateTime.MinValue)
                {
                    // First Use
                    AddDataItem(new DateCodeDataItem(Id, DateCodeDataItem.SubTypes.FIRST_USE), model.FirstUse.ToString("o"));
                }
            }
        }
    }
}
