// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A tool storage location associated with a ToolMagazine or AutomaticToolChanger.
    /// </summary>
    public class PotComposition : Composition 
    {
        public const string TypeId = "POT";
        public const string NameId = "pot";
        public new const string DescriptionText = "A tool storage location associated with a ToolMagazine or AutomaticToolChanger.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public PotComposition()  { Type = TypeId; }
    }
}
