// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;
using MTConnect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Devices
{
    public static class Organizers
    {
        private static readonly IEnumerable<string> _components = new List<string>
        {
            AdaptersComponent.TypeId,
            AuxiliariesComponent.TypeId,
            AxesComponent.TypeId,
            ControllersComponent.TypeId,
            InterfacesComponent.TypeId,
            MaterialsComponent.TypeId,
            PartsComponent.TypeId,
            ProcessesComponent.TypeId,
            ResourcesComponent.TypeId,
            SystemsComponent.TypeId,
            StructuresComponent.TypeId
        };

        private static readonly IEnumerable<string> _adapters = new List<string>
        {
            AdapterComponent.TypeId
        };

        private static readonly IEnumerable<string> _auxiliaries = new List<string>
        {
            DepositionComponent.TypeId,
            EnvironmentalComponent.TypeId,
            LoaderComponent.TypeId,
            ToolingDeliveryComponent.TypeId,
            WasteDisposalComponent.TypeId,
        };

        private static readonly IEnumerable<string> _axes = new List<string>
        {
            LinearComponent.TypeId,
            RotaryComponent.TypeId
        };

        private static readonly IEnumerable<string> _controllers = new List<string>
        {
            ControllerComponent.TypeId
        };

        private static readonly IEnumerable<string> _interfaces = new List<string>
        {
            BarFeederInterface.TypeId,
            ChuckInterface.TypeId,
            DoorInterface.TypeId,
            MaterialHandlerInterface.TypeId
        };

        private static readonly IEnumerable<string> _materials = new List<string>
        {
            StockComponent.TypeId
        };

        private static readonly IEnumerable<string> _parts = new List<string>
        {
            PartOccurrenceComponent.TypeId
        };

        private static readonly IEnumerable<string> _processes = new List<string>
        {
            ProcessOccurrenceComponent.TypeId
        };

        private static readonly IEnumerable<string> _resources = new List<string>
        {
            ResourceComponent.TypeId,
            PersonnelComponent.TypeId
        };

        // Members of the `System` substitution-group declared by the MTConnect
        // SysML model (https://github.com/mtconnect/mtconnect_sysml_model). The
        // list is the union of every `System`-derived `*Component` whose
        // `DescriptionText` matches the canonical SysML phrasing for that
        // substitution-group, and is the source of truth for which auto-wrap
        // path `Device.AddComponent()` takes. Sorted alphabetically by
        // `TypeId` so future regenerations diff cleanly.
        //
        // `Controller` is intentionally OMITTED from this list even though
        // SysML places it in the `System` substitution-group: this library
        // routes `Controller` through its own dedicated `Controllers`
        // organizer (see the `organizerType != ControllersComponent.TypeId`
        // guard in `Device.AddComponent()`), so listing it here would make
        // `GetOrganizerType("Controller")` ambiguous and order-dependent.
        private static readonly IEnumerable<string> _systems = new List<string>
        {
            AirHandlerComponent.TypeId,
            CoolantComponent.TypeId,
            CoolingComponent.TypeId,
            DielectricComponent.TypeId,
            ElectricComponent.TypeId,
            EnclosureComponent.TypeId,
            EndEffectorComponent.TypeId,
            FeederComponent.TypeId,
            HeatingComponent.TypeId,
            HydraulicComponent.TypeId,
            LubricationComponent.TypeId,
            PinToolComponent.TypeId,
            PneumaticComponent.TypeId,
            PressureComponent.TypeId,
            ProcessPowerComponent.TypeId,
            ProtectiveComponent.TypeId,
            ToolHolderComponent.TypeId,
            VacuumComponent.TypeId,
            WorkEnvelopeComponent.TypeId
        };

        private static readonly IEnumerable<string> _structures = new List<string>
        {
            LinkComponent.TypeId
        };


        /// <summary>
        /// Gets a list of Components that are used to Organize child Components
        /// </summary>
        public static IEnumerable<string> Components => _components;


        /// <summary>
        /// Gets a list of Components that are used to Organize child Adapter Components
        /// </summary>
        public static IEnumerable<string> Adapters => _adapters;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Auxiliary Components
        /// </summary>
        public static IEnumerable<string> Auxiliaries => _auxiliaries;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Axis Components
        /// </summary>
        public static IEnumerable<string> Axes => _axes;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Controller Components
        /// </summary>
        public static IEnumerable<string> Controllers => _controllers;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Interface Components
        /// </summary>
        public static IEnumerable<string> Interfaces => _interfaces;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Material Components
        /// </summary>
        public static IEnumerable<string> Materials => _materials;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Part Components
        /// </summary>
        public static IEnumerable<string> Parts => _parts;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Process Components
        /// </summary>
        public static IEnumerable<string> Processes => _processes;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Resource Components
        /// </summary>
        public static IEnumerable<string> Resources => _resources;

        /// <summary>
        /// Gets a list of Components that are used to Organize child System Components
        /// </summary>
        public static IEnumerable<string> Systems => _systems;

        /// <summary>
        /// Gets a list of Components that are used to Organize child Structure Components
        /// </summary>
        public static IEnumerable<string> Structures => _structures;


        public static string GetOrganizerType(string componentType)
        {
            if (componentType != null)
            {
                if (_adapters.Contains(componentType)) return AdaptersComponent.TypeId;
                else if (_auxiliaries.Contains(componentType)) return AuxiliariesComponent.TypeId;
                else if (_axes.Contains(componentType)) return AxesComponent.TypeId;
                else if (_controllers.Contains(componentType)) return ControllersComponent.TypeId;
                else if (_interfaces.Contains(componentType)) return InterfacesComponent.TypeId;
                else if (_materials.Contains(componentType)) return MaterialsComponent.TypeId;
                else if (_parts.Contains(componentType)) return PartsComponent.TypeId;
                else if (_processes.Contains(componentType)) return ProcessesComponent.TypeId;
                else if (_resources.Contains(componentType)) return ResourcesComponent.TypeId;
                else if (_systems.Contains(componentType)) return SystemsComponent.TypeId;
                else if (_structures.Contains(componentType)) return StructuresComponent.TypeId;
            }

            return null;
        }
    }
}