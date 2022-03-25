using MTConnect.Clients.Rest;

var agentUrl = "localhost:5006";
var client = new MTConnect.Clients.Rest.MTConnectAssetClient(agentUrl);

while (true)
{
    var doc = client.Get();
    if (doc != null)
    {
        foreach (var asset in doc.Assets)
        {
            Console.WriteLine(asset.AssetId);
        }
    };

    Console.ReadLine();
}

