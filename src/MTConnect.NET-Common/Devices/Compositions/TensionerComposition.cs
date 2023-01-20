// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that provides or applies a stretch or strain to another mechanism.
    /// </summary>
    public class TensionerComposition : Composition 
    {
        public const string TypeId = "TENSIONER";
        public const string NameId = "ten";
        public new const string DescriptionText = "A mechanism that provides or applies a stretch or strain to another mechanism.";

        public override string TypeDescription => DescriptionText;


        public TensionerComposition()  { Type = TypeId; }
    }
}