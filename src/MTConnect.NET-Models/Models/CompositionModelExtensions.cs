// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models
{
    public static class CompositionModelExtensions
    {
        public static IEnumerable<T> Convert<T>(this IEnumerable<ICompositionModel> models) where T : CompositionModel
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

        public static T Convert<T>(this ICompositionModel model) where T : CompositionModel
        {
            if (model != null)
            {
                try
                {
                    var composition = (CompositionModel)model;

                    var obj = (T)Activator.CreateInstance(typeof(T));

                    obj.Id = composition.Id;
                    obj.Uuid = composition.Uuid;
                    obj.Name = composition.Name;
                    obj.NativeName = composition.NativeName;
                    obj.Type = composition.Type;

                    obj.Manufacturer = composition.Manufacturer;
                    obj.Model = composition.Model;
                    obj.SerialNumber = composition.SerialNumber;
                    obj.Station = composition.Station;
                    obj.Description = composition.Description;

                    obj.DataItemModels = composition.DataItemModels;

                    // Copy DataItems
                    foreach (var dataItem in composition.DataItemManager.DataItemValues)
                    {
                        obj.DataItemManager.DataItemValues.Add(dataItem.Key, dataItem.Value);
                    }

                    // Copy Conditions
                    foreach (var condition in composition.DataItemManager.Conditions)
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
