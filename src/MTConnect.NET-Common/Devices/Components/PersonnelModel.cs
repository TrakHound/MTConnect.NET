// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Personnel is a Resource that provides information about an individual or individuals who either control, support, or otherwise interface with a piece of equipment.
    /// </summary>
    public class PersonnelComponent : Component 
    {
        public const string TypeId = "Personnel";
        public const string NameId = "per";
        public new const string DescriptionText = "Personnel is a Resource that provides information about an individual or individuals who either control, support, or otherwise interface with a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public PersonnelComponent()  { Type = TypeId; }
    }
}
