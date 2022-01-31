// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Observations;
using MTConnect.Streams;
using MTConnect.Streams.Events;
using MTConnect.Streams.Samples;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models
{
    /// <summary>
    /// Composition Structural Elements are used to describe the lowest level physical building blocks of a piece of equipment contained within a Component.
    /// </summary>
    public class CompositionModel : Composition, ICompositionModel
    {
        private readonly object _lock = new object();
        internal readonly Dictionary<string, object> _dataItems = new Dictionary<string, object>();
        internal readonly Dictionary<string, Condition> _conditions = new Dictionary<string, Condition>();


        /// <summary>
        /// The name of the manufacturer of the Component
        /// </summary>
        public string Manufacturer
        {
            get => base.Description?.Manufacturer;
            set
            {
                if (base.Description == null) base.Description = new Description();
                base.Description.Manufacturer = value;
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
                base.Description.Model = value;
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
                base.Description.SerialNumber = value;
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
                base.Description.Station = value;
            }
        }

        /// <summary>
        /// Any additional descriptive information the implementer chooses to include regarding the Component.
        /// </summary>
        public new string Description
        {
            get => base.Description?.CDATA;
            set
            {
                if (base.Description == null) base.Description = new Description();
                base.Description.CDATA = value;
            }
        }


        public List<IDataItemModel> DataItemModels { get; set; }

        public override List<Devices.DataItem> DataItems
        {
            get
            {
                var x = new List<Devices.DataItem>();

                if (!DataItemModels.IsNullOrEmpty())
                {
                    foreach (var dataItemModel in DataItemModels.OfType<DataItemModel>()) x.Add(dataItemModel);
                }

                return x;
            }
        }


        public CompositionModel() 
        {
            DataItemModels = new List<IDataItemModel>();
        }

        public CompositionModel(string id, string name)
        {
            Id = id;
            Name = name;
            DataItemModels = new List<IDataItemModel>();
        }


        #region "Create"

        public static ICompositionModel Create(Composition composition)
        {
            if (composition != null)
            {
                var obj = CreateCompositionModel(composition.Type);
                if (obj != null)
                {
                    obj.Id = composition.Id;
                    obj.Uuid = composition.Uuid;
                    obj.Name = composition.Name;
                    obj.NativeName = composition.NativeName;
                    obj.Type = composition.Type;

                    if (composition.Description != null)
                    {
                        obj.Manufacturer = composition.Description.Manufacturer;
                        obj.Model = composition.Description.Model;
                        obj.SerialNumber = composition.Description.SerialNumber;
                        obj.Station = composition.Description.Station;
                        obj.Description = composition.Description.CDATA;
                    }

                    obj.DataItemModels = CreateDataItemModels(composition.DataItems);

                    return obj;
                }
            }

            return null;
        }

        //public static CompositionModel Create(Composition composition)
        //{
        //    if (composition != null)
        //    {
        //        //var obj = new CompositionModel();
        //        //obj.Id = composition.Id;
        //        //obj.Uuid = composition.Uuid;
        //        //obj.Name = composition.Name;
        //        //obj.NativeName = composition.NativeName;
        //        //obj.Type = composition.Type;

        //        //if (composition.Description != null)
        //        //{
        //        //    obj.Manufacturer = composition.Description.Manufacturer;
        //        //    obj.Model = composition.Description.Model;
        //        //    obj.SerialNumber = composition.Description.SerialNumber;
        //        //    obj.Station = composition.Description.Station;
        //        //    obj.Description = composition.Description.CDATA;
        //        //}

        //        //obj.SampleRate = composition.SampleRate;
        //        //obj.SampleInterval = composition.SampleInterval;
        //        //obj.References = composition.References;
        //        //obj.Configuration = composition.Configuration;

        //        //obj.DataItemModels = CreateDataItemModels(composition.DataItems);

        //        //return obj;

        //        CompositionModel obj = null;

        //        switch (composition.Type)
        //        {
        //            case ActuatorModel.TypeId: obj = new ActuatorModel(); break;
        //            case AmplifierModel.TypeId: obj = new AmplifierModel(); break;

        //            case BallscrewModel.TypeId: obj = new BallscrewModel(); break;
        //            case BeltModel.TypeId: obj = new BeltModel(); break;
        //            case BrakeModel.TypeId: obj = new BrakeModel(); break;

        //            case ChainModel.TypeId: obj = new ChainModel(); break;
        //            case ChopperModel.TypeId: obj = new ChopperModel(); break;
        //            case ChuckModel.TypeId: obj = new ChuckModel(); break;
        //            case ChuteModel.TypeId: obj = new ChuteModel(); break;
        //            case CircuitBreakerModel.TypeId: obj = new CircuitBreakerModel(); break;
        //            case ClampModel.TypeId: obj = new ClampModel(); break;
        //            case CompressorModel.TypeId: obj = new CompressorModel(); break;
        //            case CoolingTowerModel.TypeId: obj = new CoolingTowerModel(); break;

        //            case DoorModel.TypeId: obj = new DoorModel(); break;
        //            case DrainModel.TypeId: obj = new DrainModel(); break;

        //            case EncoderModel.TypeId: obj = new EncoderModel(); break;
        //            case ExpiredPotModel.TypeId: obj = new ExpiredPotModel(); break;
        //            case ExposureUnitModel.TypeId: obj = new ExposureUnitModel(); break;
        //            case ExtrusionUnitModel.TypeId: obj = new ExtrusionUnitModel(); break;

        //            case FanModel.TypeId: obj = new FanModel(); break;
        //            case FilterModel.TypeId: obj = new FilterModel(); break;

        //            case GalvanomotorModel.TypeId: obj = new GalvanomotorModel(); break;
        //            case GripperModel.TypeId: obj = new GripperModel(); break;

        //            case HopperModel.TypeId: obj = new HopperModel(); break;

        //            case LinearPositionFeedbackModel.TypeId: obj = new LinearPositionFeedbackModel(); break;

        //            case MotorModel.TypeId: obj = new MotorModel(); break;

        //            case OilModel.TypeId: obj = new OilModel(); break;

        //            case PotModel.TypeId: obj = new PotModel(); break;
        //            case PowerSupplyModel.TypeId: obj = new PowerSupplyModel(); break;
        //            case PulleyModel.TypeId: obj = new PulleyModel(); break;
        //            case PumpModel.TypeId: obj = new PumpModel(); break;

        //            case ReelModel.TypeId: obj = new ReelModel(); break;
        //            case RemovalPotModel.TypeId: obj = new RemovalPotModel(); break;
        //            case ReturnPotModel.TypeId: obj = new ReturnPotModel(); break;

        //            case SensingElementModel.TypeId: obj = new SensingElementModel(); break;
        //            case SpreaderModel.TypeId: obj = new SpreaderModel(); break;
        //            case StagingPotModel.TypeId: obj = new StagingPotModel(); break;
        //            case StationModel.TypeId: obj = new StationModel(); break;
        //            case StorageBatteryModel.TypeId: obj = new StorageBatteryModel(); break;
        //            case SwitchModel.TypeId: obj = new SwitchModel(); break;

        //            case TableModel.TypeId: obj = new TableModel(); break;
        //            case TankModel.TypeId: obj = new TankModel(); break;
        //            case TensionerModel.TypeId: obj = new TensionerModel(); break;
        //            case TransferArmModel.TypeId: obj = new TransferArmModel(); break;
        //            case TransferPotModel.TypeId: obj = new TransferPotModel(); break;
        //            case TransformerModel.TypeId: obj = new TransformerModel(); break;

        //            case ValveModel.TypeId: obj = new ValveModel(); break;
        //            case VatModel.TypeId: obj = new VatModel(); break;

        //            case WaterModel.TypeId: obj = new WaterModel(); break;
        //            case WireModel.TypeId: obj = new WireModel(); break;
        //            case WorkpieceModel.TypeId: obj = new WorkpieceModel(); break;

        //            default: obj = new CompositionModel(); break;
        //        }

        //        if (obj != null)
        //        {
        //            obj.Id = composition.Id;
        //            obj.Uuid = composition.Uuid;
        //            obj.Name = composition.Name;
        //            obj.NativeName = composition.NativeName;
        //            obj.Type = composition.Type;

        //            if (composition.Description != null)
        //            {
        //                obj.Manufacturer = composition.Description.Manufacturer;
        //                obj.Model = composition.Description.Model;
        //                obj.SerialNumber = composition.Description.SerialNumber;
        //                obj.Station = composition.Description.Station;
        //                obj.Description = composition.Description.CDATA;
        //            }

        //            obj.SampleRate = composition.SampleRate;
        //            obj.SampleInterval = composition.SampleInterval;
        //            obj.References = composition.References;
        //            obj.Configuration = composition.Configuration;

        //            obj.DataItemModels = CreateDataItemModels(composition.DataItems);

        //            return obj;
        //        }
        //    }

        //    return null;
        //}

        private static List<IDataItemModel> CreateDataItemModels(IEnumerable<Devices.DataItem> dataItems)
        {
            if (!dataItems.IsNullOrEmpty())
            {
                var objs = new List<IDataItemModel>();

                foreach (var dataItem in dataItems)
                {
                    objs.Add(new DataItemModel(dataItem));
                }

                return objs;
            }

            return new List<IDataItemModel>();
        }

        private static ICompositionModel CreateCompositionModel(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var types = GetAllCompositionModelTypes();
                if (!types.IsNullOrEmpty())
                {
                    foreach (var t in types)
                    {
                        try
                        {
                            var obj = (ICompositionModel)Activator.CreateInstance(t);
                            if (obj.Type == type) return obj;
                        }
                        catch { }
                    }
                }
            }

            return null;
        }

        private static IEnumerable<Type> GetAllCompositionModelTypes()
        {
            // Search loaded assemblies for ICompositionModel classes
            // This allows for custom types to be included in DLL's and read at runtime
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (!assemblies.IsNullOrEmpty())
            {
                var types = assemblies.SelectMany(x => x.GetTypes());
                return types.Where(x => typeof(ICompositionModel).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }

            return Enumerable.Empty<Type>();
        }

        #endregion

        #region "DataItems"

        protected bool DataItemValueExists(string dataItemId)
        {
            if (!string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    return _dataItems.TryGetValue(dataItemId, out _);
                }
            }

            return false;
        }


        protected string GetDataItemValue(string dataItemId)
        {
            if (!string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    if (_dataItems.TryGetValue(dataItemId, out var value) && value != null)
                    {
                        return value.ToString();
                    }
                }
            }

            return default;
        }

        protected T GetDataItemValue<T>(string dataItemId) where T : struct
        {
            if (!string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    if (_dataItems.TryGetValue(dataItemId, out var value) && value != null)
                    {
                        return StringFunctions.ConvertEnum<T>(value.ToString());
                    }
                }
            }

            return default;
        }


        protected string GetStringValue(string name)
        {
            var dataItemId = Devices.DataItem.CreateId(Id, name);
            if (DataItemValueExists(dataItemId))
            {
                return GetDataItemValue(dataItemId);
            }

            return default;
        }

        protected string GetStringValue(string name, string suffix)
        {
            var dataItemId = CreateId(Id, name, suffix);
            if (DataItemValueExists(dataItemId))
            {
                return GetDataItemValue(dataItemId);
            }

            return default;
        }


        protected EventValue GetEventValue(string name)
        {
            var dataItemId = Devices.DataItem.CreateId(Id, name);
            if (DataItemValueExists(dataItemId))
            {
                return new EventValue(GetDataItemValue(dataItemId));
            }

            return default;
        }

        protected EventValue GetEventValue(string name, string suffix)
        {
            var dataItemId = CreateId(Id, name, suffix);
            if (DataItemValueExists(dataItemId))
            {
                return new EventValue(GetDataItemValue(dataItemId));
            }

            return default;
        }


        protected SampleValue GetSampleValue(string name)
        {
            var dataItemId = Devices.DataItem.CreateId(Id, name);
            if (DataItemValueExists(dataItemId))
            {
                return new SampleValue(GetDataItemValue<double>(dataItemId));
            }

            return default;
        }

        protected SampleValue GetSampleValue(string name, string suffix)
        {
            var dataItemId = CreateId(Id, name, suffix);
            if (DataItemValueExists(dataItemId))
            {
                return new SampleValue(GetDataItemValue<double>(dataItemId));
            }

            return default;
        }

        protected T GetSampleValue<T>(string name, string suffix = null) where T : SampleValue
        {
            var dataItemId = CreateId(Id, name, suffix);
            if (DataItemValueExists(dataItemId))
            {
                try
                {
                    var obj = (T)Activator.CreateInstance(typeof(T));
                    obj.Value = GetDataItemValue(dataItemId).ToDouble();

                    return obj;
                }
                catch { }
            }

            return null;
        }


        protected void UpdateDataItem(string dataItemId, object cdata)
        {
            if (!string.IsNullOrEmpty(dataItemId) && cdata != null)
            {
                lock (_lock)
                {
                    _dataItems.Remove(dataItemId);
                    _dataItems.Add(dataItemId, cdata);
                }
            }
        }


        public void AddDataItem(IDataItemModel dataItem, object value)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id) && value != null)
            {
                if (!DataItemModels.Any(o => o.Id == dataItem.Id))
                {
                    DataItemModels.Add(dataItem);
                }

                UpdateDataItem(dataItem.Id, value);
            }
        }

        public void AddDataItem(Devices.DataItem dataItem, object value)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id) && value != null)
            {
                if (!DataItemModels.Any(o => o.Id == dataItem.Id))
                {
                    DataItemModels.Add(new DataItemModel(dataItem));
                }

                UpdateDataItem(dataItem.Id, value);
            }
        }


        protected IDataItemModel GetDataItem(string name, string suffix = null)
        {
            if (!string.IsNullOrEmpty(name) && !DataItemModels.IsNullOrEmpty())
            {
                IDataItemModel dataItem = null;

                if (!string.IsNullOrEmpty(suffix))
                {
                    dataItem = DataItemModels.FirstOrDefault(o => o.Id == Devices.DataItem.CreateId(Id, name, suffix));
                }
                else
                {
                    dataItem = DataItemModels.FirstOrDefault(o => o.Id == Devices.DataItem.CreateId(Id, name));
                }

                return dataItem;
            }

            return default;
        }

        protected T GetDataItem<T>(string name, string suffix = null) where T : IDataItemModel
        {
            if (!string.IsNullOrEmpty(name) && !DataItems.IsNullOrEmpty())
            {
                IDataItemModel dataItem = null;

                if (!string.IsNullOrEmpty(suffix))
                {
                    dataItem = DataItemModels.FirstOrDefault(o => o.Id == Devices.DataItem.CreateId(Id, name, suffix));
                }
                else
                {
                    dataItem = DataItemModels.FirstOrDefault(o => o.Id == Devices.DataItem.CreateId(Id, name));
                }

                if (dataItem != null)
                {
                    try
                    {
                        var obj = (T)Activator.CreateInstance(typeof(T));
                        obj.DataItemCategory = dataItem.DataItemCategory;
                        obj.Id = dataItem.Id;
                        obj.Name = dataItem.Name;
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

        protected Condition GetCondition(string name)
        {
            var dataItemId = Devices.DataItem.CreateId(Id, name);
            if (ConditionValueExists(dataItemId))
            {
                lock (_lock)
                {
                    if (_conditions.TryGetValue(dataItemId, out var condition))
                    {
                        return condition;
                    }
                }
            }

            return default;
        }

        protected bool ConditionValueExists(string dataItemId)
        {
            if (!string.IsNullOrEmpty(dataItemId))
            {
                lock (_lock)
                {
                    return _conditions.TryGetValue(dataItemId, out _);
                }
            }

            return false;
        }


        protected void UpdateCondition(string dataItemId, Condition condition)
        {
            if (!string.IsNullOrEmpty(dataItemId) && condition != null)
            {
                lock (_lock)
                {
                    _conditions.Remove(dataItemId);
                    _conditions.Add(dataItemId, condition);
                }
            }
        }



        public void AddCondition(Devices.DataItem dataItem, Condition condition)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id) && condition != null)
            {
                if (!DataItemModels.Any(o => o.Id == dataItem.Id))
                {
                    DataItemModels.Add(new DataItemModel(dataItem));
                }

                UpdateCondition(dataItem.Id, condition);
            }
        }

        public void AddCondition(IDataItemModel dataItem, Condition condition)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id) && condition != null)
            {
                if (!DataItemModels.Any(o => o.Id == dataItem.Id))
                {
                    DataItemModels.Add(dataItem);
                }

                UpdateCondition(dataItem.Id, condition);
            }
        }

        #endregion

        #region "Adapter"

        public IEnumerable<Observation> GetObservations(long timestamp = 0)
        {
            var objs = new List<Observation>();

            if (timestamp <= 0) timestamp = UnixDateTime.Now;

            lock (_lock)
            {
                foreach (var dataItem in _dataItems.ToList())
                {
                    var obj = new Observation(dataItem.Key, dataItem.Value);
                    obj.Timestamp = timestamp;
                    objs.Add(obj);
                }
            }

            return objs;
        }

        public IEnumerable<ConditionObservation> GetConditionObservations(long timestamp = 0)
        {
            var objs = new List<ConditionObservation>();

            lock (_lock)
            {
                foreach (var condition in _conditions.ToList())
                {
                    var obj = new ConditionObservation(condition.Key, condition.Value.Level);
                    obj.NativeCode = condition.Value.NativeCode;
                    obj.NativeSeverity = condition.Value.NativeSeverity;
                    obj.Qualifier = condition.Value.Qualifier;
                    obj.Message = condition.Value.CDATA;
                    obj.Timestamp = timestamp;
                    objs.Add(obj);
                }
            }

            return objs;
        }

        #endregion

        //protected T ConvertValue<T>(string value)
        //{
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        if (Enum.TryParse(typeof(T), value, true, out var result))
        //        {
        //            return (T)result;
        //        }
        //    }

        //    return default;
        //}
    }
}
