// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism that transforms electric energy from a source to a secondary circuit.
    /// </summary>
    public class TransformerComposition : Composition 
    {
        public const string TypeId = "TRANSFORMER";
        public const string NameId = "tran";
        public new const string DescriptionText = "A mechanism that transforms electric energy from a source to a secondary circuit.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public TransformerComposition()  { Type = TypeId; }
    }
}
