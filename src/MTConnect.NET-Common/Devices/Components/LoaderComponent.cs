// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Loader is an Auxiliary comprised of all the parts involved in moving and distributing materials, parts, tooling, and other items to or from a piece of equipment.
    /// </summary>
    public class LoaderComponent : Component 
    {
        public const string TypeId = "Loader";
        public const string NameId = "load";
        public new const string DescriptionText = "Loader is an Auxiliary comprised of all the parts involved in moving and distributing materials, parts, tooling, and other items to or from a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public LoaderComponent()  { Type = TypeId; }
    }
}