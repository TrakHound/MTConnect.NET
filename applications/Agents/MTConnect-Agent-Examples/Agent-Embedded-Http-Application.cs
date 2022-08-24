// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Applications.Agents;
using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Input;
using MTConnect.Configurations;

namespace MTConnect.Applications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var agentApplication = new MTConnectHttpAgentApplication();
            agentApplication.Run(args);
            agentApplication.OnRestart += AgentRestarted;

            AddDevice(agentApplication);

            // Start Adding Observations directly to Agent
            agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("avail", Availability.AVAILABLE));
            agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("estop", EmergencyStop.ARMED));
            agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("contMode", ControllerMode.AUTOMATIC));
            agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("execution", Execution.ACTIVE));


            var i = 0;
            while (true)
            {
                agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("pos", i++));
                agentApplication.Agent.AddObservation("VMC-3Axis", new ObservationInput("dummy-test", i++));

                Console.ReadLine();
            }

            Console.ReadLine();
        }

        private static void AddDevice(MTConnectHttpAgentApplication agentApplication)
        {
            // Programmatically Add Device
            var device = new Device();
            device.Name = "VMC-3Axis";
            device.Uuid = "TSTT";

            var controller = new ControllerComponent();
            controller.Id = "cont";
            controller.AddDataItem(new EmergencyStopDataItem(controller.Id));
            device.AddComponent(controller);

            var path = new PathComponent();
            path.Id = "path";
            path.AddDataItem(new ControllerModeDataItem(path.Id));
            path.AddDataItem(new ExecutionDataItem(path.Id));
            controller.AddComponent(path);

            var axis = new LinearComponent();
            axis.Id = "x";
            axis.Name = "X";
            axis.AddDataItem(new PositionDataItem(axis.Id, PositionDataItem.SubTypes.ACTUAL));

            axis.AddDataItem(new DataItem(Devices.DataItems.DataItemCategory.SAMPLE, "x:Dummy", dataItemId: "dummy-test"));

            device.AddComponent(axis);


            // Add Device to Agent
            agentApplication.Agent.AddDevice(device);

            Console.WriteLine("Device Added : " + device.Id);
        }

        private static void AgentRestarted(object sender, AgentConfiguration configuration)
        {
            var agentApplication = (MTConnectHttpAgentApplication)sender;
            AddDevice(agentApplication);
        }
    }
}
