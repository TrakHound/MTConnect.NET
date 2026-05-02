# MTConnect > InfluxDB
Uses the MTConnect.NET MTConnectHttpClient class to read from an MTConnect Agent and write that data to an InfluxDB database

This is a simple example for basic Value Observations and would need to be edited to support Conditions, Datasets, Tables, TimeSeries, and Assets.

## Example
```c#
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using MTConnect.Clients;
using MTConnect.Observations;
using MTConnect.Streams;

var influxServer = "https://eastus-1.azure.cloud2.influxdata.com";
var influxToken = "klajsdfdslkfjdslkfjsdlkfjlsdkjflakdsjf";
var influxOrganization = "example";

// Setup InfluxDB Client
var influxClient = new InfluxDBClient(influxServer, influxToken);

// Setup MTConnect Client (over HTTP)
var mtconnectClient = new MTConnectHttpClient("http://localhost:5000");
mtconnectClient.CurrentReceived += ObservationsReceived;
mtconnectClient.SampleReceived += ObservationsReceived;
mtconnectClient.Start();

Console.ReadLine();

mtconnectClient.Stop();

void ObservationsReceived(object sender, IStreamsResponseDocument response)
{
    using (var write = influxClient.GetWriteApi())
    {
        var records = new List<PointData>();

        foreach (var observation in response.GetObservations())
        {
            var key = $"{observation.DeviceUuid}.{observation.DataItemId}";
            records.Add(PointData.Measurement(key)
            .Field("value", observation.GetValue(ValueKeys.Result))
            .Timestamp(observation.Timestamp, WritePrecision.Us));
        }

        // Write Records
        write.WritePoints(records, "MTConnect.Streams", influxOrganization);
    }
}
```
