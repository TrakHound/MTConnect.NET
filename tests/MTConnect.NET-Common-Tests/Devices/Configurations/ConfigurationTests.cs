// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using MTConnect.Devices.Configurations;
using MTConnect.Tests.Common.TestHelpers;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Devices.Configurations
{
    // Version-gated shape assertions for the v2.7 Configuration sub-element
    // family: new geometric primitives (Axis, Origin, Rotation, Scale,
    // Translation) and their data-set representation siblings (*DataSet),
    // plus the cross-package-grafted DataSet base that the universal
    // cross-package parent resolver brought into the Devices.Configurations
    // namespace.
    //
    //   - XMI: https://github.com/mtconnect/mtconnect_sysml_model @ tag v2.7
    //          UML classes under Device Information Model > Configurations:
    //            * Axis / AxisDataSet
    //            * Origin / OriginDataSet
    //            * Rotation / RotationDataSet
    //            * Scale / ScaleDataSet
    //            * Translation / TranslationDataSet
    //          plus the abstract bases (AbstractAxis, AbstractOrigin, etc.).
    //   - XSD: https://schemas.mtconnect.org/schemas/MTConnectDevices_2.7.xsd
    //          (the geometric-primitive complexTypes encode the same shape
    //          on the wire under <Configuration>).
    //   - Prose: MTConnect Standard Part_2.0_Devices_v2.7 section 10
    //          "Configuration" — describes how Component-level Configuration
    //          carries the geometric primitives that locate a Component in
    //          space.
    //
    // Every fixture below is matrix-parameterised over
    // MTConnectVersionMatrix.All per plan Design Decision D1
    // (2026-08-19). Assume.That gates every assertion to v2.7 (the version
    // that introduced the Configuration family); rows below the floor
    // surface as Inconclusive.
    /// <summary>Pins the behavior expressed by the test name: configuration tests.</summary>
    [TestFixture]
    public class ConfigurationTests
    {
        // The DataSet base (grafted from Observation.Representations via the
        // universal resolver) compiles, instantiates, and surfaces its
        // const description.
        /// <summary>Pins the behavior expressed by the test name: data set base constructs and implements i data set.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void DataSet_base_constructs_and_implements_IDataSet(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "DataSet was grafted into Devices.Configurations in MTConnect v2.7.");

            var ds = new DataSet();
            Assert.That(ds, Is.InstanceOf<IDataSet>());
            Assert.That(DataSet.DescriptionText, Is.Not.Null.And.Not.Empty);
        }

        // The five concrete sub-types follow the same shape: parameterless
        // ctor, populates X/Y/Z (or A/B/C) fields, implements IDataSet
        // (interface, not the concrete DataSet base — *DataSet types
        // polymorphically extend their Abstract<Leaf> base, gaining IDataSet
        // as a marker interface so XML/JSON serializers can narrow on it).
        /// <summary>Pins the behavior expressed by the test name: axis data set has xyz fields and implements i data set.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AxisDataSet_has_xyz_fields_and_implements_IDataSet(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "AxisDataSet was introduced in MTConnect v2.7.");

            var a = new AxisDataSet { X = 1.0, Y = 2.0, Z = 3.0 };
            Assert.That(a, Is.InstanceOf<IDataSet>());
            Assert.That(a, Is.InstanceOf<IAxisDataSet>());
            Assert.That(a.X, Is.EqualTo(1.0));
            Assert.That(a.Y, Is.EqualTo(2.0));
            Assert.That(a.Z, Is.EqualTo(3.0));
        }

        /// <summary>Pins the behavior expressed by the test name: origin data set has xyz fields and implements i data set.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void OriginDataSet_has_xyz_fields_and_implements_IDataSet(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "OriginDataSet was introduced in MTConnect v2.7.");

            var o = new OriginDataSet { X = "1", Y = "2", Z = "3" };
            Assert.That(o, Is.InstanceOf<IDataSet>());
            Assert.That(o, Is.InstanceOf<IOriginDataSet>());
        }

        /// <summary>Pins the behavior expressed by the test name: rotation data set has abc fields and implements i data set.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void RotationDataSet_has_abc_fields_and_implements_IDataSet(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "RotationDataSet was introduced in MTConnect v2.7.");

            // Rotations are reported as A (about X), B (about Y), C (about Z).
            var r = new RotationDataSet { A = "10", B = "20", C = "30" };
            Assert.That(r, Is.InstanceOf<IDataSet>());
            Assert.That(r, Is.InstanceOf<IRotationDataSet>());
        }

        /// <summary>Pins the behavior expressed by the test name: scale data set implements i data set.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void ScaleDataSet_implements_IDataSet(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "ScaleDataSet was introduced in MTConnect v2.7.");

            var s = new ScaleDataSet();
            Assert.That(s, Is.InstanceOf<IDataSet>());
            Assert.That(s, Is.InstanceOf<IScaleDataSet>());
        }

        /// <summary>Pins the behavior expressed by the test name: translation data set implements i data set.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void TranslationDataSet_implements_IDataSet(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "TranslationDataSet was introduced in MTConnect v2.7.");

            var t = new TranslationDataSet();
            Assert.That(t, Is.InstanceOf<IDataSet>());
            Assert.That(t, Is.InstanceOf<ITranslationDataSet>());
        }

        // Concrete (non-DataSet) representations of the same primitives,
        // also landed in v2.7 alongside their DataSet siblings.
        /// <summary>Pins the behavior expressed by the test name: axis inherits abstract axis and constructs.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Axis_inherits_AbstractAxis_and_constructs(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "Axis was introduced in MTConnect v2.7.");

            var a = new Axis { Value = "X" };
            Assert.That(a, Is.InstanceOf<AbstractAxis>());
            Assert.That(a, Is.InstanceOf<IAxis>());
            Assert.That(a.Value, Is.EqualTo("X"));
        }

        /// <summary>Pins the behavior expressed by the test name: origin inherits abstract origin.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Origin_inherits_AbstractOrigin(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "Origin was introduced in MTConnect v2.7.");

            var o = new Origin();
            Assert.That(o, Is.InstanceOf<AbstractOrigin>());
            Assert.That(o, Is.InstanceOf<IOrigin>());
        }

        /// <summary>Pins the behavior expressed by the test name: rotation inherits abstract rotation.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Rotation_inherits_AbstractRotation(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "Rotation was introduced in MTConnect v2.7.");

            Assert.That(new Rotation(), Is.InstanceOf<AbstractRotation>());
        }

        /// <summary>Pins the behavior expressed by the test name: scale inherits abstract scale.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Scale_inherits_AbstractScale(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "Scale was introduced in MTConnect v2.7.");

            Assert.That(new Scale(), Is.InstanceOf<AbstractScale>());
        }

        /// <summary>Pins the behavior expressed by the test name: translation inherits abstract translation.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void Translation_inherits_AbstractTranslation(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "Translation was introduced in MTConnect v2.7.");

            Assert.That(new Translation(), Is.InstanceOf<AbstractTranslation>());
        }

        // The Abstract* bases are abstract — verify so a future regen that
        // accidentally drops the abstract modifier trips here.
        /// <summary>Pins the behavior expressed by the test name: abstract axis is abstract.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AbstractAxis_is_abstract(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "AbstractAxis was introduced in MTConnect v2.7.");

            Assert.That(typeof(AbstractAxis).IsAbstract, Is.True);
        }

        /// <summary>Pins the behavior expressed by the test name: abstract origin is abstract.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AbstractOrigin_is_abstract(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "AbstractOrigin was introduced in MTConnect v2.7.");

            Assert.That(typeof(AbstractOrigin).IsAbstract, Is.True);
        }

        /// <summary>Pins the behavior expressed by the test name: abstract rotation is abstract.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AbstractRotation_is_abstract(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "AbstractRotation was introduced in MTConnect v2.7.");

            Assert.That(typeof(AbstractRotation).IsAbstract, Is.True);
        }

        /// <summary>Pins the behavior expressed by the test name: abstract scale is abstract.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AbstractScale_is_abstract(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "AbstractScale was introduced in MTConnect v2.7.");

            Assert.That(typeof(AbstractScale).IsAbstract, Is.True);
        }

        /// <summary>Pins the behavior expressed by the test name: abstract translation is abstract.</summary>
        /// <param name="v">The MTConnect Standard version under test.</param>
        [TestCaseSource(typeof(MTConnectVersionMatrix), nameof(MTConnectVersionMatrix.All))]
        public void AbstractTranslation_is_abstract(Version v)
        {
            Assume.That(v, Is.GreaterThanOrEqualTo(MTConnectVersions.Version27),
                "AbstractTranslation was introduced in MTConnect v2.7.");

            Assert.That(typeof(AbstractTranslation).IsAbstract, Is.True);
        }
    }
}
