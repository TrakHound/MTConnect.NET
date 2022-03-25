// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// PartOccurrence is a Component that organizes information about a specific part as it exists at a specific place and time, 
    /// such as a specific instance of a bracket at a specific timestamp. Part is defined as a discrete item that has both defined
    /// and measurable physical characteristics including mass, material and features and is created by applying one or more manufacturing process steps to a workpiece.
    /// </summary>
    public interface IPartOccurrenceModel
    {
        /// <summary>
        /// An identifier of a part in a manufacturing operation.
        /// </summary>
        string PartId { get; set; }

        /// <summary>
        /// Identifier given to a distinguishable, individual part.
        /// </summary>
        PartUniqueIdModel PartUniqueId { get; set; }

        /// <summary>
        /// Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.
        /// </summary>
        PartKindIdModel PartKindId { get; set; }

        /// <summary>
        /// The aggregate count of parts.
        /// </summary>
        PartCountModel PartCount { get; set; }

        /// <summary>
        /// State or condition of a part.
        /// </summary>
        PartStatus PartStatus { get; set; }

        /// <summary>
        /// An identifier of a process being executed by the device.
        /// </summary>
        string ProcessOccurrenceId { get; set; }

        /// <summary>
        /// The identifier of the person currently responsible for operating the piece of equipment.
        /// </summary>
        string OperatorUser { get; set; }

        /// <summary>
        /// The identifier of the person currently responsible for performing maintenance on the piece of equipment.
        /// </summary>
        string MaintenanceUser { get; set; }

        /// <summary>
        /// The identifier of the person currently responsible for preparing a piece of equipment for production
        /// or restoring the piece of equipment to a neutral state after production.
        /// </summary>
        string SetupUser { get; set; }
    }
}
