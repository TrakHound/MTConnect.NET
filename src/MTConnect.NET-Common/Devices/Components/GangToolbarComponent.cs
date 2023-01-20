// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// GangToolBar is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools.
    /// Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by linearly positioning the GangToolBar.
    /// </summary>
    public class GangToolbarComponent : Component 
    {
        public const string TypeId = "GangToolbar";
        public const string NameId = "gtoolbar";
        public new const string DescriptionText = "GangToolBar is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools. Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by linearly positioning the GangToolBar.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public GangToolbarComponent()  { Type = TypeId; }
    }
}