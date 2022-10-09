using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using System.Text;


var topics = new List<string>();

//var topic = "MTConnect/Devices/#/L2p1/#";
topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/#");
//topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/Linear/L2x1/#");
//topics.Add("MTConnect/Devices/OKUMA.Lathe.123456/Observations/Linear/L2z1/#");
//var topic = "MTConnect/Devices/Device/OKUMA.Lathe.123456/Observations/Path/L2p1/#";
//var topic = "MTConnect/Devices/Device/OKUMA.Lathe.123456/Observations/Device/#";


var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
    var mqttClientOptions = new MqttClientOptionsBuilder()
        .WithTcpServer("localhost")
        .Build();

    // Setup message handling before connecting so that queued messages
    // are also handled properly. When there is no event handler attached all
    // received messages get lost.
    mqttClient.ApplicationMessageReceivedAsync += e =>
    {
        Console.WriteLine("Received application message.");
        if (e.ApplicationMessage.Payload != null && e.ApplicationMessage.Payload.Length > 0)
        {
            Console.WriteLine(Encoding.ASCII.GetString(e.ApplicationMessage.Payload));
        }

        return Task.CompletedTask;
    };

    await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

    var mqttSubscribeOptionsBuilder = mqttFactory.CreateSubscribeOptionsBuilder();
    foreach (var topic in topics)
    {
        mqttSubscribeOptionsBuilder.WithTopicFilter(topic);
    }
    var mqttSubscribeOptions = mqttSubscribeOptionsBuilder.Build();

    //var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
    //    .WithTopicFilter(f => {
    //        //f.WithAtLeastOnceQoS();
    //        f.WithTopic(topic);
    //    })
    //    .Build();

    await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

    Console.WriteLine($"MQTT client subscribed..");

    Console.WriteLine("Press enter to exit.");
    Console.ReadLine();
}



Console.ReadLine();