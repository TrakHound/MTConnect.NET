// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381990_149427_42240

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that observes the surroundings of another Component.> Note: Environmental **SHOULD** be organized by Auxillaries, Systems or Parts depending on the relationship to the Component.
    /// </summary>
    public class EnvironmentalComponent : Component
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Component.
        /// </summary>
        public const string TypeId = "Environmental";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Component.
        /// </summary>
        public const string NameId = "environmental";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Component that observes the surroundings of another Component.> Note: Environmental **SHOULD** be organized by Auxillaries, Systems or Parts depending on the relationship to the Component.";

        /// <summary>
        /// The description of this Component as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Component.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public EnvironmentalComponent()
        {
            Type = TypeId;
        }
    }
}
