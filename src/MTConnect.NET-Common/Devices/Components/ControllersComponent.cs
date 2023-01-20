// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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