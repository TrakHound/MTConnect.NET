// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.
    /// </summary>
    public class PulleyComposition : Composition 
    {
        public const string TypeId = "PULLEY";
        public const string NameId = "pulley";

        public PulleyComposition()  { Type = TypeId; }
    }
}
