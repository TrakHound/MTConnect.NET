// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO;
using MTConnect.Formatters;
using MTConnect.Input;
using NUnit.Framework;

namespace MTConnect.Tests.Json
{
    /// <summary>
    /// The JSON input formatter feeds the adapter's MQTT module, which publishes the result's
    /// stream from its current position. The stream therefore has to come back rewound (issue #117).
    /// </summary>
    [TestFixture]
    public class JsonInputFormatterStreamTests
    {
        /// <summary>Formatted observations are readable from the stream's current position.</summary>
        [Test]
        public void Format_Observations_Content_Is_Readable_From_Current_Position()
        {
            var formatter = new JsonInputFormatter();
            var observations = new IObservationInput[] { new ObservationInput("avail", "AVAILABLE") };

            var result = formatter.Format(observations);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Content.Length - result.Content.Position, Is.GreaterThan(0),
                "no bytes left to read means an empty MQTT payload");

            using var reader = new StreamReader(result.Content);
            Assert.That(reader.ReadToEnd(), Does.Contain("AVAILABLE"));
        }
    }
}
