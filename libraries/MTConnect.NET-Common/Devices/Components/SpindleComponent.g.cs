// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_4_45f01b9_1643678227814_87818_1410

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that provides an axis of rotation for the purpose of rapidly rotating a part or a tool to provide sufficient surface speed for cutting operations.Spindle was **DEPRECATED** in *MTConnect Version 1.1* and was replaced by RotaryMode.
    /// </summary>
    public class SpindleComponent : Component
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Component.
        /// </summary>
        public const string TypeId = "Spindle";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Component.
        /// </summary>
        public const string NameId = "spindle";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Component that provides an axis of rotation for the purpose of rapidly rotating a part or a tool to provide sufficient surface speed for cutting operations.Spindle was **DEPRECATED** in *MTConnect Version 1.1* and was replaced by RotaryMode.";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The maximum MTConnect Version that this Component is valid for; set when the type has been deprecated.
        /// </summary>
        public override System.Version MaximumVersion => MTConnectVersions.Version11;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Component.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version10;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public SpindleComponent()
        {
            Type = TypeId;
        }
    }
}
