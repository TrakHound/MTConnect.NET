// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Input;
using MTConnect.Logging;

namespace MTConnect.Agents
{
    public interface IMTConnectAgentProcessor
    {
        string Id { get; }

        string Description { get; }


        event MTConnectLogEventHandler LogReceived;


        void Load();


        IObservationInput Process(ProcessObservation observation);

        IAsset Process(IAsset asset);
    }
}
