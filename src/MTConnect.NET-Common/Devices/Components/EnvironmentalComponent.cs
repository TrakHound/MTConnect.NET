// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Environmental is an Auxiliary that represents the information for a unit or function involved in monitoring, managing, or conditioning the environment around or within a piece of equipment.
    /// </summary>
    public class EnvironmentalComponent : Component 
    {
        public const string TypeId = "Environment";
        public const string NameId = "env";

        public override string TypeDescription => "Environmental is an Auxiliary that represents the information for a unit or function involved in monitoring, managing, or conditioning the environment around or within a piece of equipment.";

        public EnvironmentalComponent()  { Type = TypeId; }
    }
}
