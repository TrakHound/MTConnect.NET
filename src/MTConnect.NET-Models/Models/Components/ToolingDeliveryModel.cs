// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// ToolingDelivery is an Auxiliary that represents the information for a unit involved in managing, positioning, storing, and delivering tooling within a piece of equipment.
    /// </summary>
    public class ToolingDeliveryModel : AuxiliaryModel, IToolingDeliveryModel
    {
        /// <summary>
        /// AutomaticToolChanger is a ToolingDelivery that represents a tool delivery mechanism that moves tools between a ToolMagazine and a Spindle or a Turret.
        /// An AutomaticToolChanger may also transfer tools between a location outside of a piece of equipment and a ToolMagazine or Turret.
        /// </summary>
        public IAutomaticToolChangerModel AutomaticToolChanger => ComponentManager.GetComponentModel<AutomaticToolChangerModel>(typeof(AutomaticToolChangerComponent));

        /// <summary>
        /// ToolMagazine is a ToolingDelivery that represents a tool storage mechanism that holds any number of tools.Tools are located in POTs.
        /// POTs are moved into position to transfer tools into or out of the ToolMagazine by an AutomaticToolChanger.
        /// </summary>
        public IToolMagazineModel ToolMagazine => ComponentManager.GetComponentModel<ToolMagazineModel>(typeof(ToolMagazineComponent));

        /// <summary>
        /// Turret is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools.
        /// Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by rotating the Turret.
        /// </summary>
        public ITurretModel Turret => ComponentManager.GetComponentModel<TurretModel>(typeof(TurretComponent));
 

        public ToolingDeliveryModel()
        {
            Type = ToolingDeliveryComponent.TypeId;
        }

        public ToolingDeliveryModel(string componentId)
        {
            Id = componentId;
            Type = ToolingDeliveryComponent.TypeId;
        }
    }
}
