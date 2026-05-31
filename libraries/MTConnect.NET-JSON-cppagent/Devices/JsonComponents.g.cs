// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    /// <summary>
    /// cppagent-style JSON representation of a device's component tree. Each MTConnect
    /// component type is exposed as its own strongly-typed collection so the
    /// System.Text.Json serializer emits one property per type, matching the
    /// flattened object shape the C++ reference agent produces.
    /// </summary>
    public class JsonComponents
    {
        /// <summary>
        /// The set of <c>Actuator</c> components on the device.
        /// Component composed of a physical apparatus that moves or controls a mechanism or system.
        /// </summary>
        [JsonPropertyName("Actuator")]
        public IEnumerable<JsonComponent> Actuator { get; set; }


        /// <summary>
        /// The set of <c>Adapter</c> components on the device.
        /// Component that provides information about the data source for an MTConnect Agent.
        /// </summary>
        [JsonPropertyName("Adapter")]
        public IEnumerable<JsonComponent> Adapter { get; set; }


        /// <summary>
        /// The set of <c>Adapters</c> components on the device.
        /// Component that organize Adapter types.
        /// </summary>
        [JsonPropertyName("Adapters")]
        public IEnumerable<JsonComponent> Adapters { get; set; }


        /// <summary>
        /// The set of <c>AirHandler</c> components on the device.
        /// System that circulates air or regulates airflow without altering temperature or humidity.
        /// </summary>
        [JsonPropertyName("AirHandler")]
        public IEnumerable<JsonComponent> AirHandler { get; set; }


        /// <summary>
        /// The set of <c>Amplifier</c> components on the device.
        /// Leaf Component composed of an electronic component or circuit that amplifies power, electric current, or voltage.
        /// </summary>
        [JsonPropertyName("Amplifier")]
        public IEnumerable<JsonComponent> Amplifier { get; set; }


        /// <summary>
        /// The set of <c>AutomaticToolChanger</c> components on the device.
        /// ToolingDelivery composed of a tool delivery mechanism that moves tools between a ToolMagazine and a spindle a Turret.
        /// </summary>
        [JsonPropertyName("AutomaticToolChanger")]
        public IEnumerable<JsonComponent> AutomaticToolChanger { get; set; }


        /// <summary>
        /// The set of <c>Auxiliaries</c> components on the device.
        /// Component that organize Auxiliary types.
        /// </summary>
        [JsonPropertyName("Auxiliaries")]
        public IEnumerable<JsonComponent> Auxiliaries { get; set; }


        /// <summary>
        /// The set of <c>Auxiliary</c> components on the device.
        /// Abstract Component composed of removable part(s) of a piece of equipment that provides supplementary or extended functionality.
        /// </summary>
        [JsonPropertyName("Auxiliary")]
        public IEnumerable<JsonComponent> Auxiliary { get; set; }


        /// <summary>
        /// The set of <c>Axes</c> components on the device.
        /// Axis types.
        /// </summary>
        [JsonPropertyName("Axes")]
        public IEnumerable<JsonComponent> Axes { get; set; }


        /// <summary>
        /// The set of <c>Axis</c> components on the device.
        /// Abstract Component composed of a motion system that provides linear or rotational motion for a piece of equipment.
        /// </summary>
        [JsonPropertyName("Axis")]
        public IEnumerable<JsonComponent> Axis { get; set; }


        /// <summary>
        /// The set of <c>Ballscrew</c> components on the device.
        /// Leaf Component composed of a mechanical structure that transforms rotary motion into linear motion.
        /// </summary>
        [JsonPropertyName("Ballscrew")]
        public IEnumerable<JsonComponent> Ballscrew { get; set; }


        /// <summary>
        /// The set of <c>BarFeeder</c> components on the device.
        /// Loader that delivers bar stock to a piece of equipment.
        /// </summary>
        [JsonPropertyName("BarFeeder")]
        public IEnumerable<JsonComponent> BarFeeder { get; set; }


        /// <summary>
        /// The set of <c>Belt</c> components on the device.
        /// Leaf Component composed of an endless flexible band that transmits motion for a piece of equipment or conveys materials and objects.
        /// </summary>
        [JsonPropertyName("Belt")]
        public IEnumerable<JsonComponent> Belt { get; set; }


        /// <summary>
        /// The set of <c>Brake</c> components on the device.
        /// Leaf Component that slows or stops a moving object by the absorption or transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.
        /// </summary>
        [JsonPropertyName("Brake")]
        public IEnumerable<JsonComponent> Brake { get; set; }


        /// <summary>
        /// The set of <c>Chain</c> components on the device.
        /// Leaf Component composed of interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.
        /// </summary>
        [JsonPropertyName("Chain")]
        public IEnumerable<JsonComponent> Chain { get; set; }


        /// <summary>
        /// The set of <c>Chopper</c> components on the device.
        /// Leaf Component that breaks material into smaller pieces.
        /// </summary>
        [JsonPropertyName("Chopper")]
        public IEnumerable<JsonComponent> Chopper { get; set; }


        /// <summary>
        /// The set of <c>Chuck</c> components on the device.
        /// Leaf Component composed of a mechanism that holds a part or stock material in place.
        /// </summary>
        [JsonPropertyName("Chuck")]
        public IEnumerable<JsonComponent> Chuck { get; set; }


        /// <summary>
        /// The set of <c>Chute</c> components on the device.
        /// Leaf Component composed of an inclined channel that conveys material.
        /// </summary>
        [JsonPropertyName("Chute")]
        public IEnumerable<JsonComponent> Chute { get; set; }


        /// <summary>
        /// The set of <c>CircuitBreaker</c> components on the device.
        /// Leaf Component that interrupts an electric circuit.
        /// </summary>
        [JsonPropertyName("CircuitBreaker")]
        public IEnumerable<JsonComponent> CircuitBreaker { get; set; }


        /// <summary>
        /// The set of <c>Clamp</c> components on the device.
        /// Leaf Component that strengthens, support, or fastens objects in place.
        /// </summary>
        [JsonPropertyName("Clamp")]
        public IEnumerable<JsonComponent> Clamp { get; set; }


        /// <summary>
        /// The set of <c>Compressor</c> components on the device.
        /// Leaf Component composed of a pump or other mechanism that reduces volume and increases pressure of gases in order to condense the gases to drive pneumatically powered pieces of equipment.
        /// </summary>
        [JsonPropertyName("Compressor")]
        public IEnumerable<JsonComponent> Compressor { get; set; }


        /// <summary>
        /// The set of <c>Controller</c> components on the device.
        /// System that provides regulation or management of a system or component. ISO 16484-5:2017
        /// </summary>
        [JsonPropertyName("Controller")]
        public IEnumerable<JsonComponent> Controller { get; set; }


        /// <summary>
        /// The set of <c>Controllers</c> components on the device.
        /// Component that organize Controller entities.
        /// </summary>
        [JsonPropertyName("Controllers")]
        public IEnumerable<JsonComponent> Controllers { get; set; }


        /// <summary>
        /// The set of <c>Coolant</c> components on the device.
        /// System that provides distribution and management of fluids that remove heat from a piece of equipment.
        /// </summary>
        [JsonPropertyName("Coolant")]
        public IEnumerable<JsonComponent> Coolant { get; set; }


        /// <summary>
        /// The set of <c>Cooling</c> components on the device.
        /// System that extracts controlled amounts of heat to achieve a target temperature at a specified cooling rate.
        /// </summary>
        [JsonPropertyName("Cooling")]
        public IEnumerable<JsonComponent> Cooling { get; set; }


        /// <summary>
        /// The set of <c>CoolingTower</c> components on the device.
        /// Leaf Component composed of a heat exchange system that uses a fluid to transfer heat to the atmosphere.
        /// </summary>
        [JsonPropertyName("CoolingTower")]
        public IEnumerable<JsonComponent> CoolingTower { get; set; }


        /// <summary>
        /// The set of <c>CuttingTorch</c> components on the device.
        /// Auxiliary that employs a concentrated flame to both sever materials through cutting and fuse them together in joining processes.
        /// </summary>
        [JsonPropertyName("CuttingTorch")]
        public IEnumerable<JsonComponent> CuttingTorch { get; set; }


        /// <summary>
        /// The set of <c>Deposition</c> components on the device.
        /// Auxiliary that manages the addition of material or state change of material being performed in an additive manufacturing process.
        /// </summary>
        [JsonPropertyName("Deposition")]
        public IEnumerable<JsonComponent> Deposition { get; set; }


        /// <summary>
        /// The set of <c>Dielectric</c> components on the device.
        /// System that manages a chemical mixture used in a manufacturing process being performed at that piece of equipment.
        /// </summary>
        [JsonPropertyName("Dielectric")]
        public IEnumerable<JsonComponent> Dielectric { get; set; }


        /// <summary>
        /// The set of <c>Door</c> components on the device.
        /// Component composed of a mechanical mechanism or closure that can cover a physical access portal into a piece of equipment allowing or restricting access to other parts of the equipment.
        /// </summary>
        [JsonPropertyName("Door")]
        public IEnumerable<JsonComponent> Door { get; set; }


        /// <summary>
        /// The set of <c>Drain</c> components on the device.
        /// Leaf Component that allows material to flow for the purpose of drainage from, for example, a vessel or tank.
        /// </summary>
        [JsonPropertyName("Drain")]
        public IEnumerable<JsonComponent> Drain { get; set; }


        /// <summary>
        /// The set of <c>Electric</c> components on the device.
        /// System composed of the main power supply for the piece of equipment that provides distribution of that power throughout the equipment.
        /// </summary>
        [JsonPropertyName("Electric")]
        public IEnumerable<JsonComponent> Electric { get; set; }


        /// <summary>
        /// The set of <c>Electrode</c> components on the device.
        /// Auxiliary that is used for many electrical discharge manufacturing processes like welding.
        /// </summary>
        [JsonPropertyName("Electrode")]
        public IEnumerable<JsonComponent> Electrode { get; set; }


        /// <summary>
        /// The set of <c>Enclosure</c> components on the device.
        /// System composed of a structure that is used to contain or isolate a piece of equipment or area.
        /// </summary>
        [JsonPropertyName("Enclosure")]
        public IEnumerable<JsonComponent> Enclosure { get; set; }


        /// <summary>
        /// The set of <c>Encoder</c> components on the device.
        /// Leaf Component that measures position.
        /// </summary>
        [JsonPropertyName("Encoder")]
        public IEnumerable<JsonComponent> Encoder { get; set; }


        /// <summary>
        /// The set of <c>EndEffector</c> components on the device.
        /// System composed of functions that form the last link segment of a piece of equipment.
        /// </summary>
        [JsonPropertyName("EndEffector")]
        public IEnumerable<JsonComponent> EndEffector { get; set; }


        /// <summary>
        /// The set of <c>Environmental</c> components on the device.
        /// Component that observes the surroundings of another Component.> Note: Environmental **SHOULD** be organized by Auxillaries, Systems or Parts depending on the relationship to the Component.
        /// </summary>
        [JsonPropertyName("Environmental")]
        public IEnumerable<JsonComponent> Environmental { get; set; }


        /// <summary>
        /// The set of <c>ExpiredPot</c> components on the device.
        /// Leaf Component that is a Pot for a tool that is no longer usable for removal from a ToolMagazine or Turret.
        /// </summary>
        [JsonPropertyName("ExpiredPot")]
        public IEnumerable<JsonComponent> ExpiredPot { get; set; }


        /// <summary>
        /// The set of <c>ExposureUnit</c> components on the device.
        /// Leaf Component that emits a type of radiation.
        /// </summary>
        [JsonPropertyName("ExposureUnit")]
        public IEnumerable<JsonComponent> ExposureUnit { get; set; }


        /// <summary>
        /// The set of <c>ExtrusionUnit</c> components on the device.
        /// Leaf Component that dispenses liquid or powered materials.
        /// </summary>
        [JsonPropertyName("ExtrusionUnit")]
        public IEnumerable<JsonComponent> ExtrusionUnit { get; set; }


        /// <summary>
        /// The set of <c>Fan</c> components on the device.
        /// Leaf Component that produces a current of air.
        /// </summary>
        [JsonPropertyName("Fan")]
        public IEnumerable<JsonComponent> Fan { get; set; }


        /// <summary>
        /// The set of <c>FeatureOccurrence</c> components on the device.
        /// Component that provides information related to an individual feature.
        /// </summary>
        [JsonPropertyName("FeatureOccurrence")]
        public IEnumerable<JsonComponent> FeatureOccurrence { get; set; }


        /// <summary>
        /// The set of <c>Feeder</c> components on the device.
        /// System that manages the delivery of materials within a piece of equipment.
        /// </summary>
        [JsonPropertyName("Feeder")]
        public IEnumerable<JsonComponent> Feeder { get; set; }


        /// <summary>
        /// The set of <c>Filter</c> components on the device.
        /// Leaf Component through which liquids or gases are passed to remove suspended impurities or to recover solids.
        /// </summary>
        [JsonPropertyName("Filter")]
        public IEnumerable<JsonComponent> Filter { get; set; }


        /// <summary>
        /// The set of <c>Galvanomotor</c> components on the device.
        /// Leaf Component composed of an electromechanical actuator that produces deflection of a beam of light or energy in response to electric current through its coil in a magnetic field.
        /// </summary>
        [JsonPropertyName("Galvanomotor")]
        public IEnumerable<JsonComponent> Galvanomotor { get; set; }


        /// <summary>
        /// The set of <c>GangToolBar</c> components on the device.
        /// ToolingDelivery composed of a tool mounting mechanism that holds any number of tools.
        /// </summary>
        [JsonPropertyName("GangToolBar")]
        public IEnumerable<JsonComponent> GangToolBar { get; set; }


        /// <summary>
        /// The set of <c>Gripper</c> components on the device.
        /// Leaf Component that holds a part, stock material, or any other item in place.
        /// </summary>
        [JsonPropertyName("Gripper")]
        public IEnumerable<JsonComponent> Gripper { get; set; }


        /// <summary>
        /// The set of <c>Heating</c> components on the device.
        /// System that delivers controlled amounts of heat to achieve a target temperature at a specified heating rate.
        /// </summary>
        [JsonPropertyName("Heating")]
        public IEnumerable<JsonComponent> Heating { get; set; }


        /// <summary>
        /// The set of <c>Hopper</c> components on the device.
        /// Leaf Component composed of a chamber or bin in which materials are stored temporarily, typically being filled through the top and dispensed through the bottom.
        /// </summary>
        [JsonPropertyName("Hopper")]
        public IEnumerable<JsonComponent> Hopper { get; set; }


        /// <summary>
        /// The set of <c>Hydraulic</c> components on the device.
        /// System that provides movement and distribution of pressurized liquid throughout the piece of equipment.
        /// </summary>
        [JsonPropertyName("Hydraulic")]
        public IEnumerable<JsonComponent> Hydraulic { get; set; }


        /// <summary>
        /// The set of <c>Interfaces</c> components on the device.
        /// Component that organize Interface types.
        /// </summary>
        [JsonPropertyName("Interfaces")]
        public IEnumerable<JsonComponent> Interfaces { get; set; }


        /// <summary>
        /// The set of <c>Linear</c> components on the device.
        /// Axis that provides prismatic motion along a fixed axis.
        /// </summary>
        [JsonPropertyName("Linear")]
        public IEnumerable<JsonComponent> Linear { get; set; }


        /// <summary>
        /// The set of <c>LinearPositionFeedback</c> components on the device.
        /// Leaf Component that measures linear motion or position.**DEPRECATION WARNING** : May be deprecated in the future. Recommend using Encoder.
        /// </summary>
        [JsonPropertyName("LinearPositionFeedback")]
        public IEnumerable<JsonComponent> LinearPositionFeedback { get; set; }


        /// <summary>
        /// The set of <c>Link</c> components on the device.
        /// Structure that provides a connection between Component entities.
        /// </summary>
        [JsonPropertyName("Link")]
        public IEnumerable<JsonComponent> Link { get; set; }


        /// <summary>
        /// The set of <c>Loader</c> components on the device.
        /// Auxiliary that provides movement and distribution of materials, parts, tooling, and other items to or from a piece of equipment.
        /// </summary>
        [JsonPropertyName("Loader")]
        public IEnumerable<JsonComponent> Loader { get; set; }


        /// <summary>
        /// The set of <c>Lock</c> components on the device.
        /// Component that physically prohibits a Device or Component from opening or operating.
        /// </summary>
        [JsonPropertyName("Lock")]
        public IEnumerable<JsonComponent> Lock { get; set; }


        /// <summary>
        /// The set of <c>Lubrication</c> components on the device.
        /// System that provides distribution and management of fluids used to lubricate portions of the piece of equipment.
        /// </summary>
        [JsonPropertyName("Lubrication")]
        public IEnumerable<JsonComponent> Lubrication { get; set; }


        /// <summary>
        /// The set of <c>Material</c> components on the device.
        /// Resource composed of material that is consumed or used by the piece of equipment for production of parts, materials, or other types of goods.
        /// </summary>
        [JsonPropertyName("Material")]
        public IEnumerable<JsonComponent> Material { get; set; }


        /// <summary>
        /// The set of <c>Materials</c> components on the device.
        /// Resources that organize Material types.
        /// </summary>
        [JsonPropertyName("Materials")]
        public IEnumerable<JsonComponent> Materials { get; set; }


        /// <summary>
        /// The set of <c>Motor</c> components on the device.
        /// Leaf Component that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        [JsonPropertyName("Motor")]
        public IEnumerable<JsonComponent> Motor { get; set; }


        /// <summary>
        /// The set of <c>Oil</c> components on the device.
        /// Leaf Component composed of a viscous liquid.
        /// </summary>
        [JsonPropertyName("Oil")]
        public IEnumerable<JsonComponent> Oil { get; set; }


        /// <summary>
        /// The set of <c>Part</c> components on the device.
        /// Abstract Component composed of a part being processed by a piece of equipment.
        /// </summary>
        [JsonPropertyName("Part")]
        public IEnumerable<JsonComponent> Part { get; set; }


        /// <summary>
        /// The set of <c>PartOccurrence</c> components on the device.
        /// Part that exists at a specific place and time, such as a specific instance of a bracket at a specific timestamp.
        /// </summary>
        [JsonPropertyName("PartOccurrence")]
        public IEnumerable<JsonComponent> PartOccurrence { get; set; }


        /// <summary>
        /// The set of <c>Parts</c> components on the device.
        /// Component that organize Part types.
        /// </summary>
        [JsonPropertyName("Parts")]
        public IEnumerable<JsonComponent> Parts { get; set; }


        /// <summary>
        /// The set of <c>Path</c> components on the device.
        /// Component that organizes an independent operation or function within a Controller.
        /// </summary>
        [JsonPropertyName("Path")]
        public IEnumerable<JsonComponent> Path { get; set; }


        /// <summary>
        /// The set of <c>Personnel</c> components on the device.
        /// Resource composed of an individual or individuals who either control, support, or otherwise interface with a piece of equipment.
        /// </summary>
        [JsonPropertyName("Personnel")]
        public IEnumerable<JsonComponent> Personnel { get; set; }


        /// <summary>
        /// The set of <c>PinTool</c> components on the device.
        /// System composed of a tool that performs the work for a Friction Stir Welding process
        /// </summary>
        [JsonPropertyName("PinTool")]
        public IEnumerable<JsonComponent> PinTool { get; set; }


        /// <summary>
        /// The set of <c>Pneumatic</c> components on the device.
        /// System that uses compressed gasses to actuate components or do work within the piece of equipment.
        /// </summary>
        [JsonPropertyName("Pneumatic")]
        public IEnumerable<JsonComponent> Pneumatic { get; set; }


        /// <summary>
        /// The set of <c>Pot</c> components on the device.
        /// Leaf Component composed of a tool storage location associated with a ToolMagazine or AutomaticToolChanger.
        /// </summary>
        [JsonPropertyName("Pot")]
        public IEnumerable<JsonComponent> Pot { get; set; }


        /// <summary>
        /// The set of <c>Power</c> components on the device.
        /// Power was **DEPRECATED** in *MTConnect Version 1.1* and was replaced by Availability data item type.
        /// </summary>
        [JsonPropertyName("Power")]
        public IEnumerable<JsonComponent> Power { get; set; }


        /// <summary>
        /// The set of <c>PowerSupply</c> components on the device.
        /// Leaf Component that provides power to electric mechanisms.
        /// </summary>
        [JsonPropertyName("PowerSupply")]
        public IEnumerable<JsonComponent> PowerSupply { get; set; }


        /// <summary>
        /// The set of <c>Pressure</c> components on the device.
        /// System that delivers compressed gas or fluid and controls the pressure and rate of pressure change to a desired target set-point.
        /// </summary>
        [JsonPropertyName("Pressure")]
        public IEnumerable<JsonComponent> Pressure { get; set; }


        /// <summary>
        /// The set of <c>Process</c> components on the device.
        /// Abstract Component composed of a manufacturing process being executed on a piece of equipment.
        /// </summary>
        [JsonPropertyName("Process")]
        public IEnumerable<JsonComponent> Process { get; set; }


        /// <summary>
        /// The set of <c>Processes</c> components on the device.
        /// Component that organize Process types.
        /// </summary>
        [JsonPropertyName("Processes")]
        public IEnumerable<JsonComponent> Processes { get; set; }


        /// <summary>
        /// The set of <c>ProcessOccurrence</c> components on the device.
        /// Process that takes place at a specific place and time, such as a specific instance of part-milling occurring at a specific timestamp.
        /// </summary>
        [JsonPropertyName("ProcessOccurrence")]
        public IEnumerable<JsonComponent> ProcessOccurrence { get; set; }


        /// <summary>
        /// The set of <c>ProcessPower</c> components on the device.
        /// System composed of a power source associated with a piece of equipment that supplies energy to the manufacturing process separate from the Electric system.
        /// </summary>
        [JsonPropertyName("ProcessPower")]
        public IEnumerable<JsonComponent> ProcessPower { get; set; }


        /// <summary>
        /// The set of <c>Protective</c> components on the device.
        /// System that provides functions used to detect or prevent harm or damage to equipment or personnel.
        /// </summary>
        [JsonPropertyName("Protective")]
        public IEnumerable<JsonComponent> Protective { get; set; }


        /// <summary>
        /// The set of <c>Pulley</c> components on the device.
        /// Leaf Component composed of a mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.
        /// </summary>
        [JsonPropertyName("Pulley")]
        public IEnumerable<JsonComponent> Pulley { get; set; }


        /// <summary>
        /// The set of <c>Pump</c> components on the device.
        /// Leaf Component that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.
        /// </summary>
        [JsonPropertyName("Pump")]
        public IEnumerable<JsonComponent> Pump { get; set; }


        /// <summary>
        /// The set of <c>Reel</c> components on the device.
        /// Leaf Component composed of a rotary storage unit for material.
        /// </summary>
        [JsonPropertyName("Reel")]
        public IEnumerable<JsonComponent> Reel { get; set; }


        /// <summary>
        /// The set of <c>RemovalPot</c> components on the device.
        /// Leaf Component that is a Pot for a tool that has to be removed from a ToolMagazine or Turret to a location outside of the piece of equipment.
        /// </summary>
        [JsonPropertyName("RemovalPot")]
        public IEnumerable<JsonComponent> RemovalPot { get; set; }


        /// <summary>
        /// The set of <c>Resource</c> components on the device.
        /// Abstract Component composed of material or personnel involved in a manufacturing process.
        /// </summary>
        [JsonPropertyName("Resource")]
        public IEnumerable<JsonComponent> Resource { get; set; }


        /// <summary>
        /// The set of <c>Resources</c> components on the device.
        /// Component that organize Resource types.
        /// </summary>
        [JsonPropertyName("Resources")]
        public IEnumerable<JsonComponent> Resources { get; set; }


        /// <summary>
        /// The set of <c>ReturnPot</c> components on the device.
        /// Leaf Component that is a Pot for a tool that has been removed from spindle or Turret and awaiting for return to a ToolMagazine.
        /// </summary>
        [JsonPropertyName("ReturnPot")]
        public IEnumerable<JsonComponent> ReturnPot { get; set; }


        /// <summary>
        /// The set of <c>Rotary</c> components on the device.
        /// Axis that provides rotation about a fixed axis.
        /// </summary>
        [JsonPropertyName("Rotary")]
        public IEnumerable<JsonComponent> Rotary { get; set; }


        /// <summary>
        /// The set of <c>SensingElement</c> components on the device.
        /// Leaf Component that provides a signal or measured value.
        /// </summary>
        [JsonPropertyName("SensingElement")]
        public IEnumerable<JsonComponent> SensingElement { get; set; }


        /// <summary>
        /// The set of <c>Sensor</c> components on the device.
        /// Component that responds to a physical stimulus and transmits a resulting impulse or value from a sensing unit.
        /// </summary>
        [JsonPropertyName("Sensor")]
        public IEnumerable<JsonComponent> Sensor { get; set; }


        /// <summary>
        /// The set of <c>Spindle</c> components on the device.
        /// Component that provides an axis of rotation for the purpose of rapidly rotating a part or a tool to provide sufficient surface speed for cutting operations.Spindle was **DEPRECATED** in *MTConnect Version 1.1* and was replaced by RotaryMode.
        /// </summary>
        [JsonPropertyName("Spindle")]
        public IEnumerable<JsonComponent> Spindle { get; set; }


        /// <summary>
        /// The set of <c>Spreader</c> components on the device.
        /// Leaf Component that flattens or spreading materials.
        /// </summary>
        [JsonPropertyName("Spreader")]
        public IEnumerable<JsonComponent> Spreader { get; set; }


        /// <summary>
        /// The set of <c>StagingPot</c> components on the device.
        /// Leaf Component that is a Pot for a tool that is awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.
        /// </summary>
        [JsonPropertyName("StagingPot")]
        public IEnumerable<JsonComponent> StagingPot { get; set; }


        /// <summary>
        /// The set of <c>Station</c> components on the device.
        /// Leaf Component composed of a storage or mounting location for a tool associated with a Turret, GangToolBar, or ToolRack.
        /// </summary>
        [JsonPropertyName("Station")]
        public IEnumerable<JsonComponent> Station { get; set; }


        /// <summary>
        /// The set of <c>Stock</c> components on the device.
        /// Material that is used in a manufacturing process and to which work is applied in a machine or piece of equipment to produce parts.
        /// </summary>
        [JsonPropertyName("Stock")]
        public IEnumerable<JsonComponent> Stock { get; set; }


        /// <summary>
        /// The set of <c>StorageBattery</c> components on the device.
        /// Leaf Component composed of one or more cells in which chemical energy is converted into electricity and used as a source of power.
        /// </summary>
        [JsonPropertyName("StorageBattery")]
        public IEnumerable<JsonComponent> StorageBattery { get; set; }


        /// <summary>
        /// The set of <c>Structure</c> components on the device.
        /// Component composed of part(s) comprising the rigid bodies of the piece of equipment.
        /// </summary>
        [JsonPropertyName("Structure")]
        public IEnumerable<JsonComponent> Structure { get; set; }


        /// <summary>
        /// The set of <c>Structures</c> components on the device.
        /// Component that organize Structure types.
        /// </summary>
        [JsonPropertyName("Structures")]
        public IEnumerable<JsonComponent> Structures { get; set; }


        /// <summary>
        /// The set of <c>Switch</c> components on the device.
        /// Leaf Component that turns on or off an electric current or makes or breaks a circuit.
        /// </summary>
        [JsonPropertyName("Switch")]
        public IEnumerable<JsonComponent> Switch { get; set; }


        /// <summary>
        /// The set of <c>System</c> components on the device.
        /// Abstract Component that is permanently integrated into the piece of equipment.
        /// </summary>
        [JsonPropertyName("System")]
        public IEnumerable<JsonComponent> System { get; set; }


        /// <summary>
        /// The set of <c>Systems</c> components on the device.
        /// Component that organize System types.
        /// </summary>
        [JsonPropertyName("Systems")]
        public IEnumerable<JsonComponent> Systems { get; set; }


        /// <summary>
        /// The set of <c>Table</c> components on the device.
        /// Leaf Component composed of a surface for holding an object or material.
        /// </summary>
        [JsonPropertyName("Table")]
        public IEnumerable<JsonComponent> Table { get; set; }


        /// <summary>
        /// The set of <c>Tank</c> components on the device.
        /// Leaf Component generally composed of an enclosed container.
        /// </summary>
        [JsonPropertyName("Tank")]
        public IEnumerable<JsonComponent> Tank { get; set; }


        /// <summary>
        /// The set of <c>Tensioner</c> components on the device.
        /// Leaf Component that provides or applies a stretch or strain to another mechanism.
        /// </summary>
        [JsonPropertyName("Tensioner")]
        public IEnumerable<JsonComponent> Tensioner { get; set; }


        /// <summary>
        /// The set of <c>Thermostat</c> components on the device.
        /// Component composed of a sensor or an instrument that measures temperature.Thermostat was **DEPRECATED** in *MTConnect Version 1.2* and was replaced by Temperature.
        /// </summary>
        [JsonPropertyName("Thermostat")]
        public IEnumerable<JsonComponent> Thermostat { get; set; }


        /// <summary>
        /// The set of <c>ToolHolder</c> components on the device.
        /// System that securely interfaces a Component with a Device
        /// </summary>
        [JsonPropertyName("ToolHolder")]
        public IEnumerable<JsonComponent> ToolHolder { get; set; }


        /// <summary>
        /// The set of <c>ToolingDelivery</c> components on the device.
        /// Auxiliary that manages, positions, stores, and delivers tooling within a piece of equipment.
        /// </summary>
        [JsonPropertyName("ToolingDelivery")]
        public IEnumerable<JsonComponent> ToolingDelivery { get; set; }


        /// <summary>
        /// The set of <c>ToolMagazine</c> components on the device.
        /// ToolingDelivery composed of a tool storage mechanism that holds any number of tools.
        /// </summary>
        [JsonPropertyName("ToolMagazine")]
        public IEnumerable<JsonComponent> ToolMagazine { get; set; }


        /// <summary>
        /// The set of <c>ToolRack</c> components on the device.
        /// ToolingDelivery composed of a linear or matrixed tool storage mechanism that holds any number of tools.
        /// </summary>
        [JsonPropertyName("ToolRack")]
        public IEnumerable<JsonComponent> ToolRack { get; set; }


        /// <summary>
        /// The set of <c>TransferArm</c> components on the device.
        /// Leaf Component that physically moves a tool from one location to another.
        /// </summary>
        [JsonPropertyName("TransferArm")]
        public IEnumerable<JsonComponent> TransferArm { get; set; }


        /// <summary>
        /// The set of <c>TransferPot</c> components on the device.
        /// Leaf Component that is a Pot for a tool that is awaiting transfer from a ToolMagazine to spindle or Turret.
        /// </summary>
        [JsonPropertyName("TransferPot")]
        public IEnumerable<JsonComponent> TransferPot { get; set; }


        /// <summary>
        /// The set of <c>Transformer</c> components on the device.
        /// Leaf Component that transforms electric energy from a source to a secondary circuit.
        /// </summary>
        [JsonPropertyName("Transformer")]
        public IEnumerable<JsonComponent> Transformer { get; set; }


        /// <summary>
        /// The set of <c>Turret</c> components on the device.
        /// ToolingDelivery composed of a tool mounting mechanism that holds any number of tools.
        /// </summary>
        [JsonPropertyName("Turret")]
        public IEnumerable<JsonComponent> Turret { get; set; }


        /// <summary>
        /// The set of <c>Vacuum</c> components on the device.
        /// System that evacuates gases and liquids from an enclosed and sealed space to a controlled negative pressure or a molecular density below the prevailing atmospheric level.
        /// </summary>
        [JsonPropertyName("Vacuum")]
        public IEnumerable<JsonComponent> Vacuum { get; set; }


        /// <summary>
        /// The set of <c>Valve</c> components on the device.
        /// Leaf Component that halts or controls the flow of a liquid, gas, or other material through a passage, pipe, inlet, or outlet.
        /// </summary>
        [JsonPropertyName("Valve")]
        public IEnumerable<JsonComponent> Valve { get; set; }


        /// <summary>
        /// The set of <c>Vat</c> components on the device.
        /// Leaf Component generally composed of an open container.
        /// </summary>
        [JsonPropertyName("Vat")]
        public IEnumerable<JsonComponent> Vat { get; set; }


        /// <summary>
        /// The set of <c>Vibration</c> components on the device.
        /// Component composed of a sensor or an instrument that measures the amount and/or frequency of vibration within a system.Vibration was **DEPRECATED** in *MTConnect Version 1.2* and was replaced by Displacement, Frequency etc.
        /// </summary>
        [JsonPropertyName("Vibration")]
        public IEnumerable<JsonComponent> Vibration { get; set; }


        /// <summary>
        /// The set of <c>WasteDisposal</c> components on the device.
        /// Auxiliary that removes manufacturing byproducts from a piece of equipment.
        /// </summary>
        [JsonPropertyName("WasteDisposal")]
        public IEnumerable<JsonComponent> WasteDisposal { get; set; }


        /// <summary>
        /// The set of <c>Water</c> components on the device.
        /// Leaf Component composed of $$H_2 O$$.
        /// </summary>
        [JsonPropertyName("Water")]
        public IEnumerable<JsonComponent> Water { get; set; }


        /// <summary>
        /// The set of <c>Wire</c> components on the device.
        /// Leaf Component composed of a string like piece or filament of relatively rigid or flexible material provided in a variety of diameters.
        /// </summary>
        [JsonPropertyName("Wire")]
        public IEnumerable<JsonComponent> Wire { get; set; }


        /// <summary>
        /// The set of <c>WorkEnvelope</c> components on the device.
        /// System composed of the physical process execution space within a piece of equipment.
        /// </summary>
        [JsonPropertyName("WorkEnvelope")]
        public IEnumerable<JsonComponent> WorkEnvelope { get; set; }


        /// <summary>
        /// The set of <c>Workpiece</c> components on the device.
        /// Leaf Component composed of an object or material on which a form of work is performed.
        /// </summary>
        [JsonPropertyName("Workpiece")]
        public IEnumerable<JsonComponent> Workpiece { get; set; }



        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonComponents() { }

        /// <summary>
        /// Initializes a new instance from a flat sequence of <paramref name="components"/>,
        /// bucketing each one into the typed collection that matches its MTConnect type.
        /// </summary>
        public JsonComponents(IEnumerable<IComponent> components)
        {
            if (!components.IsNullOrEmpty())
            {
                Actuator = GetComponents(components, ActuatorComponent.TypeId);

                Adapter = GetComponents(components, AdapterComponent.TypeId);

                Adapters = GetComponents(components, AdaptersComponent.TypeId);

                AirHandler = GetComponents(components, AirHandlerComponent.TypeId);

                Amplifier = GetComponents(components, AmplifierComponent.TypeId);

                AutomaticToolChanger = GetComponents(components, AutomaticToolChangerComponent.TypeId);

                Auxiliaries = GetComponents(components, AuxiliariesComponent.TypeId);

                Auxiliary = GetComponents(components, AuxiliaryComponent.TypeId);

                Axes = GetComponents(components, AxesComponent.TypeId);

                Axis = GetComponents(components, AxisComponent.TypeId);

                Ballscrew = GetComponents(components, BallscrewComponent.TypeId);

                BarFeeder = GetComponents(components, BarFeederComponent.TypeId);

                Belt = GetComponents(components, BeltComponent.TypeId);

                Brake = GetComponents(components, BrakeComponent.TypeId);

                Chain = GetComponents(components, ChainComponent.TypeId);

                Chopper = GetComponents(components, ChopperComponent.TypeId);

                Chuck = GetComponents(components, ChuckComponent.TypeId);

                Chute = GetComponents(components, ChuteComponent.TypeId);

                CircuitBreaker = GetComponents(components, CircuitBreakerComponent.TypeId);

                Clamp = GetComponents(components, ClampComponent.TypeId);

                Compressor = GetComponents(components, CompressorComponent.TypeId);

                Controller = GetComponents(components, ControllerComponent.TypeId);

                Controllers = GetComponents(components, ControllersComponent.TypeId);

                Coolant = GetComponents(components, CoolantComponent.TypeId);

                Cooling = GetComponents(components, CoolingComponent.TypeId);

                CoolingTower = GetComponents(components, CoolingTowerComponent.TypeId);

                CuttingTorch = GetComponents(components, CuttingTorchComponent.TypeId);

                Deposition = GetComponents(components, DepositionComponent.TypeId);

                Dielectric = GetComponents(components, DielectricComponent.TypeId);

                Door = GetComponents(components, DoorComponent.TypeId);

                Drain = GetComponents(components, DrainComponent.TypeId);

                Electric = GetComponents(components, ElectricComponent.TypeId);

                Electrode = GetComponents(components, ElectrodeComponent.TypeId);

                Enclosure = GetComponents(components, EnclosureComponent.TypeId);

                Encoder = GetComponents(components, EncoderComponent.TypeId);

                EndEffector = GetComponents(components, EndEffectorComponent.TypeId);

                Environmental = GetComponents(components, EnvironmentalComponent.TypeId);

                ExpiredPot = GetComponents(components, ExpiredPotComponent.TypeId);

                ExposureUnit = GetComponents(components, ExposureUnitComponent.TypeId);

                ExtrusionUnit = GetComponents(components, ExtrusionUnitComponent.TypeId);

                Fan = GetComponents(components, FanComponent.TypeId);

                FeatureOccurrence = GetComponents(components, FeatureOccurrenceComponent.TypeId);

                Feeder = GetComponents(components, FeederComponent.TypeId);

                Filter = GetComponents(components, FilterComponent.TypeId);

                Galvanomotor = GetComponents(components, GalvanomotorComponent.TypeId);

                GangToolBar = GetComponents(components, GangToolBarComponent.TypeId);

                Gripper = GetComponents(components, GripperComponent.TypeId);

                Heating = GetComponents(components, HeatingComponent.TypeId);

                Hopper = GetComponents(components, HopperComponent.TypeId);

                Hydraulic = GetComponents(components, HydraulicComponent.TypeId);

                Interfaces = GetComponents(components, InterfacesComponent.TypeId);

                Linear = GetComponents(components, LinearComponent.TypeId);

                LinearPositionFeedback = GetComponents(components, LinearPositionFeedbackComponent.TypeId);

                Link = GetComponents(components, LinkComponent.TypeId);

                Loader = GetComponents(components, LoaderComponent.TypeId);

                Lock = GetComponents(components, LockComponent.TypeId);

                Lubrication = GetComponents(components, LubricationComponent.TypeId);

                Material = GetComponents(components, MaterialComponent.TypeId);

                Materials = GetComponents(components, MaterialsComponent.TypeId);

                Motor = GetComponents(components, MotorComponent.TypeId);

                Oil = GetComponents(components, OilComponent.TypeId);

                Part = GetComponents(components, PartComponent.TypeId);

                PartOccurrence = GetComponents(components, PartOccurrenceComponent.TypeId);

                Parts = GetComponents(components, PartsComponent.TypeId);

                Path = GetComponents(components, PathComponent.TypeId);

                Personnel = GetComponents(components, PersonnelComponent.TypeId);

                PinTool = GetComponents(components, PinToolComponent.TypeId);

                Pneumatic = GetComponents(components, PneumaticComponent.TypeId);

                Pot = GetComponents(components, PotComponent.TypeId);

                Power = GetComponents(components, PowerComponent.TypeId);

                PowerSupply = GetComponents(components, PowerSupplyComponent.TypeId);

                Pressure = GetComponents(components, PressureComponent.TypeId);

                Process = GetComponents(components, ProcessComponent.TypeId);

                Processes = GetComponents(components, ProcessesComponent.TypeId);

                ProcessOccurrence = GetComponents(components, ProcessOccurrenceComponent.TypeId);

                ProcessPower = GetComponents(components, ProcessPowerComponent.TypeId);

                Protective = GetComponents(components, ProtectiveComponent.TypeId);

                Pulley = GetComponents(components, PulleyComponent.TypeId);

                Pump = GetComponents(components, PumpComponent.TypeId);

                Reel = GetComponents(components, ReelComponent.TypeId);

                RemovalPot = GetComponents(components, RemovalPotComponent.TypeId);

                Resource = GetComponents(components, ResourceComponent.TypeId);

                Resources = GetComponents(components, ResourcesComponent.TypeId);

                ReturnPot = GetComponents(components, ReturnPotComponent.TypeId);

                Rotary = GetComponents(components, RotaryComponent.TypeId);

                SensingElement = GetComponents(components, SensingElementComponent.TypeId);

                Sensor = GetComponents(components, SensorComponent.TypeId);

                Spindle = GetComponents(components, SpindleComponent.TypeId);

                Spreader = GetComponents(components, SpreaderComponent.TypeId);

                StagingPot = GetComponents(components, StagingPotComponent.TypeId);

                Station = GetComponents(components, StationComponent.TypeId);

                Stock = GetComponents(components, StockComponent.TypeId);

                StorageBattery = GetComponents(components, StorageBatteryComponent.TypeId);

                Structure = GetComponents(components, StructureComponent.TypeId);

                Structures = GetComponents(components, StructuresComponent.TypeId);

                Switch = GetComponents(components, SwitchComponent.TypeId);

                System = GetComponents(components, SystemComponent.TypeId);

                Systems = GetComponents(components, SystemsComponent.TypeId);

                Table = GetComponents(components, TableComponent.TypeId);

                Tank = GetComponents(components, TankComponent.TypeId);

                Tensioner = GetComponents(components, TensionerComponent.TypeId);

                Thermostat = GetComponents(components, ThermostatComponent.TypeId);

                ToolHolder = GetComponents(components, ToolHolderComponent.TypeId);

                ToolingDelivery = GetComponents(components, ToolingDeliveryComponent.TypeId);

                ToolMagazine = GetComponents(components, ToolMagazineComponent.TypeId);

                ToolRack = GetComponents(components, ToolRackComponent.TypeId);

                TransferArm = GetComponents(components, TransferArmComponent.TypeId);

                TransferPot = GetComponents(components, TransferPotComponent.TypeId);

                Transformer = GetComponents(components, TransformerComponent.TypeId);

                Turret = GetComponents(components, TurretComponent.TypeId);

                Vacuum = GetComponents(components, VacuumComponent.TypeId);

                Valve = GetComponents(components, ValveComponent.TypeId);

                Vat = GetComponents(components, VatComponent.TypeId);

                Vibration = GetComponents(components, VibrationComponent.TypeId);

                WasteDisposal = GetComponents(components, WasteDisposalComponent.TypeId);

                Water = GetComponents(components, WaterComponent.TypeId);

                Wire = GetComponents(components, WireComponent.TypeId);

                WorkEnvelope = GetComponents(components, WorkEnvelopeComponent.TypeId);

                Workpiece = GetComponents(components, WorkpieceComponent.TypeId);

            }
        }

        private IEnumerable<JsonComponent> GetComponents(IEnumerable<IComponent> components, string type)
        {
            if (!components.IsNullOrEmpty())
            {
                var typeComponents = components.Where(o => o.Type == type);
                if (!typeComponents.IsNullOrEmpty())
                {
                    var jsonComponents = new List<JsonComponent>();
                    foreach (var component in typeComponents) jsonComponents.Add(new JsonComponent(component));
                    return jsonComponents;
                }
            }

            return null;
        }


        /// <summary>
        /// Flattens every typed component collection back into a single
        /// <see cref="IComponent"/> sequence, restoring each item's MTConnect type.
        /// </summary>
        public IEnumerable<IComponent> ToComponents()
        {
            var components = new List<IComponent>();
            if (!Actuator.IsNullOrEmpty()) foreach (var component in Actuator) components.Add(component.ToComponent(ActuatorComponent.TypeId));

            if (!Adapter.IsNullOrEmpty()) foreach (var component in Adapter) components.Add(component.ToComponent(AdapterComponent.TypeId));

            if (!Adapters.IsNullOrEmpty()) foreach (var component in Adapters) components.Add(component.ToComponent(AdaptersComponent.TypeId));

            if (!AirHandler.IsNullOrEmpty()) foreach (var component in AirHandler) components.Add(component.ToComponent(AirHandlerComponent.TypeId));

            if (!Amplifier.IsNullOrEmpty()) foreach (var component in Amplifier) components.Add(component.ToComponent(AmplifierComponent.TypeId));

            if (!AutomaticToolChanger.IsNullOrEmpty()) foreach (var component in AutomaticToolChanger) components.Add(component.ToComponent(AutomaticToolChangerComponent.TypeId));

            if (!Auxiliaries.IsNullOrEmpty()) foreach (var component in Auxiliaries) components.Add(component.ToComponent(AuxiliariesComponent.TypeId));

            if (!Auxiliary.IsNullOrEmpty()) foreach (var component in Auxiliary) components.Add(component.ToComponent(AuxiliaryComponent.TypeId));

            if (!Axes.IsNullOrEmpty()) foreach (var component in Axes) components.Add(component.ToComponent(AxesComponent.TypeId));

            if (!Axis.IsNullOrEmpty()) foreach (var component in Axis) components.Add(component.ToComponent(AxisComponent.TypeId));

            if (!Ballscrew.IsNullOrEmpty()) foreach (var component in Ballscrew) components.Add(component.ToComponent(BallscrewComponent.TypeId));

            if (!BarFeeder.IsNullOrEmpty()) foreach (var component in BarFeeder) components.Add(component.ToComponent(BarFeederComponent.TypeId));

            if (!Belt.IsNullOrEmpty()) foreach (var component in Belt) components.Add(component.ToComponent(BeltComponent.TypeId));

            if (!Brake.IsNullOrEmpty()) foreach (var component in Brake) components.Add(component.ToComponent(BrakeComponent.TypeId));

            if (!Chain.IsNullOrEmpty()) foreach (var component in Chain) components.Add(component.ToComponent(ChainComponent.TypeId));

            if (!Chopper.IsNullOrEmpty()) foreach (var component in Chopper) components.Add(component.ToComponent(ChopperComponent.TypeId));

            if (!Chuck.IsNullOrEmpty()) foreach (var component in Chuck) components.Add(component.ToComponent(ChuckComponent.TypeId));

            if (!Chute.IsNullOrEmpty()) foreach (var component in Chute) components.Add(component.ToComponent(ChuteComponent.TypeId));

            if (!CircuitBreaker.IsNullOrEmpty()) foreach (var component in CircuitBreaker) components.Add(component.ToComponent(CircuitBreakerComponent.TypeId));

            if (!Clamp.IsNullOrEmpty()) foreach (var component in Clamp) components.Add(component.ToComponent(ClampComponent.TypeId));

            if (!Compressor.IsNullOrEmpty()) foreach (var component in Compressor) components.Add(component.ToComponent(CompressorComponent.TypeId));

            if (!Controller.IsNullOrEmpty()) foreach (var component in Controller) components.Add(component.ToComponent(ControllerComponent.TypeId));

            if (!Controllers.IsNullOrEmpty()) foreach (var component in Controllers) components.Add(component.ToComponent(ControllersComponent.TypeId));

            if (!Coolant.IsNullOrEmpty()) foreach (var component in Coolant) components.Add(component.ToComponent(CoolantComponent.TypeId));

            if (!Cooling.IsNullOrEmpty()) foreach (var component in Cooling) components.Add(component.ToComponent(CoolingComponent.TypeId));

            if (!CoolingTower.IsNullOrEmpty()) foreach (var component in CoolingTower) components.Add(component.ToComponent(CoolingTowerComponent.TypeId));

            if (!CuttingTorch.IsNullOrEmpty()) foreach (var component in CuttingTorch) components.Add(component.ToComponent(CuttingTorchComponent.TypeId));

            if (!Deposition.IsNullOrEmpty()) foreach (var component in Deposition) components.Add(component.ToComponent(DepositionComponent.TypeId));

            if (!Dielectric.IsNullOrEmpty()) foreach (var component in Dielectric) components.Add(component.ToComponent(DielectricComponent.TypeId));

            if (!Door.IsNullOrEmpty()) foreach (var component in Door) components.Add(component.ToComponent(DoorComponent.TypeId));

            if (!Drain.IsNullOrEmpty()) foreach (var component in Drain) components.Add(component.ToComponent(DrainComponent.TypeId));

            if (!Electric.IsNullOrEmpty()) foreach (var component in Electric) components.Add(component.ToComponent(ElectricComponent.TypeId));

            if (!Electrode.IsNullOrEmpty()) foreach (var component in Electrode) components.Add(component.ToComponent(ElectrodeComponent.TypeId));

            if (!Enclosure.IsNullOrEmpty()) foreach (var component in Enclosure) components.Add(component.ToComponent(EnclosureComponent.TypeId));

            if (!Encoder.IsNullOrEmpty()) foreach (var component in Encoder) components.Add(component.ToComponent(EncoderComponent.TypeId));

            if (!EndEffector.IsNullOrEmpty()) foreach (var component in EndEffector) components.Add(component.ToComponent(EndEffectorComponent.TypeId));

            if (!Environmental.IsNullOrEmpty()) foreach (var component in Environmental) components.Add(component.ToComponent(EnvironmentalComponent.TypeId));

            if (!ExpiredPot.IsNullOrEmpty()) foreach (var component in ExpiredPot) components.Add(component.ToComponent(ExpiredPotComponent.TypeId));

            if (!ExposureUnit.IsNullOrEmpty()) foreach (var component in ExposureUnit) components.Add(component.ToComponent(ExposureUnitComponent.TypeId));

            if (!ExtrusionUnit.IsNullOrEmpty()) foreach (var component in ExtrusionUnit) components.Add(component.ToComponent(ExtrusionUnitComponent.TypeId));

            if (!Fan.IsNullOrEmpty()) foreach (var component in Fan) components.Add(component.ToComponent(FanComponent.TypeId));

            if (!FeatureOccurrence.IsNullOrEmpty()) foreach (var component in FeatureOccurrence) components.Add(component.ToComponent(FeatureOccurrenceComponent.TypeId));

            if (!Feeder.IsNullOrEmpty()) foreach (var component in Feeder) components.Add(component.ToComponent(FeederComponent.TypeId));

            if (!Filter.IsNullOrEmpty()) foreach (var component in Filter) components.Add(component.ToComponent(FilterComponent.TypeId));

            if (!Galvanomotor.IsNullOrEmpty()) foreach (var component in Galvanomotor) components.Add(component.ToComponent(GalvanomotorComponent.TypeId));

            if (!GangToolBar.IsNullOrEmpty()) foreach (var component in GangToolBar) components.Add(component.ToComponent(GangToolBarComponent.TypeId));

            if (!Gripper.IsNullOrEmpty()) foreach (var component in Gripper) components.Add(component.ToComponent(GripperComponent.TypeId));

            if (!Heating.IsNullOrEmpty()) foreach (var component in Heating) components.Add(component.ToComponent(HeatingComponent.TypeId));

            if (!Hopper.IsNullOrEmpty()) foreach (var component in Hopper) components.Add(component.ToComponent(HopperComponent.TypeId));

            if (!Hydraulic.IsNullOrEmpty()) foreach (var component in Hydraulic) components.Add(component.ToComponent(HydraulicComponent.TypeId));

            if (!Interfaces.IsNullOrEmpty()) foreach (var component in Interfaces) components.Add(component.ToComponent(InterfacesComponent.TypeId));

            if (!Linear.IsNullOrEmpty()) foreach (var component in Linear) components.Add(component.ToComponent(LinearComponent.TypeId));

            if (!LinearPositionFeedback.IsNullOrEmpty()) foreach (var component in LinearPositionFeedback) components.Add(component.ToComponent(LinearPositionFeedbackComponent.TypeId));

            if (!Link.IsNullOrEmpty()) foreach (var component in Link) components.Add(component.ToComponent(LinkComponent.TypeId));

            if (!Loader.IsNullOrEmpty()) foreach (var component in Loader) components.Add(component.ToComponent(LoaderComponent.TypeId));

            if (!Lock.IsNullOrEmpty()) foreach (var component in Lock) components.Add(component.ToComponent(LockComponent.TypeId));

            if (!Lubrication.IsNullOrEmpty()) foreach (var component in Lubrication) components.Add(component.ToComponent(LubricationComponent.TypeId));

            if (!Material.IsNullOrEmpty()) foreach (var component in Material) components.Add(component.ToComponent(MaterialComponent.TypeId));

            if (!Materials.IsNullOrEmpty()) foreach (var component in Materials) components.Add(component.ToComponent(MaterialsComponent.TypeId));

            if (!Motor.IsNullOrEmpty()) foreach (var component in Motor) components.Add(component.ToComponent(MotorComponent.TypeId));

            if (!Oil.IsNullOrEmpty()) foreach (var component in Oil) components.Add(component.ToComponent(OilComponent.TypeId));

            if (!Part.IsNullOrEmpty()) foreach (var component in Part) components.Add(component.ToComponent(PartComponent.TypeId));

            if (!PartOccurrence.IsNullOrEmpty()) foreach (var component in PartOccurrence) components.Add(component.ToComponent(PartOccurrenceComponent.TypeId));

            if (!Parts.IsNullOrEmpty()) foreach (var component in Parts) components.Add(component.ToComponent(PartsComponent.TypeId));

            if (!Path.IsNullOrEmpty()) foreach (var component in Path) components.Add(component.ToComponent(PathComponent.TypeId));

            if (!Personnel.IsNullOrEmpty()) foreach (var component in Personnel) components.Add(component.ToComponent(PersonnelComponent.TypeId));

            if (!PinTool.IsNullOrEmpty()) foreach (var component in PinTool) components.Add(component.ToComponent(PinToolComponent.TypeId));

            if (!Pneumatic.IsNullOrEmpty()) foreach (var component in Pneumatic) components.Add(component.ToComponent(PneumaticComponent.TypeId));

            if (!Pot.IsNullOrEmpty()) foreach (var component in Pot) components.Add(component.ToComponent(PotComponent.TypeId));

            if (!Power.IsNullOrEmpty()) foreach (var component in Power) components.Add(component.ToComponent(PowerComponent.TypeId));

            if (!PowerSupply.IsNullOrEmpty()) foreach (var component in PowerSupply) components.Add(component.ToComponent(PowerSupplyComponent.TypeId));

            if (!Pressure.IsNullOrEmpty()) foreach (var component in Pressure) components.Add(component.ToComponent(PressureComponent.TypeId));

            if (!Process.IsNullOrEmpty()) foreach (var component in Process) components.Add(component.ToComponent(ProcessComponent.TypeId));

            if (!Processes.IsNullOrEmpty()) foreach (var component in Processes) components.Add(component.ToComponent(ProcessesComponent.TypeId));

            if (!ProcessOccurrence.IsNullOrEmpty()) foreach (var component in ProcessOccurrence) components.Add(component.ToComponent(ProcessOccurrenceComponent.TypeId));

            if (!ProcessPower.IsNullOrEmpty()) foreach (var component in ProcessPower) components.Add(component.ToComponent(ProcessPowerComponent.TypeId));

            if (!Protective.IsNullOrEmpty()) foreach (var component in Protective) components.Add(component.ToComponent(ProtectiveComponent.TypeId));

            if (!Pulley.IsNullOrEmpty()) foreach (var component in Pulley) components.Add(component.ToComponent(PulleyComponent.TypeId));

            if (!Pump.IsNullOrEmpty()) foreach (var component in Pump) components.Add(component.ToComponent(PumpComponent.TypeId));

            if (!Reel.IsNullOrEmpty()) foreach (var component in Reel) components.Add(component.ToComponent(ReelComponent.TypeId));

            if (!RemovalPot.IsNullOrEmpty()) foreach (var component in RemovalPot) components.Add(component.ToComponent(RemovalPotComponent.TypeId));

            if (!Resource.IsNullOrEmpty()) foreach (var component in Resource) components.Add(component.ToComponent(ResourceComponent.TypeId));

            if (!Resources.IsNullOrEmpty()) foreach (var component in Resources) components.Add(component.ToComponent(ResourcesComponent.TypeId));

            if (!ReturnPot.IsNullOrEmpty()) foreach (var component in ReturnPot) components.Add(component.ToComponent(ReturnPotComponent.TypeId));

            if (!Rotary.IsNullOrEmpty()) foreach (var component in Rotary) components.Add(component.ToComponent(RotaryComponent.TypeId));

            if (!SensingElement.IsNullOrEmpty()) foreach (var component in SensingElement) components.Add(component.ToComponent(SensingElementComponent.TypeId));

            if (!Sensor.IsNullOrEmpty()) foreach (var component in Sensor) components.Add(component.ToComponent(SensorComponent.TypeId));

            if (!Spindle.IsNullOrEmpty()) foreach (var component in Spindle) components.Add(component.ToComponent(SpindleComponent.TypeId));

            if (!Spreader.IsNullOrEmpty()) foreach (var component in Spreader) components.Add(component.ToComponent(SpreaderComponent.TypeId));

            if (!StagingPot.IsNullOrEmpty()) foreach (var component in StagingPot) components.Add(component.ToComponent(StagingPotComponent.TypeId));

            if (!Station.IsNullOrEmpty()) foreach (var component in Station) components.Add(component.ToComponent(StationComponent.TypeId));

            if (!Stock.IsNullOrEmpty()) foreach (var component in Stock) components.Add(component.ToComponent(StockComponent.TypeId));

            if (!StorageBattery.IsNullOrEmpty()) foreach (var component in StorageBattery) components.Add(component.ToComponent(StorageBatteryComponent.TypeId));

            if (!Structure.IsNullOrEmpty()) foreach (var component in Structure) components.Add(component.ToComponent(StructureComponent.TypeId));

            if (!Structures.IsNullOrEmpty()) foreach (var component in Structures) components.Add(component.ToComponent(StructuresComponent.TypeId));

            if (!Switch.IsNullOrEmpty()) foreach (var component in Switch) components.Add(component.ToComponent(SwitchComponent.TypeId));

            if (!System.IsNullOrEmpty()) foreach (var component in System) components.Add(component.ToComponent(SystemComponent.TypeId));

            if (!Systems.IsNullOrEmpty()) foreach (var component in Systems) components.Add(component.ToComponent(SystemsComponent.TypeId));

            if (!Table.IsNullOrEmpty()) foreach (var component in Table) components.Add(component.ToComponent(TableComponent.TypeId));

            if (!Tank.IsNullOrEmpty()) foreach (var component in Tank) components.Add(component.ToComponent(TankComponent.TypeId));

            if (!Tensioner.IsNullOrEmpty()) foreach (var component in Tensioner) components.Add(component.ToComponent(TensionerComponent.TypeId));

            if (!Thermostat.IsNullOrEmpty()) foreach (var component in Thermostat) components.Add(component.ToComponent(ThermostatComponent.TypeId));

            if (!ToolHolder.IsNullOrEmpty()) foreach (var component in ToolHolder) components.Add(component.ToComponent(ToolHolderComponent.TypeId));

            if (!ToolingDelivery.IsNullOrEmpty()) foreach (var component in ToolingDelivery) components.Add(component.ToComponent(ToolingDeliveryComponent.TypeId));

            if (!ToolMagazine.IsNullOrEmpty()) foreach (var component in ToolMagazine) components.Add(component.ToComponent(ToolMagazineComponent.TypeId));

            if (!ToolRack.IsNullOrEmpty()) foreach (var component in ToolRack) components.Add(component.ToComponent(ToolRackComponent.TypeId));

            if (!TransferArm.IsNullOrEmpty()) foreach (var component in TransferArm) components.Add(component.ToComponent(TransferArmComponent.TypeId));

            if (!TransferPot.IsNullOrEmpty()) foreach (var component in TransferPot) components.Add(component.ToComponent(TransferPotComponent.TypeId));

            if (!Transformer.IsNullOrEmpty()) foreach (var component in Transformer) components.Add(component.ToComponent(TransformerComponent.TypeId));

            if (!Turret.IsNullOrEmpty()) foreach (var component in Turret) components.Add(component.ToComponent(TurretComponent.TypeId));

            if (!Vacuum.IsNullOrEmpty()) foreach (var component in Vacuum) components.Add(component.ToComponent(VacuumComponent.TypeId));

            if (!Valve.IsNullOrEmpty()) foreach (var component in Valve) components.Add(component.ToComponent(ValveComponent.TypeId));

            if (!Vat.IsNullOrEmpty()) foreach (var component in Vat) components.Add(component.ToComponent(VatComponent.TypeId));

            if (!Vibration.IsNullOrEmpty()) foreach (var component in Vibration) components.Add(component.ToComponent(VibrationComponent.TypeId));

            if (!WasteDisposal.IsNullOrEmpty()) foreach (var component in WasteDisposal) components.Add(component.ToComponent(WasteDisposalComponent.TypeId));

            if (!Water.IsNullOrEmpty()) foreach (var component in Water) components.Add(component.ToComponent(WaterComponent.TypeId));

            if (!Wire.IsNullOrEmpty()) foreach (var component in Wire) components.Add(component.ToComponent(WireComponent.TypeId));

            if (!WorkEnvelope.IsNullOrEmpty()) foreach (var component in WorkEnvelope) components.Add(component.ToComponent(WorkEnvelopeComponent.TypeId));

            if (!Workpiece.IsNullOrEmpty()) foreach (var component in Workpiece) components.Add(component.ToComponent(WorkpieceComponent.TypeId));


            return components;
        }
    }
}
