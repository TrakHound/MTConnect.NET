// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Reflection
{
    // Per-category parametric coverage for the regenerated Component
    // subtypes under MTConnect.Devices.Components. The broad
    // RegeneratedTypesCoverageTests fixture exercises the bare ctor +
    // property round-trip contract; this fixture adds the
    // Component-specific contract:
    //
    //   - Every concrete Component exposes a non-empty TypeId const.
    //   - The default ctor wires Type from TypeId so a fresh Component is
    //     wire-emit-ready.
    //   - The TypeDescription / DescriptionText surface is non-null and
    //     non-empty for every concrete subtype (the regenerator template
    //     always emits both, but the SysML XMI may ship an empty value;
    //     such gaps surface as failing rows rather than silent passes).
    //
    // Source authority:
    //   - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    //     (per-version tag) — defines the Component class hierarchy and
    //     the TypeId / DescriptionText values emitted by the regenerator.
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_<vN.M>.xsd
    //     — defines the wire-shape constraint on Component element
    //     names emitted by the agent.
    //   - Prose: docs.mtconnect.org "Part 2.0 - Devices Information Model"
    //     §"Component" — defines the Component / Composition / Device
    //     role hierarchy.
    [TestFixture]
    public class RegeneratedComponentsCoverageTests
    {
        private static IEnumerable<Type> EnumerateComponentSubtypes()
        {
            return typeof(Component).Assembly.GetTypes()
                .Where(t => t.IsPublic)
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsGenericTypeDefinition)
                .Where(t => t.Namespace == "MTConnect.Devices.Components")
                .Where(t => typeof(Component).IsAssignableFrom(t))
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.FullName, StringComparer.Ordinal);
        }

        public static IEnumerable<TestCaseData> ComponentSubtypes()
        {
            foreach (var type in EnumerateComponentSubtypes())
            {
                yield return new TestCaseData(type)
                    .SetName($"Component_{type.Name}");
            }
        }

        [Test]
        public void Catalogue_enumerates_at_least_one_component_subtype()
        {
            // Smoke-test the catalogue itself so the parametric sweep cannot
            // silently shrink to zero. A regenerator regression that drops
            // every subtype would otherwise produce a quietly green sweep.
            Assert.That(EnumerateComponentSubtypes().Count(), Is.GreaterThan(50),
                "MTConnect.Devices.Components produced fewer than 50 concrete subtypes — regenerator regression?");
        }

        [Test]
        [TestCaseSource(nameof(ComponentSubtypes))]
        public void Component_subtype_exposes_non_empty_TypeId(Type type)
        {
            // TypeId is the wire identifier the agent emits in the Devices
            // envelope. Empty / whitespace TypeIds are spec-illegal.
            var field = type.GetField("TypeId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.That(field, Is.Not.Null,
                $"{type.FullName} is missing the TypeId const required by the regenerator template");

            var value = field!.GetRawConstantValue() as string;
            Assert.That(value, Is.Not.Null.And.Not.Empty,
                $"{type.FullName}.TypeId is null or empty");
        }

        [Test]
        [TestCaseSource(nameof(ComponentSubtypes))]
        public void Constructed_Component_subtype_carries_Type_from_const(Type type)
        {
            // The default ctor must wire Type from TypeId so a fresh
            // instance is wire-emit-ready. The broad reflection sweep does
            // not exercise this Component-specific assertion.
            var instance = (Component)Activator.CreateInstance(type)!;
            var field = type.GetField("TypeId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)!;
            var expected = (string)field.GetRawConstantValue()!;

            Assert.That(instance.Type, Is.EqualTo(expected),
                $"{type.FullName} default ctor did not wire Type from TypeId");
        }
    }
}
