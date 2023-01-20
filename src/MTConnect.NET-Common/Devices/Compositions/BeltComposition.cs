// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An endless flexible band used to transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public class BeltComposition : Composition 
    {
        public const string TypeId = "BELT";
        public const string NameId = "belt";
        public new const string DescriptionText = "An endless flexible band used to transmit motion for a piece of equipment or to convey materials and objects.";

        public override string TypeDescription => DescriptionText;


        public BeltComposition()  { Type = TypeId; }
    }
}