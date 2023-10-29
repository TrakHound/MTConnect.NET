// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Devices.Json
{
    public class JsonComponents
    {
        [JsonPropertyName("Actuator")]
        public IEnumerable<JsonComponent> Actuator { get; set; }


        [JsonPropertyName("Adapter")]
        public IEnumerable<JsonComponent> Adapter { get; set; }


        [JsonPropertyName("Adapters")]
        public IEnumerable<JsonComponent> Adapters { get; set; }


        [JsonPropertyName("Amplifier")]
        public IEnumerable<JsonComponent> Amplifier { get; set; }


        [JsonPropertyName("AutomaticToolChanger")]
        public IEnumerable<JsonComponent> AutomaticToolChanger { get; set; }


        [JsonPropertyName("Auxiliaries")]
        public IEnumerable<JsonComponent> Auxiliaries { get; set; }


        [JsonPropertyName("Auxiliary")]
        public IEnumerable<JsonComponent> Auxiliary { get; set; }


        [JsonPropertyName("Axes")]
        public IEnumerable<JsonComponent> Axes { get; set; }


        [JsonPropertyName("Axis")]
        public IEnumerable<JsonComponent> Axis { get; set; }


        [JsonPropertyName("Ballscrew")]
        public IEnumerable<JsonComponent> Ballscrew { get; set; }


        [JsonPropertyName("BarFeeder")]
        public IEnumerable<JsonComponent> BarFeeder { get; set; }


        [JsonPropertyName("Belt")]
        public IEnumerable<JsonComponent> Belt { get; set; }


        [JsonPropertyName("Brake")]
        public IEnumerable<JsonComponent> Brake { get; set; }


        [JsonPropertyName("Chain")]
        public IEnumerable<JsonComponent> Chain { get; set; }


        [JsonPropertyName("Chopper")]
        public IEnumerable<JsonComponent> Chopper { get; set; }


        [JsonPropertyName("Chuck")]
        public IEnumerable<JsonComponent> Chuck { get; set; }


        [JsonPropertyName("Chute")]
        public IEnumerable<JsonComponent> Chute { get; set; }


        [JsonPropertyName("CircuitBreaker")]
        public IEnumerable<JsonComponent> CircuitBreaker { get; set; }


        [JsonPropertyName("Clamp")]
        public IEnumerable<JsonComponent> Clamp { get; set; }


        [JsonPropertyName("Compressor")]
        public IEnumerable<JsonComponent> Compressor { get; set; }


        [JsonPropertyName("Controller")]
        public IEnumerable<JsonComponent> Controller { get; set; }


        [JsonPropertyName("Controllers")]
        public IEnumerable<JsonComponent> Controllers { get; set; }


        [JsonPropertyName("Coolant")]
        public IEnumerable<JsonComponent> Coolant { get; set; }


        [JsonPropertyName("Cooling")]
        public IEnumerable<JsonComponent> Cooling { get; set; }


        [JsonPropertyName("CoolingTower")]
        public IEnumerable<JsonComponent> CoolingTower { get; set; }


        [JsonPropertyName("Deposition")]
        public IEnumerable<JsonComponent> Deposition { get; set; }


        [JsonPropertyName("Dielectric")]
        public IEnumerable<JsonComponent> Dielectric { get; set; }


        [JsonPropertyName("Door")]
        public IEnumerable<JsonComponent> Door { get; set; }


        [JsonPropertyName("Drain")]
        public IEnumerable<JsonComponent> Drain { get; set; }


        [JsonPropertyName("Electric")]
        public IEnumerable<JsonComponent> Electric { get; set; }


        [JsonPropertyName("Enclosure")]
        public IEnumerable<JsonComponent> Enclosure { get; set; }


        [JsonPropertyName("Encoder")]
        public IEnumerable<JsonComponent> Encoder { get; set; }


        [JsonPropertyName("EndEffector")]
        public IEnumerable<JsonComponent> EndEffector { get; set; }


        [JsonPropertyName("Environmental")]
        public IEnumerable<JsonComponent> Environmental { get; set; }


        [JsonPropertyName("ExpiredPot")]
        public IEnumerable<JsonComponent> ExpiredPot { get; set; }


        [JsonPropertyName("ExposureUnit")]
        public IEnumerable<JsonComponent> ExposureUnit { get; set; }


        [JsonPropertyName("ExtrusionUnit")]
        public IEnumerable<JsonComponent> ExtrusionUnit { get; set; }


        [JsonPropertyName("Fan")]
        public IEnumerable<JsonComponent> Fan { get; set; }


        [JsonPropertyName("FeatureOccurrence")]
        public IEnumerable<JsonComponent> FeatureOccurrence { get; set; }


        [JsonPropertyName("Feeder")]
        public IEnumerable<JsonComponent> Feeder { get; set; }


        [JsonPropertyName("Filter")]
        public IEnumerable<JsonComponent> Filter { get; set; }


        [JsonPropertyName("Galvanomotor")]
        public IEnumerable<JsonComponent> Galvanomotor { get; set; }


        [JsonPropertyName("GangToolBar")]
        public IEnumerable<JsonComponent> GangToolBar { get; set; }


        [JsonPropertyName("Gripper")]
        public IEnumerable<JsonComponent> Gripper { get; set; }


        [JsonPropertyName("Heating")]
        public IEnumerable<JsonComponent> Heating { get; set; }


        [JsonPropertyName("Hopper")]
        public IEnumerable<JsonComponent> Hopper { get; set; }


        [JsonPropertyName("Hydraulic")]
        public IEnumerable<JsonComponent> Hydraulic { get; set; }


        [JsonPropertyName("Interfaces")]
        public IEnumerable<JsonComponent> Interfaces { get; set; }


        [JsonPropertyName("Linear")]
        public IEnumerable<JsonComponent> Linear { get; set; }


        [JsonPropertyName("LinearPositionFeedback")]
        public IEnumerable<JsonComponent> LinearPositionFeedback { get; set; }


        [JsonPropertyName("Link")]
        public IEnumerable<JsonComponent> Link { get; set; }


        [JsonPropertyName("Loader")]
        public IEnumerable<JsonComponent> Loader { get; set; }


        [JsonPropertyName("Lock")]
        public IEnumerable<JsonComponent> Lock { get; set; }


        [JsonPropertyName("Lubrication")]
        public IEnumerable<JsonComponent> Lubrication { get; set; }


        [JsonPropertyName("Material")]
        public IEnumerable<JsonComponent> Material { get; set; }


        [JsonPropertyName("Materials")]
        public IEnumerable<JsonComponent> Materials { get; set; }


        [JsonPropertyName("Motor")]
        public IEnumerable<JsonComponent> Motor { get; set; }


        [JsonPropertyName("Oil")]
        public IEnumerable<JsonComponent> Oil { get; set; }


        [JsonPropertyName("Part")]
        public IEnumerable<JsonComponent> Part { get; set; }


        [JsonPropertyName("PartOccurrence")]
        public IEnumerable<JsonComponent> PartOccurrence { get; set; }


        [JsonPropertyName("Parts")]
        public IEnumerable<JsonComponent> Parts { get; set; }


        [JsonPropertyName("Path")]
        public IEnumerable<JsonComponent> Path { get; set; }


        [JsonPropertyName("Personnel")]
        public IEnumerable<JsonComponent> Personnel { get; set; }


        [JsonPropertyName("Pneumatic")]
        public IEnumerable<JsonComponent> Pneumatic { get; set; }


        [JsonPropertyName("Pot")]
        public IEnumerable<JsonComponent> Pot { get; set; }


        [JsonPropertyName("Power")]
        public IEnumerable<JsonComponent> Power { get; set; }


        [JsonPropertyName("PowerSupply")]
        public IEnumerable<JsonComponent> PowerSupply { get; set; }


        [JsonPropertyName("Pressure")]
        public IEnumerable<JsonComponent> Pressure { get; set; }


        [JsonPropertyName("Process")]
        public IEnumerable<JsonComponent> Process { get; set; }


        [JsonPropertyName("Processes")]
        public IEnumerable<JsonComponent> Processes { get; set; }


        [JsonPropertyName("ProcessOccurrence")]
        public IEnumerable<JsonComponent> ProcessOccurrence { get; set; }


        [JsonPropertyName("ProcessPower")]
        public IEnumerable<JsonComponent> ProcessPower { get; set; }


        [JsonPropertyName("Protective")]
        public IEnumerable<JsonComponent> Protective { get; set; }


        [JsonPropertyName("Pulley")]
        public IEnumerable<JsonComponent> Pulley { get; set; }


        [JsonPropertyName("Pump")]
        public IEnumerable<JsonComponent> Pump { get; set; }


        [JsonPropertyName("Reel")]
        public IEnumerable<JsonComponent> Reel { get; set; }


        [JsonPropertyName("RemovalPot")]
        public IEnumerable<JsonComponent> RemovalPot { get; set; }


        [JsonPropertyName("Resource")]
        public IEnumerable<JsonComponent> Resource { get; set; }


        [JsonPropertyName("Resources")]
        public IEnumerable<JsonComponent> Resources { get; set; }


        [JsonPropertyName("ReturnPot")]
        public IEnumerable<JsonComponent> ReturnPot { get; set; }


        [JsonPropertyName("Rotary")]
        public IEnumerable<JsonComponent> Rotary { get; set; }


        [JsonPropertyName("SensingElement")]
        public IEnumerable<JsonComponent> SensingElement { get; set; }


        [JsonPropertyName("Sensor")]
        public IEnumerable<JsonComponent> Sensor { get; set; }


        [JsonPropertyName("Spindle")]
        public IEnumerable<JsonComponent> Spindle { get; set; }


        [JsonPropertyName("Spreader")]
        public IEnumerable<JsonComponent> Spreader { get; set; }


        [JsonPropertyName("StagingPot")]
        public IEnumerable<JsonComponent> StagingPot { get; set; }


        [JsonPropertyName("Station")]
        public IEnumerable<JsonComponent> Station { get; set; }


        [JsonPropertyName("Stock")]
        public IEnumerable<JsonComponent> Stock { get; set; }


        [JsonPropertyName("StorageBattery")]
        public IEnumerable<JsonComponent> StorageBattery { get; set; }


        [JsonPropertyName("Structure")]
        public IEnumerable<JsonComponent> Structure { get; set; }


        [JsonPropertyName("Structures")]
        public IEnumerable<JsonComponent> Structures { get; set; }


        [JsonPropertyName("Switch")]
        public IEnumerable<JsonComponent> Switch { get; set; }


        [JsonPropertyName("System")]
        public IEnumerable<JsonComponent> System { get; set; }


        [JsonPropertyName("Systems")]
        public IEnumerable<JsonComponent> Systems { get; set; }


        [JsonPropertyName("Table")]
        public IEnumerable<JsonComponent> Table { get; set; }


        [JsonPropertyName("Tank")]
        public IEnumerable<JsonComponent> Tank { get; set; }


        [JsonPropertyName("Tensioner")]
        public IEnumerable<JsonComponent> Tensioner { get; set; }


        [JsonPropertyName("Thermostat")]
        public IEnumerable<JsonComponent> Thermostat { get; set; }


        [JsonPropertyName("ToolingDelivery")]
        public IEnumerable<JsonComponent> ToolingDelivery { get; set; }


        [JsonPropertyName("ToolMagazine")]
        public IEnumerable<JsonComponent> ToolMagazine { get; set; }


        [JsonPropertyName("ToolRack")]
        public IEnumerable<JsonComponent> ToolRack { get; set; }


        [JsonPropertyName("TransferArm")]
        public IEnumerable<JsonComponent> TransferArm { get; set; }


        [JsonPropertyName("TransferPot")]
        public IEnumerable<JsonComponent> TransferPot { get; set; }


        [JsonPropertyName("Transformer")]
        public IEnumerable<JsonComponent> Transformer { get; set; }


        [JsonPropertyName("Turret")]
        public IEnumerable<JsonComponent> Turret { get; set; }


        [JsonPropertyName("Vacuum")]
        public IEnumerable<JsonComponent> Vacuum { get; set; }


        [JsonPropertyName("Valve")]
        public IEnumerable<JsonComponent> Valve { get; set; }


        [JsonPropertyName("Vat")]
        public IEnumerable<JsonComponent> Vat { get; set; }


        [JsonPropertyName("Vibration")]
        public IEnumerable<JsonComponent> Vibration { get; set; }


        [JsonPropertyName("WasteDisposal")]
        public IEnumerable<JsonComponent> WasteDisposal { get; set; }


        [JsonPropertyName("Water")]
        public IEnumerable<JsonComponent> Water { get; set; }


        [JsonPropertyName("Wire")]
        public IEnumerable<JsonComponent> Wire { get; set; }


        [JsonPropertyName("WorkEnvelope")]
        public IEnumerable<JsonComponent> WorkEnvelope { get; set; }


        [JsonPropertyName("Workpiece")]
        public IEnumerable<JsonComponent> Workpiece { get; set; }



        public JsonComponents() { }

        public JsonComponents(IEnumerable<IComponent> components)
        {
            if (!components.IsNullOrEmpty())
            {
                Actuator = GetComponents(components, ActuatorComponent.TypeId);

                Adapter = GetComponents(components, AdapterComponent.TypeId);

                Adapters = GetComponents(components, AdaptersComponent.TypeId);

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

                Deposition = GetComponents(components, DepositionComponent.TypeId);

                Dielectric = GetComponents(components, DielectricComponent.TypeId);

                Door = GetComponents(components, DoorComponent.TypeId);

                Drain = GetComponents(components, DrainComponent.TypeId);

                Electric = GetComponents(components, ElectricComponent.TypeId);

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


        public IEnumerable<IComponent> ToComponents()
        {
            var components = new List<IComponent>();
            if (!Actuator.IsNullOrEmpty()) foreach (var component in Actuator) components.Add(component.ToComponent());

            if (!Adapter.IsNullOrEmpty()) foreach (var component in Adapter) components.Add(component.ToComponent());

            if (!Adapters.IsNullOrEmpty()) foreach (var component in Adapters) components.Add(component.ToComponent());

            if (!Amplifier.IsNullOrEmpty()) foreach (var component in Amplifier) components.Add(component.ToComponent());

            if (!AutomaticToolChanger.IsNullOrEmpty()) foreach (var component in AutomaticToolChanger) components.Add(component.ToComponent());

            if (!Auxiliaries.IsNullOrEmpty()) foreach (var component in Auxiliaries) components.Add(component.ToComponent());

            if (!Auxiliary.IsNullOrEmpty()) foreach (var component in Auxiliary) components.Add(component.ToComponent());

            if (!Axes.IsNullOrEmpty()) foreach (var component in Axes) components.Add(component.ToComponent());

            if (!Axis.IsNullOrEmpty()) foreach (var component in Axis) components.Add(component.ToComponent());

            if (!Ballscrew.IsNullOrEmpty()) foreach (var component in Ballscrew) components.Add(component.ToComponent());

            if (!BarFeeder.IsNullOrEmpty()) foreach (var component in BarFeeder) components.Add(component.ToComponent());

            if (!Belt.IsNullOrEmpty()) foreach (var component in Belt) components.Add(component.ToComponent());

            if (!Brake.IsNullOrEmpty()) foreach (var component in Brake) components.Add(component.ToComponent());

            if (!Chain.IsNullOrEmpty()) foreach (var component in Chain) components.Add(component.ToComponent());

            if (!Chopper.IsNullOrEmpty()) foreach (var component in Chopper) components.Add(component.ToComponent());

            if (!Chuck.IsNullOrEmpty()) foreach (var component in Chuck) components.Add(component.ToComponent());

            if (!Chute.IsNullOrEmpty()) foreach (var component in Chute) components.Add(component.ToComponent());

            if (!CircuitBreaker.IsNullOrEmpty()) foreach (var component in CircuitBreaker) components.Add(component.ToComponent());

            if (!Clamp.IsNullOrEmpty()) foreach (var component in Clamp) components.Add(component.ToComponent());

            if (!Compressor.IsNullOrEmpty()) foreach (var component in Compressor) components.Add(component.ToComponent());

            if (!Controller.IsNullOrEmpty()) foreach (var component in Controller) components.Add(component.ToComponent());

            if (!Controllers.IsNullOrEmpty()) foreach (var component in Controllers) components.Add(component.ToComponent());

            if (!Coolant.IsNullOrEmpty()) foreach (var component in Coolant) components.Add(component.ToComponent());

            if (!Cooling.IsNullOrEmpty()) foreach (var component in Cooling) components.Add(component.ToComponent());

            if (!CoolingTower.IsNullOrEmpty()) foreach (var component in CoolingTower) components.Add(component.ToComponent());

            if (!Deposition.IsNullOrEmpty()) foreach (var component in Deposition) components.Add(component.ToComponent());

            if (!Dielectric.IsNullOrEmpty()) foreach (var component in Dielectric) components.Add(component.ToComponent());

            if (!Door.IsNullOrEmpty()) foreach (var component in Door) components.Add(component.ToComponent());

            if (!Drain.IsNullOrEmpty()) foreach (var component in Drain) components.Add(component.ToComponent());

            if (!Electric.IsNullOrEmpty()) foreach (var component in Electric) components.Add(component.ToComponent());

            if (!Enclosure.IsNullOrEmpty()) foreach (var component in Enclosure) components.Add(component.ToComponent());

            if (!Encoder.IsNullOrEmpty()) foreach (var component in Encoder) components.Add(component.ToComponent());

            if (!EndEffector.IsNullOrEmpty()) foreach (var component in EndEffector) components.Add(component.ToComponent());

            if (!Environmental.IsNullOrEmpty()) foreach (var component in Environmental) components.Add(component.ToComponent());

            if (!ExpiredPot.IsNullOrEmpty()) foreach (var component in ExpiredPot) components.Add(component.ToComponent());

            if (!ExposureUnit.IsNullOrEmpty()) foreach (var component in ExposureUnit) components.Add(component.ToComponent());

            if (!ExtrusionUnit.IsNullOrEmpty()) foreach (var component in ExtrusionUnit) components.Add(component.ToComponent());

            if (!Fan.IsNullOrEmpty()) foreach (var component in Fan) components.Add(component.ToComponent());

            if (!FeatureOccurrence.IsNullOrEmpty()) foreach (var component in FeatureOccurrence) components.Add(component.ToComponent());

            if (!Feeder.IsNullOrEmpty()) foreach (var component in Feeder) components.Add(component.ToComponent());

            if (!Filter.IsNullOrEmpty()) foreach (var component in Filter) components.Add(component.ToComponent());

            if (!Galvanomotor.IsNullOrEmpty()) foreach (var component in Galvanomotor) components.Add(component.ToComponent());

            if (!GangToolBar.IsNullOrEmpty()) foreach (var component in GangToolBar) components.Add(component.ToComponent());

            if (!Gripper.IsNullOrEmpty()) foreach (var component in Gripper) components.Add(component.ToComponent());

            if (!Heating.IsNullOrEmpty()) foreach (var component in Heating) components.Add(component.ToComponent());

            if (!Hopper.IsNullOrEmpty()) foreach (var component in Hopper) components.Add(component.ToComponent());

            if (!Hydraulic.IsNullOrEmpty()) foreach (var component in Hydraulic) components.Add(component.ToComponent());

            if (!Interfaces.IsNullOrEmpty()) foreach (var component in Interfaces) components.Add(component.ToComponent());

            if (!Linear.IsNullOrEmpty()) foreach (var component in Linear) components.Add(component.ToComponent());

            if (!LinearPositionFeedback.IsNullOrEmpty()) foreach (var component in LinearPositionFeedback) components.Add(component.ToComponent());

            if (!Link.IsNullOrEmpty()) foreach (var component in Link) components.Add(component.ToComponent());

            if (!Loader.IsNullOrEmpty()) foreach (var component in Loader) components.Add(component.ToComponent());

            if (!Lock.IsNullOrEmpty()) foreach (var component in Lock) components.Add(component.ToComponent());

            if (!Lubrication.IsNullOrEmpty()) foreach (var component in Lubrication) components.Add(component.ToComponent());

            if (!Material.IsNullOrEmpty()) foreach (var component in Material) components.Add(component.ToComponent());

            if (!Materials.IsNullOrEmpty()) foreach (var component in Materials) components.Add(component.ToComponent());

            if (!Motor.IsNullOrEmpty()) foreach (var component in Motor) components.Add(component.ToComponent());

            if (!Oil.IsNullOrEmpty()) foreach (var component in Oil) components.Add(component.ToComponent());

            if (!Part.IsNullOrEmpty()) foreach (var component in Part) components.Add(component.ToComponent());

            if (!PartOccurrence.IsNullOrEmpty()) foreach (var component in PartOccurrence) components.Add(component.ToComponent());

            if (!Parts.IsNullOrEmpty()) foreach (var component in Parts) components.Add(component.ToComponent());

            if (!Path.IsNullOrEmpty()) foreach (var component in Path) components.Add(component.ToComponent());

            if (!Personnel.IsNullOrEmpty()) foreach (var component in Personnel) components.Add(component.ToComponent());

            if (!Pneumatic.IsNullOrEmpty()) foreach (var component in Pneumatic) components.Add(component.ToComponent());

            if (!Pot.IsNullOrEmpty()) foreach (var component in Pot) components.Add(component.ToComponent());

            if (!Power.IsNullOrEmpty()) foreach (var component in Power) components.Add(component.ToComponent());

            if (!PowerSupply.IsNullOrEmpty()) foreach (var component in PowerSupply) components.Add(component.ToComponent());

            if (!Pressure.IsNullOrEmpty()) foreach (var component in Pressure) components.Add(component.ToComponent());

            if (!Process.IsNullOrEmpty()) foreach (var component in Process) components.Add(component.ToComponent());

            if (!Processes.IsNullOrEmpty()) foreach (var component in Processes) components.Add(component.ToComponent());

            if (!ProcessOccurrence.IsNullOrEmpty()) foreach (var component in ProcessOccurrence) components.Add(component.ToComponent());

            if (!ProcessPower.IsNullOrEmpty()) foreach (var component in ProcessPower) components.Add(component.ToComponent());

            if (!Protective.IsNullOrEmpty()) foreach (var component in Protective) components.Add(component.ToComponent());

            if (!Pulley.IsNullOrEmpty()) foreach (var component in Pulley) components.Add(component.ToComponent());

            if (!Pump.IsNullOrEmpty()) foreach (var component in Pump) components.Add(component.ToComponent());

            if (!Reel.IsNullOrEmpty()) foreach (var component in Reel) components.Add(component.ToComponent());

            if (!RemovalPot.IsNullOrEmpty()) foreach (var component in RemovalPot) components.Add(component.ToComponent());

            if (!Resource.IsNullOrEmpty()) foreach (var component in Resource) components.Add(component.ToComponent());

            if (!Resources.IsNullOrEmpty()) foreach (var component in Resources) components.Add(component.ToComponent());

            if (!ReturnPot.IsNullOrEmpty()) foreach (var component in ReturnPot) components.Add(component.ToComponent());

            if (!Rotary.IsNullOrEmpty()) foreach (var component in Rotary) components.Add(component.ToComponent());

            if (!SensingElement.IsNullOrEmpty()) foreach (var component in SensingElement) components.Add(component.ToComponent());

            if (!Sensor.IsNullOrEmpty()) foreach (var component in Sensor) components.Add(component.ToComponent());

            if (!Spindle.IsNullOrEmpty()) foreach (var component in Spindle) components.Add(component.ToComponent());

            if (!Spreader.IsNullOrEmpty()) foreach (var component in Spreader) components.Add(component.ToComponent());

            if (!StagingPot.IsNullOrEmpty()) foreach (var component in StagingPot) components.Add(component.ToComponent());

            if (!Station.IsNullOrEmpty()) foreach (var component in Station) components.Add(component.ToComponent());

            if (!Stock.IsNullOrEmpty()) foreach (var component in Stock) components.Add(component.ToComponent());

            if (!StorageBattery.IsNullOrEmpty()) foreach (var component in StorageBattery) components.Add(component.ToComponent());

            if (!Structure.IsNullOrEmpty()) foreach (var component in Structure) components.Add(component.ToComponent());

            if (!Structures.IsNullOrEmpty()) foreach (var component in Structures) components.Add(component.ToComponent());

            if (!Switch.IsNullOrEmpty()) foreach (var component in Switch) components.Add(component.ToComponent());

            if (!System.IsNullOrEmpty()) foreach (var component in System) components.Add(component.ToComponent());

            if (!Systems.IsNullOrEmpty()) foreach (var component in Systems) components.Add(component.ToComponent());

            if (!Table.IsNullOrEmpty()) foreach (var component in Table) components.Add(component.ToComponent());

            if (!Tank.IsNullOrEmpty()) foreach (var component in Tank) components.Add(component.ToComponent());

            if (!Tensioner.IsNullOrEmpty()) foreach (var component in Tensioner) components.Add(component.ToComponent());

            if (!Thermostat.IsNullOrEmpty()) foreach (var component in Thermostat) components.Add(component.ToComponent());

            if (!ToolingDelivery.IsNullOrEmpty()) foreach (var component in ToolingDelivery) components.Add(component.ToComponent());

            if (!ToolMagazine.IsNullOrEmpty()) foreach (var component in ToolMagazine) components.Add(component.ToComponent());

            if (!ToolRack.IsNullOrEmpty()) foreach (var component in ToolRack) components.Add(component.ToComponent());

            if (!TransferArm.IsNullOrEmpty()) foreach (var component in TransferArm) components.Add(component.ToComponent());

            if (!TransferPot.IsNullOrEmpty()) foreach (var component in TransferPot) components.Add(component.ToComponent());

            if (!Transformer.IsNullOrEmpty()) foreach (var component in Transformer) components.Add(component.ToComponent());

            if (!Turret.IsNullOrEmpty()) foreach (var component in Turret) components.Add(component.ToComponent());

            if (!Vacuum.IsNullOrEmpty()) foreach (var component in Vacuum) components.Add(component.ToComponent());

            if (!Valve.IsNullOrEmpty()) foreach (var component in Valve) components.Add(component.ToComponent());

            if (!Vat.IsNullOrEmpty()) foreach (var component in Vat) components.Add(component.ToComponent());

            if (!Vibration.IsNullOrEmpty()) foreach (var component in Vibration) components.Add(component.ToComponent());

            if (!WasteDisposal.IsNullOrEmpty()) foreach (var component in WasteDisposal) components.Add(component.ToComponent());

            if (!Water.IsNullOrEmpty()) foreach (var component in Water) components.Add(component.ToComponent());

            if (!Wire.IsNullOrEmpty()) foreach (var component in Wire) components.Add(component.ToComponent());

            if (!WorkEnvelope.IsNullOrEmpty()) foreach (var component in WorkEnvelope) components.Add(component.ToComponent());

            if (!Workpiece.IsNullOrEmpty()) foreach (var component in Workpiece) components.Add(component.ToComponent());


            return components;
        }
    }
}