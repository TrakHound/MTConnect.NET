// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using System.Collections.Generic;

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// An MTConnect Component that represents an SHDR Adapter
    /// </summary>
    public class ShdrAdapterComponent : AdapterComponent
    {
        public string Uri { get; set; }


        /// <summary>
        /// Add a new Adapter Component to the Agent Device
        /// </summary>
        public ShdrAdapterComponent(IShdrAdapterClientConfiguration configuration, string idSuffix = null, IDevice device = null, IContainer container = null)
        {
            if (configuration != null && !string.IsNullOrEmpty(configuration.Hostname))
            {
                Id = configuration.Id;
                if (!string.IsNullOrEmpty(idSuffix)) Id = Id != null ? $"{Id}_{idSuffix}" : idSuffix;

                Name = "adapterShdr";
                Uri = $"shdr://{configuration.Hostname}:{configuration.Port}";


                var dataItems = new List<IDataItem>();

                // Add Connection Status
                var connectionStatusDataItem = new ConnectionStatusDataItem(Id) { Device = device, Container = container };
                dataItems.Add(connectionStatusDataItem);

                // Add Adapter URI DataItem
                if (configuration.OutputConnectionInformation)
                {
                    var adapterUriDataItem = new AdapterUriDataItem(Id) { Device = device, Container = container };
                    var adapterUriConstraint = new Constraints();
                    adapterUriConstraint.Values = new List<string> { Uri };
                    adapterUriDataItem.Constraints = adapterUriConstraint;
                    dataItems.Add(adapterUriDataItem);
                }

                // Add Observation Update Rate
                dataItems.Add(new ObservationUpdateRateDataItem(Id) { Device = device, Container = container });

                // Add Asset Update Rate
                dataItems.Add(new AssetUpdateRateDataItem(Id) { Device = device, Container = container });

                DataItems = dataItems;
            }
        }
    }
}