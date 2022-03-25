// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations.Input;
using MTConnect.Streams;
using MTConnect.Observations;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models
{
    /// <summary>
    /// MTConnect Device Model used to access MTConnectDevices, MTConnectSreams, and MTConnectAssets data in a single object
    /// </summary>
    public abstract class AbstractDeviceModel : Device
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, IAsset> _assets = new Dictionary<string, IAsset>();
        private ComponentManager _componentManager;


        internal ComponentManager ComponentManager => _componentManager;

        internal DataItemManager DataItemManager => _componentManager.DataItemManager;


        public override IDescription Description => DeviceDescription;

        public Description DeviceDescription { get; set; }


        public new string Id
        {
            get => base.Id;
            set
            {
                base.Id = value;
                ComponentManager.Id = value;
            }
        }

        /// <summary>
        /// The name of the manufacturer of the Component
        /// </summary>
        public string Manufacturer
        {
            get => base.Description?.Manufacturer;
            set
            {
                if (base.Description == null) base.Description = new Description();
                DeviceDescription.Manufacturer = value;
            }
        }

        /// <summary>
        /// The model description of the Component
        /// </summary>
        public string Model
        {
            get => base.Description?.Model;
            set
            {
                if (base.Description == null) base.Description = new Description();
                DeviceDescription.Model = value;
            }
        }

        /// <summary>
        /// The component's serial number
        /// </summary>
        public string SerialNumber
        {
            get => base.Description?.SerialNumber;
            set
            {
                if (base.Description == null) base.Description = new Description();
                DeviceDescription.SerialNumber = value;
            }
        }

        /// <summary>
        /// The station where the Component is located when a component is part of a manufacturing unit or cell with multiple stations that share the same physical controller.
        /// </summary>
        public string Station
        {
            get => base.Description?.Station;
            set
            {
                if (base.Description == null) base.Description = new Description();
                DeviceDescription.Station = value;
            }
        }

        /// <summary>
        /// Any additional descriptive information the implementer chooses to include regarding the Component.
        /// </summary>
        public new string DescriptionText
        {
            get => base.Description?.CDATA;
            set
            {
                if (base.Description == null) base.Description = new Description();
                DeviceDescription.CDATA = value;
            }
        }


        /// <summary>
        /// List of Child ComponentModels 
        /// </summary>
        public IEnumerable<IComponentModel> ComponentModels => ComponentManager.ComponentModels;

        public override IEnumerable<Component> Components => ComponentManager.Components;


        /// <summary>
        /// List of Child CompositionModels 
        /// </summary>
        public IEnumerable<ICompositionModel> CompositionModels => ComponentManager.CompositionModels;

        public override IEnumerable<Composition> Compositions => ComponentManager.Compositions;


        /// <summary>
        /// List of Child DataItemModels 
        /// </summary>
        public IEnumerable<IDataItemModel> DataItemModels => ComponentManager.DataItemModels;

        public override IEnumerable<DataItem> DataItems => ComponentManager.DataItems;


        public AbstractDeviceModel()
        {
            Init();
        }

        public AbstractDeviceModel(string deviceName, string deviceId = "dev")
        {
            Init(deviceName, deviceId);
        }

        public AbstractDeviceModel(Device device)
        {
            Init(device);
        }


        protected void Init()
        {
            _componentManager = new ComponentManager();
        }

        protected void Init(string deviceName, string deviceId = "dev")
        {
            Id = deviceId;
            Name = deviceName;
            Uuid = deviceId;
            _componentManager = new ComponentManager(deviceId);
        }

        protected void Init(Device device)
        {
            if (device != null)
            {
                Id = device.Id;
                Name = device.Name;
                Uuid = device.Uuid;

                if (device.Description != null)
                {
                    Manufacturer = device.Description.Manufacturer;
                    Model = device.Description.Model;
                    SerialNumber = device.Description.SerialNumber;
                    Station = device.Description.Station;
                    DescriptionText = device.Description.CDATA;
                }

                _componentManager = new ComponentManager(device.Id);

                // Add Components
                var componentModels = ComponentManager.CreateComponentModels(device.Components);
                if (!componentModels.IsNullOrEmpty())
                {
                    foreach (var componentModel in componentModels)
                    {
                        _componentManager.AddComponentModel(componentModel);
                    }
                }

                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        var dataItemModel = new DataItemModel(dataItem);
                        _componentManager.AddDataItem(dataItemModel);
                    }
                }
            }
        }


        public void UpdateObservations(IDeviceStream stream) => ComponentManager.UpdateObservations(stream);


        #region "DataItems"

        public bool DataItemValueExists(string type, string subType = null) => DataItemManager.DataItemValueExists(type, subType);

        public string GetDataItemValue(string type, string subType = null) => DataItemManager.GetDataItemValue(type, subType);

        public T GetDataItemValue<T>(string type, string subType = null) where T : struct => DataItemManager.GetDataItemValue<T>(type, subType);



        public EventValue GetEventValue(string type, string subType = null) => DataItemManager.GetEventValue(type, subType);

        public T GetEventValue<T>(string type, string subType = null) where T : EventValue => DataItemManager.GetEventValue<T>(type, subType);


        public SampleValue GetSampleValue(string type, string subType = null) => DataItemManager.GetSampleValue(type, subType);

        public T GetSampleValue<T>(string type, string subType = null) where T : SampleValue => DataItemManager.GetSampleValue<T>(type, subType);


        public void UpdateDataItem(object value, string type, string subType = null) => DataItemManager.UpdateDataItem(value, type, subType);


        public void AddDataItem(IDataItemModel dataItem, object value) => DataItemManager.AddDataItem(dataItem, value);

        public void AddDataItem(Devices.DataItem dataItem, object value) => DataItemManager.AddDataItem(dataItem, value);


        public IDataItemModel GetDataItem(string type, string subType = null) => DataItemManager.GetDataItem(type, subType);

        public T GetDataItem<T>(string type, string subType = null) where T : IDataItemModel => DataItemManager.GetDataItem<T>(type, subType);

        #endregion

        #region "Conditions"

        public ConditionObservation GetCondition(string type, string subType = null) => DataItemManager.GetCondition(type, subType);

        public bool ConditionValueExists(string type, string subType = null) => DataItemManager.ConditionValueExists(type, subType);


        public void UpdateCondition(ConditionObservation condition, string type, string subType = null) => DataItemManager.UpdateCondition(condition, type, subType);


        public void AddCondition(Devices.DataItem dataItem, ConditionObservation condition) => DataItemManager.AddCondition(dataItem, condition);

        public void AddCondition(IDataItemModel dataItem, ConditionObservation condition) => DataItemManager.AddCondition(dataItem, condition);

        #endregion

        #region "Assets"

        public IAsset GetAsset(string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                lock (_lock)
                {
                    if (_assets.TryGetValue(assetId, out var asset))
                    {
                        return asset;
                    }
                }
            }

            return default;
        }

        public T GetAsset<T>(string assetId) where T: IAsset
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                lock (_lock)
                {
                    if (_assets.TryGetValue(assetId, out IAsset asset))
                    {
                        return (T)asset;
                    }
                }
            }

            return default;
        }

        public IEnumerable<IAsset> GetAssets()
        {
            lock (_lock)
            {
                if (!_assets.IsNullOrEmpty())
                {
                    return _assets.Values.ToList();
                }
            }

            return default;
        }

        public IEnumerable<MTConnect.Assets.CuttingTools.CuttingToolAsset> GetCuttingTools()
        {
            lock (_lock)
            {
                if (!_assets.IsNullOrEmpty())
                {
                    var cuttingToolAssets = _assets.Values.Where(o => o.Type == MTConnect.Assets.CuttingTools.CuttingToolAsset.TypeId);
                    if (!cuttingToolAssets.IsNullOrEmpty())
                    {
                        var cuttingTools = new List<MTConnect.Assets.CuttingTools.CuttingToolAsset>();
                        foreach (var asset in cuttingToolAssets)
                        {
                            cuttingTools.Add((MTConnect.Assets.CuttingTools.CuttingToolAsset)asset);
                        }
                        return cuttingTools;
                    }
                }
            }

            return default;
        }


        protected bool AssetExists(string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                lock (_lock)
                {
                    return _assets.TryGetValue(assetId, out _);
                }
            }

            return false;
        }

        protected void UpdateAsset(string assetId, IAsset asset)
        {
            if (!string.IsNullOrEmpty(assetId) && asset != null)
            {
                lock (_lock)
                {
                    _assets.Remove(assetId);
                    _assets.Add(assetId, asset);
                }
            }
        }


        public void AddAsset(IAsset asset)
        {
            if (asset != null)
            {
                UpdateAsset(asset.AssetId, asset);
            }
        }

        #endregion

        #region "Adapter"

        public IEnumerable<Observation> GetObservations() => ComponentManager.GetObservations();
        //public IEnumerable<ObservationInput> GetObservations(long timestamp = 0) => ComponentManager.GetObservations(timestamp);

        public IEnumerable<ConditionObservationInput> GetConditionObservations(long timestamp = 0) => ComponentManager.GetConditionObservations(timestamp);

        public IEnumerable<IAsset> GetAdapterAssets()
        {
            var objs = new List<IAsset>();

            lock (_lock)
            {
                foreach (var asset in _assets.ToList())
                {
                    objs.Add(asset.Value);
                }
            }

            return objs;
        }

        #endregion

    }
}
