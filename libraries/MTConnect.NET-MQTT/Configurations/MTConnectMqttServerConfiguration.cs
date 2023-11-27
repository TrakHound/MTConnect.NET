using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Configurations
{
    public class MTConnectMqttServerConfiguration : IMTConnectMqttServerConfiguration
    {
        public int CurrentInterval { get; set; }

        public int SampleInterval { get; set; }


        public string TopicPrefix { get; set; }

        public string ProbeTopic { get; set; }

        public string CurrentTopic { get; set; }

        public string SampleTopic { get; set; }

        public string AssetTopic { get; set; }


        public MTConnectMqttServerConfiguration()
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
