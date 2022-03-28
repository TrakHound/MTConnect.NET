// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Interfaces organizes Interface component types.
    /// </summary>
    public class InterfacesComponent : Component 
    {
        public const string TypeId = "Interfaces";
        public const string NameId = "ints";
        public new const string DescriptionText = "Interfaces organizes Interface component types.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public InterfacesComponent()  { Type = TypeId; }
    }
}
