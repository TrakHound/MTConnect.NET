// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models
{
    public static class ComponentModelExtensions
    {
        public static IEnumerable<T> Convert<T>(this IEnumerable<IComponentModel> models) where T : ComponentModel
        {
            if (!models.IsNullOrEmpty())
            {
                var objs = new List<T>();

                foreach (var model in models)
                {
                    var obj = model.Convert<T>();
                    if (obj != null) objs.Add(obj);
                }

                return objs;
            }

            return Enumerable.Empty<T>();
        }

        public static T Convert<T>(this IComponentModel model) where T : ComponentModel
        {
            if (model != null)
            {
                try
                {
                    var component = (ComponentModel)model;

                    var obj = (T)Activator.CreateInstance(typeof(T));

                    obj.Id = component.Id;
                    obj.Uuid = component.Uuid;
                    obj.Name = component.Name;
                    obj.NativeName = component.NativeName;
                    obj.Type = component.Type;

                    obj.Manufacturer = component.Manufacturer;
                    obj.Model = component.Model;
                    obj.SerialNumber = component.SerialNumber;
                    obj.Station = component.Station;
                    obj.Description = component.Description;

                    obj.SampleRate = component.SampleRate;
                    obj.SampleInterval = component.SampleInterval;
                    obj.References = component.References;
                    obj.Configuration = component.Configuration;

                    obj.ComponentModels = component.ComponentModels;
                    obj.CompositionModels = component.CompositionModels;
                    obj.DataItemModels = component.DataItemModels;

                    // Copy DataItems
                    foreach (var dataItem in component.DataItemManager.DataItemValues)
                    {
                        obj.DataItemManager.DataItemValues.Add(dataItem.Key, dataItem.Value);
                    }

                    // Copy Conditions
                    foreach (var condition in component.DataItemManager.Conditions)
                    {
                        obj.DataItemManager.Conditions.Add(condition.Key, condition.Value);
                    }

                    return obj;
                }
                catch { }
            }            

            return default;
        }
    }
}