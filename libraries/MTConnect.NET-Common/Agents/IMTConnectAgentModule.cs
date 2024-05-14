// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Logging;

namespace MTConnect.Agents
{
    public interface IMTConnectAgentModule
    {
        string Id { get; }

        string Description { get; }


        event MTConnectLogEventHandler LogReceived;


        void StartBeforeLoad(bool initializeDataItems);

        void StartAfterLoad(bool initializeDataItems);

        void Stop();
    }
}
