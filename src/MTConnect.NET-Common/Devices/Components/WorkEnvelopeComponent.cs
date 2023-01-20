// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

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
        public new const string DescriptionText = "WorkEnvelope is a System that organizes information about the physical process execution space within a piece of equipment. The WorkEnvelope MAY provide information regarding the physical workspace and the conditions within that workspace.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        public WorkEnvelopeComponent()  { Type = TypeId; }
    }
}
