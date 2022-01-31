// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism to measure position.
    /// </summary>
    public class EncoderComposition : Composition 
    {
        public const string TypeId = "ENCODER";
        public const string NameId = "enc";

        public EncoderComposition()  { Type = TypeId; }
    }
}
