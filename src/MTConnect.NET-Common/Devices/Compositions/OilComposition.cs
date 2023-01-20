// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A viscous liquid.
    /// </summary>
    public class OilComposition : Composition 
    {
        public const string TypeId = "OIL";
        public const string NameId = "oil";
        public new const string DescriptionText = "A viscous liquid.";

        public override string TypeDescription => DescriptionText;


        public OilComposition()  { Type = TypeId; }
    }
}