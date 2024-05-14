// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605547395898_219029_214

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Process that takes place at a specific place and time, such as a specific instance of part-milling occurring at a specific timestamp.
    /// </summary>
    public class ProcessOccurrenceComponent : Component
    {
        public const string TypeId = "ProcessOccurrence";
        public const string NameId = "processOccurrence";
        public new const string DescriptionText = "Process that takes place at a specific place and time, such as a specific instance of part-milling occurring at a specific timestamp.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ProcessOccurrenceComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}