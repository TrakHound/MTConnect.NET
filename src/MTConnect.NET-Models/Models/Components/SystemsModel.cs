// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Systems organizes System component types
    /// </summary>
    public class SystemsModel : ComponentModel, ISystemsModel
    {
        /// <summary>
        /// Hydraulic is a System that represents the information for a system comprised of all the parts involved in moving and distributing pressurized liquid throughout the piece of equipment.
        /// </summary>
        public IHydraulicModel Hydraulic => ComponentManager.GetComponentModel<HydraulicModel>(typeof(HydraulicComponent));

        /// <summary>
        /// Hydraulic is a System that represents the information for a system comprised of all the parts involved in moving and distributing pressurized liquid throughout the piece of equipment.
        /// </summary>
        public IPneumaticModel Pneumatic => ComponentManager.GetComponentModel<PneumaticModel>(typeof(PneumaticComponent));

        /// <summary>
        /// Coolant is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids that remove heat from a piece of equipment.
        /// </summary>
        public ICoolantModel Coolant => ComponentManager.GetComponentModel<CoolantModel>(typeof(CoolantComponent));

        /// <summary>
        /// Lubrication is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids used to lubricate portions of the piece of equipment.
        /// </summary>
        public ILubricationModel Lubrication => ComponentManager.GetComponentModel<LubricationModel>(typeof(LubricationComponent));

        /// <summary>
        /// Electric is a System that represents the information for the main power supply for device piece of equipment and the distribution of that power throughout the equipment.
        /// The electric system will provide all the data with regard to electric current, voltage, frequency, etc. that applies to the piece of equipment as a functional unit.
        /// </summary>
        public IElectricModel Electric => ComponentManager.GetComponentModel<ElectricModel>(typeof(ElectricComponent));

        /// <summary>
        /// Enclosure is a System that represents the information for a structure used to contain or isolate a piece of equipment or area. 
        /// The Enclosure system may provide information regarding access to the internal components of a piece of equipment or the conditions within the enclosure.
        /// </summary>
        public IEnclosureModel Enclosure => ComponentManager.GetComponentModel<EnclosureModel>(typeof(EnclosureComponent));

        /// <summary>
        /// Protective is a System that represents the information for those functions that detect or prevent harm or damage to equipment or personnel. 
        /// Protective does not include the information relating to the Enclosure system.
        /// </summary>
        public IProtectiveModel Protective => ComponentManager.GetComponentModel<ProtectiveModel>(typeof(ProtectiveComponent));

        /// <summary>
        /// ProcessPower is a System that represents the information for a power source associated with a piece of equipment that supplies energy to the manufacturing process 
        /// separate from the Electric system.For example, this could be the power source for an EDM machining process, an electroplating line, or a welding system.
        /// </summary>
        public IProcessPowerModel ProcessPower => ComponentManager.GetComponentModel<ProcessPowerModel>(typeof(ProcessPowerComponent));

        /// <summary>
        /// Feeder is a System that represents the information for a system that manages the delivery of materials within a piece of equipment. 
        /// For example, this could describe the wire delivery system for an EDM or welding process; 
        /// conveying system or pump and valve system distributing material to a blending station; or a fuel delivery system feeding a furnace.
        /// </summary>
        public IFeederModel Feeder => ComponentManager.GetComponentModel<FeederModel>(typeof(FeederComponent));

        /// <summary>
        /// Dielectric is a System that represents the information for a system that manages a chemical mixture used in a manufacturing
        /// process being performed at that piece of equipment.For example, this could describe
        /// the dielectric system for an EDM process or the chemical bath used in a plating process.
        /// </summary>
        public IDielectricModel Dielectric => ComponentManager.GetComponentModel<DielectricModel>(typeof(DielectricComponent));

        /// <summary>
        /// EndEffector is a System that represents the information for those functions that form the last link segment of a piece of equipment.
        /// It is the part of a piece of equipment that interacts with the manufacturing process.
        /// </summary>
        public IEndEffectorModel EndEffector => ComponentManager.GetComponentModel<EndEffectorModel>(typeof(EndEffectorComponent));

        /// <summary>
        /// WorkEnvelope is a System that organizes information about the physical process execution space within a piece of equipment. 
        /// The WorkEnvelope MAY provide information regarding the physical workspace and the conditions within that workspace.
        /// </summary>
        public IWorkEnvelopeModel WorkEnvelope => ComponentManager.GetComponentModel<WorkEnvelopeModel>(typeof(WorkEnvelopeComponent));

        /// <summary>
        /// Heating is a System used to deliver controlled amounts of heat to achieve a target temperature at a specified heating rate.
        /// </summary>
        public IHeatingModel Heating => ComponentManager.GetComponentModel<HeatingModel>(typeof(HeatingComponent));

        /// <summary>
        /// Cooling is a System used to to extract controlled amounts of heat to achieve a target temperature at a specified cooling rate.
        /// </summary>
        public ICoolingModel Cooling => ComponentManager.GetComponentModel<CoolingModel>(typeof(CoolingComponent));

        /// <summary>
        /// Pressure is a System that delivers compressed gas or fluid and controls the pressure and rate of pressure change to a desired target set-point.
        /// </summary>
        public IPressureModel Pressure => ComponentManager.GetComponentModel<PressureModel>(typeof(PressureComponent));

        /// <summary>
        /// Vacuum is a System that evacuates gases and liquids from an enclosed and sealed space to a controlled negative pressure or a molecular density below the prevailing atmospheric level.
        /// </summary>
        public IVacuumModel Vacuum => ComponentManager.GetComponentModel<VacuumModel>(typeof(VacuumComponent));
 

        public SystemsModel() 
        {
            Type = SystemsComponent.TypeId;
        }

        public SystemsModel(string componentId)
        {
            Id = componentId;
            Type = SystemsComponent.TypeId;
        }
    }
}
