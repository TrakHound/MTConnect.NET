// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using NUnit.Framework;

namespace MTConnect.Tests.JsonCppagent
{
    /// <summary>
    /// <c>JsonFunctions.ConvertStream</c> must return its stream rewound to the start. Callers
    /// that hand the stream to a reader which starts at the current position (for example the
    /// MQTT payload builder) otherwise read nothing and publish an empty payload (issue #117).
    /// </summary>
    [TestFixture]
    public class JsonFunctionsConvertStreamTests
    {
        /// <summary>The returned stream is positioned at the start and holds the serialized JSON.</summary>
        [Test]
        public void ConvertStream_Returns_Stream_At_Start()
        {
            using var stream = MTConnect.JsonFunctions.ConvertStream(new { name = "value" });

            Assert.That(stream, Is.Not.Null);
            Assert.That(stream!.Length, Is.GreaterThan(0));
            Assert.That(stream.Position, Is.EqualTo(0), "the stream must be readable from its current position");

            using var reader = new StreamReader(stream);
            Assert.That(reader.ReadToEnd(), Does.Contain("\"name\"").And.Contain("value"));
        }
    }
}
