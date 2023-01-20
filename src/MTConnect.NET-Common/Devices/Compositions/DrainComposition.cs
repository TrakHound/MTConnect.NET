// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that allows material to flow for the purpose of drainage from, for example, a vessel or tank.
    /// </summary>
    public class DrainComposition : Composition 
    {
        public const string TypeId = "DRAIN";
        public const string NameId = "drain";
        public new const string DescriptionText = "A mechanism that allows material to flow for the purpose of drainage from, for example, a vessel or tank.";

        public override string TypeDescription => DescriptionText;


        public DrainComposition()  { Type = TypeId; }
    }
}