// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572382004_482583_42267

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that organizes an independent operation or function within a Controller.
    /// </summary>
    public class PathComponent : Component
    {
        public const string TypeId = "Path";
        public const string NameId = "path";
        public new const string DescriptionText = "Component that organizes an independent operation or function within a Controller.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public PathComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}