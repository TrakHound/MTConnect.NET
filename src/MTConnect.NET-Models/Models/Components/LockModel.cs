// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Lock is a Component that represents a mechanism which physically prohibits a device or component from opening or operating.
    /// </summary>
    public class LockModel : ComponentModel
    {
        public LockModel() 
        {
            Type = LockComponent.TypeId;
        }

        public LockModel(string componentId)
        {
            Id = componentId;
            Type = LockComponent.TypeId;
        }
    }
}
