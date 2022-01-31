// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Auxiliaries organizes Auxiliary component types.
    /// </summary>
    public class AuxiliariesModel : ComponentModel, IAuxiliariesModel
    {
        /// <summary>
        /// Environmental is an Auxiliary that represents the information for a unit or function involved in monitoring, managing, or conditioning the environment around or within a piece of equipment.
        /// </summary>
        public EnvironmentalModel Environmental => GetComponentModel<EnvironmentalModel>(typeof(EnvironmentalComponent));

        /// <summary>
        /// ToolingDelivery is an Auxiliary that represents the information for a unit involved in managing, positioning, storing, and delivering tooling within a piece of equipment.
        /// </summary>
        public ToolingDeliveryModel ToolingDelivery => GetComponentModel<ToolingDeliveryModel>(typeof(ToolingDeliveryComponent));

        /// <summary>
        /// WasteDisposal is an Auxiliary that represents the information for a unit comprised of all the parts involved in removing manufacturing byproducts from a piece of equipment.
        /// </summary>
        public WasteDisposalModel WasteDisposal => GetComponentModel<WasteDisposalModel>(typeof(WasteDisposalComponent));


        public AuxiliariesModel() 
        {
            Type = AuxiliariesComponent.TypeId;
        }

        public AuxiliariesModel(string componentId)
        {
            Id = componentId;
            Type = AuxiliariesComponent.TypeId;
        }
    }
}
