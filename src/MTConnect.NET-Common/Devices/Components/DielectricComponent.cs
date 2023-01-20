// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Dielectric is a System that represents the information for a system that manages a chemical mixture used in a manufacturing
    /// process being performed at that piece of equipment.For example, this could describe
    /// the dielectric system for an EDM process or the chemical bath used in a plating process.
    /// </summary>
    public class DielectricComponent : Component 
    {
        public const string TypeId = "Dielectric";
        public const string NameId = "dielectric";
        public new const string DescriptionText = "Dielectric is a System that represents the information for a system that manages a chemical mixture used in a manufacturing process being performed at that piece of equipment.For example, this could describe the dielectric system for an EDM process or the chemical bath used in a plating process.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public DielectricComponent()  { Type = TypeId; }
    }
}
