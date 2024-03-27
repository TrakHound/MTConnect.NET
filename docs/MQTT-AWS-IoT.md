# MQTT to AWS IoT

## Overview
- AWS IoT Setup
  - [Getting Started](https://docs.aws.amazon.com/iot/latest/developerguide/iot-gs.html) - AWS Docs for setting up AWS IoT service
  - [YouTube](https://www.youtube.com/watch?v=Vk-fKN_81o8) - Good step by step (no/low audio)
- MQTT Explorer
  - [Download](https://mqtt-explorer.com/) - Download the MQTT Explorer app to browse the MQTT broker
  - [GitHub](https://github.com/thomasnordquist/MQTT-Explorer)

## Notes
- AWS IoT requires using an SSL certificate. You will need to download the certificate(s) and private key files from your AWS IoT account (follow the instructions in the YouTube video above).
- AWS IoT limits the number of "forward slashes" in a Topic to 7. This limitation requires the use of the Agent configuration parameter "MqttFormat" to be set to "Flat"

## SSL Certificates
The SSL cerfificates can be created in the AWS IoT console and should be downloaded to the PC/device the MTConnect Agent. 
Files in this example are copied to the MTConnect Relay Agent's installation directory under the subdirectory "/certs".

## MTConnect Relay Agent
- [GitHub](https://github.com/TrakHound/MTConnect.NET/tree/master/applications/Agents/MTConnect-Agent-MQTT-Relay)
- [Release](https://github.com/TrakHound/MTConnect.NET/releases/latest)

### Configuration
```yaml
...

# The hostname of the MQTT broker to publish messages to
server: akljadkfjdlsf-ats.iot.us-east-1.amazonaws.com

# The port number of the MQTT broker to publish messages to
port: 8883

# Set TLS configuration
tls:
  pem:
    certificateAuthority: certs/AmazonRootCA1.pem
    certificatePath: certs/asfdslkafjdslkfjdklsdjf-certificate.pem.crt
    privateKeyPath: certs/sdlkajlksdajfldskjfdldlskfjdslkaj-private.pem.key

...
```
