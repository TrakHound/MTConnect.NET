using MTConnect.Clients.Rest;


var agentUrl = "localhost:5006";
var client = new MTConnect.Clients.Rest.MTConnectProbeClient(agentUrl);

var stpw = System.Diagnostics.Stopwatch.StartNew();

while (true)
{
    stpw.Restart();

    var doc = await client.GetAsync(CancellationToken.None);
    stpw.Stop();
    if (doc != null)
    {
        Console.WriteLine($"Async : Document Received : {stpw.ElapsedMilliseconds}ms");
    }

    stpw.Restart();

    doc = client.Get();
    stpw.Stop();
    if (doc != null)
    {
        Console.WriteLine($"Document Received : {stpw.ElapsedMilliseconds}ms");
    }

    Console.ReadLine();
}


