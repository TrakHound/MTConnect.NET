using MTConnect.Clients;

//var deviceName = "OKUMA-Lathe";
//var deviceName = "M12346";

//var agentUrl = "localhost:5006";
//var agentUrl = "localhost:5005";
//var agentUrl = "http://localhost:5001";
//var agentUrl = "http://DESKTOP-HV74M4N:5001";
//var agentUrl = "https://localhost:5002";
//var agentUrl = "https://DESKTOP-HV74M4N:5002";
//var agentUrl = "localhost:5000";
//var agentUrl = "192.168.1.136:5000";
//var agentUrl = "mtconnect.mazakcorp.com:5719";
var agentUrl = "https://trakhound.com";


for (int i = 0; i < 1; i++)
{
    var client = new MTConnectHttpClient(agentUrl);
    //var client = new MTConnectClient(agentUrl, deviceName);
    client.Interval = 1000;
    client.Heartbeat = 10000;
    //client.ContentEncodings = null;
    //client.ContentType = null;
    client.OnProbeReceived += (sender, document) =>
    {
        Console.WriteLine("Probe Received");

        foreach (var device in document.Devices)
        {
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
    client.Start();
    //client.StartFromBuffer();
    //client.Start("//*[@type=\"PATH_POSITION\"]");
}

Console.ReadLine();
