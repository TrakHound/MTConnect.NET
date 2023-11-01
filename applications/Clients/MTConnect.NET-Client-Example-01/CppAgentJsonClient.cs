using MTConnect.Clients;
using MTConnect.Observations;

var agentUrl = "localhost:5001";

var client = new MTConnectHttpClient(agentUrl);
client.Interval = 100;
client.Heartbeat = 10000;
client.ContentType = "application/json";
client.DocumentFormat = "json-cppagent";
client.DeviceReceived += (sender, device) =>
{
    Console.WriteLine($"Device Received : {device.Uuid}");
};
client.ObservationReceived += (sender, observation) =>
{
    switch (observation.Representation)
    {
        case MTConnect.Devices.DataItemRepresentation.VALUE:
            Console.WriteLine($"Observation Received : {observation.DataItemId} = {observation.GetValue("Result")} @ {observation.Timestamp}");
            break;

        case MTConnect.Devices.DataItemRepresentation.DATA_SET:
            if (!observation.IsUnavailable)
            {
                var entries = DataSetObservation.GetEntries(observation.Values);
                foreach (var entry in entries)
                {
                    Console.WriteLine($"Observation Received : {observation.DataItemId} : DATA_SET : {entry.Key} = {entry.Value} @ {observation.Timestamp}");
                }
            }
            else
            {
                Console.WriteLine($"Observation Received : {observation.DataItemId} = {observation.GetValue("Result")} @ {observation.Timestamp}");
            }
            break;

        case MTConnect.Devices.DataItemRepresentation.TABLE:
            if (!observation.IsUnavailable)
            {
                var entries = TableObservation.GetEntries(observation.Values);
                foreach (var entry in entries)
                {
                    foreach (var cell in entry.Cells)
                    {
                        Console.WriteLine($"Observation Received : {observation.DataItemId} : TABLE : {entry.Key} : {cell.Key} = {cell.Value} @ {observation.Timestamp}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Observation Received : {observation.DataItemId} = {observation.GetValue("Result")} @ {observation.Timestamp}");
            }
            break;

        case MTConnect.Devices.DataItemRepresentation.TIME_SERIES:
            if (!observation.IsUnavailable)
            {
                var samples = TimeSeriesObservation.GetSamples(observation.Values).ToList();
                Console.WriteLine($"Observation Received : {observation.DataItemId} : TIME_SERIES : {string.Join(" ", samples)} @ {observation.Timestamp}");
            }
            else
            {
                Console.WriteLine($"Observation Received : {observation.DataItemId} = {observation.GetValue("Result")} @ {observation.Timestamp}");
            }
            break;
    }
};
client.AssetReceived += (sender, asset) =>
{
    Console.WriteLine($"Asset Received : {asset.AssetId}");
};

client.Start();

Console.ReadLine();
