// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605101651646_782838_139

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that provides information about the data source for an MTConnect Agent.
    /// </summary>
    public class AdapterComponent : Component
    {
        public const string TypeId = "Adapter";
        public const string NameId = "adapterComponent";
        public new const string DescriptionText = "Component that provides information about the data source for an MTConnect Agent.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public AdapterComponent() { Type = TypeId; }
    }
}