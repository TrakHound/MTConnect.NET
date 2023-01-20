// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Systems organizes System component types
    /// </summary>
    public interface ISystemsModel : IComponentModel
    {
        /// <summary>
        /// Hydraulic is a System that represents the information for a system comprised of all the parts involved in moving and distributing pressurized liquid throughout the piece of equipment.
        /// </summary>
        IHydraulicModel Hydraulic { get; }

        /// <summary>
        /// Hydraulic is a System that represents the information for a system comprised of all the parts involved in moving and distributing pressurized liquid throughout the piece of equipment.
        /// </summary>
        IPneumaticModel Pneumatic { get; }

        /// <summary>
        /// Coolant is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids that remove heat from a piece of equipment.
        /// </summary>
        ICoolantModel Coolant { get; }

        /// <summary>
        /// Lubrication is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids used to lubricate portions of the piece of equipment.
        /// </summary>
        ILubricationModel Lubrication { get; }

        /// <summary>
        /// Electric is a System that represents the information for the main power supply for device piece of equipment and the distribution of that power throughout the equipment.
        /// The electric system will provide all the data with regard to electric current, voltage, frequency, etc. that applies to the piece of equipment as a functional unit.
        /// </summary>
        IElectricModel Electric { get; }

        /// <summary>
        /// Enclosure is a System that represents the information for a structure used to contain or isolate a piece of equipment or area. 
        /// The Enclosure system may provide information regarding access to the internal components of a piece of equipment or the conditions within the enclosure.
        /// </summary>
        IEnclosureModel Enclosure { get; }

        /// <summary>
        /// Protective is a System that represents the information for those functions that detect or prevent harm or damage to equipment or personnel. 
        /// Protective does not include the information relating to the Enclosure system.
        /// </summary>
        IProtectiveModel Protective { get; }

        /// <summary>
        /// ProcessPower is a System that represents the information for a power source associated with a piece of equipment that supplies energy to the manufacturing process 
        /// separate from the Electric system.For example, this could be the power source for an EDM machining process, an electroplating line, or a welding system.
        /// </summary>
        IProcessPowerModel ProcessPower { get; }

        /// <summary>
        /// Feeder is a System that represents the information for a system that manages the delivery of materials within a piece of equipment. 
        /// For example, this could describe the wire delivery system for an EDM or welding process; 
        /// conveying system or pump and valve system distributing material to a blending station; or a fuel delivery system feeding a furnace.
        /// </summary>
        IFeederModel Feeder { get; }

        /// <summary>
        /// Dielectric is a System that represents the information for a system that manages a chemical mixture used in a manufacturing
        /// process being performed at that piece of equipment.For example, this could describe
        /// the dielectric system for an EDM process or the chemical bath used in a plating process.
        /// </summary>
        IDielectricModel Dielectric { get; }

        /// <summary>
        /// EndEffector is a System that represents the information for those functions that form the last link segment of a piece of equipment.
        /// It is the part of a piece of equipment that interacts with the manufacturing process.
        /// </summary>
        IEndEffectorModel EndEffector { get; }

        /// <summary>
        /// WorkEnvelope is a System that organizes information about the physical process execution space within a piece of equipment. 
        /// The WorkEnvelope MAY provide information regarding the physical workspace and the conditions within that workspace.
        /// </summary>
        IWorkEnvelopeModel WorkEnvelope { get; }

        /// <summary>
        /// Heating is a System used to deliver controlled amounts of heat to achieve a target temperature at a specified heating rate.
        /// </summary>
        IHeatingModel Heating { get; }

        /// <summary>
        /// Cooling is a System used to to extract controlled amounts of heat to achieve a target temperature at a specified cooling rate.
        /// </summary>
        ICoolingModel Cooling { get; }

        /// <summary>
        /// Pressure is a System that delivers compressed gas or fluid and controls the pressure and rate of pressure change to a desired target set-point.
        /// </summary>
        IPressureModel Pressure { get; }

        /// <summary>
        /// Vacuum is a System that evacuates gases and liquids from an enclosed and sealed space to a controlled negative pressure or a molecular density below the prevailing atmospheric level.
        /// </summary>
        IVacuumModel Vacuum { get; }
    }
}