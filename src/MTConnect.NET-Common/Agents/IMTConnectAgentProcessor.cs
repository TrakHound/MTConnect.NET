// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Observations.Input;

namespace MTConnect.Agents
{
    public interface IMTConnectAgentProcessor
    {
        IObservationInput Process(ProcessObservation observation);

        IAsset Process(IAsset asset);
    }
}
