// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Cooling is a System used to to extract controlled amounts of heat to achieve a target temperature at a specified cooling rate.
    /// </summary>
    public class CoolingModel : SystemModel, ICoolingModel
    {
        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        public TemperatureModel Temperature
        {
            get => GetTemperature();
            set => SetTemperature(value);
        }


        public CoolingModel()
        {
            Type = CoolingComponent.TypeId;
        }

        public CoolingModel(string componentId)
        {
            Id = componentId;
            Type = CoolingComponent.TypeId;
        }


        protected TemperatureModel GetTemperature()
        {
            var x = new TemperatureModel();

            // Actual
            x.Actual = (TemperatureValue)DataItemManager.GetSampleValue(TemperatureDataItem.NameId, TemperatureDataItem.GetSubTypeId(TemperatureDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = DataItemManager.GetDataItem(TemperatureDataItem.NameId, TemperatureDataItem.GetSubTypeId(TemperatureDataItem.SubTypes.ACTUAL));

            // Commanded
            x.Commanded = (TemperatureValue)DataItemManager.GetSampleValue(TemperatureDataItem.NameId, TemperatureDataItem.GetSubTypeId(TemperatureDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = DataItemManager.GetDataItem(TemperatureDataItem.NameId, TemperatureDataItem.GetSubTypeId(TemperatureDataItem.SubTypes.COMMANDED));

            return x;

        }

        protected void SetTemperature(TemperatureModel model)
        {
            if (model != null)
            {
                // Actual
                DataItemManager.AddDataItem(new TemperatureDataItem(Id, TemperatureDataItem.SubTypes.ACTUAL), model.Actual);

                // Commanded
                DataItemManager.AddDataItem(new TemperatureDataItem(Id, TemperatureDataItem.SubTypes.COMMANDED), model.Commanded);
            }
        }
    }
}
