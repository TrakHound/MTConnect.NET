// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using System.Collections.Generic;
using System.Linq;

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

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


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
                Name = "Agent";
                Uuid = _agent.Uuid;
                MTConnectVersion = _agent.Version;
            }
        }

        public void InitializeDataItems()
        {
            var dataItems = new List<IDataItem>();

            // Add Availibility DataItem to Agent
            var availabilityDataItem = new AvailabilityDataItem(Id);
            dataItems.Add(availabilityDataItem);

            // Add Device Added DataItem to Agent
            dataItems.Add(new DeviceAddedDataItem(Id));

            // Add Device Removed DataItem to Agent
            dataItems.Add(new DeviceRemovedDataItem(Id));

            // Add Device Changed DataItem to Agent
            dataItems.Add(new DeviceChangedDataItem(Id));

            // Add Asset Added DataItem to Agent
            dataItems.Add(new AssetChangedDataItem(Id));

            // Add Asset Removed DataItem to Agent
            dataItems.Add(new AssetRemovedDataItem(Id));

            DataItems = dataItems;
        }

        public void InitializeObservations()
        {
            if (_agent != null)
            {
                // Initialize Availability
                _agent.AddObservation(Uuid, DataItem.CreateId(Id, AvailabilityDataItem.NameId), Observations.Events.Values.Availability.AVAILABLE);

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
                    adapters = new AdaptersComponent() { Id = AdaptersId };

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
                    adapterComponents.Add(component);
                    adapters.Components = adapterComponents;

                    var components = new List<IComponent>();
                    components.Add(adapters);
                    Components = components;
                }
            }
        }
    }
}
