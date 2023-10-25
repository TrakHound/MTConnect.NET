// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_91b028d_1587650651134_415529_403

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of an object or material on which a form of work is performed.
    /// </summary>
    public class WorkpieceComponent : Component
    {
        public const string TypeId = "Workpiece";
        public const string NameId = "workpieceComponent";
        public new const string DescriptionText = "Leaf Component composed of an object or material on which a form of work is performed.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16; 


        public WorkpieceComponent() { Type = TypeId; }
    }
}