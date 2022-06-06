// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect.Models
{
    internal class ComponentManager
    {
        private readonly DataItemManager _dataItemManager;

        internal DataItemManager DataItemManager => _dataItemManager;


        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                _dataItemManager.Id = value;
            }
        }

        /// <summary>
        /// List of Child ComponentModels 
        /// </summary>
        public List<IComponentModel> ComponentModels { get; set; }

        public List<Component> Components
        {
            get
            {
                var x = new List<Component>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    foreach (var componentModel in ComponentModels.OfType<ComponentModel>()) x.Add(componentModel);
                }

                return x;
            }
        }


        /// <summary>
        /// List of Child CompositionModels 
        /// </summary>
        public List<ICompositionModel> CompositionModels { get; set; }

        public List<Composition> Compositions
        {
            get
            {
                var x = new List<Composition>();

                if (!CompositionModels.IsNullOrEmpty())
                {
                    foreach (var compositionModel in CompositionModels.OfType<CompositionModel>()) x.Add(compositionModel);
                }

                return x;
            }
        }


        /// <summary>
        /// List of Child DataItemModels 
        /// </summary>
        public IEnumerable<IDataItemModel> DataItemModels => DataItemManager.GetDataItems();
        //public List<IDataItemModel> DataItemModels { get; set; }

        public List<DataItem> DataItems
        {
            get
            {
                var x = new List<DataItem>();

                if (!DataItemModels.IsNullOrEmpty())
                {
                    foreach (var dataItemModel in DataItemModels.OfType<DataItemModel>()) x.Add(dataItemModel);
                }

                if (!CompositionModels.IsNullOrEmpty())
                {
                    foreach (var composition in CompositionModels)
                    {
                        if (!composition.DataItemModels.IsNullOrEmpty())
                        {
                            foreach (var dataItemModel in composition.DataItemModels.OfType<DataItemModel>())
                            {
                                x.Add(dataItemModel);
                            }
                        }
                    }
                }

                return x;
            }
        }


        public EventHandler<IObservation> ObservationUpdated { get; set; }


        public ComponentManager() 
        {
            _dataItemManager = new DataItemManager();
            _dataItemManager.ObservationUpdated += OnObservationUpdated;
            ComponentModels = new List<IComponentModel>();
        }

        public ComponentManager(string id)
        {
            _dataItemManager = new DataItemManager(id);
            _dataItemManager.ObservationUpdated += OnObservationUpdated;
            ComponentModels = new List<IComponentModel>();
            Id = id;
        }


        #region "Create"

        public static IComponentModel Create(IComponent component)
        {
            if (component != null)
            {
                var obj = CreateComponentModel(component.Type);
                if (obj != null)
                {
                    obj.Id = component.Id;
                    obj.Uuid = component.Uuid;
                    obj.Name = component.Name;
                    obj.NativeName = component.NativeName;
                    obj.Type = component.Type;

                    if (component.Description != null)
                    {
                        obj.Manufacturer = component.Description.Manufacturer;
                        obj.Model = component.Description.Model;
                        obj.SerialNumber = component.Description.SerialNumber;
                        obj.Station = component.Description.Station;
                        obj.DescriptionText = component.Description.CDATA;
                    }

                    obj.SampleRate = component.SampleRate;
                    obj.SampleInterval = component.SampleInterval;
                    obj.References = component.References;
                    obj.Configuration = component.Configuration;

                    obj.ComponentModels = CreateComponentModels(component.Components);
                    obj.CompositionModels = CreateCompositionModels(component.Compositions);
                    obj.DataItemModels = CreateDataItemModels(component.DataItems);

                    return obj;
                }
            }

            return null;
        }

        public static ICompositionModel Create(IComposition component)
        {
            if (component != null)
            {
                var obj = CreateCompositionModel(component.Type);
                if (obj != null)
                {
                    obj.Id = component.Id;
                    obj.Uuid = component.Uuid;
                    obj.Name = component.Name;
                    obj.NativeName = component.NativeName;
                    obj.Type = component.Type;

                    if (component.Description != null)
                    {
                        obj.Manufacturer = component.Description.Manufacturer;
                        obj.Model = component.Description.Model;
                        obj.SerialNumber = component.Description.SerialNumber;
                        obj.Station = component.Description.Station;
                        obj.DescriptionText = component.Description.CDATA;
                    }

                    obj.SampleRate = component.SampleRate;
                    obj.SampleInterval = component.SampleInterval;
                    obj.References = component.References;
                    obj.Configuration = component.Configuration;

                    //obj.ComponentModels = CreateComponentModels(component.Components);
                    //obj.CompositionModels = CreateCompositionModels(component.Compositions);
                    obj.DataItemModels = CreateDataItemModels(component.DataItems);

                    return obj;
                }
            }

            return null;
        }

        public static List<IComponentModel> CreateComponentModels(IEnumerable<IComponent> components)
        {
            if (!components.IsNullOrEmpty())
            {
                var objs = new List<IComponentModel>();

                foreach (var component in components)
                {
                    var obj = Create(component);
                    if (obj != null) objs.Add(obj);
                }

                return objs;
            }

            return new List<IComponentModel>();
        }

        public static List<ICompositionModel> CreateCompositionModels(IEnumerable<IComposition> compositions)
        {
            if (!compositions.IsNullOrEmpty())
            {
                var objs = new List<ICompositionModel>();

                foreach (var composition in compositions)
                {
                    var obj = Create(composition);
                    if (obj != null) objs.Add(obj);
                }

                return objs;
            }

            return new List<ICompositionModel>();
        }

        public static List<IDataItemModel> CreateDataItemModels(IEnumerable<IDataItem> dataItems)
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



        public static ComponentModel CreateComponentModel(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var types = GetAllComponentModelTypes();
                if (!types.IsNullOrEmpty())
                {
                    foreach (var t in types)
                    {
                        try
                        {
                            var obj = (ComponentModel)Activator.CreateInstance(t);
                            if (obj.Type == type) return obj;
                        }
                        catch { }
                    }
                }
            }

            return null;
        }

        public static CompositionModel CreateCompositionModel(string type)
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
                            var obj = (CompositionModel)Activator.CreateInstance(t);
                            if (obj.Type == type) return obj;
                        }
                        catch { }
                    }
                }
            }

            return null;
        }

        private static IEnumerable<Type> GetAllComponentModelTypes()
        {
            // Search loaded assemblies for IComponentModel classes
            // This allows for custom types to be included in DLL's and read at runtime
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (!assemblies.IsNullOrEmpty())
            {
                var types = assemblies.SelectMany(x => x.GetTypes());
                return types.Where(x => typeof(IComponentModel).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }

            return Enumerable.Empty<Type>();
        }

        private static IEnumerable<Type> GetAllCompositionModelTypes()
        {
            // Search loaded assemblies for IComponentModel classes
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

        #region "Components"

        public T GetComponentModel<T>(Type componentType, string name = null) where T : ComponentModel
        {
            // Get the TypeId for the ComponentModel Type
            var typeId = GetComponentTypeId(componentType);

            // Find the ComponentModel that matches the TypeId
            var obj = (T)ComponentModels?.FirstOrDefault(o => o.Type == typeId && (name == null || o.Name == name));
            if (obj == null)
            {
                if (name == null)
                {
                    // Get the NameId for the ComponentModel Type
                    name = GetComponentNameId(componentType);
                }

                // If Not found then add a new component using the Type information
                obj = AddComponentModel<T>(name);
            }

            return obj;
        }

        public void AddComponentModels(IEnumerable<IComponentModel> components)
        {
            if (!components.IsNullOrEmpty())
            {
                foreach (var component in components)
                {
                    AddComponentModel(component);
                }
            }
        }

        public void AddComponentModel(IComponentModel component)
        {
            if (component != null)
            {
                component.ObservationUpdated += OnObservationUpdated;

                if (ComponentModels == null) ComponentModels = new List<IComponentModel>();
                ComponentModels.Add(component);

                if (!component.CompositionModels.IsNullOrEmpty())
                {
                    foreach (var composition in component.CompositionModels)
                    {
                        AddCompositionModel(composition);
                    }
                }

                if (!component.DataItemModels.IsNullOrEmpty())
                {
                    foreach (var dataItemModel in component.DataItemModels)
                    {
                        DataItemManager.AddDataItem(dataItemModel, null);
                    }
                }
            }
        }

        public T AddComponentModel<T>(string name) where T : ComponentModel
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var model = (T)Activator.CreateInstance(typeof(T));
                    model.Id = Component.CreateId(Id, name);
                    model.Name = name;

                    AddComponentModel(model);

                    if (!model.Compositions.IsNullOrEmpty())
                    {
                        foreach (var composition in model.Compositions)
                        {
                            //AddCompositionModel(composition);
                        }
                    }

                    if (!model.DataItems.IsNullOrEmpty())
                    {
                        foreach (var dataItem in model.DataItems)
                        {
                            DataItemManager.AddDataItem(dataItem, null);
                        }
                    }

                    return model;
                }
                catch { }
            }

            return null;
        }

        #endregion

        #region "Compositions"

        public T GetCompositionModel<T>(Type compositionType, string name = null) where T : CompositionModel
        {
            // Get the TypeId for the CompositionModel Type
            var typeId = GetCompositionTypeId(compositionType);

            // Find the CompositionModel that matches the TypeId
            var obj = (T)CompositionModels?.FirstOrDefault(o => o.Type == typeId && (name == null || o.Name == name));
            if (obj == null)
            {
                if (name == null)
                {
                    // Get the NameId for the CompositionModel Type
                    name = GetCompositionNameId(compositionType);
                }

                // If Not found then add a new composition using the Type information
                obj = AddCompositionModel<T>(name);
            }

            return obj;
        }

        public void AddCompositionModel(ICompositionModel composition)
        {
            if (composition != null)
            {
                composition.ObservationUpdated += OnObservationUpdated;

                if (CompositionModels == null) CompositionModels = new List<ICompositionModel>();
                CompositionModels.Add(composition);
            }
        }

        public void AddCompositionModels(IEnumerable<ICompositionModel> compositions)
        {
            if (!compositions.IsNullOrEmpty())
            {
                foreach (var composition in compositions)
                {
                    AddCompositionModel(composition);
                }
            }
        }

        public T AddCompositionModel<T>(string name) where T : CompositionModel
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var model = (T)Activator.CreateInstance(typeof(T));
                    model.Id = Component.CreateId(Id, name);
                    model.Name = name;

                    AddCompositionModel(model);
                    return model;
                }
                catch { }
            }

            return null;
        }


        #endregion

        #region "DataItems"

        public void AddDataItem(IDataItemModel dataItemModel)
        {
            if (dataItemModel != null)
            {
                DataItemManager.AddDataItem(dataItemModel, null);
            }
        }

        public void AddDataItems(IEnumerable<IDataItemModel> dataItemModels)
        {
            if (!dataItemModels.IsNullOrEmpty())
            {
                foreach (var dataItemModel in dataItemModels)
                {
                    AddDataItem(dataItemModel);
                }
            }
        }

        #endregion

        #region "Observations"

        private void OnObservationUpdated(object sender, IObservation observation)
        {
            if (ObservationUpdated != null) ObservationUpdated.Invoke(this, observation);
        }


        public IObservation GetObservation(string type, string subType = null)
        {
            return _dataItemManager.GetObservation(type, subType);
        }

        public IEnumerable<IObservation> GetObservations()
        {
            var objs = new List<IObservation>();

            if (!DataItems.IsNullOrEmpty())
            {
                foreach (var dataItem in DataItems)
                {
                    var observation = _dataItemManager.GetObservation(dataItem.Type, dataItem.SubType);
                    if (observation != null) objs.Add(observation);

                    //var value = _dataItemManager.GetDataItemValue(dataItem.Type, dataItem.SubType);
                    //if (value != null)
                    //{
                    //    Observation observation = null;

                    //    switch (dataItem.Category)
                    //    {
                    //        case DataItemCategory.EVENT:

                    //            observation = EventObservation.Create(dataItem);
                    //            switch (dataItem.Representation)
                    //            {
                    //                case DataItemRepresentation.VALUE: observation.AddValue(ValueKeys.CDATA, value); break;
                    //            }
                    //            break;

                    //        case DataItemCategory.SAMPLE:

                    //            observation = SampleObservation.Create(dataItem);
                    //            switch (dataItem.Representation)
                    //            {
                    //                case DataItemRepresentation.VALUE: observation.AddValue(ValueKeys.CDATA, value); break;
                    //            }
                    //            break;
                    //    }

                    //    if (observation != null) objs.Add(observation);
                    //}
                }
            }

            if (!CompositionModels.IsNullOrEmpty())
            {
                foreach (var compositionModel in CompositionModels)
                {
                    objs.AddRange(compositionModel.GetObservations());
                }
            }

            if (!ComponentModels.IsNullOrEmpty())
            {
                foreach (var componentModel in ComponentModels)
                {
                    objs.AddRange(componentModel.GetObservations());
                }
            }

            return objs;
        }



        //public IEnumerable<Observation> GetObservations(long timestamp = 0)
        //{
        //    var objs = new List<Observation>();

        //    if (timestamp <= 0) timestamp = UnixDateTime.Now;

        //    foreach (var dataItem in _dataItemManager.DataItemValues)
        //    {
        //        var obj = new ObservationInput(dataItem.Key, dataItem.Value);
        //        obj.Timestamp = timestamp;
        //        objs.Add(obj);
        //    }

        //    if (!CompositionModels.IsNullOrEmpty())
        //    {
        //        foreach (CompositionModel compositionModel in CompositionModels.OfType<CompositionModel>())
        //        {
        //            objs.AddRange(compositionModel.GetObservations(timestamp));
        //        }
        //    }

        //    if (!ComponentModels.IsNullOrEmpty())
        //    {
        //        foreach (var componentModel in ComponentModels.OfType<ComponentModel>())
        //        {
        //            objs.AddRange(componentModel.GetObservations(timestamp));
        //        }
        //    }

        //    return objs;
        //}

        //public IEnumerable<ObservationInput> GetObservations(long timestamp = 0)
        //{
        //    var objs = new List<ObservationInput>();

        //    if (timestamp <= 0) timestamp = UnixDateTime.Now;

        //    foreach (var dataItem in _dataItemManager.DataItemValues)
        //    {
        //        var obj = new ObservationInput(dataItem.Key, dataItem.Value);
        //        obj.Timestamp = timestamp;
        //        objs.Add(obj);
        //    }

        //    if (!CompositionModels.IsNullOrEmpty())
        //    {
        //        foreach (CompositionModel compositionModel in CompositionModels.OfType<CompositionModel>())
        //        {
        //            objs.AddRange(compositionModel.GetObservations(timestamp));
        //        }
        //    }

        //    if (!ComponentModels.IsNullOrEmpty())
        //    {
        //        foreach (var componentModel in ComponentModels.OfType<ComponentModel>())
        //        {
        //            objs.AddRange(componentModel.GetObservations(timestamp));
        //        }
        //    }

        //    return objs;
        //}

        public IEnumerable<ConditionObservationInput> GetConditionObservations(long timestamp = 0)
        {
            var objs = new List<ConditionObservationInput>();

            if (timestamp <= 0) timestamp = UnixDateTime.Now;

            foreach (var condition in _dataItemManager.Conditions)
            {
                var obj = new ConditionObservationInput(condition.Key, condition.Value.Level);
                obj.NativeCode = condition.Value.NativeCode;
                obj.NativeSeverity = condition.Value.NativeSeverity;
                obj.Qualifier = condition.Value.Qualifier;
                obj.Text = condition.Value.CDATA;
                obj.Timestamp = timestamp;
                objs.Add(obj);
            }

            if (!CompositionModels.IsNullOrEmpty())
            {
                foreach (var compositionModel in CompositionModels.OfType<CompositionModel>())
                {
                    objs.AddRange(compositionModel.GetConditionObservations(timestamp));
                }
            }

            if (!ComponentModels.IsNullOrEmpty())
            {
                foreach (var componentModel in ComponentModels.OfType<ComponentModel>())
                {
                    objs.AddRange(componentModel.GetConditionObservations(timestamp));
                }
            }

            return objs;
        }


        public void UpdateObservations(IDeviceStream stream)
        {
            if (stream != null && !stream.Observations.IsNullOrEmpty())
            {
                if (!DataItemModels.IsNullOrEmpty())
                {
                    foreach (var dataItem in DataItemModels)
                    {
                        if (dataItem.DataItemCategory == DataItemCategory.CONDITION)
                        {
                            var obj = stream.Conditions.FirstOrDefault(o => o.DataItemId == dataItem.DataItemId);
                            if (obj != null) DataItemManager.AddCondition(dataItem, obj);
                        }
                        else if (dataItem.DataItemCategory == DataItemCategory.EVENT)
                        {
                            var obj = stream.EventValues.FirstOrDefault(o => o.DataItemId == dataItem.DataItemId);
                            if (obj != null) DataItemManager.AddDataItem(dataItem, obj.CDATA);
                        }
                        else if (dataItem.DataItemCategory == DataItemCategory.SAMPLE)
                        {
                            var obj = stream.SampleValues.FirstOrDefault(o => o.DataItemId == dataItem.DataItemId);
                            if (obj != null) DataItemManager.AddDataItem(dataItem, obj.CDATA);
                        }
                    }
                }

                // Add Compositions
                if (!CompositionModels.IsNullOrEmpty())
                {
                    foreach (var composition in CompositionModels.OfType<CompositionModel>())
                    {
                        UpdateObservations(composition, stream);
                    }
                }

                // Add Components
                if (!ComponentModels.IsNullOrEmpty())
                {
                    foreach (var subcomponent in ComponentModels.OfType<ComponentModel>())
                    {
                        subcomponent.ComponentManager.UpdateObservations(stream);
                    }
                }
            }
        }

        public void UpdateObservations(CompositionModel composition, IDeviceStream stream)
        {
            if (composition != null && !composition.DataItems.IsNullOrEmpty() && stream != null && !stream.Observations.IsNullOrEmpty())
            {
                foreach (var dataItem in composition.DataItemModels)
                {
                    if (dataItem.DataItemCategory == DataItemCategory.CONDITION)
                    {
                        var obj = stream.Conditions.FirstOrDefault(o => o.DataItemId == dataItem.DataItemId);
                        if (obj != null) composition.AddCondition(dataItem, obj);
                    }
                    else if(dataItem.DataItemCategory == DataItemCategory.EVENT)
                    {
                        var obj = stream.EventValues.FirstOrDefault(o => o.DataItemId == dataItem.DataItemId);
                        if (obj != null) composition.AddDataItem(dataItem, obj.CDATA);
                    }
                    else if (dataItem.DataItemCategory == DataItemCategory.SAMPLE)
                    {
                        var obj = stream.SampleValues.FirstOrDefault(o => o.DataItemId == dataItem.DataItemId);
                        if (obj != null) composition.AddDataItem(dataItem, obj.CDATA);
                    }
                }
            }
        }

        #endregion


        private string GetComponentTypeId(Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (!fieldInfos.IsNullOrEmpty())
            {
                var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
                if (!constants.IsNullOrEmpty())
                {
                    var fieldInfo = constants.FirstOrDefault(o => o.Name == "TypeId");
                    if (fieldInfo != null)
                    {
                        try
                        {
                            var obj = (Component)Activator.CreateInstance(type);
                            if (obj != null)
                            {
                                return fieldInfo.GetValue(obj).ToString();
                            }
                        }
                        catch { }
                    }
                }
            }

            return "";
        }

        private string GetComponentNameId(Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (!fieldInfos.IsNullOrEmpty())
            {
                var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
                if (!constants.IsNullOrEmpty())
                {
                    var fieldInfo = constants.FirstOrDefault(o => o.Name == "NameId");
                    if (fieldInfo != null)
                    {
                        try
                        {
                            var obj = (Component)Activator.CreateInstance(type);
                            if (obj != null)
                            {
                                return fieldInfo.GetValue(obj).ToString();
                            }
                        }
                        catch { }
                    }
                }
            }

            return "";
        }

        private string GetCompositionTypeId(Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (!fieldInfos.IsNullOrEmpty())
            {
                var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
                if (!constants.IsNullOrEmpty())
                {
                    var fieldInfo = constants.FirstOrDefault(o => o.Name == "TypeId");
                    if (fieldInfo != null)
                    {
                        try
                        {
                            var obj = (Composition)Activator.CreateInstance(type);
                            if (obj != null)
                            {
                                return fieldInfo.GetValue(obj).ToString();
                            }
                        }
                        catch { }
                    }
                }
            }

            return "";
        }

        private string GetCompositionNameId(Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (!fieldInfos.IsNullOrEmpty())
            {
                var constants = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
                if (!constants.IsNullOrEmpty())
                {
                    var fieldInfo = constants.FirstOrDefault(o => o.Name == "NameId");
                    if (fieldInfo != null)
                    {
                        try
                        {
                            var obj = (Composition)Activator.CreateInstance(type);
                            if (obj != null)
                            {
                                return fieldInfo.GetValue(obj).ToString();
                            }
                        }
                        catch { }
                    }
                }
            }

            return "";
        }
    }
}
