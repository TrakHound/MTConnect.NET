// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Events;
using MTConnect.Devices.Samples;
using MTConnect.Observations.Events.Values;
using System;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Adapter is a Component that represents the connectivity state of a data source for the MTConnect Agent.
    /// </summary>
    public class AdapterModel : ComponentModel, IAdapterModel
    {
        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public ConnectionStatus ConnectionStatus
        {
            get => DataItemManager.GetDataItemValue<ConnectionStatus>(DataItem.CreateId(Id, Devices.Events.ConnectionStatusDataItem.NameId));
            set => DataItemManager.AddDataItem(new ConnectionStatusDataItem(Id), value);
        }
        public IDataItemModel ConnectionStatusDataItem => DataItemManager.GetDataItem(Devices.Events.ConnectionStatusDataItem.NameId);

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public double ObservationUpdateRate
        {
            get => DataItemManager.GetSampleValue(DataItem.CreateId(Id, Devices.Samples.ObservationUpdateRateDataItem.NameId)).ToDouble();
            set => DataItemManager.AddDataItem(new ObservationUpdateRateDataItem(Id), value);
        }
        public IDataItemModel ObservationUpdateRateDataItem => DataItemManager.GetDataItem(Devices.Samples.ObservationUpdateRateDataItem.NameId);

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public double AssetUpdateRate
        {
            get => DataItemManager.GetSampleValue(DataItem.CreateId(Id, Devices.Samples.AssetUpdateRateDataItem.NameId)).ToDouble();
            set => DataItemManager.AddDataItem(new AssetUpdateRateDataItem(Id), value);
        }
        public IDataItemModel AssetUpdateRateDataItem => DataItemManager.GetDataItem(Devices.Samples.AssetUpdateRateDataItem.NameId);

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public Version AdapterSoftwareVersion
        {
            get
            {
                var valueString = DataItemManager.GetDataItemValue(DataItem.CreateId(Id, Devices.Events.AdapterSoftwareVersionDataItem.NameId));
                if (!string.IsNullOrEmpty(valueString))
                {
                    if (Version.TryParse(valueString, out var version))
                    {
                        return version;
                    }
                }

                return null;
            }
            set => DataItemManager.AddDataItem(new AdapterSoftwareVersionDataItem(Id), value);
        }
        public IDataItemModel AdapterSoftwareVersionDataItem => DataItemManager.GetDataItem(Devices.Events.AdapterSoftwareVersionDataItem.NameId);

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public Version MTConnectVersion
        {
            get
            {
                var valueString = DataItemManager.GetDataItemValue(DataItem.CreateId(Id, Devices.Events.MTConnectVersionDataItem.NameId));
                if (!string.IsNullOrEmpty(valueString))
                {
                    if (Version.TryParse(valueString, out var version))
                    {
                        return version;
                    }
                }

                return null;
            }
            set => DataItemManager.AddDataItem(new MTConnectVersionDataItem(Id), value);
        }
        public IDataItemModel MTConnectVersionDataItem => DataItemManager.GetDataItem(Devices.Events.MTConnectVersionDataItem.NameId);


        public AdapterModel() 
        {
            Type = AdapterComponent.TypeId;
        }

        public AdapterModel(string componentId)
        {
            Id = componentId;
            Type = AdapterComponent.TypeId;
        }
    }
}
