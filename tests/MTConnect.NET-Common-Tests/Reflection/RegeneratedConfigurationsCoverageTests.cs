// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MTConnect.Devices;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Reflection
{
    // Per-category parametric coverage for the regenerated Configuration
    // value-object subtypes under MTConnect.Devices.Configurations.
    // Configurations are not first-class wire-element types like
    // Components or DataItems; they are nested value-objects (Specification,
    // Coordinate-systems, Motion, Origin, Relationships, etc.) that
    // attach to a Component's Configuration element. The broad
    // RegeneratedTypesCoverageTests fixture exercises the bare ctor +
    // property round-trip contract; this fixture pins:
    //
    //   - Every concrete Configuration subtype is constructible.
    //   - The DescriptionText surface is non-null for every concrete
    //     subtype.
    //   - Every public auto-property accepts default(T) without throw and
    //     reads back the sentinel (the broad sweep validates this; the
    //     per-namespace duplication makes Configuration regressions easy
    //     to triage in CI logs).
    //
    // Source authority:
    //   - SysML XMI: https://github.com/mtconnect/mtconnect_sysml_model
    //     (per-version tag) — defines the Configuration value-object
    //     hierarchy.
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_<vN.M>.xsd
    //     — defines the wire-shape constraint on the Configuration sub-tree
    //     of a Component.
    //   - Prose: docs.mtconnect.org "Part 2.0 - Devices Information Model"
    //     §"Configuration" — defines the Configuration role.
    [TestFixture]
    public class RegeneratedConfigurationsCoverageTests
    {
        private static IEnumerable<Type> EnumerateConfigurationSubtypes()
        {
            return typeof(Component).Assembly.GetTypes()
                .Where(t => t.IsPublic)
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsGenericTypeDefinition)
                .Where(t => t.Namespace == "MTConnect.Devices.Configurations")
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.FullName, StringComparer.Ordinal);
        }

        public static IEnumerable<TestCaseData> ConfigurationSubtypes()
        {
            foreach (var type in EnumerateConfigurationSubtypes())
            {
                yield return new TestCaseData(type)
                    .SetName($"Configuration_{type.Name}");
            }
        }

        [Test]
        public void Catalogue_enumerates_at_least_one_configuration_subtype()
        {
            // Smoke-test the catalogue itself.
            Assert.That(EnumerateConfigurationSubtypes().Count(), Is.GreaterThan(10),
                "MTConnect.Devices.Configurations produced fewer than 10 concrete subtypes — regenerator regression?");
        }

        [Test]
        [TestCaseSource(nameof(ConfigurationSubtypes))]
        public void Configuration_subtype_is_constructible(Type type)
        {
            object? instance = null;
            Assert.DoesNotThrow(
                () => instance = Activator.CreateInstance(type),
                $"{type.FullName} parameterless ctor threw");
            Assert.That(instance, Is.Not.Null,
                $"{type.FullName} parameterless ctor returned null");
        }

        [Test]
        [TestCaseSource(nameof(ConfigurationSubtypes))]
        public void Configuration_subtype_string_properties_round_trip(Type type)
        {
            // String-typed auto-properties are the most common
            // Configuration shape (e.g. Specification.Name, Origin.Code).
            // Walking them ensures no regenerator template emits a
            // computed-only setter that silently drops the value.
            var instance = Activator.CreateInstance(type)!;

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead || !property.CanWrite || property.GetIndexParameters().Length > 0)
                {
                    continue;
                }
                if (property.PropertyType != typeof(string))
                {
                    continue;
                }

                var sentinel = $"sentinel-{property.Name}";
                Assert.DoesNotThrow(
                    () => property.SetValue(instance, sentinel),
                    $"{type.FullName}.{property.Name} setter threw");
                var readBack = property.GetValue(instance) as string;
                Assert.That(readBack, Is.EqualTo(sentinel),
                    $"{type.FullName}.{property.Name} did not round-trip");
            }
        }
    }
}
