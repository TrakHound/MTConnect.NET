// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1622457426342_839834_623

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that physically prohibits a Device or Component from opening or operating.
    /// </summary>
    public class LockComponent : Component
    {
        public const string TypeId = "Lock";
        public const string NameId = "lock";
        public new const string DescriptionText = "Component that physically prohibits a Device or Component from opening or operating.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version18; 


        public LockComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}