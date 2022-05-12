// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using MTConnect.Devices.Components;
using MTConnect.Interfaces;

namespace MTConnect.Devices
{
    public static class Organizers
    {
        private static readonly IEnumerable<string> _components = new List<string>
        {
            AdaptersComponent.TypeId,
            AuxiliariesComponent.TypeId,
            AxesComponent.TypeId,
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

        private static readonly IEnumerable<string> _interfaces = new List<string>
        {
            BarFeederInterface.TypeId
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

        private static readonly IEnumerable<string> _systems = new List<string>
        {
            ControllerComponent.TypeId,
            CoolantComponent.TypeId,
            DielectricComponent.TypeId,
            ElectricComponent.TypeId,
            EnclosureComponent.TypeId,
            EndEffectorComponent.TypeId,
            FeederComponent.TypeId,
            HydraulicComponent.TypeId,
            LubricationComponent.TypeId,
            PneumaticComponent.TypeId,
            ProcessPowerComponent.TypeId,
            ProtectiveComponent.TypeId,
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
    }
}
