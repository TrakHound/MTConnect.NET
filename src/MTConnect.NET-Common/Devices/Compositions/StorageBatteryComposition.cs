// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A component consisting of one or more cells, in which chemical energy is converted into electricity and used as a source of power.
    /// </summary>
    public class StorageBatteryComposition : Composition 
    {
        public const string TypeId = "STORAGE_BATTERY";
        public const string NameId = "stbatt";
        public new const string DescriptionText = "A component consisting of one or more cells, in which chemical energy is converted into electricity and used as a source of power.";

        public override string TypeDescription => DescriptionText;


        public StorageBatteryComposition()  { Type = TypeId; }
    }
}