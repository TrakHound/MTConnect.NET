// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.
    /// </summary>
    public class ChainComposition : Composition 
    {
        public const string TypeId = "CHAIN";
        public const string NameId = "chain";
        public new const string DescriptionText = "An interconnected series of objects that band together and are used to transmit motion for a piece of equipment or to convey materials and objects.";

        public override string TypeDescription => DescriptionText;


        public ChainComposition()  { Type = TypeId; }
    }
}