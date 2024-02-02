// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382000_589988_42261

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that provides distribution and management of fluids used to lubricate portions of the piece of equipment.
    /// </summary>
    public class LubricationComponent : Component
    {
        public const string TypeId = "Lubrication";
        public const string NameId = "lubrication";
        public new const string DescriptionText = "System that provides distribution and management of fluids used to lubricate portions of the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public LubricationComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}