// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Events;
using MTConnect.Devices.Samples;
using MTConnect.Streams.Events;
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
            get => GetDataItemValue<ConnectionStatus>(DataItem.CreateId(Id, Devices.Events.ConnectionStatusDataItem.NameId));
            set => AddDataItem(new ConnectionStatusDataItem(Id), value);
        }
        public IDataItemModel ConnectionStatusDataItem => GetDataItem(Devices.Events.ConnectionStatusDataItem.NameId);

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public double ObservationUpdateRate
        {
            get => GetSampleValue(DataItem.CreateId(Id, Devices.Samples.ObservationUpdateRateDataItem.NameId)).ToDouble();
            set => AddDataItem(new ObservationUpdateRateDataItem(Id), value);
        }
        public IDataItemModel ObservationUpdateRateDataItem => GetDataItem(Devices.Samples.ObservationUpdateRateDataItem.NameId);

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public double AssetUpdateRate
        {
            get => GetSampleValue(DataItem.CreateId(Id, Devices.Samples.AssetUpdateRateDataItem.NameId)).ToDouble();
            set => AddDataItem(new AssetUpdateRateDataItem(Id), value);
        }
        public IDataItemModel AssetUpdateRateDataItem => GetDataItem(Devices.Samples.AssetUpdateRateDataItem.NameId);

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public Version AdapterSoftwareVersion
        {
            get
            {
                var valueString = GetDataItemValue(DataItem.CreateId(Id, Devices.Events.AdapterSoftwareVersionDataItem.NameId));
                if (!string.IsNullOrEmpty(valueString))
                {
                    if (Version.TryParse(valueString, out var version))
                    {
                        return version;
                    }
                }

                return null;
            }
            set => AddDataItem(new AdapterSoftwareVersionDataItem(Id), value);
        }
        public IDataItemModel AdapterSoftwareVersionDataItem => GetDataItem(Devices.Events.AdapterSoftwareVersionDataItem.NameId);

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public Version MTConnectVersion
        {
            get
            {
                var valueString = GetDataItemValue(DataItem.CreateId(Id, Devices.Events.MTConnectVersionDataItem.NameId));
                if (!string.IsNullOrEmpty(valueString))
                {
                    if (Version.TryParse(valueString, out var version))
                    {
                        return version;
                    }
                }

                return null;
            }
            set => AddDataItem(new MTConnectVersionDataItem(Id), value);
        }
        public IDataItemModel MTConnectVersionDataItem => GetDataItem(Devices.Events.MTConnectVersionDataItem.NameId);


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
