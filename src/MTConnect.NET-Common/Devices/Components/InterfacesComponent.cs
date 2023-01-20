// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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