using MTConnect.Clients.Rest;

//var deviceName = "OKUMA-Lathe";
//var deviceName = "M12346";
var baseUrl = "127.0.0.1:5005";
//var baseUrl = "127.0.0.1:5005";
//var baseUrl = "localhost:5000";
//var baseUrl = "mtconnect.mazakcorp.com:5719";

//var probe = new MTConnectProbeClient(baseUrl, deviceName);
//var doc = probe.Get();
//if (doc != null)
//{

//}

for (int i = 0; i < 1; i++)
{
    var client = new MTConnectClient(baseUrl);
    //var client = new MTConnectClient(baseUrl, deviceName);
    client.Interval = 100;
    //client.Interval = 1000;
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

        foreach (var deviceStream in document.Streams)
        {
            foreach (var dataSet in deviceStream.EventDataSets)
            {

            }
        }

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
    client.Start();
    //client.Start("//*[@type=\"PATH_POSITION\"]");
}

Console.ReadLine();


//var client = new MTConnectCurrentClient(agentUrl, deviceName, MTConnect.DocumentFormat.XML);
//var agentUrl = "localhost:5000";

//var client = new MTConnectProbeClient(agentUrl, deviceName);

//while (true)
//{
//    var doc = await client.GetAsync(CancellationToken.None);
//    if (doc != null)
//    {
//        foreach (MTConnect.Devices.Device device in doc.Devices)
//        {
//            Console.WriteLine(device.Id);
//        }
//    }

//    Console.ReadLine();
//}

//var client = new MTConnectAssetClient(agentUrl, MTConnect.DocumentFormat.XML);
////var client = new MTConnectAssetClient(agentUrl, MTConnect.DocumentFormat.JSON);



//while (true)
//{
//    var doc = await client.GetAsync(CancellationToken.None);
//    if (doc != null)
//    {
//        foreach (var asset in doc.Assets.Assets)
//        {
//            Console.WriteLine(asset.AssetId);
//        }
//    }

//    Console.ReadLine();
//}



////var client = new MTConnectCurrentClient(agentUrl, deviceName, MTConnect.DocumentFormat.XML);
//var client = new MTConnectCurrentClient(agentUrl, deviceName, MTConnect.DocumentFormat.JSON);

//while (true)
//{
//    var doc = await client.GetAsync(CancellationToken.None);
//    if (doc != null)
//    {
//        var deviceStream = doc.Streams.FirstOrDefault(d => d.Name == deviceName);
//        if (deviceStream != null)
//        {
//            foreach (var componentStream in deviceStream.ComponentStreams)
//            {
//                foreach (var dataItem in componentStream.DataItems)
//                {
//                    Console.WriteLine($"{dataItem.DataItemId} = {dataItem.CDATA}");
//                }
//            }
//        }
//    }

//    Console.ReadLine();
//}
