using MTConnect.Devices.Configurations;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.V2_6_V2_7
{
    // Pins the v2.7 Configuration sub-element family: new geometric primitives
    // (Axis, Origin, Rotation, Scale, Translation) and their data-set
    // representation siblings (*DataSet) — plus the cross-package-grafted
    // DataSet base that the universal cross-package parent resolver brought
    // into the Devices.Configurations namespace.
    [TestFixture]
    public class V2_7ConfigurationDataSetTests
    {
        // The DataSet base (grafted from Observation.Representations via the
        // universal resolver) compiles, instantiates, and surfaces its
        // const description.
        [Test]
        public void DataSet_base_constructs_and_implements_IDataSet()
        {
            var ds = new DataSet();
            Assert.That(ds, Is.InstanceOf<IDataSet>());
            Assert.That(DataSet.DescriptionText, Is.Not.Null.And.Not.Empty);
        }

        // The five concrete sub-types follow the same shape: parameterless ctor,
        // populates X/Y/Z (or A/B/C) fields, implements IDataSet (interface,
        // not the concrete DataSet base — *DataSet types polymorphically
        // extend their Abstract<Leaf> base, gaining IDataSet as a marker
        // interface so XML/JSON serialisers can narrow on it).
        [Test]
        public void AxisDataSet_has_xyz_fields_and_implements_IDataSet()
        {
            var a = new AxisDataSet { X = 1.0, Y = 2.0, Z = 3.0 };
            Assert.That(a, Is.InstanceOf<IDataSet>());
            Assert.That(a, Is.InstanceOf<IAxisDataSet>());
            Assert.That(a.X, Is.EqualTo(1.0));
            Assert.That(a.Y, Is.EqualTo(2.0));
            Assert.That(a.Z, Is.EqualTo(3.0));
        }

        [Test]
        public void OriginDataSet_has_xyz_fields_and_implements_IDataSet()
        {
            var o = new OriginDataSet { X = "1", Y = "2", Z = "3" };
            Assert.That(o, Is.InstanceOf<IDataSet>());
            Assert.That(o, Is.InstanceOf<IOriginDataSet>());
        }

        [Test]
        public void RotationDataSet_has_abc_fields_and_implements_IDataSet()
        {
            // Rotations are reported as A (about X), B (about Y), C (about Z).
            var r = new RotationDataSet { A = "10", B = "20", C = "30" };
            Assert.That(r, Is.InstanceOf<IDataSet>());
            Assert.That(r, Is.InstanceOf<IRotationDataSet>());
        }

        [Test]
        public void ScaleDataSet_implements_IDataSet()
        {
            var s = new ScaleDataSet();
            Assert.That(s, Is.InstanceOf<IDataSet>());
            Assert.That(s, Is.InstanceOf<IScaleDataSet>());
        }

        [Test]
        public void TranslationDataSet_implements_IDataSet()
        {
            var t = new TranslationDataSet();
            Assert.That(t, Is.InstanceOf<IDataSet>());
            Assert.That(t, Is.InstanceOf<ITranslationDataSet>());
        }

        // Concrete (non-DataSet) representations of the same primitives, also
        // landed in v2.7 alongside their DataSet siblings.
        [Test]
        public void Axis_inherits_AbstractAxis_and_constructs()
        {
            var a = new Axis { Value = "X" };
            Assert.That(a, Is.InstanceOf<AbstractAxis>());
            Assert.That(a, Is.InstanceOf<IAxis>());
            Assert.That(a.Value, Is.EqualTo("X"));
        }

        [Test]
        public void Origin_inherits_AbstractOrigin()
        {
            var o = new Origin();
            Assert.That(o, Is.InstanceOf<AbstractOrigin>());
            Assert.That(o, Is.InstanceOf<IOrigin>());
        }

        [Test]
        public void Rotation_inherits_AbstractRotation()
        {
            Assert.That(new Rotation(), Is.InstanceOf<AbstractRotation>());
        }

        [Test]
        public void Scale_inherits_AbstractScale()
        {
            Assert.That(new Scale(), Is.InstanceOf<AbstractScale>());
        }

        [Test]
        public void Translation_inherits_AbstractTranslation()
        {
            Assert.That(new Translation(), Is.InstanceOf<AbstractTranslation>());
        }

        // The Abstract* bases are abstract — verify so a future regen that
        // accidentally drops the abstract modifier trips here.
        [Test]
        public void AbstractAxis_is_abstract()
        {
            Assert.That(typeof(AbstractAxis).IsAbstract, Is.True);
        }

        [Test]
        public void AbstractOrigin_is_abstract()
        {
            Assert.That(typeof(AbstractOrigin).IsAbstract, Is.True);
        }

        [Test]
        public void AbstractRotation_is_abstract()
        {
            Assert.That(typeof(AbstractRotation).IsAbstract, Is.True);
        }

        [Test]
        public void AbstractScale_is_abstract()
        {
            Assert.That(typeof(AbstractScale).IsAbstract, Is.True);
        }

        [Test]
        public void AbstractTranslation_is_abstract()
        {
            Assert.That(typeof(AbstractTranslation).IsAbstract, Is.True);
        }
    }
}
