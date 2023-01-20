// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Coolant is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids that remove heat from a piece of equipment.
    /// </summary>
    public class CoolantComponent : Component 
    {
        public const string TypeId = "Coolant";
        public const string NameId = "coolant";
        public new const string DescriptionText = "Coolant is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids that remove heat from a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version12;


        public CoolantComponent()  { Type = TypeId; }
    }
}