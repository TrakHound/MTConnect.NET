// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Observations.Samples.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Environmental is an Auxiliary that represents the information for a unit or function involved in monitoring, managing, or conditioning the environment around or within a piece of equipment.
    /// </summary>
    public class EnvironmentalModel : AuxiliaryModel, IEnvironmentalModel
    {
        /// <summary>
        /// The measurement of temperature.
        /// </summary>
        public TemperatureValue Temperature
        {
            get => DataItemManager.GetSampleValue<TemperatureValue>(Devices.DataItems.Samples.TemperatureDataItem.NameId);
            set => DataItemManager.AddDataItem(new TemperatureDataItem(Id), value);
        }
        public IDataItemModel TemperatureDataItem => DataItemManager.GetDataItem(Devices.DataItems.Samples.TemperatureDataItem.NameId);

        /// <summary>
        /// The amount of water vapor expressed in grams per cubic meter.
        /// </summary>
        public HumidityAbsoluteValue HumidityAbsolute
        {
            get => DataItemManager.GetSampleValue<HumidityAbsoluteValue>(Devices.DataItems.Samples.HumidityAbsoluteDataItem.NameId);
            set => DataItemManager.AddDataItem(new HumidityAbsoluteDataItem(Id), value);
        }
        public IDataItemModel HumidityAbsoluteDataItem => DataItemManager.GetDataItem(Devices.DataItems.Samples.HumidityAbsoluteDataItem.NameId);

        /// <summary>
        /// The amount of water vapor present expressed as a percent to reach saturation at the same temperature.
        /// </summary>
        public HumidityRelativeValue HumidityRelative
        {
            get => DataItemManager.GetSampleValue<HumidityRelativeValue>(Devices.DataItems.Samples.HumidityRelativeDataItem.NameId);
            set => DataItemManager.AddDataItem(new HumidityRelativeDataItem(Id), value);
        }
        public IDataItemModel HumidityRelativeDataItem => DataItemManager.GetDataItem(Devices.DataItems.Samples.HumidityRelativeDataItem.NameId);

        /// <summary>
        /// The ratio of the water vapor present over the total weight of the water vapor and air present expressed as a percent.
        /// </summary>
        public HumiditySpecificValue HumiditySpecific
        {
            get => DataItemManager.GetSampleValue<HumiditySpecificValue>(Devices.DataItems.Samples.HumiditySpecificDataItem.NameId);
            set => DataItemManager.AddDataItem(new HumiditySpecificDataItem(Id), value);
        }
        public IDataItemModel HumiditySpecificDataItem => DataItemManager.GetDataItem(Devices.DataItems.Samples.HumiditySpecificDataItem.NameId);


        public EnvironmentalModel()
        {
            Type = EnvironmentalComponent.TypeId;
        }

        public EnvironmentalModel(string componentId)
        {
            Id = componentId;
            Type = EnvironmentalComponent.TypeId;
        }
    }
}
