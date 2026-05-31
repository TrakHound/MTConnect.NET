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
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Component.
        /// </summary>
        public const string TypeId = "CuttingTorch";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Component.
        /// </summary>
        public const string NameId = "cuttingTorch";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Auxiliary that employs a concentrated flame to both sever materials through cutting and fuse them together in joining processes.";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Component.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version26;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public CuttingTorchComponent()
        {
            Type = TypeId;
        }
    }
}
