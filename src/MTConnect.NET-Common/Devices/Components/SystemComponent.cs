// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System is an abstract Component that represents part(s) of a piece of equipment that is permanently integrated into the piece of equipment.
    /// </summary>
    public class SystemComponent : Component 
    {
        public const string TypeId = "System";
        public const string NameId = "sys";
        public new const string DescriptionText = "System is an abstract Component that represents part(s) of a piece of equipment that is permanently integrated into the piece of equipment.";

        public override string TypeDescription => DescriptionText;


        public SystemComponent()  { Type = TypeId; }
    }
}