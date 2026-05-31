// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312738873_845166_44712

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an electromechanical actuator that produces deflection of a beam of light or energy in response to electric current through its coil in a magnetic field.
    /// </summary>
    public class GalvanomotorComposition : Composition 
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Composition.
        /// </summary>
        public const string TypeId = "GALVANOMOTOR";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Composition.
        /// </summary>
        public const string NameId = "galvanomotorComposition";

        /// <summary>
        /// The description of this Composition as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Composition composed of an electromechanical actuator that produces deflection of a beam of light or energy in response to electric current through its coil in a magnetic field.";

        /// <summary>
        /// The description of this Composition as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Composition.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version15;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public GalvanomotorComposition() { Type = TypeId; }
    }
}