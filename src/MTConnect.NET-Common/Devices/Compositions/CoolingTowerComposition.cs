// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A heat exchange system that uses a fluid to transfer heat to the atmosphere.
    /// </summary>
    public class CoolingTowerComposition : Composition 
    {
        public const string TypeId = "COOLING_TOWER";
        public const string NameId = "cooltower";
        public new const string DescriptionText = "A heat exchange system that uses a fluid to transfer heat to the atmosphere.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public CoolingTowerComposition()  { Type = TypeId; }
    }
}