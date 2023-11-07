// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using System.IO;
using System.Text.Json;

namespace MTConnect.Processors
{
    public class MTConnectPythonProcessor : IMTConnectAgentProcessor
    {
        private readonly Microsoft.Scripting.Hosting.ScriptEngine _pythonEngine;


        public MTConnectPythonProcessor()       
        {
            _pythonEngine = IronPython.Hosting.Python.CreateEngine();
        }


        public IObservationInput Process(ProcessObservation observation)
        {
            //if (observation.DataItemKey == "L2estop")
            //{
            //    observation.AddValue("Result", "PATRICK");
            //}

            try
            {
                //var eng = IronPython.Hosting.Python.CreateEngine();

                var path = @"c:\temp\mtconnect-process.py";

                var src = File.ReadAllText(path);
                var scope = _pythonEngine.CreateScope();
                _pythonEngine.Execute(src, scope);
                //var process = scope.GetVariable("process");
                var process = scope.GetVariable<Func<ProcessObservation, ProcessObservation>>("process");
                //dynamic run = scope.GetVariable("process");
                //System.Console.WriteLine(process());
                //System.Console.WriteLine(process(observation));

                var processedObservation = process(observation);
                if (processedObservation != null)
                {
                    var outputObservation = new ObservationInput();
                    outputObservation.DeviceKey = processedObservation.DataItem.Device.Uuid;
                    outputObservation.DataItemKey = processedObservation.DataItem.Id;
                    outputObservation.Values = processedObservation.Values;
                    outputObservation.Timestamp = processedObservation.Timestamp.ToUnixTime();
                    return outputObservation;
                }


                //Console.WriteLine(JsonSerializer.Serialize(output));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return null;
        }

        public IAsset Process(IAsset asset)
        {
            return asset;
        }
    }
}