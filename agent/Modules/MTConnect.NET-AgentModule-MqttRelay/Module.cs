using MQTTnet.Client;
using MQTTnet;
using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Streams.Output;
using MTConnect.Observations;
using System.Security.Cryptography.X509Certificates;
using MTConnect.Formatters;

namespace MTConnect
{
    public class Module : IMTConnectAgentModule
    {
        public const string ConfigurationTypeId = "mqtt2-relay";

        private readonly MTConnectMqttServer _server;
        private readonly ModuleConfiguration _configuration;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private CancellationTokenSource _stop;


        public string Id { get; }

        public string Description { get; }


        public Module(IMTConnectAgentBroker mtconnectAgent, object configuration)
        {
            _configuration = AgentApplicationConfiguration.GetConfiguration<ModuleConfiguration>(configuration);

            _server = new MTConnectMqttServer(mtconnectAgent, _configuration);
            _server.ProbeReceived += ProbeReceived;
            _server.CurrentReceived += CurrentReceived;
            _server.SampleReceived += SampleReceived;
            _server.AssetReceived += AssetReceived;

            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
        }


        public void StartBeforeLoad() { }

        public void StartAfterLoad()
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(Worker, _stop.Token);
        }

        public void Stop()
        {
            _server.Stop();

            if (_stop != null) _stop.Cancel();

            try
            {
                if (_mqttClient != null)
                {
                    _mqttClient.DisconnectAsync(MqttClientDisconnectReason.NormalDisconnection);
                }
            }
            catch { }
        }

        private async Task Worker()
        {
            do
            {
                try
                {
                    try
                    {
                        // Declare new MQTT Client Options with Tcp Server
                        var clientOptionsBuilder = new MqttClientOptionsBuilder().WithTcpServer(_configuration.Server, _configuration.Port);

                        // Set Client ID
                        if (!string.IsNullOrEmpty(_configuration.ClientId))
                        {
                            clientOptionsBuilder.WithClientId(_configuration.ClientId);
                        }

                        var certificates = new List<X509Certificate2>();

                        // Add CA (Certificate Authority)
                        if (!string.IsNullOrEmpty(_configuration.CertificateAuthority))
                        {
                            certificates.Add(new X509Certificate2(GetFilePath(_configuration.CertificateAuthority)));
                        }

                        // Add Client Certificate & Private Key
                        if (!string.IsNullOrEmpty(_configuration.PemCertificate) && !string.IsNullOrEmpty(_configuration.PemPrivateKey))
                        {

#if NET5_0_OR_GREATER
                            certificates.Add(new X509Certificate2(X509Certificate2.CreateFromPemFile(GetFilePath(_configuration.PemCertificate), GetFilePath(_configuration.PemPrivateKey)).Export(X509ContentType.Pfx)));
#else
                            throw new Exception("PEM Certificates Not Supported in .NET Framework 4.8 or older");
#endif

                            clientOptionsBuilder.WithCleanSession();
                            clientOptionsBuilder.WithTls(new MqttClientOptionsBuilderTlsParameters()
                            {
                                UseTls = true,
                                SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                                IgnoreCertificateRevocationErrors = _configuration.AllowUntrustedCertificates,
                                IgnoreCertificateChainErrors = _configuration.AllowUntrustedCertificates,
                                AllowUntrustedCertificates = _configuration.AllowUntrustedCertificates,
                                Certificates = certificates
                            });
                        }

                        // Add Credentials
                        if (!string.IsNullOrEmpty(_configuration.Username) && !string.IsNullOrEmpty(_configuration.Password))
                        {
                            if (_configuration.UseTls)
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password).WithTls();
                            }
                            else
                            {
                                clientOptionsBuilder.WithCredentials(_configuration.Username, _configuration.Password);
                            }
                        }

                        // Build MQTT Client Options
                        var clientOptions = clientOptionsBuilder.Build();

                        await _mqttClient.ConnectAsync(clientOptions, CancellationToken.None);

                        //if (Connected != null) Connected.Invoke(this, new EventArgs());

                        _server.Start();

                        while (!_stop.Token.IsCancellationRequested && _mqttClient.IsConnected)
                        {
                            await Task.Delay(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        //if (ConnectionError != null) ConnectionError.Invoke(this, ex);
                    }

                    //if (Disconnected != null) Disconnected.Invoke(this, new EventArgs());

                    await Task.Delay(_configuration.ReconnectInterval, _stop.Token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex) { }

            } while (!_stop.Token.IsCancellationRequested);
        }



        private async void ProbeReceived(IDevice device, IDevicesResponseDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, responseDocument);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{device.Uuid}/{_configuration.ProbeTopic}";

                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.Topic = topic;
                    message.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                    message.Payload = formatResult.Content;

                    var publishResult = await _mqttClient.PublishAsync(message);
                    if (publishResult.IsSuccess)
                    {

                    }
                }
            }
        }

        private async void CurrentReceived(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, ref responseDocument);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{device.Uuid}/{_configuration.CurrentTopic}";

                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.Topic = topic;
                    message.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                    message.Payload = formatResult.Content;

                    var publishResult = await _mqttClient.PublishAsync(message);
                    if (publishResult.IsSuccess)
                    {

                    }
                }
            }
        }

        private async void SampleReceived(IDevice device, IStreamsResponseOutputDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, ref responseDocument);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{device.Uuid}/{_configuration.SampleTopic}";

                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.Topic = topic;
                    message.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                    message.Payload = formatResult.Content;

                    var publishResult = await _mqttClient.PublishAsync(message);
                    if (publishResult.IsSuccess)
                    {

                    }
                }
            }
        }

        private async void AssetReceived(IDevice device, IAssetsResponseDocument responseDocument)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                var formatResult = ResponseDocumentFormatter.Format(_configuration.DocumentFormat, responseDocument);
                if (formatResult.Success)
                {
                    var topic = $"{_configuration.TopicPrefix}/{device.Uuid}/{_configuration.AssetTopic}";

                    var message = new MqttApplicationMessage();
                    message.Retain = true;
                    message.Topic = topic;
                    message.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
                    message.Payload = formatResult.Content;

                    var publishResult = await _mqttClient.PublishAsync(message);
                    if (publishResult.IsSuccess)
                    {

                    }
                }
            }
        }


        private static string GetFilePath(string path)
        {
            var x = path;
            if (!Path.IsPathRooted(x))
            {
                x = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, x);
            }

            return x;
        }
    }
}
