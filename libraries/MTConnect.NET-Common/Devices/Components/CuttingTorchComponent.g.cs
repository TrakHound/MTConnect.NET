// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1744800465544_90322_23856

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliary that employs a concentrated flame to both sever materials through cutting and fuse them together in joining processes.
    /// </summary>
    public class CuttingTorchComponent : Component
    {
        public const string TypeId = "CuttingTorch";
        public const string NameId = "cuttingTorch";
        public new const string DescriptionText = "Auxiliary that employs a concentrated flame to both sever materials through cutting and fuse them together in joining processes.";

        public override string TypeDescription => DescriptionText;
        
         


        public CuttingTorchComponent()
        {
            Type = TypeId;
        }
    }
}
