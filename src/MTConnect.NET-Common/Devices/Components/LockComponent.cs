// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Lock is a Component that represents a mechanism which physically prohibits a device or component from opening or operating.
    /// </summary>
    public class LockComponent : Component 
    {
        public const string TypeId = "Lock";
        public const string NameId = "lock";
        public new const string DescriptionText = "Lock is a Component that represents a mechanism which physically prohibits a device or component from opening or operating.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version18;


        public LockComponent()  { Type = TypeId; }
    }
}