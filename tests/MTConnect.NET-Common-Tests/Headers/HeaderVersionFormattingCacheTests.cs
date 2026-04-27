// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.

// Pins the caching contract for the broker's `Header.version`
// formatter. Each Devices/Streams/Assets/Error response on a hot
// path passes the same configured Version through the formatter,
// so the formatted four-segment string must be memoized rather
// than allocated per response.
//
// Independent of HeaderVersionRegressionTests (which pins the
// emitted value): this fixture pins the *identity* of the
// returned string across repeated calls to prove the cache.

using System;
using System.Reflection;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Headers
{
    [TestFixture]
    public class HeaderVersionFormattingCacheTests
    {
        private static MethodInfo GetFormatter()
        {
            var brokerType = typeof(MTConnect.Agents.MTConnectAgentBroker);
            var method = brokerType.GetMethod(
                "FormatHeaderVersion",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null,
                "MTConnectAgentBroker.FormatHeaderVersion(Version) must exist as a private static method.");
            return method!;
        }

        private static string Invoke(MethodInfo formatter, Version version)
        {
            return (string)formatter.Invoke(null, new object[] { version })!;
        }

        [Test]
        public void FormatHeaderVersion_returns_same_string_instance_on_repeated_calls_for_same_version()
        {
            var formatter = GetFormatter();
            var version = new Version(2, 5);

            var first = Invoke(formatter, version);
            var second = Invoke(formatter, version);

            Assert.That(second, Is.SameAs(first),
                "Repeated calls with the same Version must return the cached string instance, not allocate a new one.");
        }

        [Test]
        public void FormatHeaderVersion_returns_same_string_instance_for_distinct_but_equal_version_instances()
        {
            var formatter = GetFormatter();
            var versionA = new Version(2, 7);
            var versionB = new Version(2, 7);
            Assert.That(versionA, Is.Not.SameAs(versionB), "Sanity: two distinct Version instances under test.");
            Assert.That(versionA, Is.EqualTo(versionB), "Sanity: the two Version instances must be Equals-equal.");

            var first = Invoke(formatter, versionA);
            var second = Invoke(formatter, versionB);

            Assert.That(second, Is.SameAs(first),
                "Cache must key on Version equality, not reference identity, so equal Version instances reuse the formatted string.");
        }

        [Test]
        public void FormatHeaderVersion_caches_independently_per_version()
        {
            var formatter = GetFormatter();

            var v25 = Invoke(formatter, new Version(2, 5));
            var v27 = Invoke(formatter, new Version(2, 7));

            Assert.That(v25, Is.Not.EqualTo(v27),
                "Different MTConnect releases must produce different formatted strings.");

            var v25Again = Invoke(formatter, new Version(2, 5));
            var v27Again = Invoke(formatter, new Version(2, 7));

            Assert.That(v25Again, Is.SameAs(v25));
            Assert.That(v27Again, Is.SameAs(v27));
        }
    }
}
