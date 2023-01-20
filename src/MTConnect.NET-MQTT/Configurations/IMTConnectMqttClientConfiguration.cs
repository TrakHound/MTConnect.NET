// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Configurations
{
    public interface IMTConnectMqttClientConfiguration
    {
        string Server { get; set; }

        int Port { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        bool UseTls { get; set; }

        int RetryInterval { get; set; }
    }
}
