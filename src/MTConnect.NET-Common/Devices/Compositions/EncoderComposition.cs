// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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