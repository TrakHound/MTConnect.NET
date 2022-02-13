// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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


        public LinkComponent()  { Type = TypeId; }
    }
}
