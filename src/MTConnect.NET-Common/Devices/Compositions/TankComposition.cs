// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A receptacle or container for holding material.
    /// </summary>
    public class TankComposition : Composition 
    {
        public const string TypeId = "TANK";
        public const string NameId = "tank";
        public new const string DescriptionText = "A receptacle or container for holding material.";

        public override string TypeDescription => DescriptionText;


        public TankComposition()  { Type = TypeId; }
    }
}