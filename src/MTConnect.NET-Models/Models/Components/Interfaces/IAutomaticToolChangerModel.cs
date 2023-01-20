// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.Compositions;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// AutomaticToolChanger is a ToolingDelivery that represents a tool delivery mechanism that moves tools between a ToolMagazine and a Spindle or a Turret.
    /// An AutomaticToolChanger may also transfer tools between a location outside of a piece of equipment and a ToolMagazine or Turret.
    /// </summary>
    public interface IAutomaticToolChangerModel : IAuxiliaryModel
    {
        /// <summary>
        /// A mechanism for physically moving a tool from one location to another.
        /// </summary>
        ITransferArmModel TransferArm { get; set; }

        /// <summary>
        /// A POT for a tool awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.
        /// </summary>
        ITransferPotModel TransferPot { get; set; }

        /// <summary>
        /// A POT for a tool removed from Spindle or Turret and awaiting for return to a ToolMagazine.
        /// </summary>
        IReturnPotModel ReturnPot { get; set; }

        /// <summary>
        /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        IMotorModel Motor { get; set; }
    }
}
