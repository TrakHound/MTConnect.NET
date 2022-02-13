// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Pneumatic is a System that uses compressed gasses to actuate components or do work within the piece of equipment.
    /// </summary>
    public class PneumaticComponent : Component 
    {
        public const string TypeId = "Pneumatic";
        public const string NameId = "air";
        public new const string DescriptionText = "Pneumatic is a System that uses compressed gasses to actuate components or do work within the piece of equipment.";

        public override string TypeDescription => DescriptionText;


        public PneumaticComponent()  { Type = TypeId; }
    }
}
