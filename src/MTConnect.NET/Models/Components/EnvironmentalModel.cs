// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;
using MTConnect.Devices.Samples;
using MTConnect.Streams.Samples;

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
            get => GetSampleValue<TemperatureValue>(Devices.Samples.TemperatureDataItem.NameId);
            set => AddDataItem(new TemperatureDataItem(Id), value);
        }
        public IDataItemModel TemperatureDataItem => GetDataItem(Devices.Samples.TemperatureDataItem.NameId);

        /// <summary>
        /// The amount of water vapor expressed in grams per cubic meter.
        /// </summary>
        public HumidityAbsoluteValue HumidityAbsolute
        {
            get => GetSampleValue<HumidityAbsoluteValue>(Devices.Samples.HumidityAbsoluteDataItem.NameId);
            set => AddDataItem(new HumidityAbsoluteDataItem(Id), value);
        }
        public IDataItemModel HumidityAbsoluteDataItem => GetDataItem(Devices.Samples.HumidityAbsoluteDataItem.NameId);

        /// <summary>
        /// The amount of water vapor present expressed as a percent to reach saturation at the same temperature.
        /// </summary>
        public HumidityRelativeValue HumidityRelative
        {
            get => GetSampleValue<HumidityRelativeValue>(Devices.Samples.HumidityRelativeDataItem.NameId);
            set => AddDataItem(new HumidityRelativeDataItem(Id), value);
        }
        public IDataItemModel HumidityRelativeDataItem => GetDataItem(Devices.Samples.HumidityRelativeDataItem.NameId);

        /// <summary>
        /// The ratio of the water vapor present over the total weight of the water vapor and air present expressed as a percent.
        /// </summary>
        public HumiditySpecificValue HumiditySpecific
        {
            get => GetSampleValue<HumiditySpecificValue>(Devices.Samples.HumiditySpecificDataItem.NameId);
            set => AddDataItem(new HumiditySpecificDataItem(Id), value);
        }
        public IDataItemModel HumiditySpecificDataItem => GetDataItem(Devices.Samples.HumiditySpecificDataItem.NameId);


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
