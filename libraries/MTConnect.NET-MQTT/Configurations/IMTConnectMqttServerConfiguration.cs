using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Configurations
{
    public interface IMTConnectMqttServerConfiguration
    {
        int CurrentInterval { get; }

        int SampleInterval { get; }


        string ProbeTopic { get; }

        string CurrentTopic { get; }

        string SampleTopic { get; }

        string AssetTopic { get; }
    }
}
