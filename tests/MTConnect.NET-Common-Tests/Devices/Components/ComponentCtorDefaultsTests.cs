// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Reflection;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Devices.Components
{
    /// <summary>
    /// Pins the contract that every concrete <see cref="Component"/>
    /// subclass leaves <c>Name</c> <c>null</c> after default
    /// construction. Back-filling <c>Name</c> from the lowercase
    /// <c>NameId</c> constant on each subclass would silently inject
    /// the placeholder values <c>"axes"</c>, <c>"linear"</c>,
    /// <c>"systems"</c>, etc., into wire output for any consumer that
    /// does not set <c>Component.Name</c> explicitly.
    /// </summary>
    [TestFixture]
    [Category("DeviceComponentDefaultsRemoved")]
    public class ComponentCtorDefaultsTests
    {
        /// <summary>
        /// Walks every concrete <see cref="Component"/>-derived type
        /// in the production assembly that has a public default
        /// constructor and lives in
        /// <c>MTConnect.Devices.Components</c> (i.e. every generated
        /// component subclass) and asserts the default constructor
        /// leaves <c>Name</c> <c>null</c>. The out-of-scope set is
        /// imported from <see cref="NameBackfillRemovalOutOfScope"/>
        /// to keep this fixture and the regression fixture in lockstep.
        /// </summary>
        [Test]
        public void Every_concrete_Component_subclass_default_ctor_leaves_Name_null()
        {
            foreach (var subclass in EnumerateConcreteComponentSubclasses())
            {
                if (NameBackfillRemovalOutOfScope.ComponentTypeNames.Contains(subclass.FullName!))
                    continue;

                var instance = (Component)Activator.CreateInstance(subclass)!;
                Assert.That(instance.Name, Is.Null,
                    $"{subclass.FullName} default ctor back-filled Name.");
            }
        }

        /// <summary>
        /// Verifies that <c>Type</c> remains non-null after default
        /// construction — the campaign removes only the <c>Name</c>
        /// back-fill, not the <c>Type</c> assignment.
        /// </summary>
        [Test]
        public void Every_concrete_Component_subclass_default_ctor_still_sets_Type()
        {
            foreach (var subclass in EnumerateConcreteComponentSubclasses())
            {
                var instance = (Component)Activator.CreateInstance(subclass)!;
                Assert.That(instance.Type, Is.Not.Null.And.Not.Empty,
                    $"{subclass.FullName} default ctor failed to set Type.");
            }
        }

        /// <summary>
        /// Returns the generated concrete <c>*Component</c> subclasses
        /// — i.e. those in the <c>MTConnect.Devices.Components</c>
        /// namespace. The base <c>MTConnect.Devices.Component</c> is
        /// concrete (and is callable directly), but it is the base
        /// class itself, not a generated subclass; its default ctor
        /// doesn't set <c>Type</c> and doesn't back-fill <c>Name</c>
        /// either, so it is out of scope for the back-fill assertion.
        /// </summary>
        private static System.Collections.Generic.IEnumerable<Type> EnumerateConcreteComponentSubclasses()
        {
            var componentType = typeof(Component);
            var assembly = componentType.Assembly;

            var concreteSubclasses = assembly.GetTypes()
                .Where(t => componentType.IsAssignableFrom(t)
                            && t != componentType
                            && t.Namespace == "MTConnect.Devices.Components"
                            && !t.IsAbstract
                            && t.GetConstructor(Type.EmptyTypes) is not null)
                .ToList();

            // Sanity check — the production assembly defines many
            // concrete component subclasses. If reflection finds
            // none the test is silently passing for the wrong
            // reason.
            Assert.That(concreteSubclasses.Count, Is.GreaterThan(50),
                "Expected the assembly to expose many concrete Component subclasses.");

            return concreteSubclasses;
        }

        [Test]
        public void Object_initializer_still_sets_Component_Name()
        {
            var component = new AxesComponent { Name = "explicit" };
            Assert.That(component.Name, Is.EqualTo("explicit"));
        }

        [Test]
        public void Direct_setter_still_sets_Component_Name()
        {
            var component = new ControllerComponent();
            component.Name = "controller-1";
            Assert.That(component.Name, Is.EqualTo("controller-1"));
        }
    }
}
