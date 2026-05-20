// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Describes one participating piece of equipment in an Interface handshake, identifying it and stating whether and with what precedence it must take part.
    /// </summary>
    public class Collaborator
    {
        /// <summary>
        /// The identifier of the collaborating equipment, correlated with the Component or device that fulfils this role in the Interface.
        /// </summary>
        public string CollaboratorId { get; set; }

        /// <summary>
        /// The category of equipment (robot, conveyor, CNC, buffer) that this collaborator represents.
        /// </summary>
        public CollaboratorType CollaboratorType { get; set; }

        /// <summary>
        /// When true, the handshake can proceed even if this collaborator is absent or does not respond.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// The relative precedence of this collaborator when more than one is eligible to service the same request.
        /// </summary>
        public Priority Priority { get; set; }
    }
}
