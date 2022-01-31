using MTConnect.Clients.Rest;

var deviceName = "OKUMA.Lathe";
var baseUrl = "localhost:5006";

var client = new MTConnectAssetClient(baseUrl, deviceName);
var document = client.Get();
foreach (var asset in document.Assets)
{
    Console.WriteLine(asset.AssetId);
}


//var client = new MTConnectProbeClient(baseUrl, deviceName);
//var document = client.Get();
//foreach (var device in document.Devices)
//{
//    // Device
//    Console.WriteLine(device.Id);

//    // DataItems
//    foreach (var dataItem in device.DataItems)
//    {
//        Console.WriteLine(dataItem.Id);
//    }

//    // Components
//    foreach (var component in device.Components)
//    {
//        Console.WriteLine(component.Id);
//    }
//}


//client.OnProbeReceived += (sender, document) =>
//{
//    foreach (var device in document.Devices)
//    {
//        // Device
//        Console.WriteLine(device.Id);

//        // DataItems
//        foreach (var dataItem in device.DataItems)
//        {
//            Console.WriteLine(dataItem.Id);
//        }

//        // Components
//        foreach (var component in device.Components)
//        {
//            Console.WriteLine(component.Id);
//        }
//    }
//};
//client.OnCurrentReceived += (sender, document) =>
//{
//    foreach (var deviceStream in document.Streams)
//    {
//        // Device
//        Console.WriteLine(deviceStream.Name);

//        // Component Streams
//        foreach (var componentStream in deviceStream.ComponentStreams)
//        {
//            Console.WriteLine(componentStream.Name);

//            // DataItems
//            foreach (var dataItem in componentStream.DataItems)
//            {
//                Console.WriteLine(dataItem.DataItemId);
//            }
//        }
//    }
//};
//client.Start();

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
