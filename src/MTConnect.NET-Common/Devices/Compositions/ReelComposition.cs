// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A rotary storage unit for material.
    /// </summary>
    public class ReelComposition : Composition 
    {
        public const string TypeId = "REEL";
        public const string NameId = "reel";
        public new const string DescriptionText = "A rotary storage unit for material.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public ReelComposition()  { Type = TypeId; }
    }
}