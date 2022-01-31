// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// WorkEnvelope is a System that organizes information about the physical process execution space within a piece of equipment. 
    /// The WorkEnvelope MAY provide information regarding the physical workspace and the conditions within that workspace.
    /// </summary>
    public class WorkEnvelopeComponent : Component 
    {
        public const string TypeId = "WorkEnvelope";
        public const string NameId = "workenv";

        public WorkEnvelopeComponent()  { Type = TypeId; }
    }
}
