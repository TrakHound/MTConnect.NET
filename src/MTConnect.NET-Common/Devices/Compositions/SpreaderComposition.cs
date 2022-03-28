// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A mechanism for flattening or spreading materials.
    /// </summary>
    public class SpreaderComposition : Composition 
    {
        public const string TypeId = "SPREADER";
        public const string NameId = "spread";
        public new const string DescriptionText = "A mechanism for flattening or spreading materials.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        public SpreaderComposition()  { Type = TypeId; }
    }
}
