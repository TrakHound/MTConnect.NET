// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Lubrication is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids used to lubricate portions of the piece of equipment.
    /// </summary>
    public class LubricationComponent : Component 
    {
        public const string TypeId = "Lubrication";
        public const string NameId = "lube";
        public new const string DescriptionText = "Lubrication is a System that represents the information for a system comprised of all the parts involved in distribution and management of fluids used to lubricate portions of the piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version11;


        public LubricationComponent()  { Type = TypeId; }
    }
}