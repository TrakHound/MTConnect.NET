// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Interface that coordinates the operations between a piece of equipment and another associated piece of equipment used to automatically handle various types of materials or services associated with the original piece of equipment.
    /// </summary>
    public class MaterialHandlerInterface : Interface
    {
        /// <summary>
        /// The fixed Component type identifier ("MaterialHandlerInterface") written to the Type attribute and used to recognize this Interface during deserialization.
        /// </summary>
        public const string TypeId = "MaterialHandlerInterface";

        /// <summary>
        /// The lowercase element name ("materialHandler") used when this Interface is emitted in lower-camel-case document formats.
        /// </summary>
        public const string NameId = "materialHandler";

        /// <summary>
        /// The canonical single-line human-readable description of this Interface; surfaced through <see cref="TypeDescription"/>.
        /// </summary>
        public new const string DescriptionText = "Interface that coordinates the operations between a piece of equipment and another associated piece of equipment used to automatically handle various types of materials or services associated with the original piece of equipment.";

        /// <summary>
        /// Returns the human-readable description for this Interface (<see cref="DescriptionText"/>).
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The earliest MTConnect Standard version (1.3) in which the MaterialHandlerInterface type is defined.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        /// <summary>
        /// Initializes a new MaterialHandlerInterface, stamping the Component Type with <see cref="TypeId"/>.
        /// </summary>
        public MaterialHandlerInterface()
        {
            Type = TypeId;
        }
    }
}
