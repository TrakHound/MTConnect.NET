// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Models.Compositions;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// AutomaticToolChanger is a ToolingDelivery that represents a tool delivery mechanism that moves tools between a ToolMagazine and a Spindle or a Turret.
    /// An AutomaticToolChanger may also transfer tools between a location outside of a piece of equipment and a ToolMagazine or Turret.
    /// </summary>
    public class AutomaticToolChangerModel : AuxiliaryModel, IAutomaticToolChangerModel
    {
        /// <summary>
        /// A mechanism for physically moving a tool from one location to another.
        /// </summary>
        public ITransferArmModel TransferArm
        {
            get => ComponentManager.GetCompositionModel<TransferArmModel>(typeof(TransferArmComposition));
            set => ComponentManager.AddCompositionModel((TransferArmModel)value);
        }

        /// <summary>
        /// A POT for a tool awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.
        /// </summary>
        public ITransferPotModel TransferPot
        {
            get => ComponentManager.GetCompositionModel<TransferPotModel>(typeof(TransferPotComposition));
            set => ComponentManager.AddCompositionModel((TransferPotModel)value);
        }

        /// <summary>
        /// A POT for a tool removed from Spindle or Turret and awaiting for return to a ToolMagazine.
        /// </summary>
        public IReturnPotModel ReturnPot
        {
            get => ComponentManager.GetCompositionModel<ReturnPotModel>(typeof(ReturnPotComposition));
            set => ComponentManager.AddCompositionModel((ReturnPotModel)value);
        }

        /// <summary>
        /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        public IMotorModel Motor
        {
            get => ComponentManager.GetCompositionModel<MotorModel>(typeof(MotorComposition));
            set => ComponentManager.AddCompositionModel((MotorModel)value);
        }


        public AutomaticToolChangerModel()
        {
            Type = AutomaticToolChangerComponent.TypeId;
        }

        public AutomaticToolChangerModel(string componentId)
        {
            Id = componentId;
            Type = AutomaticToolChangerComponent.TypeId;
        }
    }
}