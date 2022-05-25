// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Models.Components;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;
using System;

namespace MTConnect.Models
{
    /// <summary>
    /// MTConnect Device Model used to access MTConnectDevices, MTConnectSreams, and MTConnectAssets data in a single object
    /// </summary>
    public class DeviceModel : AbstractDeviceModel, IDeviceModel
    {
        /// <summary>
        /// Represents the Agent’s ability to communicate with the data source.
        /// </summary>
        public Availability Availability
        {
            get => DataItemManager.GetDataItemValue<Availability>(Devices.DataItems.Events.AvailabilityDataItem.TypeId);
            set => DataItemManager.AddDataItem(new AvailabilityDataItem(Id), value);
        }
        public IDataItemModel AvailabilityDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.AvailabilityDataItem.TypeId);


        /// <summary>
        /// The reference version of the MTConnect Standard supported by the Adapter.
        /// </summary>
        public override Version MTConnectVersion
        {
            get
            {
                var versionString = DataItemManager.GetDataItemValue(Devices.DataItems.Events.MTConnectVersionDataItem.TypeId);
                if (!string.IsNullOrEmpty(versionString))
                {
                    if (Version.TryParse(versionString, out var version))
                    {
                        return version;
                    }
                }
                return null;
            }
            set => DataItemManager.AddDataItem(new MTConnectVersionDataItem(Id), value);
        }
        public IDataItemModel MTConnectVersionDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.MTConnectVersionDataItem.TypeId);

        /// <summary>
        /// Network details of a component.
        /// </summary>
        public NetworkModel Network
        {
            get => DataItemManager.GetNetwork();
            set => DataItemManager.SetNetwork(value);
        }

        /// <summary>
        /// The Operating System of a component.
        /// </summary>
        public OperatingSystemModel OperatingSystem
        {
            get => DataItemManager.GetOperatingSystem();
            set => DataItemManager.SetOperatingSystem(value);
        }

        /// <summary>
        /// Axis is an abstract Component that represents linear or rotational motion for a piece of equipment.
        /// </summary>
        public IAxesModel Axes => ComponentManager.GetComponentModel<AxesModel>(typeof(AxesComponent));

        /// <summary>
        /// Controller represents the computational regulation and management function of a piece of equipment.
        /// </summary>
        public IControllerModel Controller => ComponentManager.GetComponentModel<ControllerModel>(typeof(ControllerComponent));

        /// <summary>
        /// Systems organizes System component types
        /// </summary>
        public ISystemsModel Systems => ComponentManager.GetComponentModel<SystemsModel>(typeof(SystemsComponent));

        /// <summary>
        /// Auxiliaries organizes Auxiliary component types.
        /// </summary>
        public IAuxiliariesModel Auxiliaries => ComponentManager.GetComponentModel<AuxiliariesModel>(typeof(AuxiliariesComponent));



        public DeviceModel()
        {
            Init();
        }

        public DeviceModel(string deviceName, string deviceId = "dev")
        {
            Init(deviceName, deviceId);
        }

        public DeviceModel(IDevice device)
        {
            Init(device);
        }


        public virtual CartesianCoordinateAxesModel AddAxes(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new CartesianCoordinateAxesModel
                {
                    Id = Component.CreateId(Id, name),
                    Name = name
                };

                ComponentManager.AddComponentModel(model);
                return model;
            }

            return null;
        }


        public virtual ControllerModel AddController(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new ControllerModel
                {
                    Id = Component.CreateId(Id, name),
                    Name = name
                };

                ComponentManager.AddComponentModel(model);
                return model;
            }

            return null;
        }


        public T GetController<T>() where T : ControllerModel
        {
            return (T)Controller;
        }


        public virtual SystemsModel AddSystems(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new SystemsModel
                {
                    Id = Component.CreateId(Id, name),
                    Name = name
                };

                ComponentManager.AddComponentModel(model);
                return model;
            }

            return null;
        }


        public virtual AuxiliariesModel AddAuxiliaries(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new AuxiliariesModel
                {
                    Id = Component.CreateId(Id, name),
                    Name = name
                };

                ComponentManager.AddComponentModel(model);
                return model;
            }

            return null;
        }
    }
}
