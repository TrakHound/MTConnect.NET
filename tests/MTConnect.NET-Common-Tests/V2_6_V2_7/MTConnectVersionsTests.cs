using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.V2_6_V2_7
{
    // Constants-level pins on `MTConnectVersions` for v2.6 and v2.7.
    //
    //   - XMI:   https://github.com/mtconnect/mtconnect_sysml_model/tree/v2.6
    //                                                                   /v2.7
    //            (the SysML model defines the version constants the .NET layer
    //            mirrors here as a static class.)
    //   - XSD:   https://schemas.mtconnect.org/schemas/MTConnectDevices_2.6.xsd
    //                                                  MTConnectDevices_2.7.xsd
    //            (each XSD's targetNamespace embeds the version it represents.)
    //   - Prose: MTConnect Standard `Part_1.0_Overview_v2.7.pdf` §1 "Versioning"
    //            (the document numbering scheme — v1.0 through v2.7 with v1.9
    //            intentionally skipped — is described here.)
    [TestFixture]
    public class MTConnectVersionsTests
    {
        // Source: MTConnect SysML model, tag v2.6.
        // The model's version-list element introduces 2.6 between 2.5 and (later)
        // 2.7 with no in-between fractional versions.
        [Test]
        public void Version26_constant_equals_2_6()
        {
            Assert.That(MTConnectVersions.Version26, Is.EqualTo(new Version(2, 6)));
        }

        // Source: MTConnect SysML model, tag v2.7.
        [Test]
        public void Version27_constant_equals_2_7()
        {
            Assert.That(MTConnectVersions.Version27, Is.EqualTo(new Version(2, 7)));
        }

        // Locks Max to Version27 against accidental rollback.
        [Test]
        public void Max_equals_Version27()
        {
            Assert.That(MTConnectVersions.Max, Is.EqualTo(MTConnectVersions.Version27));
        }

        // Pin that the version list contains no 1.9 entry.
        // Source: MTConnect Standard Part_1.0_Overview prose §1 "Versioning";
        // confirmed by the absence of an XMI tag `v1.9` in
        // `mtconnect/mtconnect_sysml_model` (tags: v2.5 b61907fb78,
        // v2.6 08185447bf, v2.7 25796ac591).
        [Test]
        public void Every_published_version_constant_is_distinct_and_monotonic()
        {
            var versions = typeof(MTConnectVersions)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Version))
                .Select(f => (Name: f.Name, Value: (Version)f.GetValue(null)!))
                .OrderBy(x => x.Value)
                .ToList();

            // 17 expected: v1.0-v1.8 (9) + v2.0-v2.7 (8). The Standard skipped v1.9
            // entirely so there is no Version19 constant.
            Assert.That(versions.Count, Is.EqualTo(17),
                "Expected 17 version constants (v1.0-v1.8 plus v2.0-v2.7). Got " +
                string.Join(", ", versions.Select(x => x.Name)));

            var distinct = versions.Select(x => x.Value).Distinct().Count();
            Assert.That(distinct, Is.EqualTo(versions.Count),
                "Two constants share the same Version value");

            for (int i = 1; i < versions.Count; i++)
            {
                Assert.That(versions[i].Value, Is.GreaterThan(versions[i - 1].Value),
                    $"{versions[i].Name} ({versions[i].Value}) should be > {versions[i - 1].Name} ({versions[i - 1].Value})");
            }

            Assert.That(versions.First().Value, Is.EqualTo(new Version(1, 0)));
            Assert.That(versions.Last().Value, Is.EqualTo(MTConnectVersions.Max));
        }

        // Pin that no `Version19` constant exists. Asserts on the named field —
        // silent insertion would invalidate downstream matrices.
        // Source: MTConnect SysML model tag list — no `v1.9` tag.
        [Test]
        public void Version19_field_does_not_exist()
        {
            var version19 = typeof(MTConnectVersions)
                .GetField("Version19", BindingFlags.Public | BindingFlags.Static);
            Assert.That(version19, Is.Null,
                "MTConnectVersions.Version19 must not exist — the MTConnect Standard skipped v1.9.");
        }
    }
}
