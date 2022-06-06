// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Models.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Input;
//using MTConnect.Streams;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect.Models
{
    /// <summary>
    /// An abstract XML Element.
    /// Replaced in the XML document by types of Component elements representing physical and logical parts of the Device.
    /// There can be multiple types of Component XML Elements in the document.
    /// </summary>
    public class CompositionModel : Composition, ICompositionModel
    {
        private readonly ComponentManager _componentManager;


        internal ComponentManager ComponentManager => _componentManager;

        internal DataItemManager DataItemManager => _componentManager.DataItemManager;

        public EventHandler<IObservation> ObservationUpdated { get; set; }


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
        /// The application on a component.
        /// </summary>
        public ApplicationModel Application
        {
            get => DataItemManager.GetApplication();
            set => DataItemManager.SetApplication(value);
        }

        /// <summary>
        /// The embedded software of a component.
        /// </summary>
        public FirmwareModel Firmware
        {
            get => DataItemManager.GetFirmware();
            set => DataItemManager.SetFirmware(value);
        }

        /// <summary>
        /// The hardware of a component.
        /// </summary>
        public HardwareModel Hardware
        {
            get => DataItemManager.GetHardware();
            set => DataItemManager.SetHardware(value);
        }

        /// <summary>
        /// The software library on a component.
        /// </summary>
        public LibraryModel Library
        {
            get => DataItemManager.GetLibrary();
            set => DataItemManager.SetLibrary(value);
        }


        public List<IComponentModel> ComponentModels
        {
            get => ComponentManager.ComponentModels;
            set => ComponentManager.AddComponentModels(value);
        }

        //public override List<Component> Components
        //{
        //    get
        //    {
        //        var x = new List<Component>();

        //        if (!ComponentModels.IsNullOrEmpty())
        //        {
        //            foreach (var componentModel in ComponentModels.OfType<ComponentModel>()) x.Add(componentModel);
        //        }

        //        return x;
        //    }
        //}


        //public List<ICompositionModel> CompositionModels { get; set; }

        //public override List<Composition> Compositions
        //{
        //    get
        //    {
        //        var x = new List<Composition>();

        //        if (!CompositionModels.IsNullOrEmpty())
        //        {
        //            foreach (var compositionModel in CompositionModels.OfType<CompositionModel>()) x.Add(compositionModel);
        //        }

        //        return x;
        //    }
        //}


        public List<IDataItemModel> DataItemModels
        {
            get
            {
                var x = new List<IDataItemModel>();
                if (!ComponentManager.DataItemModels.IsNullOrEmpty())
                {
                    foreach (var dataItemModel in ComponentManager.DataItemModels)
                    {
                        x.Add(dataItemModel);
                    }
                }
                return x;
            }
            set => ComponentManager.AddDataItems(value);
        }

        public override List<DataItem> DataItems
        {
            get
            {
                var x = new List<DataItem>();

                if (!DataItemModels.IsNullOrEmpty())
                {
                    foreach (var dataItemModel in DataItemModels.OfType<DataItemModel>()) x.Add(dataItemModel);
                }

                return x;
            }
        }


        public CompositionModel()
        {
            _componentManager = new ComponentManager();
            _componentManager.ObservationUpdated += OnObservationUpdated;

            ComponentModels = new List<IComponentModel>();
            //CompositionModels = new List<ICompositionModel>();
            DataItemModels = new List<IDataItemModel>();
        }

        public CompositionModel(string id, string name)
        {
            _componentManager = new ComponentManager(id);
            _componentManager.ObservationUpdated += OnObservationUpdated;

            ComponentModels = new List<IComponentModel>();
            //CompositionModels = new List<ICompositionModel>();
            DataItemModels = new List<IDataItemModel>();
            Id = id;
            Name = name;
        }


        //#region "Create"

        //public static IComponentModel Create(Component component)
        //{
        //    if (component != null)
        //    {
        //        var obj = CreateComponentModel(component.Type);
        //        if (obj != null)
        //        {
        //            obj.Id = component.Id;
        //            obj.Uuid = component.Uuid;
        //            obj.Name = component.Name;
        //            obj.NativeName = component.NativeName;
        //            obj.Type = component.Type;

        //            if (component.Description != null)
        //            {
        //                obj.Manufacturer = component.Description.Manufacturer;
        //                obj.Model = component.Description.Model;
        //                obj.SerialNumber = component.Description.SerialNumber;
        //                obj.Station = component.Description.Station;
        //                obj.Description = component.Description.CDATA;
        //            }

        //            obj.SampleRate = component.SampleRate;
        //            obj.SampleInterval = component.SampleInterval;
        //            obj.References = component.References;
        //            obj.Configuration = component.Configuration;

        //            obj.ComponentModels = CreateComponentModels(component.Components);
        //            obj.CompositionModels = CreateCompositionModels(component.Compositions);
        //            obj.DataItemModels = CreateDataItemModels(component.DataItems);

        //            return obj;
        //        }
        //    }

        //    return null;
        //}

        ////public static ComponentModel Create(Component component)
        ////{
        ////    if (component != null)
        ////    {
        ////        //var obj = new ComponentModel();
        ////        //obj.Id = component.Id;
        ////        //obj.Uuid = component.Uuid;
        ////        //obj.Name = component.Name;
        ////        //obj.NativeName = component.NativeName;
        ////        //obj.Type = component.Type;

        ////        //if (component.Description != null)
        ////        //{
        ////        //    obj.Manufacturer = component.Description.Manufacturer;
        ////        //    obj.Model = component.Description.Model;
        ////        //    obj.SerialNumber = component.Description.SerialNumber;
        ////        //    obj.Station = component.Description.Station;
        ////        //    obj.Description = component.Description.CDATA;
        ////        //}

        ////        //obj.SampleRate = component.SampleRate;
        ////        //obj.SampleInterval = component.SampleInterval;
        ////        //obj.References = component.References;
        ////        //obj.Configuration = component.Configuration;

        ////        //obj.ComponentModels = CreateComponentModels(component.Components);
        ////        //obj.CompositionModels = CreateCompositionModels(component.Compositions);
        ////        //obj.DataItemModels = CreateDataItemModels(component.DataItems);

        ////        //return obj;

        ////        ComponentModel obj = null;

        ////        switch (component.Type)
        ////        {
        ////            case CartesianCoordinateAxesModel.TypeId: obj = new CartesianCoordinateAxesModel(); break;
        ////            case LinearAxisModel.TypeId: obj = new LinearAxisModel(); break;
        ////            case RotaryAxisModel.TypeId: obj = new RotaryAxisModel(); break;

        ////            case ControllerComponent.TypeId: obj = new ControllerModel(); break;
        ////            case PathModel.TypeId: obj = new PathModel(); break;

        ////            case SystemsModel.TypeId: obj = new SystemsModel(); break;
        ////            case HydraulicModel.TypeId: obj = new HydraulicModel(); break;
        ////            case PneumaticModel.TypeId: obj = new PneumaticModel(); break;
        ////            case CoolantModel.TypeId: obj = new CoolantModel(); break;
        ////            case LubricationModel.TypeId: obj = new LubricationModel(); break;
        ////            case ElectricModel.TypeId: obj = new ElectricModel(); break;
        ////            case EnclosureModel.TypeId: obj = new EnclosureModel(); break;
        ////            case ProtectiveModel.TypeId: obj = new ProtectiveModel(); break;
        ////            case ProcessPowerModel.TypeId: obj = new ProcessPowerModel(); break;
        ////            case FeederModel.TypeId: obj = new FeederModel(); break;
        ////            case DielectricModel.TypeId: obj = new DielectricModel(); break;
        ////            case EndEffectorModel.TypeId: obj = new EndEffectorModel(); break;
        ////            case WorkEnvelopeModel.TypeId: obj = new WorkEnvelopeModel(); break;
        ////            case HeatingModel.TypeId: obj = new HeatingModel(); break;
        ////            case CoolingModel.TypeId: obj = new CoolingModel(); break;
        ////            case PressureModel.TypeId: obj = new PressureModel(); break;
        ////            case VacuumModel.TypeId: obj = new VacuumModel(); break;

        ////            case AuxiliariesModel.TypeId: obj = new AuxiliariesModel(); break;
        ////            case LoaderModel.TypeId: obj = new LoaderModel(); break;
        ////            case WasteDisposalModel.TypeId: obj = new WasteDisposalModel(); break;

        ////            case ToolingDeliveryModel.TypeId: obj = new ToolingDeliveryModel(); break;
        ////            case AutomaticToolChangerModel.TypeId: obj = new AutomaticToolChangerModel(); break;
        ////            case ToolMagazineModel.TypeId: obj = new ToolMagazineModel(); break;
        ////            case TurretModel.TypeId: obj = new TurretModel(); break;
        ////            case GangToolBarModel.TypeId: obj = new GangToolBarModel(); break;
        ////            case ToolRackModel.TypeId: obj = new ToolRackModel(); break;

        ////            case BarFeederModel.TypeId: obj = new BarFeederModel(); break;
        ////            case EnvironmentalModel.TypeId: obj = new EnvironmentalModel(); break;
        ////            case SensorModel.TypeId: obj = new SensorModel(); break;
        ////            case DepositionModel.TypeId: obj = new DepositionModel(); break;

        ////            case ResourcesModel.TypeId: obj = new ResourcesModel(); break;
        ////            case MaterialsModel.TypeId: obj = new MaterialsModel(); break;
        ////            case StockModel.TypeId: obj = new StockModel(); break;
        ////            case PersonnelModel.TypeId: obj = new PersonnelModel(); break;

        ////            case InterfacesModel.TypeId: obj = new InterfacesModel(); break;
        ////            case InterfaceModel.TypeId: obj = new InterfaceModel(); break;
        ////            case AdaptersModel.TypeId: obj = new AdaptersModel(); break;
        ////            case AdapterModel.TypeId: obj = new AdapterModel(); break;
        ////            case StructureModel.TypeId: obj = new StructureModel(); break;
        ////            case LinkModel.TypeId: obj = new LinkModel(); break;
        ////            case ActuatorModel.TypeId: obj = new ActuatorModel(); break;
        ////            case DoorModel.TypeId: obj = new DoorModel(); break;

        ////            case ProcessesModel.TypeId: obj = new ProcessesModel(); break;
        ////            case PartOccurrenceModel.TypeId: obj = new PartOccurrenceModel(); break;
        ////            case ProcessOccurrenceModel.TypeId: obj = new ProcessOccurrenceModel(); break;

        ////            default: obj = new ComponentModel(); break;
        ////        }

        ////        if (obj != null)
        ////        {
        ////            obj.Id = component.Id;
        ////            obj.Uuid = component.Uuid;
        ////            obj.Name = component.Name;
        ////            obj.NativeName = component.NativeName;
        ////            obj.Type = component.Type;

        ////            if (component.Description != null)
        ////            {
        ////                obj.Manufacturer = component.Description.Manufacturer;
        ////                obj.Model = component.Description.Model;
        ////                obj.SerialNumber = component.Description.SerialNumber;
        ////                obj.Station = component.Description.Station;
        ////                obj.Description = component.Description.CDATA;
        ////            }

        ////            obj.SampleRate = component.SampleRate;
        ////            obj.SampleInterval = component.SampleInterval;
        ////            obj.References = component.References;
        ////            obj.Configuration = component.Configuration;

        ////            obj.ComponentModels = CreateComponentModels(component.Components);
        ////            obj.CompositionModels = CreateCompositionModels(component.Compositions);
        ////            obj.DataItemModels = CreateDataItemModels(component.DataItems);

        ////            return obj;
        ////        }
        ////    }

        ////    return null;
        ////}

        //private static List<IComponentModel> CreateComponentModels(IEnumerable<Component> components)
        //{
        //    if (!components.IsNullOrEmpty())
        //    {
        //        var objs = new List<IComponentModel>();

        //        foreach (var component in components)
        //        {
        //            var obj = Create(component);
        //            if (obj != null) objs.Add(obj);
        //        }

        //        return objs;
        //    }

        //    return new List<IComponentModel>();
        //}

        //private static List<ICompositionModel> CreateCompositionModels(IEnumerable<Composition> compositions)
        //{
        //    if (!compositions.IsNullOrEmpty())
        //    {
        //        var objs = new List<ICompositionModel>();

        //        foreach (var composition in compositions)
        //        {
        //            var obj = CompositionModel.Create(composition);
        //            if (obj != null) objs.Add(obj);
        //        }

        //        return objs;
        //    }

        //    return new List<ICompositionModel>();
        //}

        //private static List<IDataItemModel> CreateDataItemModels(IEnumerable<Devices.DataItem> dataItems)
        //{
        //    if (!dataItems.IsNullOrEmpty())
        //    {
        //        var objs = new List<IDataItemModel>();

        //        foreach (var dataItem in dataItems)
        //        {
        //            objs.Add(new DataItemModel(dataItem));
        //        }

        //        return objs;
        //    }

        //    return new List<IDataItemModel>();
        //}



        //private static IComponentModel CreateComponentModel(string type)
        //{
        //    if (!string.IsNullOrEmpty(type))
        //    {
        //        var types = GetAllComponentModelTypes();
        //        if (!types.IsNullOrEmpty())
        //        {
        //            foreach (var t in types)
        //            {
        //                try
        //                {
        //                    var obj = (IComponentModel)Activator.CreateInstance(t);
        //                    if (obj.Type == type) return obj;
        //                }
        //                catch { }
        //            }
        //        }
        //    }

        //    return null;
        //}

        //private static IEnumerable<Type> GetAllComponentModelTypes()
        //{
        //    // Search loaded assemblies for IComponentModel classes
        //    // This allows for custom types to be included in DLL's and read at runtime
        //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //    if (!assemblies.IsNullOrEmpty())
        //    {
        //        var types = assemblies.SelectMany(x => x.GetTypes());
        //        return types.Where(x => typeof(IComponentModel).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        //    }

        //    return Enumerable.Empty<Type>();
        //}

        //#endregion

        #region "Components"

        //protected T GetComponentModel<T>(Type componentType, string name = null) where T : ComponentModel
        //{
        //    // Get the TypeId for the ComponentModel Type
        //    var typeId = GetComponentTypeId(componentType);

        //    // Find the ComponentModel that matches the TypeId
        //    var obj = (T)ComponentModels?.FirstOrDefault(o => o.Type == typeId && (name == null || o.Name == name));
        //    if (obj == null)
        //    {
        //        if (name == null)
        //        {
        //            // Get the NameId for the ComponentModel Type
        //            name = GetComponentNameId(componentType);
        //        }

        //        // If Not found then add a new component using the Type information
        //        obj = AddComponentModel<T>(name);
        //    }

        //    return obj;
        //}

        public void AddComponentModel(ComponentModel component) => ComponentManager.AddComponentModel(component);

        public T AddComponentModel<T>(string name) where T : ComponentModel => ComponentManager.AddComponentModel<T>(name);

        #endregion

        #region "Compositions"

        //protected T GetCompositionModel<T>(Type compositionType, string name = null) where T : CompositionModel
        //{
        //    // Get the TypeId for the CompositionModel Type
        //    var typeId = GetCompositionTypeId(compositionType);

        //    // Find the CompositionModel that matches the TypeId
        //    var obj = (T)CompositionModels?.FirstOrDefault(o => o.Type == typeId && (name == null || o.Name == name));
        //    if (obj == null)
        //    {
        //        if (name == null)
        //        {
        //            // Get the NameId for the CompositionModel Type
        //            name = GetCompositionNameId(compositionType);
        //        }

        //        // If Not found then add a new composition using the Type information
        //        obj = AddCompositionModel<T>(name);
        //    }

        //    return obj;
        //}

        //public void AddCompositionModel(CompositionModel composition) => ComponentManager.AddCompositionModel(composition);

        //public T AddCompositionModel<T>(string name) where T : CompositionModel => ComponentManager.AddCompositionModel<T>(name);

        #endregion

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

        //#region "DataItems"

        //protected bool DataItemValueExists(string dataItemId)
        //{
        //    if (!string.IsNullOrEmpty(dataItemId))
        //    {
        //        lock (_lock)
        //        {
        //            return _dataItems.TryGetValue(dataItemId, out _);
        //        }
        //    }

        //    return false;
        //}


        //protected string GetDataItemValue(string dataItemId)
        //{
        //    if (!string.IsNullOrEmpty(dataItemId))
        //    {
        //        lock (_lock)
        //        {
        //            if (_dataItems.TryGetValue(dataItemId, out var value) && value != null)
        //            {
        //                return value.ToString();
        //            }
        //        }
        //    }

        //    return default;
        //}

        //protected T GetDataItemValue<T>(string dataItemId) where T : struct
        //{
        //    if (!string.IsNullOrEmpty(dataItemId))
        //    {
        //        lock (_lock)
        //        {
        //            if (_dataItems.TryGetValue(dataItemId, out var value) && value != null)
        //            {
        //                return StringFunctions.ConvertEnum<T>(value.ToString());
        //            }
        //        }
        //    }

        //    return default;
        //}


        //protected string GetDataItemValue(string name)
        //{
        //    var dataItemId = Devices.DataItem.CreateId(Id, name);
        //    if (DataItemValueExists(dataItemId))
        //    {
        //        return GetDataItemValue(dataItemId);
        //    }

        //    return default;
        //}

        //protected string GetDataItemValue(string name, string suffix = null)
        //{
        //    var dataItemId = CreateId(Id, name, suffix);
        //    if (DataItemValueExists(dataItemId))
        //    {
        //        return GetDataItemValue(dataItemId);
        //    }

        //    return default;
        //}


        //protected EventValue GetEventValue(string name)
        //{
        //    var dataItemId = Devices.DataItem.CreateId(Id, name);
        //    if (DataItemValueExists(dataItemId))
        //    {
        //        return new EventValue(GetDataItemValue(dataItemId));
        //    }

        //    return default;
        //}

        //protected EventValue GetEventValue(string name, string suffix = null)
        //{
        //    var dataItemId = CreateId(Id, name, suffix);
        //    if (DataItemValueExists(dataItemId))
        //    {
        //        return new EventValue(GetDataItemValue(dataItemId));
        //    }

        //    return default;
        //}

        //protected T GetEventValue<T>(string name, string suffix = null) where T : EventValue
        //{
        //    var dataItemId = CreateId(Id, name, suffix);
        //    if (DataItemValueExists(dataItemId))
        //    {
        //        try
        //        {
        //            var obj = (T)Activator.CreateInstance(typeof(T));
        //            obj.Value = GetDataItemValue(dataItemId).ToDouble();

        //            return obj;
        //        }
        //        catch { }
        //    }

        //    return null;
        //}


        //protected SampleValue GetSampleValue(string name)
        //{
        //    var dataItemId = Devices.DataItem.CreateId(Id, name);
        //    if (DataItemValueExists(dataItemId))
        //    {
        //        return new SampleValue(GetDataItemValue<double>(dataItemId));
        //    }

        //    return default;
        //}

        //protected SampleValue GetSampleValue(string name, string suffix = null)
        //{
        //    var dataItemId = CreateId(Id, name, suffix);
        //    if (DataItemValueExists(dataItemId))
        //    {
        //        return new SampleValue(GetDataItemValue<double>(dataItemId));
        //    }

        //    return null;
        //}

        //protected T GetSampleValue<T>(string name, string suffix = null) where T : SampleValue
        //{
        //    var dataItemId = CreateId(Id, name, suffix);
        //    if (DataItemValueExists(dataItemId))
        //    {
        //        try
        //        {
        //            var obj = (T)Activator.CreateInstance(typeof(T));
        //            obj.Value = GetDataItemValue(dataItemId).ToDouble();

        //            return obj;
        //        }
        //        catch { }
        //    }

        //    return null;
        //}


        //protected void UpdateDataItem(string dataItemId, object cdata)
        //{
        //    if (!string.IsNullOrEmpty(dataItemId) && cdata != null)
        //    {
        //        lock (_lock)
        //        {
        //            _dataItems.Remove(dataItemId);
        //            _dataItems.Add(dataItemId, cdata);
        //        }
        //    }
        //}


        //public void AddDataItem(IDataItemModel dataItem, object value)
        //{
        //    if (dataItem != null && !string.IsNullOrEmpty(dataItem.DataItemId) && value != null)
        //    {
        //        if (!DataItemModels.Any(o => o.DataItemId == dataItem.DataItemId))
        //        {
        //            DataItemModels.Add(dataItem);
        //        }

        //        UpdateDataItem(dataItem.DataItemId, value);
        //    }
        //}

        //public void AddDataItem(Devices.DataItem dataItem, object value)
        //{
        //    if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id) && value != null)
        //    {
        //        if (!DataItemModels.Any(o => o.DataItemId == dataItem.Id))
        //        {
        //            DataItemModels.Add(new DataItemModel(dataItem));
        //        }

        //        UpdateDataItem(dataItem.Id, value);
        //    }
        //}


        //protected IDataItemModel GetDataItem(string name, string suffix = null)
        //{
        //    if (!string.IsNullOrEmpty(name) && !DataItemModels.IsNullOrEmpty())
        //    {
        //        IDataItemModel dataItem = null;

        //        if (!string.IsNullOrEmpty(suffix))
        //        {
        //            dataItem = DataItemModels.FirstOrDefault(o => o.DataItemId == Devices.DataItem.CreateId(Id, name, suffix));
        //        }
        //        else
        //        {
        //            dataItem = DataItemModels.FirstOrDefault(o => o.DataItemId == Devices.DataItem.CreateId(Id, name));
        //        }

        //        return dataItem;
        //    }

        //    return default;
        //}

        //protected T GetDataItem<T>(string name, string suffix = null) where T : IDataItemModel
        //{
        //    if (!string.IsNullOrEmpty(name) && !DataItems.IsNullOrEmpty())
        //    {
        //        IDataItemModel dataItem = null;

        //        if (!string.IsNullOrEmpty(suffix))
        //        {
        //            dataItem = DataItemModels.FirstOrDefault(o => o.DataItemId == Devices.DataItem.CreateId(Id, name, suffix));
        //        }
        //        else
        //        {
        //            dataItem = DataItemModels.FirstOrDefault(o => o.DataItemId == Devices.DataItem.CreateId(Id, name));
        //        }

        //        if (dataItem != null)
        //        {
        //            try
        //            {
        //                var obj = (T)Activator.CreateInstance(typeof(T));
        //                obj.DataItemCategory = dataItem.DataItemCategory;
        //                obj.DataItemId = dataItem.DataItemId;
        //                obj.DataItemName = dataItem.DataItemName;
        //                obj.Type = dataItem.Type;
        //                obj.SubType = dataItem.SubType;
        //                obj.NativeUnits = dataItem.NativeUnits;
        //                obj.NativeScale = dataItem.NativeScale;
        //                obj.SampleRate = dataItem.SampleRate;
        //                //obj.Source = dataItem.Source;
        //                //obj.Relationships = dataItem.Relationships;
        //                obj.Representation = dataItem.Representation;
        //                obj.ResetTrigger = dataItem.ResetTrigger;
        //                obj.CoordinateSystem = dataItem.CoordinateSystem;
        //                //obj.Constraints = dataItem.Constraints;
        //                //obj.Definition = dataItem.Definition;
        //                obj.Units = dataItem.Units;
        //                obj.Statistic = dataItem.Statistic;
        //                obj.SignificantDigits = dataItem.SignificantDigits;
        //                //obj.Filters = dataItem.Filters;
        //                obj.InitialValue = dataItem.InitialValue;

        //                return obj;
        //            }
        //            catch { }
        //        }
        //    }

        //    return default;
        //}

        //#endregion

        //#region "Conditions"

        //protected Condition GetCondition(string name)
        //{
        //    var dataItemId = Devices.DataItem.CreateId(Id, name);
        //    if (ConditionValueExists(dataItemId))
        //    {
        //        lock (_lock)
        //        {
        //            if (_conditions.TryGetValue(dataItemId, out var condition))
        //            {
        //                return condition;
        //            }
        //        }
        //    }

        //    return default;
        //}

        //protected bool ConditionValueExists(string dataItemId)
        //{
        //    if (!string.IsNullOrEmpty(dataItemId))
        //    {
        //        lock (_lock)
        //        {
        //            return _conditions.TryGetValue(dataItemId, out _);
        //        }
        //    }

        //    return false;
        //}


        //protected void UpdateCondition(string dataItemId, Condition condition)
        //{
        //    if (!string.IsNullOrEmpty(dataItemId) && condition != null)
        //    {
        //        condition.DataItemId = dataItemId;

        //        lock (_lock)
        //        {
        //            _conditions.Remove(dataItemId);
        //            _conditions.Add(dataItemId, condition);
        //        }
        //    }
        //}



        //public void AddCondition(Devices.DataItem dataItem, Condition condition)
        //{
        //    if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id) && condition != null)
        //    {
        //        if (!DataItemModels.Any(o => o.DataItemId == dataItem.Id))
        //        {
        //            DataItemModels.Add(new DataItemModel(dataItem));
        //        }

        //        UpdateCondition(dataItem.Id, condition);
        //    }
        //}

        //public void AddCondition(IDataItemModel dataItem, Condition condition)
        //{
        //    if (dataItem != null && !string.IsNullOrEmpty(dataItem.DataItemId) && condition != null)
        //    {
        //        if (!DataItemModels.Any(o => o.DataItemId == dataItem.DataItemId))
        //        {
        //            DataItemModels.Add(dataItem);
        //        }

        //        UpdateCondition(dataItem.DataItemId, condition);
        //    }
        //}

        //#endregion

        #region "Adapter"

        private void OnObservationUpdated(object sender, IObservation observation)
        {
            if (ObservationUpdated != null) ObservationUpdated.Invoke(this, observation);
        }


        public IEnumerable<IObservation> GetObservations() => ComponentManager.GetObservations();
        //public IEnumerable<ObservationInput> GetObservations(long timestamp = 0) => ComponentManager.GetObservations(timestamp);

        public IEnumerable<ConditionObservationInput> GetConditionObservations(long timestamp = 0) => ComponentManager.GetConditionObservations(timestamp);

        #endregion


        //#region "Application"

        //private ApplicationModel GetApplication()
        //{
        //    var x = new ApplicationModel();

        //    x.InstallDate = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.INSTALL_DATE));
        //    x.InstallDateDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.INSTALL_DATE));

        //    x.License = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.LICENSE));
        //    x.LicenseDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.LICENSE));

        //    x.Manufacturer = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.MANUFACTURER));
        //    x.ManufacturerDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.MANUFACTURER));

        //    x.ReleaseDate = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.RELEASE_DATE));
        //    x.ReleaseDateDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.RELEASE_DATE));

        //    x.Version = GetDataItemValue(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.VERSION));
        //    x.VersionDataItem = GetDataItem(ApplicationDataItem.NameId, ApplicationDataItem.GetSubTypeId(ApplicationDataItem.SubTypes.VERSION));

        //    return x;

        //}

        //private void SetApplication(ApplicationModel model)
        //{
        //    if (model != null)
        //    {
        //        AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.INSTALL_DATE), model.InstallDate);
        //        AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.LICENSE), model.License);
        //        AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.MANUFACTURER), model.Manufacturer);
        //        AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.RELEASE_DATE), model.ReleaseDate);
        //        AddDataItem(new ApplicationDataItem(Id, ApplicationDataItem.SubTypes.VERSION), model.Version);
        //    }
        //}

        //#endregion

        //#region "Firmware"

        //private FirmwareModel GetFirmware()
        //{
        //    var x = new FirmwareModel();

        //    x.InstallDate = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.INSTALL_DATE));
        //    x.InstallDateDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.INSTALL_DATE));

        //    x.License = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.LICENSE));
        //    x.LicenseDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.LICENSE));

        //    x.Manufacturer = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.MANUFACTURER));
        //    x.ManufacturerDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.MANUFACTURER));

        //    x.ReleaseDate = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.RELEASE_DATE));
        //    x.ReleaseDateDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.RELEASE_DATE));

        //    x.Version = GetDataItemValue(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.VERSION));
        //    x.VersionDataItem = GetDataItem(FirmwareDataItem.NameId, FirmwareDataItem.GetSubTypeId(FirmwareDataItem.SubTypes.VERSION));

        //    return x;

        //}

        //private void SetFirmware(FirmwareModel model)
        //{
        //    if (model != null)
        //    {
        //        AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.INSTALL_DATE), model.InstallDate);
        //        AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.LICENSE), model.License);
        //        AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.MANUFACTURER), model.Manufacturer);
        //        AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.RELEASE_DATE), model.ReleaseDate);
        //        AddDataItem(new FirmwareDataItem(Id, FirmwareDataItem.SubTypes.VERSION), model.Version);
        //    }
        //}

        //#endregion

        //#region "Hardware"

        //private HardwareModel GetHardware()
        //{
        //    var x = new HardwareModel();

        //    x.InstallDate = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.INSTALL_DATE));
        //    x.InstallDateDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.INSTALL_DATE));

        //    x.License = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.LICENSE));
        //    x.LicenseDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.LICENSE));

        //    x.Manufacturer = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.MANUFACTURER));
        //    x.ManufacturerDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.MANUFACTURER));

        //    x.ReleaseDate = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.RELEASE_DATE));
        //    x.ReleaseDateDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.RELEASE_DATE));

        //    x.Version = GetDataItemValue(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.VERSION));
        //    x.VersionDataItem = GetDataItem(HardwareDataItem.NameId, HardwareDataItem.GetSubTypeId(HardwareDataItem.SubTypes.VERSION));

        //    return x;

        //}

        //private void SetHardware(HardwareModel model)
        //{
        //    if (model != null)
        //    {
        //        AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.INSTALL_DATE), model.InstallDate);
        //        AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.LICENSE), model.License);
        //        AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.MANUFACTURER), model.Manufacturer);
        //        AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.RELEASE_DATE), model.ReleaseDate);
        //        AddDataItem(new HardwareDataItem(Id, HardwareDataItem.SubTypes.VERSION), model.Version);
        //    }
        //}

        //#endregion

        //#region "Library"

        //private LibraryModel GetLibrary()
        //{
        //    var x = new LibraryModel();

        //    x.InstallDate = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.INSTALL_DATE));
        //    x.InstallDateDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.INSTALL_DATE));

        //    x.License = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.LICENSE));
        //    x.LicenseDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.LICENSE));

        //    x.Manufacturer = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.MANUFACTURER));
        //    x.ManufacturerDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.MANUFACTURER));

        //    x.ReleaseDate = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.RELEASE_DATE));
        //    x.ReleaseDateDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.RELEASE_DATE));

        //    x.Version = GetDataItemValue(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.VERSION));
        //    x.VersionDataItem = GetDataItem(LibraryDataItem.NameId, LibraryDataItem.GetSubTypeId(LibraryDataItem.SubTypes.VERSION));

        //    return x;

        //}

        //private void SetLibrary(LibraryModel model)
        //{
        //    if (model != null)
        //    {
        //        AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.INSTALL_DATE), model.InstallDate);
        //        AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.LICENSE), model.License);
        //        AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.MANUFACTURER), model.Manufacturer);
        //        AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.RELEASE_DATE), model.ReleaseDate);
        //        AddDataItem(new LibraryDataItem(Id, LibraryDataItem.SubTypes.VERSION), model.Version);
        //    }
        //}

        //#endregion


        //private string GetComponentTypeId(Type type)
        //{
        //    var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        //    if (!fieldInfos.IsNullOrEmpty())
        //    {
        //        var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        //        if (!constants.IsNullOrEmpty())
        //        {
        //            var fieldInfo = constants.FirstOrDefault(o => o.Name == "TypeId");
        //            if (fieldInfo != null)
        //            {
        //                try
        //                {
        //                    var obj = (Component)Activator.CreateInstance(type);
        //                    if (obj != null)
        //                    {
        //                        return fieldInfo.GetValue(obj).ToString();
        //                    }
        //                }
        //                catch { }
        //            }
        //        }
        //    }

        //    return "";
        //}

        //private string GetComponentNameId(Type type)
        //{
        //    var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        //    if (!fieldInfos.IsNullOrEmpty())
        //    {
        //        var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        //        if (!constants.IsNullOrEmpty())
        //        {
        //            var fieldInfo = constants.FirstOrDefault(o => o.Name == "NameId");
        //            if (fieldInfo != null)
        //            {
        //                try
        //                {
        //                    var obj = (Component)Activator.CreateInstance(type);
        //                    if (obj != null)
        //                    {
        //                        return fieldInfo.GetValue(obj).ToString();
        //                    }
        //                }
        //                catch { }
        //            }
        //        }
        //    }

        //    return "";
        //}

        //private string GetCompositionTypeId(Type type)
        //{
        //    var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        //    if (!fieldInfos.IsNullOrEmpty())
        //    {
        //        var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        //        if (!constants.IsNullOrEmpty())
        //        {
        //            var fieldInfo = constants.FirstOrDefault(o => o.Name == "TypeId");
        //            if (fieldInfo != null)
        //            {
        //                try
        //                {
        //                    var obj = (Composition)Activator.CreateInstance(type);
        //                    if (obj != null)
        //                    {
        //                        return fieldInfo.GetValue(obj).ToString();
        //                    }
        //                }
        //                catch { }
        //            }
        //        }
        //    }

        //    return "";
        //}

        //private string GetCompositionNameId(Type type)
        //{
        //    var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        //    if (!fieldInfos.IsNullOrEmpty())
        //    {
        //        var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        //        if (!constants.IsNullOrEmpty())
        //        {
        //            var fieldInfo = constants.FirstOrDefault(o => o.Name == "NameId");
        //            if (fieldInfo != null)
        //            {
        //                try
        //                {
        //                    var obj = (Composition)Activator.CreateInstance(type);
        //                    if (obj != null)
        //                    {
        //                        return fieldInfo.GetValue(obj).ToString();
        //                    }
        //                }
        //                catch { }
        //            }
        //        }
        //    }

        //    return "";
        //}
    }
}
