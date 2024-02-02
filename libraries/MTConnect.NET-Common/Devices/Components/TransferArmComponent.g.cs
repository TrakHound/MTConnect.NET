// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605552257830_675148_2688

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that physically moves a tool from one location to another.
    /// </summary>
    public class TransferArmComponent : Component
    {
        public const string TypeId = "TransferArm";
        public const string NameId = "transferArm";
        public new const string DescriptionText = "Leaf Component that physically moves a tool from one location to another.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public TransferArmComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}