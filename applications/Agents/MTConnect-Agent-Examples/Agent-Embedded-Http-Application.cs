// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Applications.Agents;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Input;

namespace MTConnect.Applications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var agentApplication = new MTConnectHttpAgentApplication();
            agentApplication.Run(args);


            // Start Adding Observations directly to Agent
            agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("avail", Availability.AVAILABLE));
            agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("estop", EmergencyStop.ARMED));
            agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("mode", ControllerMode.AUTOMATIC));
            agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("execution", Execution.ACTIVE));


            var i = 0;
            while (true)
            {
                agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("Xact", i++));

                Console.ReadLine();
            }
        }
    }
}
