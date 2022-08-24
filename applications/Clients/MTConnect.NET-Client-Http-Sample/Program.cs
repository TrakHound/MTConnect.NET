
var url = "http://localhost:5000/sample?interval=100&count=1000";
var stream = new MTConnect.Client.Http.Sample.ConsoleWriteStream(url);

while (true)
{
    await stream.Run(CancellationToken.None);

    Console.WriteLine("Stream Ended");
    await Task.Delay(3000);
}

