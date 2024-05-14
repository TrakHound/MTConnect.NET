// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_91b028d_1587649840347_727529_300

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System composed of the physical process execution space within a piece of equipment.
    /// </summary>
    public class WorkEnvelopeComponent : Component
    {
        public const string TypeId = "WorkEnvelope";
        public const string NameId = "workEnvelope";
        public new const string DescriptionText = "System composed of the physical process execution space within a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16; 


        public WorkEnvelopeComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}