// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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