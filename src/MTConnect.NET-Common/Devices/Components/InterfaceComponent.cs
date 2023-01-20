// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Interface is a Component that coordinates actions and activities between pieces of equipment.
    /// </summary>
    public class InterfaceComponent : Component 
    {
        public const string TypeId = "Interface";
        public const string NameId = "int";
        public new const string DescriptionText = "Interface is a Component that coordinates actions and activities between pieces of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public InterfaceComponent()  { Type = TypeId; }
    }
}