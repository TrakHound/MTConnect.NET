// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Adapters organizes Adapter component types.
    /// </summary>
    public interface IAdaptersModel : IComponentModel
    {
        /// <summary>
        /// Adapter is a Component that represents the connectivity state of a data source for the MTConnect Agent.
        /// </summary>
        IEnumerable<IAdapterModel> Adapters { get; }


        /// <summary>
        /// Gets the Adapter Component with the specified Name
        /// (If the component doesn't exist then it will be created)
        /// </summary>
        /// <param name="name">The name of the Adapter Component</param>
        IAdapterModel GetAdapter(string name);
    }
}