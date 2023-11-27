// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Conditions;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Samples.Values;
using System;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
    /// </summary>
    public class MotorModel : CompositionModel, IMotorModel
    {
        /// <summary>
        /// The measurement of the actual versus the standard rating of a piece of equipment.
        /// </summary>
        public LoadValue Load
        {
            get => GetSampleValue<LoadValue>(Devices.DataItems.Samples.LoadDataItem.NameId);
            set => AddDataItem(new LoadDataItem(Id), value);
        }
        public IDataItemModel LoadDataItem => GetDataItem(Devices.DataItems.Samples.LoadDataItem.NameId);

        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        public TemperatureValue Temperature
        {
            get => GetSampleValue<TemperatureValue>(Devices.DataItems.Samples.TemperatureDataItem.NameId, Devices.DataItems.Samples.TemperatureDataItem.GetSubTypeId(Devices.DataItems.Samples.TemperatureDataItem.SubTypes.ACTUAL));
            set => AddDataItem(new TemperatureDataItem(Id, Devices.DataItems.Samples.TemperatureDataItem.SubTypes.ACTUAL), value);
        }
        public IDataItemModel TemperatureDataItem => GetDataItem(Devices.DataItems.Samples.TemperatureDataItem.NameId, Devices.DataItems.Samples.TemperatureDataItem.GetSubTypeId(Devices.DataItems.Samples.TemperatureDataItem.SubTypes.ACTUAL));

        /// <summary>
        /// The measurement of an electrical current
        /// </summary>
        public AmperageACModel AmperageAC
        {
            get => GetAmperageAC();
            set => SetAmperageAC(value);
        }

        /// <summary>
        /// The measurement of an electrical current
        /// </summary>
        public AmperageDCModel AmperageDC
        {
            get => GetAmperageDC();
            set => SetAmperageDC(value);
        }

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
            set => AddCondition(new Devices.DataItems.Conditions.HardwareCondition(Id), value);
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


        public MotorModel() 
        {
            Type = MotorComposition.TypeId;
        }

        public MotorModel(string compositionId)
        {
            Id = compositionId;
            Type = MotorComposition.TypeId;
        }


        protected AmperageACModel GetAmperageAC()
        {
            var x = new AmperageACModel();

            // Actual
            x.Actual = GetSampleValue<AmperageACValue>(AmperageACDataItem.NameId, AmperageACDataItem.GetSubTypeId(AmperageACDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = GetDataItem(AmperageACDataItem.NameId, AmperageACDataItem.GetSubTypeId(AmperageACDataItem.SubTypes.ACTUAL));

            // Commanded
            x.Commanded = GetSampleValue<AmperageACValue>(AmperageACDataItem.NameId, AmperageACDataItem.GetSubTypeId(AmperageACDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = GetDataItem(AmperageACDataItem.NameId, AmperageACDataItem.GetSubTypeId(AmperageACDataItem.SubTypes.COMMANDED));

            // Programmed
            x.Programmed = GetSampleValue<AmperageACValue>(AmperageACDataItem.NameId, AmperageACDataItem.GetSubTypeId(AmperageACDataItem.SubTypes.PROGRAMMED));
            x.ProgrammedDataItem = GetDataItem(AmperageACDataItem.NameId, AmperageACDataItem.GetSubTypeId(AmperageACDataItem.SubTypes.PROGRAMMED));

            return x;

        }

        protected void SetAmperageAC(AmperageACModel model)
        {
            if (model != null)
            {
                // Actual
                AddDataItem(new AmperageACDataItem(Id, AmperageACDataItem.SubTypes.ACTUAL), model.Actual);

                // Commanded
                AddDataItem(new AmperageACDataItem(Id, AmperageACDataItem.SubTypes.COMMANDED), model.Commanded);

                // Programmed
                AddDataItem(new AmperageACDataItem(Id, AmperageACDataItem.SubTypes.PROGRAMMED), model.Programmed);
            }
        }


        protected AmperageDCModel GetAmperageDC()
        {
            var x = new AmperageDCModel();

            // Actual
            x.Actual = (AmperageDCValue)GetSampleValue(AmperageDCDataItem.NameId, AmperageDCDataItem.GetSubTypeId(AmperageDCDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = GetDataItem(AmperageDCDataItem.NameId, AmperageDCDataItem.GetSubTypeId(AmperageDCDataItem.SubTypes.ACTUAL));

            // Commanded
            x.Commanded = (AmperageDCValue)GetSampleValue(AmperageDCDataItem.NameId, AmperageDCDataItem.GetSubTypeId(AmperageDCDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = GetDataItem(AmperageDCDataItem.NameId, AmperageDCDataItem.GetSubTypeId(AmperageDCDataItem.SubTypes.COMMANDED));

            // Programmed
            x.Programmed = (AmperageDCValue)GetSampleValue(AmperageDCDataItem.NameId, AmperageDCDataItem.GetSubTypeId(AmperageDCDataItem.SubTypes.PROGRAMMED));
            x.ProgrammedDataItem = GetDataItem(AmperageDCDataItem.NameId, AmperageDCDataItem.GetSubTypeId(AmperageDCDataItem.SubTypes.PROGRAMMED));

            return x;

        }

        protected void SetAmperageDC(AmperageDCModel model)
        {
            if (model != null)
            {
                // Actual
                AddDataItem(new AmperageDCDataItem(Id, AmperageDCDataItem.SubTypes.ACTUAL), model.Actual);

                // Commanded
                AddDataItem(new AmperageDCDataItem(Id, AmperageDCDataItem.SubTypes.COMMANDED), model.Commanded);

                // Programmed
                AddDataItem(new AmperageDCDataItem(Id, AmperageDCDataItem.SubTypes.PROGRAMMED), model.Programmed);
            }
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