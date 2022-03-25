// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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


        public LockComponent()  { Type = TypeId; }
    }
}
