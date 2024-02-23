// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605547467172_656422_264

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Part that exists at a specific place and time, such as a specific instance of a bracket at a specific timestamp.
    /// </summary>
    public class PartOccurrenceComponent : Component
    {
        public const string TypeId = "PartOccurrence";
        public const string NameId = "partOccurrence";
        public new const string DescriptionText = "Part that exists at a specific place and time, such as a specific instance of a bracket at a specific timestamp.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public PartOccurrenceComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}