// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Resources organizes Resource component types.
    /// </summary>
    public class ResourcesComponent : Component 
    {
        public const string TypeId = "Resources";
        public const string NameId = "resources";
        public new const string DescriptionText = "Resources organizes Resource component types.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public ResourcesComponent()  { Type = TypeId; }
    }
}