// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MTConnect.Devices
{
    /// <summary>
    /// Agent is a Device representing the MTConnect Agent and all its connected data sources.
    /// </summary>
    public class Agent : Device 
    {
        public new const string TypeId = "Agent";
        public new const string DescriptionText = "Agent is a Device representing the MTConnect Agent and all its connected data sources.";
        private const string AdaptersId = "__adapters__";


        private readonly MTConnectAgent _agent;


        public override string TypeDescription => DescriptionText;

        public override Version MinimumVersion => MTConnectVersions.Version17;


        public Agent()
        {
            Type = TypeId;
            DataItems = new List<DataItem>();
            Components = new List<Component>();
            Compositions = new List<Composition>();
        }

        public Agent(MTConnectAgent agent)
        {
            Type = TypeId;
            DataItems = new List<DataItem>();
            Components = new List<Component>();
            Compositions = new List<Composition>();

            _agent = agent;
            if (_agent != null)
            {
                Id = $"agent_{_agent.Uuid.ToMD5Hash().Substring(0, 10)}";
                Name = "agent";
                Uuid = _agent.Uuid;
                MTConnectVersion = _agent.MTConnectVersion;
            }
        }

        public void InitializeDataItems()
        {
            var dataItems = new List<IDataItem>();

            // Add Availibility DataItem to Agent
            dataItems.Add(new AvailabilityDataItem(Id) { Device = this, Container = this });

            // Add Application DataItems to Agent
            dataItems.Add(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.MANUFACTURER) { Device = this, Container = this });
            dataItems.Add(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.VERSION) { Device = this, Container = this });

            // Add OperatingSystem DataItems to Agent
            dataItems.Add(new OperatingSystemDataItem(Id, OperatingSystemDataItem.SubTypes.MANUFACTURER) { Device = this, Container = this });
            dataItems.Add(new OperatingSystemDataItem(Id, OperatingSystemDataItem.SubTypes.VERSION) { Device = this, Container = this });

            // Add Device Added DataItem to Agent
            dataItems.Add(new DeviceAddedDataItem(Id) { Device = this, Container = this });

            // Add Device Removed DataItem to Agent
            dataItems.Add(new DeviceRemovedDataItem(Id) { Device = this, Container = this });

            // Add Device Changed DataItem to Agent
            dataItems.Add(new DeviceChangedDataItem(Id) { Device = this, Container = this });

            // Add Asset Added DataItem to Agent
            dataItems.Add(new AssetChangedDataItem(Id) { Device = this, Container = this });

            // Add Asset Removed DataItem to Agent
            dataItems.Add(new AssetRemovedDataItem(Id) { Device = this, Container = this });

            // Add Asset Count DataItem to Agent
            dataItems.Add(new AssetCountDataItem(Id) { Device = this, Container = this });

            DataItems = dataItems;
        }

        public void InitializeObservations()
        {
            if (_agent != null)
            {
                // Initialize Availability
                _agent.AddObservation(Uuid, DataItem.CreateId(Id, AvailabilityDataItem.NameId), Observations.Events.Availability.AVAILABLE);


                // Initialize Application Manufacturer
                var applicationManufacturerName = $"{ApplicationDataItem.NameId}_{ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.MANUFACTURER)}";
                var applicationManufacturerId = DataItem.CreateId(Id, applicationManufacturerName);
                try
                {
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                    _agent.AddObservation(Uuid, applicationManufacturerId, fileVersionInfo.CompanyName);
                }
                catch { }

                // Initialize Application Version
                var applicationVersionName = $"{ApplicationDataItem.NameId}_{ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.VERSION)}";
                var applicationVersionId = DataItem.CreateId(Id, applicationVersionName);
                _agent.AddObservation(Uuid, applicationVersionId, _agent.Version);


                // Initialize OperatingSystem Manufacturer
                var osManufacturerName = $"{OperatingSystemDataItem.NameId}_{OperatingSystemDataItem.GetSubTypeId(OperatingSystemDataItem.SubTypes.MANUFACTURER)}";
                var osManufacturerId = DataItem.CreateId(Id, osManufacturerName);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) _agent.AddObservation(Uuid, osManufacturerId, OSPlatform.Windows.ToString());
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) _agent.AddObservation(Uuid, osManufacturerId, OSPlatform.Linux.ToString());
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) _agent.AddObservation(Uuid, osManufacturerId, OSPlatform.OSX.ToString());
                //else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) _agent.AddObservation(Uuid, osManufacturerId, OSPlatform.FreeBSD.ToString());

                // Initialize OperatingSystem Version
                var osVersionName = $"{OperatingSystemDataItem.NameId}_{OperatingSystemDataItem.GetSubTypeId(OperatingSystemDataItem.SubTypes.VERSION)}";
                var osVersionId = DataItem.CreateId(Id, osVersionName);
                _agent.AddObservation(Uuid, osVersionId, Environment.OSVersion.ToString());


                _agent.InitializeDataItems(this);
            }
        }


        /// <summary>
        /// Add a new Adapter Component to the Agent Device
        /// </summary>
        public void AddAdapterComponent(IComponent component)
        {
            if (component != null)
            {
                // Adapters Organizer Component
                Component adapters = null;
                if (Components.IsNullOrEmpty())
                {
                    // Create New Adapters Organizer Component
                    adapters = new AdaptersComponent() { Id = AdaptersId, Parent = this };

                    var agentComponents = new List<IComponent>();
                    agentComponents.Add(adapters);
                    Components = agentComponents;
                }
                else
                {
                    adapters = Components.FirstOrDefault(o => o.Id == AdaptersId) as Component;
                }

                if (adapters != null)
                {
                    var adapterComponents = new List<IComponent>();
                    if (!adapters.Components.IsNullOrEmpty()) adapterComponents.AddRange(adapters.Components);
                    adapterComponents.Add(component);
                    adapters.Components = adapterComponents;

                    var components = new List<IComponent>();
                    components.Add(adapters);
                    Components = components;
                }

                // Update MTConnectAgent cache
                _agent.UpdateAgentDevice();

                _agent.InitializeDataItems(this);
            }
        }
    }
}