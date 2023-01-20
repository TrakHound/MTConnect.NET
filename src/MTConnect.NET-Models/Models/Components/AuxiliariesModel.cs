// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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
        public EnvironmentalModel Environmental => ComponentManager.GetComponentModel<EnvironmentalModel>(typeof(EnvironmentalComponent));

        /// <summary>
        /// ToolingDelivery is an Auxiliary that represents the information for a unit involved in managing, positioning, storing, and delivering tooling within a piece of equipment.
        /// </summary>
        public ToolingDeliveryModel ToolingDelivery => ComponentManager.GetComponentModel<ToolingDeliveryModel>(typeof(ToolingDeliveryComponent));

        /// <summary>
        /// WasteDisposal is an Auxiliary that represents the information for a unit comprised of all the parts involved in removing manufacturing byproducts from a piece of equipment.
        /// </summary>
        public WasteDisposalModel WasteDisposal => ComponentManager.GetComponentModel<WasteDisposalModel>(typeof(WasteDisposalComponent));


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