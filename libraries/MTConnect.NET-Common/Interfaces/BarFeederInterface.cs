// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Interface that coordinates the operations between a bar feeder and another piece of equipment.
    /// Bar feeder is a piece of equipment that pushes bar stock (i.e., long pieces of material of various shapes) into an associated piece of equipment - most typically a lathe or turning center.
    /// </summary>
    public class BarFeederInterface : Interface
    {
        /// <summary>
        /// The fixed Component type identifier ("BarFeederInterface") written to the Type attribute and used to recognize this Interface during deserialization.
        /// </summary>
        public const string TypeId = "BarFeederInterface";

        /// <summary>
        /// The lowercase element name ("barFeeder") used when this Interface is emitted in lower-camel-case document formats.
        /// </summary>
        public const string NameId = "barFeeder";

        /// <summary>
        /// The canonical single-line human-readable description of this Interface; surfaced through <see cref="TypeDescription"/>.
        /// </summary>
        public new const string DescriptionText = "Interface that coordinates the operations between a bar feeder and another piece of equipment. Bar feeder is a piece of equipment that pushes bar stock (i.e., long pieces of material of various shapes) into an associated piece of equipment - most typically a lathe or turning center.";

        /// <summary>
        /// Returns the human-readable description for this Interface (<see cref="DescriptionText"/>).
        /// </summary>
        public override string TypeDescription => DescriptionText;

        /// <summary>
        /// The earliest MTConnect Standard version (1.3) in which the BarFeederInterface type is defined.
        /// </summary>
        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        /// <summary>
        /// Initializes a new BarFeederInterface, stamping the Component Type with <see cref="TypeId"/>.
        /// </summary>
        public BarFeederInterface()
        {
            Type = TypeId;
        }
    }
}
