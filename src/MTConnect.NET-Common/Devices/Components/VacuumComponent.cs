// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Vacuum is a System that evacuates gases and liquids from an enclosed and sealed space to a controlled negative pressure or a molecular density below the prevailing atmospheric level.
    /// </summary>
    public class VacuumComponent : Component 
    {
        public const string TypeId = "Vacuum";
        public const string NameId = "vac";
        public new const string DescriptionText = "Vacuum is a System that evacuates gases and liquids from an enclosed and sealed space to a controlled negative pressure or a molecular density below the prevailing atmospheric level.";

        public override string TypeDescription => DescriptionText;


        public VacuumComponent()  { Type = TypeId; }
    }
}
