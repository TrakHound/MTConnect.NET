using MTConnect.Clients.Rest;

var agentUrl = "localhost:5000";
//var agentUrl = "mtconnect.mazakcorp.com:5719";

for (int i = 0; i < 1; i++)
{
    var client = new MTConnectClient(agentUrl);
    //var client = new MTConnectClient(baseUrl, deviceName);
    client.Interval = 100;
    client.Heartbeat = 10000;
    client.CurrentOnly = true;
    //client.ContentEncodings = null;
    //client.ContentType = null;
    client.OnProbeReceived += (sender, document) =>
    {
        Console.WriteLine("Probe Received");

        foreach (var device in document.Devices)
        {
            // Device
            Console.WriteLine(device.Id);

            //// DataItems
            //foreach (var dataItem in device.DataItems)
            //{
            //    Console.WriteLine($"DataItemId = {dataItem.Id}");
            //    Console.WriteLine($"Type = {dataItem.Type} : {dataItem.TypeDescription}");
            //    Console.WriteLine($"SubType = {dataItem.SubType} : {dataItem.SubTypeDescription}");
            //    Console.WriteLine("----------------");
            //}

            //// Components
            //foreach (var component in device.Components)
            //{
            //    Console.WriteLine(component.Id);
            //}
        }
    };
    client.OnCurrentReceived += (sender, document) =>
    {
        Console.WriteLine($"MTConnectStreams : Current : {document.GetObservations().Count()} Observations");

        //foreach (var deviceStream in document.Streams)
        //{
        //    // Device
        //    Console.WriteLine(deviceStream.Name);

        //    // Component Streams
        //    foreach (var componentStream in deviceStream.ComponentStreams)
        //    {
        //        Console.WriteLine(componentStream.Name);

        //        // DataItems
        //        foreach (var dataItem in componentStream.DataItems)
        //        {
        //            Console.WriteLine(dataItem.DataItemId);
        //        }
        //    }
        //}
    };
    client.OnSampleReceived += (sender, document) =>
    {
        Console.WriteLine($"MTConnectStreams : Sample : {document.GetObservations().Count()} Observations");

        //foreach (var deviceStream in document.Streams)
        //{
        //    // Device
        //    Console.WriteLine(deviceStream.Name);

        //    // Component Streams
        //    foreach (var componentStream in deviceStream.ComponentStreams)
        //    {
        //        Console.WriteLine(componentStream.Name);

        //        // DataItems
        //        foreach (var dataItem in componentStream.DataItems)
        //        {
        //            Console.WriteLine(dataItem.DataItemId);
        //        }
        //    }
        //}
    };
    client.OnAssetsReceived += (sender, document) =>
    {
        Console.WriteLine($"MTConnectAssets : {document.Assets.Count()} Assets");

        foreach (var asset in document.Assets)
        {
            //Console.WriteLine(asset.GetType().ToString());
        }
    };
    client.StartFromBuffer();
    //client.Start();
    //client.Start("//*[@type=\"PATH_POSITION\"]");
}

Console.ReadLine();
