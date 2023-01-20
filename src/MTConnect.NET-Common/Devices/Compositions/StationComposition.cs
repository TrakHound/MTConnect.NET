// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A storage or mounting location for a tool associated with a Turret, GangToolBar, or ToolRack.
    /// </summary>
    public class StationComposition : Composition 
    {
        public const string TypeId = "STATION";
        public const string NameId = "station";
        public new const string DescriptionText = "A storage or mounting location for a tool associated with a Turret, GangToolBar, or ToolRack.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public StationComposition()  { Type = TypeId; }
    }
}