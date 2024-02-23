// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106473_162844_44441

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.
    /// </summary>
    public class PulleyComponent : Component
    {
        public const string TypeId = "Pulley";
        public const string NameId = "pulley";
        public new const string DescriptionText = "Leaf Component composed of a mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public PulleyComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}