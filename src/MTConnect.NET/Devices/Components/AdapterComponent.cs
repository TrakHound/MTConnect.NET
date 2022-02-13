// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Adapter is a Component that represents the connectivity state of a data source for the MTConnect Agent.
    /// </summary>
    public class AdapterComponent : Component 
    {
        public const string TypeId = "Adapter";
        public const string NameId = "adapter";
        public new const string DescriptionText = "Adapter is a Component that represents the connectivity state of a data source for the MTConnect Agent.";

        public override string TypeDescription => DescriptionText;


        public AdapterComponent()  { Type = TypeId; }
    }
}
