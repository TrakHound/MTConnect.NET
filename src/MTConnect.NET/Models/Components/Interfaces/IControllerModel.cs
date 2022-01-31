// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Streams.Events;
using System.Collections.Generic;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Controller represents the computational regulation and management function of a piece of equipment.
    /// </summary>
    public interface IControllerModel : IComponentModel
    {
        /// <summary>
        /// The current state of the emergency stop signal for a piece of equipment, controller path, or any other component or subsystem of a piece of equipment.
        /// </summary>
        EmergencyStop EmergencyStop { get; set; }
        IDataItemModel EmergencyStopDataItem { get; }

        /// <summary>
        /// The current operating mode of the Controller component.
        /// </summary>
        ControllerMode ControllerMode { get; set; }
        IDataItemModel ControllerModeDataItem { get; }

        /// <summary>
        /// The current intended production status of the device or component.
        /// </summary>
        FunctionalMode FunctionalMode { get; set; }
        IDataItemModel FunctionalModeDataItem { get; }


        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        Streams.Condition SystemCondition { get; set; }

        /// <summary>
        /// An indication that an error occurred in the logic program or programmable logic controller(PLC) associated with a piece of equipment.
        /// </summary>
        Streams.Condition LogicCondition { get; set; }

        /// <summary>
        /// An indication that an error occurred in the motion program associated with a piece of equipment.
        /// </summary>
        Streams.Condition MotionCondition { get; set; }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        Streams.Condition CommunicationsCondition { get; set; }


        IEnumerable<IPathModel> Paths { get; }

        IPathModel GetPath(string name);
    }
}
