
//var url = "http://mtconnect.mazakcorp.com:5719/sample?interval=100&count=1000";
//var url = "http://localhost:5005/sample?interval=100&count=1000";
var url = "http://localhost:5005/current?interval=1000";
var stream = new MTConnect.Client.Http.Sample.ConsoleWriteStream(url);

while (true)
{
    await stream.Run(CancellationToken.None);

    Console.WriteLine("Stream Ended");
    await Task.Delay(3000);
}

