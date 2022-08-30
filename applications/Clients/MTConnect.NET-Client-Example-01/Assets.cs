using MTConnect.Clients.Rest;

var agentUrl = "localhost:5000";
//var agentUrl = "mtconnect.mazakcorp.com:5719";

var client = new MTConnectAssetClient(agentUrl, MTConnect.DocumentFormat.XML);
while (true)
{
    var stpw = System.Diagnostics.Stopwatch.StartNew();

    var doc = await client.GetAsync(CancellationToken.None);

    stpw.Stop();
    var ms = (double)((double)stpw.ElapsedTicks / 10000);

    if (doc != null)
    {
        if (doc.Assets.Count() > 0)
        {
            Console.WriteLine(doc.Assets.Count() + " Assets Found in " + ms + "ms");
            Console.WriteLine("---------------");

            foreach (var asset in doc.Assets)
            {
                Console.WriteLine(asset.AssetId);
            }
        }
        else
        {
            Console.WriteLine("No Assets Found in " + ms + "ms");
        }
    }
    else
    {
        Console.WriteLine("Error During Request in " + ms + "ms");
    }

    Console.ReadLine();
}
