// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381991_562093_42243

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that manages the delivery of materials within a piece of equipment.
    /// </summary>
    public class FeederComponent : Component
    {
        public const string TypeId = "Feeder";
        public const string NameId = "feeder";
        public new const string DescriptionText = "System that manages the delivery of materials within a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public FeederComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}