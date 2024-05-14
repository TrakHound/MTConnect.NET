// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572382021_741508_42300

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliary that manages, positions, stores, and delivers tooling within a piece of equipment.
    /// </summary>
    public class ToolingDeliveryComponent : Component
    {
        public const string TypeId = "ToolingDelivery";
        public const string NameId = "toolingDelivery";
        public new const string DescriptionText = "Auxiliary that manages, positions, stores, and delivers tooling within a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ToolingDeliveryComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}