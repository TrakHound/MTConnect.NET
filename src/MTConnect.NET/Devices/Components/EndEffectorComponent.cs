// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// EndEffector is a System that represents the information for those functions that form the last link segment of a piece of equipment.
    /// It is the part of a piece of equipment that interacts with the manufacturing process.
    /// </summary>
    public class EndEffectorComponent : Component 
    {
        public const string TypeId = "EndEffector";
        public const string NameId = "endeff";

        public EndEffectorComponent()  { Type = TypeId; }
    }
}
