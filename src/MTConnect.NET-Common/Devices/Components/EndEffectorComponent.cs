// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

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

        public override string TypeDescription => "EndEffector is a System that represents the information for those functions that form the last link segment of a piece of equipment. It is the part of a piece of equipment that interacts with the manufacturing process.";

        public override System.Version MinimumVersion => MTConnectVersions.Version15;

        public EndEffectorComponent()  { Type = TypeId; }
    }
}