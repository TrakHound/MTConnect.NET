// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106477_119326_44468

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that provides or applies a stretch or strain to another mechanism.
    /// </summary>
    public class TensionerComponent : Component
    {
        public const string TypeId = "Tensioner";
        public const string NameId = "tensioner";
        public new const string DescriptionText = "Leaf Component that provides or applies a stretch or strain to another mechanism.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public TensionerComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}