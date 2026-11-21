![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect MQTT Relay Module
This Agent Module writes data to an **External** MQTT Broker

## Nuget
<table>
    <thead>
        <tr>
            <td style="font-weight: bold;">Package Name</td>
            <td style="font-weight: bold;">Downloads</td>
            <td style="font-weight: bold;">Link</td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>MTConnect.NET-AgentModule-MqttRelay</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-AgentModule-MqttRelay?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-AgentModule-MqttRelay">https://www.nuget.org/packages/MTConnect.NET-AgentModule-MqttRelay</a></td>
        </tr>
    </tbody>
</table>

## Configuration
```yaml
modules:
  - mqtt-relay:
      server: localhost
      port: 1883
      topicPrefix: enterprise/site/area/line/cell/MTConnect
      topicStructure: Document
```

* `server` - The MQTT broker hostname. Default is `localhost`

* `port` - The MQTT broker port number. Default is `1883`

* `timeout` - The timeout (in milliseconds) to use for connection and read/write. Default is `5000`

* `reconnectInterval` - The interval (in milliseconds) to delay between disconnections. Default is `10000`

* `username` - Sets the Username to use for authentication
 
* `password` - Sets the Password to use for authentication
 
* `clientId` - Sets the Client ID to use for the connection

* `qos` - Sets the Quality Of Service (Qos) to use. 0 = At Most Once, 1 = At least Once, 2 = Exactly Once. Only applies when `topicStructure` is `Entity`. When `topicStructure` is `Document`, messages are published with QoS 1 (At least Once)

* `useTls` - Sets whether to use TLS or not (true or false)

* `topicPrefix` - The prefix to add to the MQTT topics that are published. Default is `MTConnect`
 
* `topicStructure` - (Document or Entity) Sets how MQTT topics and messages are structured. Default is `Document`

* `currentInterval` - Sets the Interval (in milliseconds) to send Current messages at. Default is `5000`

* `sampleInterval` - Sets the Interval (in milliseconds) to send Sample messages at. Default is `500`

* `documentFormat` - The Document Format ID to use to format the payload. Default is `json-cppagent`

* `indentOutput` - Sets whether to indent the output in each payload. Default is `false`

* `durableRelay` - Enables durable relay mode. When set to `true`, the module saves the sequence number of the last observation it sent to disk and, after a network reconnect or agent restart, relays the observations it missed from the Agent's observation buffer. Observations that are no longer in the Agent's buffer cannot be relayed. Default is `false`.

* `tls` - Sets the TLS settings

    * `pfx` - The PFX certificate settings
        * `certificatePath` - The path to the (.pfx) file
        * `certificatePassword` - The certificate password

    * `pem` - The PEM certificate settings
        * `certificatePath` - The path to the (.pem) file
        * `privateKeyPath` - The path to the key containing the private key
        * `privateKeyPassword` - The certificate password
        * `certificateAuthority` - The path to the (.pem) file containing the Certificate Authority

    * `verifyClientCertificate` - Sets whether the MQTT broker's certificate must be trusted ("true" or "false"). When `false` (default), untrusted certificates are accepted

    * `omitCAValidation` - When `true`, the Certificate Authority is not included in the certificates sent to the MQTT broker ("true" or "false"). Default is `false`

### AWS IoT Configuration Example
```yaml
modules:
  - mqtt-relay:
      server: akljadkfjdlsf-ats.iot.us-east-1.amazonaws.com
      port: 8883
      tls:
        pem:
          certificateAuthority: certs/AmazonRootCA1.pem
          certificatePath: certs/asfdslkafjdslkfjdklsdjf-certificate.pem.crt
          privateKeyPath: certs/sdlkajlksdajfldskjfdldlskfjdslkaj-private.pem.key
      documentFormat: json-cppagent
      currentInterval: 5000
      sampleInterval: 500
      topicPrefix: enterprise/site/area/line/cell/MTConnect
```

### AWS Greengrass Moquette Configuration Example
```yaml
modules:
  - mqtt-relay:
      server: localhost
      port: 8883
      clientId: mtconnect-test # Set the ClientId to the AWS Thing ID
      tls:
        verifyClientCertificate: false
        pem:
          certificateAuthority: certs/AmazonRootCA1.pem
          certificatePath: certs/2316549874654321654984984158961634984794-certificate.pem.crt
          privateKeyPath: certs/2316549874654321654984984158961634984794-private.pem.key
      documentFormat: json-cppagent
      currentInterval: 5000
      sampleInterval: 500
      topicPrefix: enterprise/site/area/line/cell/MTConnect
```

### HiveMQ Configuration Example
```yaml
modules:
  - mqtt-relay:
      server: 5679887d308d402888f32.s1.eu.hivemq.cloud
      port: 8883
      username: mtconnect
      password: mtconnect
      useTls: true
      documentFormat: json-cppagent
      currentInterval: 5000
      sampleInterval: 500
      topicPrefix: enterprise/site/area/line/cell/MTConnect
```

## Topics
When `topicStructure` is `Document`, the following topics are published. Probe, Asset and Agent Available messages are retained; Current and Sample messages are not.

```
[TOPIC_PREFIX]/Probe/[DEVICE_UUID]
[TOPIC_PREFIX]/Current/[DEVICE_UUID]
[TOPIC_PREFIX]/Sample/[DEVICE_UUID]
[TOPIC_PREFIX]/Asset/[DEVICE_UUID]/[ASSET_ID]
[TOPIC_PREFIX]/Agent/[AGENT_UUID]/Available
```

## Migration: Availability Topic Moved Out of `Probe/#`

In earlier releases the MqttRelay agent module published the agent's
Availability state (Last Will and Testament plus the on-connect retained
"AVAILABLE" message) on a four-segment topic that fell under the
`{TopicPrefix}/Probe/#` wildcard:

```
{TopicPrefix}/Probe/{AgentUuid}/Available   (raw "AVAILABLE" / "UNAVAILABLE" UTF-8 bytes)
```

That broke the contract that every payload under the Probe wildcard is a
JSON document envelope. Subscribers binding to `{TopicPrefix}/Probe/#`
expecting only Probe document JSON would receive a non-JSON Availability
payload and either crash or silently drop the message.

Starting with this release the module emits agent Availability on a
dedicated, non-overlapping topic:

```
{TopicPrefix}/Agent/{AgentUuid}/Available   (raw "AVAILABLE" / "UNAVAILABLE" UTF-8 bytes)
```

This matches the cppagent reference implementation
([mtconnect/cppagent](https://github.com/mtconnect/cppagent)) which also
publishes JSON document envelopes only under the Probe wildcard and emits
agent status on a dedicated agent / status topic. The Probe wildcard is
now pure-JSON for any subscriber.

### Action required when upgrading

* Subscribers that relied on the old `{TopicPrefix}/Probe/{AgentUuid}/Available`
  topic to track agent Availability must subscribe to
  `{TopicPrefix}/Agent/{AgentUuid}/Available` instead.
* Any retained-message cleanup tooling pointed at the old topic should be
  updated to clear the new topic on broker rotations.

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
