// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Adapters organizes Adapter component types.
    /// </summary>
    public class AdaptersModel : ComponentModel, IAdaptersModel
    {
        /// <summary>
        /// Adapter is a Component that represents the connectivity state of a data source for the MTConnect Agent.
        /// </summary>
        public IEnumerable<IAdapterModel> Adapters
        {
            get
            {
                var x = new List<IAdapterModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var models = ComponentModels.Where(o => o.Type == AdapterComponent.TypeId);
                    if (!models.IsNullOrEmpty())
                    {
                        foreach (var model in models) x.Add((AdapterModel)model);
                    }
                }

                return x;
            }
        }


        public AdaptersModel() 
        {
            Type = AdaptersComponent.TypeId;
        }

        public AdaptersModel(string componentId)
        {
            Id = componentId;
            Type = AdaptersComponent.TypeId;
        }


        /// <summary>
        /// Gets the Adapter Component with the specified Name
        /// (If the component doesn't exist then it will be created)
        /// </summary>
        /// <param name="name">The name of the Adapter Component</param>
        public IAdapterModel GetAdapter(string name) => ComponentManager.GetComponentModel<AdapterModel>(typeof(AdapterComponent), name);
    }
}