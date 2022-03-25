// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
