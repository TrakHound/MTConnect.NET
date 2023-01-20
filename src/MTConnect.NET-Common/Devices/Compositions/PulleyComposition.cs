// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.
    /// </summary>
    public class PulleyComposition : Composition 
    {
        public const string TypeId = "PULLEY";
        public const string NameId = "pulley";
        public new const string DescriptionText = "A mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.";

        public override string TypeDescription => DescriptionText;


        public PulleyComposition()  { Type = TypeId; }
    }
}