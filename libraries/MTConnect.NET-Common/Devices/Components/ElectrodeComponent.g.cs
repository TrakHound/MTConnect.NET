// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1744800275968_819729_23778

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliary that is used for many electrical discharge manufacturing processes like welding.
    /// </summary>
    public class ElectrodeComponent : Component
    {
        public const string TypeId = "Electrode";
        public const string NameId = "electrode";
        public new const string DescriptionText = "Auxiliary that is used for many electrical discharge manufacturing processes like welding.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version26; 


        public ElectrodeComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}