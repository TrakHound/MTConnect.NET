// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations.Output;
using System.Collections.Generic;

namespace MTConnect.Streams.Output
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public class ComponentStreamOutput : IComponentStreamOutput
    {
        private IObservationOutput[] _observations;


        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        public IComponent Component { get; set; }

        /// <summary>
        /// The type of the Component that the ComponentStream is associated with
        /// </summary>
        public string ComponentType { get; set; }

        /// <summary>
        /// The identifier of the Structural Element as defined by the id attribute of the corresponding Structural Element in the MTConnectDevices XML document.
        /// </summary>
        public string ComponentId { get; set; }

        /// <summary>
        /// The name of the ComponentStream element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// NativeName identifies the common name normally associated with the ComponentStream element.
        /// </summary>
        public string NativeName { get; set; }

        /// <summary>
        /// Uuid of the ComponentStream element.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Returns All Observations for the ComponentStream
        /// </summary>
        public IObservationOutput[] Observations
        {
            get => _observations;
            set => _observations = value;
        }


        public ComponentStreamOutput() { }

        public ComponentStreamOutput(IComponentStream componentStream)
        {
            if (componentStream != null)
            {
                Component = componentStream.Component;
                ComponentType = componentStream.ComponentType;
                ComponentId = componentStream.ComponentId;
                Name = componentStream.Name;
                NativeName = componentStream.NativeName;
                Uuid = componentStream.Uuid;

                if (componentStream.Observations != null)
                {
                    var observations = new List<IObservationOutput>();
                    foreach (var observation in componentStream.Observations)
                    {
                        observations.Add(new ObservationOutput(observation));
                    }
                    Observations = observations.ToArray();
                }
            }
        }
    }  
}