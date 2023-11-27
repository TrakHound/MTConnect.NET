// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Observations.Output;

namespace MTConnect.Streams.Output
{
    /// <summary>
    /// ComponentStream is a XML container that organizes the data associated with each Structural Element defined for that piece of equipment in the associated MTConnectDevices XML document
    /// </summary>
    public interface IComponentStreamOutput
    {
        /// <summary>
        /// Component identifies the Structural Element associated with the ComponentStream element.
        /// </summary>
        IComponent Component { get; }

        /// <summary>
        /// The type of the Component that the ComponentStream is associated with
        /// </summary>
        string ComponentType { get; }

        /// <summary>
        /// The identifier of the Structural Element as defined by the id attribute of the corresponding Structural Element in the MTConnectDevices XML document.
        /// </summary>
        string ComponentId { get; }

        /// <summary>
        /// The name of the ComponentStream element.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// NativeName identifies the common name normally associated with the ComponentStream element.
        /// </summary>
        string NativeName { get; }

        /// <summary>
        /// Uuid of the ComponentStream element.
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// Gets All Observations (Samples, Events, & Conditions)
        /// </summary>
        IObservationOutput[] Observations { get; }
    }  
}