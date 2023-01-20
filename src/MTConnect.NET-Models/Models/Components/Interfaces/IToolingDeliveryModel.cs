// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Models.Components
{
    /// <summary>
    /// ToolingDelivery is an Auxiliary that represents the information for a unit involved in managing, positioning, storing, and delivering tooling within a piece of equipment.
    /// </summary>
    public interface IToolingDeliveryModel : IAuxiliaryModel
    {
        /// <summary>
        /// AutomaticToolChanger is a ToolingDelivery that represents a tool delivery mechanism that moves tools between a ToolMagazine and a Spindle or a Turret.
        /// An AutomaticToolChanger may also transfer tools between a location outside of a piece of equipment and a ToolMagazine or Turret.
        /// </summary>
        IAutomaticToolChangerModel AutomaticToolChanger { get; }

        /// <summary>
        /// ToolMagazine is a ToolingDelivery that represents a tool storage mechanism that holds any number of tools.Tools are located in POTs.
        /// POTs are moved into position to transfer tools into or out of the ToolMagazine by an AutomaticToolChanger.
        /// </summary>
        IToolMagazineModel ToolMagazine { get; }

        /// <summary>
        /// Turret is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools.
        /// Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by rotating the Turret.
        /// </summary>
        ITurretModel Turret { get; }
    }
}
