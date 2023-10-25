// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605552257415_810787_2672

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a storage or mounting location for a tool associated with a Turret, GangToolBar, or ToolRack.
    /// </summary>
    public class StationComponent : Component
    {
        public const string TypeId = "Station";
        public const string NameId = "stationComponent";
        public new const string DescriptionText = "Leaf Component composed of a storage or mounting location for a tool associated with a Turret, GangToolBar, or ToolRack.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public StationComponent() { Type = TypeId; }
    }
}