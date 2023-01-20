// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that provides a signal or measured value.
    /// </summary>
    public class SensingElementComposition : Composition 
    {
        public const string TypeId = "SENSING_ELEMENT";
        public const string NameId = "senelement";
        public new const string DescriptionText = "A mechanism that provides a signal or measured value.";

        public override string TypeDescription => DescriptionText;


        public SensingElementComposition()  { Type = TypeId; }
    }
}