// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism used to strengthen, support, or fasten objects in place.
    /// </summary>
    public class ClampComposition : Composition 
    {
        public const string TypeId = "CLAMP";
        public const string NameId = "clamp";
        public new const string DescriptionText = "A mechanism used to strengthen, support, or fasten objects in place.";

        public override string TypeDescription => DescriptionText;


        public ClampComposition()  { Type = TypeId; }
    }
}