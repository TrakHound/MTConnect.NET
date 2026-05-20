// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Configurations
{
    /// <summary>
    /// Default <see cref="IMTConnectMqttDocumentServerConfiguration"/> implementation. Extends
    /// the interface contract with the topic-tree layout the server uses: a common prefix and
    /// per-document-type leaf segments for Probe, Current, Sample, and Asset publishes.
    /// </summary>
    public class MTConnectMqttDocumentServerConfiguration : IMTConnectMqttDocumentServerConfiguration
    {
        /// <inheritdoc />
        public int CurrentInterval { get; set; }

        /// <inheritdoc />
        public int SampleInterval { get; set; }


        /// <summary>The MQTT topic prefix the server publishes under (e.g. <c>MTConnect</c>).</summary>
        public string TopicPrefix { get; set; }

        /// <summary>The leaf segment used for <c>probe</c> response publishes; the full topic is <c>{TopicPrefix}/{ProbeTopic}/{deviceUuid}</c>.</summary>
        public string ProbeTopic { get; set; }

        /// <summary>The leaf segment used for <c>current</c> response publishes; the full topic is <c>{TopicPrefix}/{CurrentTopic}/{deviceUuid}</c>.</summary>
        public string CurrentTopic { get; set; }

        /// <summary>The leaf segment used for <c>sample</c> response publishes; the full topic is <c>{TopicPrefix}/{SampleTopic}/{deviceUuid}</c>.</summary>
        public string SampleTopic { get; set; }

        /// <summary>The leaf segment used for asset publishes; the full topic is <c>{TopicPrefix}/{AssetTopic}/{deviceUuid}/{assetId}</c>.</summary>
        public string AssetTopic { get; set; }


        /// <summary>
        /// Initialises the configuration with the conventional defaults: a 5-second Current
        /// interval, a 500 ms Sample interval, the <c>MTConnect</c> topic prefix, and the
        /// <c>Probe</c>/<c>Current</c>/<c>Sample</c>/<c>Asset</c> leaf segments.
        /// </summary>
        public MTConnectMqttDocumentServerConfiguration()
        {
            CurrentInterval = 5000;
            SampleInterval = 500;
            TopicPrefix = "MTConnect";
            ProbeTopic = "Probe";
            CurrentTopic = "Current";
            SampleTopic = "Sample";
            AssetTopic = "Asset";
        }
    }
}
