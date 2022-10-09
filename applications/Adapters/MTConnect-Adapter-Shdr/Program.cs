using MTConnect.Adapters.Shdr;
using MTConnect.Observations;
using MTConnect.Shdr;

namespace MTConnect.Applications.Adapters.Shdr
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var adapter = new ShdrAdapter("OKUMA-Lathe");
            //var adapter = new ShdrQueueAdapter(7878);
            //var adapter = new ShdrIntervalAdapter(7878);
            var adapter = new ShdrIntervalQueueAdapter(7878);
            adapter.Interval = 10;
            adapter.MultilineAssets = true;
            adapter.MultilineDevices = true;
            adapter.AgentConnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Connected");
            adapter.AgentDisconnected += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Disconnected");
            adapter.PingReceived += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Agent Ping Received");
            adapter.PongSent += (sender, connectionId) => Console.WriteLine($"Agent Connection (ID = {connectionId}) : Pong Sent to Agent");
            adapter.LineSent += (sender, args) => Console.WriteLine($"Agent Connection (ID = {args.ClientId}) : Line Sent : {args.Message}");

            adapter.Start();

            var i = 0;
            var m = 0;
            var c = 0;
            var s = 0;
            var samples = new List<double>() { s };
            var d = 0;
            var dataSetEntries = new List<IDataSetEntry>() { new DataSetEntry(d.ToString(), d) };
            var t = 0;
            var tableCells = new List<ITableCell>() { new TableCell(t.ToString(), t) };
            var tableEntries = new List<ITableEntry>() { new TableEntry(d.ToString(), tableCells) };
            var f = 0;

            while (true)
            {
                var key = Console.ReadKey();
                Console.CursorLeft = 0;

                if (key.Key == ConsoleKey.Spacebar)
                {
                    while (true)
                    {
                        adapter.AddDataItem("L2X1load", i);
                        i++;

                        await Task.Delay(5);
                    }
                }
                else if (key.Key == ConsoleKey.M)
                {
                    adapter.AddMessage("L2p1message", "This is a message", m.ToString());
                    m++;
                }
                else if (key.Key == ConsoleKey.C)
                {
                    var condition = new ShdrCondition("L2p1system");
                    condition.AddWarning("This is a warning", c.ToString());
                    adapter.AddCondition(condition);
                    c++;
                }
                else if (key.Key == ConsoleKey.S)
                {
                    adapter.AddTimeSeries(new ShdrTimeSeries("L2p1Sensor", samples, 100));
                    samples.Add(s++);
                }
                else if (key.Key == ConsoleKey.D)
                {
                    dataSetEntries.Add(new DataSetEntry(d.ToString(), d));
                    var dataSet = new ShdrDataSet("L2p1Variables", dataSetEntries);
                    adapter.AddDataSet(dataSet);
                    d++;
                }
                else if (key.Key == ConsoleKey.T)
                {
                    tableEntries.Add(new TableEntry(t.ToString(), tableCells));
                    var table = new ShdrTable("L2p1ToolTable", tableEntries);
                    adapter.AddTable(table);
                    t++;
                }
                else if (key.Key == ConsoleKey.F)
                {
                    adapter.SendAsset(CreateFileAsset(f));
                    f++;
                }
                else if (key.Key == ConsoleKey.Tab)
                {
                    adapter.SendCurrent();
                }
                else if (key.Key == ConsoleKey.L)
                {
                    adapter.SendLast();
                }
                else if (key.Key == ConsoleKey.U)
                {
                    adapter.SetUnavailable();
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    adapter.SendBuffer();
                }
            }
        }

        public static Assets.IAsset CreateFileAsset(int i)
        {
            var asset = new Assets.Files.FileAsset();
            asset.DateTime = DateTime.UtcNow;
            asset.AssetId = "file.patrick" + i;
            asset.Size = 12346;
            asset.VersionId = "test-v1";
            asset.State = Assets.Files.FileState.PRODUCTION;
            asset.Name = "file-123.txt";
            asset.MediaType = "text/plain";
            asset.ApplicationCategory = Assets.Files.ApplicationCategory.DEVICE;
            asset.ApplicationType = Assets.Files.ApplicationType.DATA;
            asset.FileLocation = new Assets.Files.FileLocation(@"C:\temp\file-123.txt");
            asset.CreationTime = DateTime.Now;

            return asset;
        }
    }
}
