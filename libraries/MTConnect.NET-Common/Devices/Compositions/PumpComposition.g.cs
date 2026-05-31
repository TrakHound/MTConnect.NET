// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738877_834141_44728

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an apparatus that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.
    /// </summary>
    public class PumpComposition : Composition 
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Composition.
        /// </summary>
        public const string TypeId = "PUMP";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Composition.
        /// </summary>
        public const string NameId = "pumpComposition";

        /// <summary>
        /// The description of this Composition as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Composition composed of an apparatus that raises, drives, exhausts, or compresses fluids or gases by means of a piston, plunger, or set of rotating vanes.";

        /// <summary>
        /// The description of this Composition as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Composition.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public PumpComposition() { Type = TypeId; }
    }
}