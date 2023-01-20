// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Any mechanism for producing a current of air.
    /// </summary>
    public class FanComposition : Composition 
    {
        public const string TypeId = "FAN";
        public const string NameId = "fan";
        public new const string DescriptionText = "Any mechanism for producing a current of air.";

        public override string TypeDescription => DescriptionText;


        public FanComposition()  { Type = TypeId; }
    }
}