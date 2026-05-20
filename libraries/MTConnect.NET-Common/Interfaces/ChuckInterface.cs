// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Interface that coordinates the operations between two pieces of equipment, one of which controls the operation of a chuck.
    /// The piece of equipment that is controlling the chuck MUST provide the data item ChuckState as part of the set of information provided.
    /// </summary>
    public class ChuckInterface : Interface
    {
        /// <summary>
        /// The fixed Component type identifier ("ChuckInterface") written to the Type attribute and used to recognize this Interface during deserialization.
        /// </summary>
        public const string TypeId = "ChuckInterface";

        /// <summary>
        /// The lowercase element name ("chuck") used when this Interface is emitted in lower-camel-case document formats.
        /// </summary>
        public const string NameId = "chuck";

        /// <summary>
        /// The canonical single-line human-readable description of this Interface; surfaced through <see cref="TypeDescription"/>.
        /// </summary>
        public new const string DescriptionText = "Interface that coordinates the operations between two pieces of equipment, one of which controls the operation of a chuck. The piece of equipment that is controlling the chuck MUST provide the data item ChuckState as part of the set of information provided.";

        /// <summary>
        /// Returns the human-readable description for this Interface (<see cref="DescriptionText"/>).
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The earliest MTConnect Standard version (1.3) in which the ChuckInterface type is defined.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        /// <summary>
        /// Initializes a new ChuckInterface, stamping the Component Type with <see cref="TypeId"/>.
        /// </summary>
        public ChuckInterface()
        {
            Type = TypeId;
        }
    }
}
