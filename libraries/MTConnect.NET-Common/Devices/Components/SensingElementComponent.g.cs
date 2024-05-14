// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106474_940737_44450

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that provides a signal or measured value.
    /// </summary>
    public class SensingElementComponent : Component
    {
        public const string TypeId = "SensingElement";
        public const string NameId = "sensingElement";
        public new const string DescriptionText = "Leaf Component that provides a signal or measured value.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public SensingElementComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}