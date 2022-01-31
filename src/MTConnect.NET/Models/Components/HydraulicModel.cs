// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.Events;
using MTConnect.Models.Compositions;
using MTConnect.Streams.Events;

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
            get => GetDataItemValue<PowerState>(DataItem.CreateId(Id, PowerStateDataItem.NameId));
            set => AddDataItem(new PowerStateDataItem(Id), value);
        }

        /// <summary>
        /// A viscous liquid.
        /// </summary>
        public IOilModel Oil
        {
            get => GetCompositionModel<OilModel>(typeof(OilComposition));
            set => AddCompositionModel((OilModel)value);
        }

        /// <summary>
        /// An apparatus raising, driving, exhausting, or compressing fluids or gases by means of a piston, plunger, or set of rotating vanes.
        /// </summary>
        public IPumpModel Pump
        {
            get => GetCompositionModel<PumpModel>(typeof(PumpComposition));
            set => AddCompositionModel((PumpModel)value);
        }

        /// <summary>
        /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        public IMotorModel Motor
        {
            get => GetCompositionModel<MotorModel>(typeof(MotorComposition));
            set => AddCompositionModel((MotorModel)value);
        }

        /// <summary>
        /// A receptacle or container for holding material.
        /// </summary>
        public ITankModel Tank
        {
            get => GetCompositionModel<TankModel>(typeof(TankComposition));
            set => AddCompositionModel((TankModel)value);
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
