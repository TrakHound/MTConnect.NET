// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1605552393091_777875_3127

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Pot for a tool removed from spindle or Turret and awaiting for return to a ToolMagazine.
    /// </summary>
    public class ReturnPotComposition : Composition 
    {
        /// <summary>
        /// The MTConnect <c>type</c> value that identifies this Composition.
        /// </summary>
        public const string TypeId = "RETURN_POT";

        /// <summary>
        /// The default <c>name</c> assigned to an instance of this Composition.
        /// </summary>
        public const string NameId = "returnPotComposition";

        /// <summary>
        /// The description of this Composition as defined by the MTConnect Standard.
        /// </summary>
        public new const string DescriptionText = "Pot for a tool removed from spindle or Turret and awaiting for return to a ToolMagazine.";

        /// <summary>
        /// The description of this Composition as defined by the MTConnect Standard.
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The minimum MTConnect Version that introduced this Composition.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        /// <summary>
        /// Initializes a new instance with its <c>Type</c> set to <see cref="TypeId"/>.
        /// </summary>
        public ReturnPotComposition() { Type = TypeId; }
    }
}