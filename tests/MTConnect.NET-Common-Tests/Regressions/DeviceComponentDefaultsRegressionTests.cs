// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Regressions
{
    /// <summary>
    /// Regression pins for the contract that <see cref="Device"/> and
    /// every concrete <see cref="Component"/> subclass leave identity
    /// and naming fields <c>null</c> after default construction.
    ///
    /// These tests outlast the red-category fixtures: the red fixtures
    /// describe the work that landed; this fixture protects the
    /// contract over time. The reflection guard catches any future
    /// subclass that re-introduces the back-fill.
    /// </summary>
    [TestFixture]
    public class DeviceComponentDefaultsRegressionTests
    {
        // ---- Device ------------------------------------------------

        [Test]
        public void Device_default_constructor_leaves_identity_fields_null()
        {
            var device = new Device();
            Assert.Multiple(() =>
            {
                Assert.That(device.Id, Is.Null, "Device.Id");
                Assert.That(device.Name, Is.Null, "Device.Name");
                Assert.That(device.Uuid, Is.Null, "Device.Uuid");
            });
        }

        [Test]
        public void Agent_default_constructor_leaves_identity_fields_null()
        {
            var agent = new Agent();
            Assert.Multiple(() =>
            {
                Assert.That(agent.Id, Is.Null, "Agent.Id");
                Assert.That(agent.Name, Is.Null, "Agent.Name");
                Assert.That(agent.Uuid, Is.Null, "Agent.Uuid");
            });
        }

        [Test]
        public void Sequential_default_Devices_share_null_Uuid()
        {
            // The original #136 defect produced a fresh GUID per
            // construction. Pinned here because the bug is silent —
            // the symptom is identity drift, not a thrown exception.
            var first = new Device();
            var second = new Device();
            Assert.That(first.Uuid, Is.Null);
            Assert.That(second.Uuid, Is.Null);
        }

        [Test]
        public void Object_initializer_continues_to_set_Device_identity()
        {
            var device = new Device { Id = "id-A", Name = "name-A", Uuid = "uuid-A" };
            Assert.Multiple(() =>
            {
                Assert.That(device.Id, Is.EqualTo("id-A"));
                Assert.That(device.Name, Is.EqualTo("name-A"));
                Assert.That(device.Uuid, Is.EqualTo("uuid-A"));
            });
        }

        // ---- Reflection guard --------------------------------------

        [Test]
        public void No_Device_subclass_default_constructor_back_fills_identity()
        {
            var failures = new List<string>();
            foreach (var subclass in EnumerateConcreteWithDefaultCtor(typeof(Device)))
            {
                var instance = (Device)Activator.CreateInstance(subclass)!;
                if (instance.Id is not null)
                {
                    failures.Add($"{subclass.FullName} default ctor set Id = '{instance.Id}'.");
                }
                if (instance.Name is not null)
                {
                    failures.Add($"{subclass.FullName} default ctor set Name = '{instance.Name}'.");
                }
                if (instance.Uuid is not null)
                {
                    failures.Add($"{subclass.FullName} default ctor set Uuid = '{instance.Uuid}'.");
                }
            }

            Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures));
        }

        [Test]
        public void No_Component_subclass_default_constructor_back_fills_Name()
        {
            var componentType = typeof(Component);
            var failures = new List<string>();

            foreach (var subclass in EnumerateConcreteWithDefaultCtor(componentType))
            {
                if (subclass == componentType)
                {
                    // Base class — has no NameId to back-fill from.
                    continue;
                }

                var instance = (Component)Activator.CreateInstance(subclass)!;
                if (instance.Name is not null)
                {
                    failures.Add($"{subclass.FullName} default ctor set Name = '{instance.Name}'.");
                }
            }

            Assert.That(failures, Is.Empty, string.Join(Environment.NewLine, failures));
        }

        [Test]
        public void Reflection_guard_walks_a_meaningful_set_of_Component_subclasses()
        {
            // Sanity check on the reflection walker — if the assembly
            // ever stops exposing component subclasses (e.g. a
            // refactor splits them off), the guard above silently
            // passes. This test fails loudly in that case.
            var componentSubclasses = EnumerateConcreteWithDefaultCtor(typeof(Component))
                .Where(t => t != typeof(Component) && t.Namespace == "MTConnect.Devices.Components")
                .ToList();
            Assert.That(componentSubclasses.Count, Is.GreaterThan(50));
        }

        // ---- Helper ------------------------------------------------

        private static IEnumerable<Type> EnumerateConcreteWithDefaultCtor(Type baseType)
        {
            return baseType.Assembly.GetTypes()
                .Where(t => baseType.IsAssignableFrom(t)
                            && !t.IsAbstract
                            && t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, binder: null, types: Type.EmptyTypes, modifiers: null) is not null);
        }
    }
}
