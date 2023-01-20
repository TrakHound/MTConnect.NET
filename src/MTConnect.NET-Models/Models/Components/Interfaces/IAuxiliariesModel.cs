// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Auxiliaries organizes Auxiliary component types.
    /// </summary>
    public interface IAuxiliariesModel : IComponentModel
    {
        /// <summary>
        /// Environmental is an Auxiliary that represents the information for a unit or function involved in monitoring, managing, or conditioning the environment around or within a piece of equipment.
        /// </summary>
        EnvironmentalModel Environmental { get; }

        /// <summary>
        /// ToolingDelivery is an Auxiliary that represents the information for a unit involved in managing, positioning, storing, and delivering tooling within a piece of equipment.
        /// </summary>
        ToolingDeliveryModel ToolingDelivery { get; }

        /// <summary>
        /// WasteDisposal is an Auxiliary that represents the information for a unit comprised of all the parts involved in removing manufacturing byproducts from a piece of equipment.
        /// </summary>
        WasteDisposalModel WasteDisposal { get; }
    }
}
