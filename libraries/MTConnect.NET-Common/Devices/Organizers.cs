// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;
using MTConnect.Interfaces;
using System;
using System.Collections.Generic;

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


        // Single source of truth for the `member TypeId -> organizer
        // TypeId` mapping. Built once at type-init from the same
        // organizer lists that back the public accessors so the dual
        // representation cannot drift; an `IsOrganizer(typeId)` call
        // and a `GetOrganizerType(typeId)` call see the same membership.
        //
        // Lookup is O(1) and order-independent — the `Controller`
        // carve-out (Controller belongs to `Controllers`, not `Systems`)
        // is enforced by `_systems` not listing `Controller`, so an O(1)
        // lookup yields the same answer.
        private static readonly Dictionary<string, string> _typeToOrganizer = BuildTypeToOrganizer();

        // O(1) membership set for the first-class organizer container
        // TypeIds. Built from the same `_components` list that backs the
        // public `Components` accessor so the two cannot disagree; the
        // accessor keeps returning the ordered enumerable for stable
        // public iteration, while `IsOrganizer` reads the set.
        private static readonly HashSet<string> _organizerTypeIds =
            new HashSet<string>(_components, StringComparer.Ordinal);

        private static Dictionary<string, string> BuildTypeToOrganizer()
        {
            var map = new Dictionary<string, string>();

            // Use Dictionary.Add so a duplicate key (the same TypeId
            // appearing in two organizer lists) throws at type-init,
            // surfacing any cross-organizer ambiguity loudly rather
            // than silently picking the last write.
            void AddAll(IEnumerable<string> members, string organizerType)
            {
                foreach (var m in members) map.Add(m, organizerType);
            }

            AddAll(_adapters, AdaptersComponent.TypeId);
            AddAll(_auxiliaries, AuxiliariesComponent.TypeId);
            AddAll(_axes, AxesComponent.TypeId);
            AddAll(_controllers, ControllersComponent.TypeId);
            AddAll(_interfaces, InterfacesComponent.TypeId);
            AddAll(_materials, MaterialsComponent.TypeId);
            AddAll(_parts, PartsComponent.TypeId);
            AddAll(_processes, ProcessesComponent.TypeId);
            AddAll(_resources, ResourcesComponent.TypeId);
            AddAll(_systems, SystemsComponent.TypeId);
            AddAll(_structures, StructuresComponent.TypeId);

            return map;
        }


        /// <summary>
        /// Returns the organizer container TypeId that
        /// <paramref name="componentType"/> belongs under, or
        /// <c>null</c> when <paramref name="componentType"/> is not a
        /// known member of any organizer.
        /// </summary>
        public static string GetOrganizerType(string componentType)
        {
            if (componentType == null) return null;
            return _typeToOrganizer.TryGetValue(componentType, out var organizer) ? organizer : null;
        }


        /// <summary>
        /// Returns <c>true</c> when <paramref name="typeId"/> is one of
        /// the eleven first-class organizer container TypeIds
        /// (<c>Adapters</c>, <c>Auxiliaries</c>, <c>Axes</c>,
        /// <c>Controllers</c>, <c>Interfaces</c>, <c>Materials</c>,
        /// <c>Parts</c>, <c>Processes</c>, <c>Resources</c>,
        /// <c>Systems</c>, <c>Structures</c>); <c>false</c> for any
        /// other TypeId, including SysML members that get auto-wrapped
        /// under one of those organizers, and for <c>null</c> or empty
        /// inputs.
        /// </summary>
        public static bool IsOrganizer(string typeId)
        {
            if (string.IsNullOrEmpty(typeId)) return false;
            return _organizerTypeIds.Contains(typeId);
        }
    }
}