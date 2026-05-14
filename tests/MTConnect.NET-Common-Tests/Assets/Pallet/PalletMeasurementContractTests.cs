// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Reflection;
using MTConnect.Assets.Pallet;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Assets.Pallet
{
    /// <summary>
    /// Pins the class-level contract that every Pallet measurement
    /// subclass renders with the rich <c>TypeId</c> + three-constructor
    /// scaffolding produced by the SysML measurement template, in
    /// parity with the long-standing CuttingTools measurement DTOs.
    ///
    /// Per the MTConnect SysML model, the <c>PhysicalAsset &gt; Pallet
    /// &gt; Measurements</c> package declares ten concrete measurement
    /// subclasses (Weight, Height, Width, Length, Swing plus their
    /// Loaded* counterparts) that generalize from an abstract
    /// <c>Measurement</c> base. The C# generator must emit each one
    /// with:
    /// <list type="bullet">
    ///   <item>a <c>const string TypeId</c> equal to the SysML class name;</item>
    ///   <item>a default constructor that stamps <c>Type = TypeId</c>;</item>
    ///   <item>a <c>(double value)</c> constructor that stamps both
    ///   <c>Type</c> and <c>Value</c>;</item>
    ///   <item>a <c>(IMeasurement)</c> copy constructor chaining to
    ///   the partial base.</item>
    /// </list>
    ///
    /// Sources:
    /// - SysML XMI: <https://github.com/mtconnect/mtconnect_sysml_model>
    ///   v2.7. The Pallet measurement subclasses sit under UML package
    ///   "Asset Information Model > Pallet > Measurements" with the
    ///   abstract base UML ID
    ///   <c>_2024x_68e0225_1727793846441_986747_23754</c>.
    /// - Reference implementation: cppagent's generic
    ///   <c>PhysicalAsset::getMeasurementsFactory()</c> handles every
    ///   physical-asset measurement element via a single regex-matched
    ///   factory; the per-type DTO scaffolding is a .NET-side ergonomic
    ///   convenience that mirrors what CuttingTools already gets.
    /// </summary>
    [TestFixture]
    [Category("PalletMeasurementContract")]
    public class PalletMeasurementContractTests
    {
        private static readonly (Type Type, string ExpectedTypeId)[] _palletMeasurements = new[]
        {
            (typeof(WeightMeasurement),       "Weight"),
            (typeof(HeightMeasurement),       "Height"),
            (typeof(WidthMeasurement),        "Width"),
            (typeof(LengthMeasurement),       "Length"),
            (typeof(SwingMeasurement),        "Swing"),
            (typeof(LoadedWeightMeasurement), "LoadedWeight"),
            (typeof(LoadedHeightMeasurement), "LoadedHeight"),
            (typeof(LoadedWidthMeasurement),  "LoadedWidth"),
            (typeof(LoadedLengthMeasurement), "LoadedLength"),
            (typeof(LoadedSwingMeasurement),  "LoadedSwing"),
        };

        private static System.Collections.Generic.IEnumerable<TestCaseData> PalletMeasurementCases =>
            _palletMeasurements.Select(p => new TestCaseData(p.Type, p.ExpectedTypeId)
                .SetName($"{{m}}({p.Type.Name})"));

        [Test]
        [TestCaseSource(nameof(PalletMeasurementCases))]
        public void TypeId_Const_Equals_SysML_ClassName(Type measurementType, string expectedTypeId)
        {
            // The TypeId const is the wire-side discriminator the
            // serializer uses to round-trip the measurement element
            // name. It must equal the SysML class name verbatim.
            var field = measurementType.GetField(
                "TypeId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.That(field, Is.Not.Null,
                $"{measurementType.Name} must expose a public static TypeId const field");
            Assert.That(field!.IsLiteral, Is.True,
                $"{measurementType.Name}.TypeId must be a const literal");
            Assert.That(field.FieldType, Is.EqualTo(typeof(string)),
                $"{measurementType.Name}.TypeId must be a string");
            Assert.That(field.GetRawConstantValue(), Is.EqualTo(expectedTypeId),
                $"{measurementType.Name}.TypeId must equal '{expectedTypeId}'");
        }

        [Test]
        [TestCaseSource(nameof(PalletMeasurementCases))]
        public void CodeId_Const_Is_Empty(Type measurementType, string expectedTypeId)
        {
            // Pallet measurements carry no MeasurementCode (the SysML
            // Pallet Measurement abstract class has no `code` property,
            // unlike the CuttingTool ToolingMeasurement which binds to
            // MeasurementCodeEnum). The CodeId const must therefore be
            // emitted as the empty string.
            var field = measurementType.GetField(
                "CodeId",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.That(field, Is.Not.Null,
                $"{measurementType.Name} must expose a public static CodeId const field");
            Assert.That(field!.IsLiteral, Is.True,
                $"{measurementType.Name}.CodeId must be a const literal");
            Assert.That(field.GetRawConstantValue(), Is.EqualTo(string.Empty),
                $"{measurementType.Name}.CodeId must be empty for Pallet measurements");
        }

        [Test]
        [TestCaseSource(nameof(PalletMeasurementCases))]
        public void Default_Constructor_Stamps_Type(Type measurementType, string expectedTypeId)
        {
            var ctor = measurementType.GetConstructor(Type.EmptyTypes);
            Assert.That(ctor, Is.Not.Null,
                $"{measurementType.Name} must declare a public default constructor");

            var instance = (Measurement)ctor!.Invoke(Array.Empty<object>());

            Assert.That(instance.Type, Is.EqualTo(expectedTypeId),
                $"the default constructor must stamp Type = '{expectedTypeId}'");
        }

        [Test]
        [TestCaseSource(nameof(PalletMeasurementCases))]
        public void DoubleValue_Constructor_Stamps_Type_And_Value(Type measurementType, string expectedTypeId)
        {
            var ctor = measurementType.GetConstructor(new[] { typeof(double) });
            Assert.That(ctor, Is.Not.Null,
                $"{measurementType.Name} must declare a public (double) constructor");

            const double sentinelValue = 123.45;
            var instance = (Measurement)ctor!.Invoke(new object[] { sentinelValue });

            Assert.That(instance.Type, Is.EqualTo(expectedTypeId),
                $"the (double) constructor must stamp Type = '{expectedTypeId}'");
            Assert.That(instance.Value, Is.EqualTo(sentinelValue),
                $"the (double) constructor must stamp Value");
        }

        [Test]
        [TestCaseSource(nameof(PalletMeasurementCases))]
        public void IMeasurement_Constructor_Copies_Fields_And_Stamps_Type(Type measurementType, string expectedTypeId)
        {
            var ctor = measurementType.GetConstructor(new[] { typeof(IMeasurement) });
            Assert.That(ctor, Is.Not.Null,
                $"{measurementType.Name} must declare a public (IMeasurement) constructor");

            // Build a source measurement with every transferable field
            // populated so the copy constructor's behavior can be
            // verified end-to-end. WeightMeasurement is used as the
            // source because every concrete subtype shares the same
            // base Measurement contract.
            var source = new WeightMeasurement(99.0)
            {
                Nominal = 100.0,
                Minimum = 50.0,
                Maximum = 150.0,
                SignificantDigits = 4,
                NativeUnits = "MILLIGRAM",
                Units = "KILOGRAM",
            };

            var instance = (Measurement)ctor!.Invoke(new object[] { source });

            Assert.That(instance.Type, Is.EqualTo(expectedTypeId),
                $"the (IMeasurement) constructor must stamp Type = '{expectedTypeId}'");
            Assert.That(instance.Value, Is.EqualTo(99.0),
                "Value must be copied from source");
            Assert.That(instance.Nominal, Is.EqualTo(100.0),
                "Nominal must be copied from source");
            Assert.That(instance.Minimum, Is.EqualTo(50.0),
                "Minimum must be copied from source");
            Assert.That(instance.Maximum, Is.EqualTo(150.0),
                "Maximum must be copied from source");
            Assert.That(instance.SignificantDigits, Is.EqualTo(4),
                "SignificantDigits must be copied from source");
            Assert.That(instance.NativeUnits, Is.EqualTo("MILLIGRAM"),
                "NativeUnits must be copied from source");
            Assert.That(instance.Units, Is.EqualTo("KILOGRAM"),
                "Units must be copied from source");
        }

        [Test]
        [TestCaseSource(nameof(PalletMeasurementCases))]
        public void Concrete_Subclass_Derives_From_Base_Measurement(Type measurementType, string expectedTypeId)
        {
            // The rich template chains the (IMeasurement) ctor to the
            // base Measurement(IMeasurement). Pinning the inheritance
            // chain here documents that contract so a future refactor
            // re-rooting the subclasses elsewhere fails this test
            // before silently breaking the constructor chain.
            Assert.That(measurementType.BaseType, Is.EqualTo(typeof(Measurement)),
                $"{measurementType.Name} must directly derive from MTConnect.Assets.Pallet.Measurement");
        }
    }
}
