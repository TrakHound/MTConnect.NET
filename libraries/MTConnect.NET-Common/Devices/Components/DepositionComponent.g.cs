// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381981_362854_42222

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliary that manages the addition of material or state change of material being performed in an additive manufacturing process.
    /// </summary>
    public class DepositionComponent : Component
    {
        public const string TypeId = "Deposition";
        public const string NameId = "deposition";
        public new const string DescriptionText = "Auxiliary that manages the addition of material or state change of material being performed in an additive manufacturing process.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public DepositionComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}