using MTConnect.Observations;
using MTConnect.Streams;
using MTConnect.Clients.Rest;

//var agentUrl = "https://smstestbed.nist.gov/vds/";
//var agentUrl = "https://smstestbed.nist.gov/vds/GFAgie01";
var agentUrl = "localhost:5000";
//var agentUrl = "localhost:5006";
//var agentUrl = "mtconnect.trakhound.com";
var client = new MTConnect.Clients.Rest.MTConnectClient(agentUrl);
client.Interval = 500;
client.Heartbeat = 10000;
//client.CurrentOnly = true;

client.OnProbeReceived += (s, doc) =>
{
    Console.WriteLine($"ProbeReceived : {doc.GetDataItems().Count()} DataItems");
};

client.OnCurrentReceived += ObservationsReceived;
client.OnSampleReceived += ObservationsReceived;
client.Start();


void ObservationsReceived(object sender, IStreamsResponseDocument document)
{
    Console.WriteLine($"{document.GetObservations().Count()} : ObservationsReceived");

    //if (document.Streams != null)
    //{
    //    foreach (var deviceStream in document.Streams)
    //    {
    //        Console.WriteLine($"{deviceStream.Name} : {deviceStream.Uuid}");
    //        Console.WriteLine("----------");

    //        if (deviceStream.ComponentStreams != null)
    //        {
    //            foreach (var componentStream in deviceStream.ComponentStreams)
    //            {
    //                Console.WriteLine($"{componentStream.ComponentType} : {componentStream.ComponentId} : {componentStream.Name}");
    //                Console.WriteLine($"{componentStream.Observations.Count()} Observations");
    //                Console.WriteLine("----------");

    //                //foreach (var observation in componentStream.SampleValues)
    //                //{
    //                //    Console.WriteLine($"{observation.Representation} : {observation.DataItemId} = {observation.CDATA} @ {observation.Timestamp.ToString("o")}");
    //                //}

    //                //foreach (var observation in componentStream.SampleDataSets)
    //                //{
    //                //    foreach (var entry in observation.Entries)
    //                //    {
    //                //        Console.WriteLine($"{observation.Representation} : {observation.DataItemId} : {entry.Key} = {entry.Value} @ {observation.Timestamp.ToString("o")}");
    //                //    }
    //                //}

    //                //foreach (var observation in componentStream.SampleTables)
    //                //{
    //                //    foreach (var entry in observation.Entries)
    //                //    {
    //                //        foreach (var cell in entry.Cells)
    //                //        {
    //                //            Console.WriteLine($"{observation.Representation} : {observation.DataItemId} : {entry.Key} : {cell.Key} = {cell.Value} @ {observation.Timestamp.ToString("o")}");
    //                //        }
    //                //    }
    //                //}

    //                //foreach (var observation in componentStream.SampleTimeSeries)
    //                //{
    //                //    foreach (var sample in observation.Samples)
    //                //    {
    //                //        Console.WriteLine($"{observation.Representation} : {observation.DataItemId} : {sample} @ {observation.Timestamp.ToString("o")}");
    //                //    }
    //                //}


    //                //foreach (var observation in componentStream.EventValues)
    //                //{
    //                //    Console.WriteLine($"{observation.Representation} : {observation.DataItemId} = {observation.CDATA} @ {observation.Timestamp.ToString("o")}");
    //                //}

    //                //foreach (var observation in componentStream.EventDataSets)
    //                //{
    //                //    foreach (var entry in observation.Entries)
    //                //    {
    //                //        Console.WriteLine($"{observation.Representation} : {observation.DataItemId} : {entry.Key} = {entry.Value} @ {observation.Timestamp.ToString("o")}");
    //                //    }
    //                //}

    //                //foreach (var observation in componentStream.EventTables)
    //                //{
    //                //    foreach (var entry in observation.Entries)
    //                //    {
    //                //        foreach (var cell in entry.Cells)
    //                //        {
    //                //            Console.WriteLine($"{observation.Representation} : {observation.DataItemId} : {entry.Key} : {cell.Key} = {cell.Value} @ {observation.Timestamp.ToString("o")}");
    //                //        }
    //                //    }
    //                //}

    //                //foreach (var observation in componentStream.Conditions)
    //                //{
    //                //    foreach (var value in observation.Values)
    //                //    {
    //                //        Console.WriteLine($"{observation.Representation} : {observation.DataItemId} : {value.Key} = {value.Value} @ {observation.Timestamp.ToString("o")}");
    //                //    }
    //                //}

    //                Console.WriteLine();
    //            }
    //        }
    //    }
    //}
}

Console.ReadLine();

