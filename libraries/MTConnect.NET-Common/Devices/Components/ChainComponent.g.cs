// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106462_246830_44378

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public class ChainComponent : Component
    {
        public const string TypeId = "Chain";
        public const string NameId = "chain";
        public new const string DescriptionText = "Leaf Component composed of interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ChainComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}