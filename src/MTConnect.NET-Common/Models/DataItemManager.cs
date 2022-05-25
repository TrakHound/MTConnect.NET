// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Models.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models
{
    internal class DataItemManager
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, object> _dataItemValues = new Dictionary<string, object>();
        private readonly Dictionary<string, ConditionObservation> _conditions = new Dictionary<string, ConditionObservation>();


        public string Id { get; set; }

        public List<IDataItemModel> DataItemModels { get; set; }

        public Dictionary<string, object> DataItemValues
        {
            get
            {
                lock (_lock) return _dataItemValues;
            }
        } 

        public Dictionary<string, ConditionObservation> Conditions
        {
            get
            {
                lock (_lock) return _conditions;
            }
        }

        public EventHandler<IObservation> ObservationUpdated { get; set; }


        public DataItemManager() 
        {
            DataItemModels = new List<IDataItemModel>();
        }

        public DataItemManager(string id)
        {
            Id = id;
            DataItemModels = new List<IDataItemModel>();
        }


        private string CreateKey(string type, string subType)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (!string.IsNullOrEmpty(subType))
                {
                    return $"{type}:{subType}";
                }
                else return type;
            }

            return null;
        }


        public IObservation GetObservation(string type, string subType = null)
        {
            var dataItem = GetDataItem(type, subType);
            if (dataItem != null)
            {
                var value = GetDataItemValue(type, subType);
                if (value != null)
                {
                    Observation observation = null;

                    switch (dataItem.Category)
                    {
                        case DataItemCategory.EVENT:

                            observation = EventObservation.Create(dataItem);
                            switch (dataItem.Representation)
                            {
                                case DataItemRepresentation.VALUE: observation.AddValue(ValueKeys.CDATA, value); break;
                            }
                            break;

                        case DataItemCategory.SAMPLE:

                            observation = SampleObservation.Create(dataItem);
                            switch (dataItem.Representation)
                            {
                                case DataItemRepresentation.VALUE: observation.AddValue(ValueKeys.CDATA, value); break;
                            }
                            break;
                    }

                    return observation;
                }
            }

            return null;
        }


        #region "DataItems"

        public bool DataItemValueExists(string type, string subType = null)
        {
            var key = CreateKey(type, subType);
            if (key != null)
            {
                lock (_lock)
                {
                    return _dataItemValues.TryGetValue(key, out _);
                }
            }

            return false;
        }

        public string GetDataItemValue(string type, string subType = null)
        {
            var key = CreateKey(type, subType);
            if (key != null)
            {
                lock (_lock)
                {
                    if (_dataItemValues.TryGetValue(key, out var value) && value != null)
                    {
                        return value.ToString();
                    }
                }
            }

            return default;
        }

        public T GetDataItemValue<T>(string type, string subType = null) where T : struct
        {
            var key = CreateKey(type, subType);
            if (key != null)
            {
                lock (_lock)
                {
                    if (_dataItemValues.TryGetValue(key, out var value) && value != null)
                    {
                        return StringFunctions.ConvertEnum<T>(value.ToString());
                    }
                }
            }

            return default;
        }



        public EventValue GetEventValue(string type, string subType = null)
        {
            if (DataItemValueExists(type, subType))
            {
                return new EventValue(GetDataItemValue(type, subType));
            }

            return default;
        }

        public T GetEventValue<T>(string type, string subType = null) where T : EventValue
        {
            if (DataItemValueExists(type, subType))
            {
                try
                {
                    var obj = (T)Activator.CreateInstance(typeof(T));
                    obj.Value = GetDataItemValue(type, subType).ToDouble();

                    return obj;
                }
                catch { }
            }

            return null;
        }


        public SampleValue GetSampleValue(string type, string subType = null)
        {
            if (DataItemValueExists(type, subType))
            {
                return new SampleValue(GetDataItemValue<double>(type, subType));
            }

            return default;
        }

        public T GetSampleValue<T>(string type, string subType = null) where T : SampleValue
        {
            if (DataItemValueExists(type, subType))
            {
                try
                {
                    var obj = (T)Activator.CreateInstance(typeof(T));
                    obj.Value = GetDataItemValue(type, subType).ToDouble();

                    return obj;
                }
                catch { }
            }

            return null;
        }


        public void UpdateDataItem(object value, string type, string subType = null)
        {
            var key = CreateKey(type, subType);
            if (key != null)
            {
                lock (_lock)
                {
                    _dataItemValues.Remove(key);
                    _dataItemValues.Add(key, value);
                }

                if (ObservationUpdated != null) ObservationUpdated.Invoke(Id, GetObservation(type, subType));
            }
        }

        //public void UpdateDataItem(object value, string type, string subType = null)
        //{
        //    var key = CreateKey(type, subType);
        //    if (key != null && value != null)
        //    {
        //        lock (_lock)
        //        {
        //            _dataItemValues.Remove(key);
        //            _dataItemValues.Add(key, value);
        //        }
        //    }
        //}


        public void AddDataItem(IDataItemModel dataItem, object value)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.DataItemId))
            {
                if (!DataItemModels.Any(o => o.DataItemId == dataItem.DataItemId))
                {
                    DataItemModels.Add(dataItem);
                }

                UpdateDataItem(value, dataItem.Type, dataItem.SubType);
            }
        }

        public void AddDataItem(IDataItem dataItem, object value)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Type))
            {
                if (!DataItemModels.Any(o => o.Type == dataItem.Type))
                {
                    DataItemModels.Add(new DataItemModel(dataItem));
                }

                UpdateDataItem(value, dataItem.Type, dataItem.SubType);
            }
        }


        public IEnumerable<IDataItemModel> GetDataItems() => DataItemModels.ToList();

        public IDataItemModel GetDataItem(string type, string subType = null)
        {
            if (!string.IsNullOrEmpty(type) && !DataItemModels.IsNullOrEmpty())
            {
                IDataItemModel dataItem = null;

                if (!string.IsNullOrEmpty(subType))
                {
                    dataItem = DataItemModels.FirstOrDefault(o => o.Type == type && o.SubType == subType);
                }
                else
                {
                    dataItem = DataItemModels.FirstOrDefault(o => o.Type == type);
                }

                return dataItem;
            }

            return default;
        }

        public T GetDataItem<T>(string type, string subType = null) where T : IDataItemModel
        {
            if (!string.IsNullOrEmpty(type) && !DataItemModels.IsNullOrEmpty())
            {
                IDataItemModel dataItem = null;

                if (!string.IsNullOrEmpty(subType))
                {
                    dataItem = DataItemModels.FirstOrDefault(o => o.DataItemId == Devices.DataItem.CreateDataItemId(Id, type, subType));
                }
                else
                {
                    dataItem = DataItemModels.FirstOrDefault(o => o.DataItemId == Devices.DataItem.CreateDataItemId(Id, type));
                }

                if (dataItem != null)
                {
                    try
                    {
                        var obj = (T)Activator.CreateInstance(typeof(T));
                        obj.DataItemCategory = dataItem.DataItemCategory;
                        obj.DataItemId = dataItem.DataItemId;
                        obj.DataItemName = dataItem.DataItemName;
                        obj.Type = dataItem.Type;
                        obj.SubType = dataItem.SubType;
                        obj.NativeUnits = dataItem.NativeUnits;
                        obj.NativeScale = dataItem.NativeScale;
                        obj.SampleRate = dataItem.SampleRate;
                        //obj.Source = dataItem.Source;
                        //obj.Relationships = dataItem.Relationships;
                        obj.Representation = dataItem.Representation;
                        obj.ResetTrigger = dataItem.ResetTrigger;
                        obj.CoordinateSystem = dataItem.CoordinateSystem;
                        //obj.Constraints = dataItem.Constraints;
                        //obj.Definition = dataItem.Definition;
                        obj.Units = dataItem.Units;
                        obj.Statistic = dataItem.Statistic;
                        obj.SignificantDigits = dataItem.SignificantDigits;
                        //obj.Filters = dataItem.Filters;
                        obj.InitialValue = dataItem.InitialValue;

                        return obj;
                    }
                    catch { }
                }
            }

            return default;
        }

        #endregion

        #region "Conditions"

        public ConditionObservation GetCondition(string type, string subType = null)
        {
            if (ConditionValueExists(type, subType))
            {
                var key = CreateKey(type, subType);

                lock (_lock)
                {
                    if (_conditions.TryGetValue(key, out var condition))
                    {
                        return condition;
                    }
                }
            }

            return default;
        }

        public bool ConditionValueExists(string type, string subType = null)
        {
            var key = CreateKey(type, subType);
            if (key != null)
            {
                lock (_lock)
                {
                    return _conditions.TryGetValue(key, out _);
                }
            }

            return false;
        }


        public void UpdateCondition(ConditionObservation condition, string type, string subType = null)
        {
            var key = CreateKey(type, subType);
            if (key != null && condition != null)
            {
                lock (_lock)
                {
                    _conditions.Remove(key);
                    _conditions.Add(key, condition);
                }
            }
        }


        public void AddCondition(Devices.DataItem dataItem, ConditionObservation condition)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id) && condition != null)
            {
                if (!DataItemModels.Any(o => o.DataItemId == dataItem.Id))
                {
                    DataItemModels.Add(new DataItemModel(dataItem));
                }

                UpdateCondition(condition, dataItem.Type, dataItem.SubType);
            }
        }

        public void AddCondition(IDataItemModel dataItem, ConditionObservation condition)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.DataItemId) && condition != null)
            {
                if (!DataItemModels.Any(o => o.DataItemId == dataItem.DataItemId))
                {
                    DataItemModels.Add(dataItem);
                }

                UpdateCondition(condition, dataItem.Type, dataItem.SubType);
            }
        }

        #endregion


        #region "Application"

        public ApplicationModel GetApplication()
        {
            var x = new ApplicationModel();

            x.InstallDate = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.INSTALL_DATE));
            x.InstallDateDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.INSTALL_DATE));

            x.License = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.LICENSE));
            x.LicenseDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.LICENSE));

            x.Manufacturer = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.MANUFACTURER));
            x.ManufacturerDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.MANUFACTURER));

            x.ReleaseDate = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.RELEASE_DATE));
            x.ReleaseDateDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.RELEASE_DATE));

            x.Version = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.VERSION));
            x.VersionDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.VERSION));

            return x;

        }

        public void SetApplication(ApplicationModel model)
        {
            if (model != null)
            {
                AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.INSTALL_DATE), model.InstallDate);
                AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.LICENSE), model.License);
                AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.MANUFACTURER), model.Manufacturer);
                AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.RELEASE_DATE), model.ReleaseDate);
                AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.VERSION), model.Version);
            }
        }

        #endregion

        #region "Firmware"

        public FirmwareModel GetFirmware()
        {
            var x = new FirmwareModel();

            x.InstallDate = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.INSTALL_DATE));
            x.InstallDateDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.INSTALL_DATE));

            x.License = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.LICENSE));
            x.LicenseDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.LICENSE));

            x.Manufacturer = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.MANUFACTURER));
            x.ManufacturerDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.MANUFACTURER));

            x.ReleaseDate = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.RELEASE_DATE));
            x.ReleaseDateDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.RELEASE_DATE));

            x.Version = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.VERSION));
            x.VersionDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.VERSION));

            return x;

        }

        public void SetFirmware(FirmwareModel model)
        {
            if (model != null)
            {
                AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.INSTALL_DATE), model.InstallDate);
                AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.LICENSE), model.License);
                AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.MANUFACTURER), model.Manufacturer);
                AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.RELEASE_DATE), model.ReleaseDate);
                AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.VERSION), model.Version);
            }
        }

        #endregion

        #region "Hardware"

        public HardwareModel GetHardware()
        {
            var x = new HardwareModel();

            x.InstallDate = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.INSTALL_DATE));
            x.InstallDateDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.INSTALL_DATE));

            x.License = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.LICENSE));
            x.LicenseDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.LICENSE));

            x.Manufacturer = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.MANUFACTURER));
            x.ManufacturerDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.MANUFACTURER));

            x.ReleaseDate = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.RELEASE_DATE));
            x.ReleaseDateDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.RELEASE_DATE));

            x.Version = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.VERSION));
            x.VersionDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.VERSION));

            return x;

        }

        public void SetHardware(HardwareModel model)
        {
            if (model != null)
            {
                AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.INSTALL_DATE), model.InstallDate);
                AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.LICENSE), model.License);
                AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.MANUFACTURER), model.Manufacturer);
                AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.RELEASE_DATE), model.ReleaseDate);
                AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.VERSION), model.Version);
            }
        }

        #endregion

        #region "Library"

        public LibraryModel GetLibrary()
        {
            var x = new LibraryModel();

            x.InstallDate = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.INSTALL_DATE));
            x.InstallDateDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.INSTALL_DATE));

            x.License = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.LICENSE));
            x.LicenseDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.LICENSE));

            x.Manufacturer = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.MANUFACTURER));
            x.ManufacturerDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.MANUFACTURER));

            x.ReleaseDate = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.RELEASE_DATE));
            x.ReleaseDateDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.RELEASE_DATE));

            x.Version = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.VERSION));
            x.VersionDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.VERSION));

            return x;

        }

        public void SetLibrary(LibraryModel model)
        {
            if (model != null)
            {
                AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.INSTALL_DATE), model.InstallDate);
                AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.LICENSE), model.License);
                AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.MANUFACTURER), model.Manufacturer);
                AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.RELEASE_DATE), model.ReleaseDate);
                AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.VERSION), model.Version);
            }
        }

        #endregion


        #region "Network"

        public NetworkModel GetNetwork()
        {
            var x = new NetworkModel();

            x.IPv4Address = GetDataItemValue(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.IPV4_ADDRESS.ToString());
            x.IPv4AddressDataItem = GetDataItem(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.IPV4_ADDRESS.ToString());

            x.IPv6Address = GetDataItemValue(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.IPV6_ADDRESS.ToString());
            x.IPv6AddressDataItem = GetDataItem(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.IPV6_ADDRESS.ToString());

            x.Gateway = GetDataItemValue(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.GATEWAY.ToString());
            x.GatewayDataItem = GetDataItem(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.GATEWAY.ToString());

            x.SubnetMask = GetDataItemValue(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.SUBNET_MASK.ToString());
            x.SubnetMaskDataItem = GetDataItem(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.SUBNET_MASK.ToString());

            x.MacAddress = GetDataItemValue(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.MAC_ADDRESS.ToString());
            x.MacAddressDataItem = GetDataItem(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.MAC_ADDRESS.ToString());

            x.VLanId = GetDataItemValue(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.VLAN_ID.ToString());
            x.VLanIdDataItem = GetDataItem(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.VLAN_ID.ToString());

            x.Wireless = GetDataItemValue(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.WIRELESS.ToString());
            x.WirelessDataItem = GetDataItem(NetworkDataItem.TypeId, NetworkDataItem.SubTypes.WIRELESS.ToString());

            return x;

        }

        public void SetNetwork(NetworkModel network)
        {
            if (network != null)
            {
                AddDataItem(new NetworkDataItem(Id, NetworkDataItem.SubTypes.IPV4_ADDRESS), network.IPv4Address);
                AddDataItem(new NetworkDataItem(Id, NetworkDataItem.SubTypes.IPV6_ADDRESS), network.IPv6Address);
                AddDataItem(new NetworkDataItem(Id, NetworkDataItem.SubTypes.GATEWAY), network.Gateway);
                AddDataItem(new NetworkDataItem(Id, NetworkDataItem.SubTypes.SUBNET_MASK), network.SubnetMask);
                AddDataItem(new NetworkDataItem(Id, NetworkDataItem.SubTypes.VLAN_ID), network.VLanId);
                AddDataItem(new NetworkDataItem(Id, NetworkDataItem.SubTypes.WIRELESS), network.Wireless);
            }
        }

        #endregion

        #region "Operating System"

        public OperatingSystemModel GetOperatingSystem()
        {
            var x = new OperatingSystemModel();

            x.Type = GetDataItemValue<OperatingSystemType>(OperatingSystemDataItem.TypeId);
            x.TypeDataItem = GetDataItem(OperatingSystemDataItem.TypeId);

            x.Version = GetDataItemValue(OperatingSystemDataItem.TypeId, OperatingSystemDataItem.SubTypes.VERSION.ToString());
            x.VersionDataItem = GetDataItem(OperatingSystemDataItem.TypeId, OperatingSystemDataItem.SubTypes.VERSION.ToString());

            return x;

        }

        public void SetOperatingSystem(OperatingSystemModel operatingSystem)
        {
            if (operatingSystem != null)
            {
                AddDataItem(new OperatingSystemDataItem(Id), operatingSystem.Type);
                AddDataItem(new OperatingSystemDataItem(Id, OperatingSystemDataItem.SubTypes.VERSION), operatingSystem.Version);
            }
        }

        #endregion

    }
}
