// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Link is a Structure providing a connection between Components.
    /// </summary>
    public class LinkComponent : Component 
    {
        public const string TypeId = "Link";
        public const string NameId = "link";
        public new const string DescriptionText = "Link is a Structure providing a connection between Components.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public LinkComponent()  { Type = TypeId; }
    }
}