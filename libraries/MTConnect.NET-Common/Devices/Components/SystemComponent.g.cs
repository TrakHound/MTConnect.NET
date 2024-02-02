// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382020_336298_42297

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Abstract Component that is permanently integrated into the piece of equipment.
    /// </summary>
    public abstract class SystemComponent : Component
    {
        public const string TypeId = "System";
        public const string NameId = "system";
        public new const string DescriptionText = "Abstract Component that is permanently integrated into the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public SystemComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}