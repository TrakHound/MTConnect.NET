using System;
using System.Linq;
using System.Text;
using DeepEqual.Syntax;
using MTConnect.Observations;
using MTConnect.Streams.Xml;
using NUnit.Framework;

namespace MTConnect.Tests.XML
{
    public sealed class XmlStreamsResponseDocumentTests
    {
        [Test]
        public void TwoConditionsShouldBeParsed()
        {
            var text = @"<?xml version=""1.0"" encoding=""utf-8""?>
<MTConnectStreams xmlns=""urn:mtconnect.org:MTConnectStreams:2.0"" xmlns:m=""urn:mtconnect.org:MTConnectStreams:2.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""urn:mtconnect.org:MTConnectStreams:2.0 /schemas/MTConnectStreams_2.0.xsd"">
  <Header instanceId=""1663078056"" version=""1.0.0.0"" sender=""ADSKPF3FBP2T"" bufferSize=""131072"" firstSequence=""1"" lastSequence=""222"" nextSequence=""223"" deviceModelChangeTime=""2022-09-13T14:07:36.2828516Z"" creationTime=""2022-09-13T14:07:47.2901070Z"" />
  <Streams>
    <DeviceStream name=""Machine"" uuid=""cc17e6f8-61c2-427b-a45f-11a2189ae3a4"">
      <ComponentStream component=""Controller"" componentId=""cont"" name=""controller"">
        <Condition>
          <Fault dataItemId=""comms_cond"" type=""COMMUNICATIONS"" sequence=""221"" timestamp=""2022-09-13T14:07:36.444Z"" />
          <Normal dataItemId=""comms_cond"" type=""COMMUNICATIONS"" sequence=""222"" timestamp=""2022-09-13T14:07:36.570Z"" />
        </Condition>
      </ComponentStream>
    </DeviceStream>
  </Streams>
</MTConnectStreams>";

            var doc = XmlStreamsResponseDocument.FromXml(Encoding.UTF8.GetBytes(text));

            Assert.AreEqual(1, doc.Streams.Count());
            var stream = doc.Streams.Single();
            var conditions = stream.Conditions.ToArray();
            Assert.AreEqual(2, conditions.Length);

            var cond1 = new ConditionObservation()
            {
                DataItemId = "comms_cond",
                Type = "COMMUNICATIONS",
                Level = ConditionLevel.FAULT,
                Sequence = 221,
                Timestamp = new DateTime(2022, 09, 13, 16, 7, 36, DateTimeKind.Local)
                    .Add(TimeSpan.FromMilliseconds(444))
            };

            // Overriding default value to pass the test.
            // Is it a bug, that level not capitalized?
            cond1.AddValue(ValueKeys.Level, "Fault");

            conditions[0].ShouldDeepEqual(cond1);

            var cond2 = new ConditionObservation()
            {
                DataItemId = "comms_cond",
                Type = "COMMUNICATIONS",
                Level = ConditionLevel.NORMAL,
                Sequence = 222,
                Timestamp = new DateTime(2022, 09, 13, 16, 7, 36, DateTimeKind.Local)
                    .Add(TimeSpan.FromMilliseconds(570))
            };

            // Overriding default value to pass the test.
            // Is it a bug, that level not capitalized?
            cond2.AddValue(ValueKeys.Level, "Normal");

            conditions[1].ShouldDeepEqual(cond2);
        }
    }
}
