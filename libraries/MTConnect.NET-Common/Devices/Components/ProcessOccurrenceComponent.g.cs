// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605547395898_219029_214

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Process that takes place at a specific place and time, such as a specific instance of part-milling occurring at a specific timestamp.
    /// </summary>
    public class ProcessOccurrenceComponent : Component
    {
        public const string TypeId = "ProcessOccurrence";
        public const string NameId = "processOccurrenceComponent";
        public new const string DescriptionText = "Process that takes place at a specific place and time, such as a specific instance of part-milling occurring at a specific timestamp.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ProcessOccurrenceComponent() { Type = TypeId; }
    }
}