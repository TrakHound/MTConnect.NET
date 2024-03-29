// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Path is a Component that represents the information for an independent operation or function within a Controller.
    /// </summary>
    public class PathComponent : Component 
    {
        public const string TypeId = "Path";
        public const string NameId = "path";
        public new const string DescriptionText = "Path is a Component that represents the information for an independent operation or function within a Controller.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public PathComponent()  { Type = TypeId; }
    }
}