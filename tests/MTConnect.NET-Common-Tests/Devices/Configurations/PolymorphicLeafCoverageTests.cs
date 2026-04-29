// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MTConnect.Devices.Configurations;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Devices.Configurations
{
    /// <summary>
    /// Parametric reflection guard for the Configuration polymorphic-leaf
    /// surface introduced for MTConnect v2.7. Each
    /// <c>IAbstract&lt;Leaf&gt;</c> interface in
    /// <c>MTConnect.Devices.Configurations</c> has exactly two implementing
    /// classes: a simple value-typed leaf (<c>Axis</c>, <c>Origin</c>,
    /// <c>Rotation</c>, <c>Scale</c>, <c>Translation</c>) and a DataSet
    /// variant (<c>&lt;Leaf&gt;DataSet</c>). Both implement the
    /// <c>I&lt;Leaf&gt;</c> hierarchy; only the DataSet variant additionally
    /// implements <c>I&lt;Leaf&gt;DataSet</c> and <c>IDataSet</c>.
    /// </summary>
    /// <remarks>
    /// Sources:
    /// <list type="bullet">
    /// <item>SysML XMI — <see href="https://github.com/mtconnect/mtconnect_sysml_model"/>
    /// (UML class hierarchy under
    /// <c>MTConnectSysMLModel/Devices/Configurations</c>).</item>
    /// <item>XSD — <see href="https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd"/>
    /// (xs:choice between Axis/AxisDataSet, Origin/OriginDataSet, etc.).</item>
    /// </list>
    /// </remarks>
    [TestFixture]
    public class PolymorphicLeafCoverageTests
    {
        private static readonly Assembly CommonAssembly = typeof(IDataSet).Assembly;

        private static readonly (Type Abstract, Type Simple, Type DataSet, Type ISimple, Type IDataSet)[] LeafShapes =
        {
            (typeof(IAbstractAxis), typeof(Axis), typeof(AxisDataSet), typeof(IAxis), typeof(IAxisDataSet)),
            (typeof(IAbstractOrigin), typeof(Origin), typeof(OriginDataSet), typeof(IOrigin), typeof(IOriginDataSet)),
            (typeof(IAbstractRotation), typeof(Rotation), typeof(RotationDataSet), typeof(IRotation), typeof(IRotationDataSet)),
            (typeof(IAbstractScale), typeof(Scale), typeof(ScaleDataSet), typeof(IScale), typeof(IScaleDataSet)),
            (typeof(IAbstractTranslation), typeof(Translation), typeof(TranslationDataSet), typeof(ITranslation), typeof(ITranslationDataSet)),
        };

        public static IEnumerable<TestCaseData> AbstractInterfaces() =>
            LeafShapes.Select(s => new TestCaseData(s.Abstract).SetName(s.Abstract.Name));

        public static IEnumerable<TestCaseData> SimpleLeaves() =>
            LeafShapes.Select(s => new TestCaseData(s.Simple, s.Abstract, s.ISimple).SetName(s.Simple.Name));

        public static IEnumerable<TestCaseData> DataSetLeaves() =>
            LeafShapes.Select(s => new TestCaseData(s.DataSet, s.Abstract, s.ISimple, s.IDataSet).SetName(s.DataSet.Name));

        // ---------------- positive ----------------

        [TestCaseSource(nameof(AbstractInterfaces))]
        public void Abstract_interface_has_exactly_two_implementing_classes(Type abstractInterface)
        {
            var implementors = CommonAssembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false }
                            && abstractInterface.IsAssignableFrom(t))
                .ToList();

            Assert.That(implementors.Count, Is.EqualTo(2),
                $"{abstractInterface.Name} must have exactly two concrete implementors; "
                + $"found {implementors.Count}: {string.Join(", ", implementors.Select(t => t.Name))}");
        }

        [TestCaseSource(nameof(SimpleLeaves))]
        public void Simple_leaf_implements_simple_interface_and_inherits_abstract_interface(
            Type simpleType, Type abstractInterface, Type simpleInterface)
        {
            Assert.That(simpleType.IsClass, Is.True);
            Assert.That(simpleType.IsAbstract, Is.False);
            Assert.That(simpleInterface.IsAssignableFrom(simpleType), Is.True,
                $"{simpleType.Name} must implement {simpleInterface.Name}");
            Assert.That(abstractInterface.IsAssignableFrom(simpleType), Is.True,
                $"{simpleType.Name} must inherit {abstractInterface.Name}");
        }

        [TestCaseSource(nameof(SimpleLeaves))]
        public void Simple_leaf_does_not_implement_IDataSet(
            Type simpleType, Type abstractInterface, Type simpleInterface)
        {
            Assert.That(typeof(IDataSet).IsAssignableFrom(simpleType), Is.False,
                $"{simpleType.Name} (simple variant) must NOT implement IDataSet");
        }

        [TestCaseSource(nameof(SimpleLeaves))]
        public void Simple_leaf_constructs_via_parameterless_ctor(
            Type simpleType, Type abstractInterface, Type simpleInterface)
        {
            object? instance = null;
            Assert.DoesNotThrow(() => instance = Activator.CreateInstance(simpleType),
                $"{simpleType.Name} must have a public parameterless ctor");
            Assert.That(instance, Is.Not.Null);
        }

        [TestCaseSource(nameof(DataSetLeaves))]
        public void DataSet_leaf_implements_dataset_simple_and_abstract_interfaces(
            Type dataSetType, Type abstractInterface, Type simpleInterface, Type dataSetInterface)
        {
            Assert.That(dataSetInterface.IsAssignableFrom(dataSetType), Is.True,
                $"{dataSetType.Name} must implement {dataSetInterface.Name}");
            Assert.That(abstractInterface.IsAssignableFrom(dataSetType), Is.True,
                $"{dataSetType.Name} must inherit {abstractInterface.Name}");
            Assert.That(typeof(IDataSet).IsAssignableFrom(dataSetType), Is.True,
                $"{dataSetType.Name} must implement IDataSet");
        }

        [TestCaseSource(nameof(DataSetLeaves))]
        public void DataSet_leaf_constructs_via_parameterless_ctor(
            Type dataSetType, Type abstractInterface, Type simpleInterface, Type dataSetInterface)
        {
            object? instance = null;
            Assert.DoesNotThrow(() => instance = Activator.CreateInstance(dataSetType),
                $"{dataSetType.Name} must have a public parameterless ctor");
            Assert.That(instance, Is.Not.Null);
        }

        // ---------------- negative ----------------

        [TestCaseSource(nameof(AbstractInterfaces))]
        public void Abstract_interface_carries_no_member_definitions(Type abstractInterface)
        {
            // The Abstract* interfaces are pure type-discriminator markers
            // per the SysML 2.7 model (no shared member surface). Verify
            // the regen produces them as empty interfaces — a mistakenly
            // promoted member would couple consumers to one variant's
            // shape and break LSP for the polymorphic property.
            var members = abstractInterface.GetMembers(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            Assert.That(members, Is.Empty,
                $"{abstractInterface.Name} must declare no members; found: "
                + string.Join(", ", members.Select(m => m.Name)));
        }

        [TestCaseSource(nameof(SimpleLeaves))]
        public void Simple_leaf_inherits_AbstractBase(
            Type simpleType, Type abstractInterface, Type simpleInterface)
        {
            // The simple variants extend AbstractAxis/AbstractOrigin/etc.
            // — pin so a future regen that flattens the hierarchy fails
            // here.
            Assert.That(simpleType.BaseType, Is.Not.Null);
            Assert.That(simpleType.BaseType!.IsAbstract, Is.True,
                $"{simpleType.Name}'s base type ({simpleType.BaseType.Name}) must be abstract");
            Assert.That(simpleType.BaseType.Name, Does.StartWith("Abstract"));
        }

        [TestCaseSource(nameof(DataSetLeaves))]
        public void DataSet_leaf_inherits_AbstractBase(
            Type dataSetType, Type abstractInterface, Type simpleInterface, Type dataSetInterface)
        {
            Assert.That(dataSetType.BaseType, Is.Not.Null);
            Assert.That(dataSetType.BaseType!.IsAbstract, Is.True,
                $"{dataSetType.Name}'s base type ({dataSetType.BaseType.Name}) must be abstract");
            Assert.That(dataSetType.BaseType.Name, Does.StartWith("Abstract"));
        }
    }
}
