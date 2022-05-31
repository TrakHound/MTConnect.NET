// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organizes Controller entities.
    /// </summary>
    public class ControllersComponent : Component 
    {
        public const string TypeId = "Controllers";
        public const string NameId = "controllers";
        public new const string DescriptionText = "Component that organizes Controller entities.";

        public override string TypeDescription => DescriptionText;


        public ControllersComponent()  { Type = TypeId; }
    }
}
