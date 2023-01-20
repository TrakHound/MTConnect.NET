// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Systems organizes System component types
    /// </summary>
    public class SystemsComponent : Component 
    {
        public const string TypeId = "Systems";
        public const string NameId = "sys";
        public new const string DescriptionText = "Systems organizes System component types";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public SystemsComponent()  { Type = TypeId; }
    }
}