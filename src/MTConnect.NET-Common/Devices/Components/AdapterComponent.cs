// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public AdapterComponent()  { Type = TypeId; }
    }
}