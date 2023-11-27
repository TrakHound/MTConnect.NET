// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Models.Compositions;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Hydraulic is a System that represents the information for a system comprised of all the parts involved in moving and distributing pressurized liquid throughout the piece of equipment.
    /// </summary>
    public class HydraulicModel : SystemModel, IHydraulicModel
    {
        /// <summary>
        /// The indication of the status of the source of energy for a Structural Element to allow it to perform
        /// its intended function or the state of an enabling signal providing permission for the Structural Element to perform its functions.
        /// </summary>
        public PowerState PowerState
        {
            get => DataItemManager.GetDataItemValue<PowerState>(DataItem.CreateId(Id, PowerStateDataItem.NameId));
            set => DataItemManager.AddDataItem(new PowerStateDataItem(Id), value);
        }

        /// <summary>
        /// A viscous liquid.
        /// </summary>
        public IOilModel Oil
        {
            get => ComponentManager.GetCompositionModel<OilModel>(typeof(OilComposition));
            set => ComponentManager.AddCompositionModel((OilModel)value);
        }

        /// <summary>
        /// An apparatus raising, driving, exhausting, or compressing fluids or gases by means of a piston, plunger, or set of rotating vanes.
        /// </summary>
        public IPumpModel Pump
        {
            get => ComponentManager.GetCompositionModel<PumpModel>(typeof(PumpComposition));
            set => ComponentManager.AddCompositionModel((PumpModel)value);
        }

        /// <summary>
        /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        public IMotorModel Motor
        {
            get => ComponentManager.GetCompositionModel<MotorModel>(typeof(MotorComposition));
            set => ComponentManager.AddCompositionModel((MotorModel)value);
        }

        /// <summary>
        /// A receptacle or container for holding material.
        /// </summary>
        public ITankModel Tank
        {
            get => ComponentManager.GetCompositionModel<TankModel>(typeof(TankComposition));
            set => ComponentManager.AddCompositionModel((TankModel)value);
        }


        public HydraulicModel()
        {
            Type = HydraulicComponent.TypeId;
        }

        public HydraulicModel(string componentId)
        {
            Id = componentId;
            Type = HydraulicComponent.TypeId;
        }
    }
}