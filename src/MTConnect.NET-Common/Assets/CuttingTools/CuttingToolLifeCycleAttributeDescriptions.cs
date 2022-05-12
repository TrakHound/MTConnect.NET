// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Assets.CuttingTools
{
    public static class CuttingToolLifeCycleAttributeDescriptions
    {
        /// <summary>
        /// The tool group this tool is assigned in the part program.
        /// </summary>
        public const string ProgramToolGroup = "The tool group this tool is assigned in the part program.";

        /// <summary>
        /// The number of the tool as referenced in the part program.
        /// </summary>
        public const string ProgramToolNumber = "The number of the tool as referenced in the part program.";

        /// <summary>
        /// Identifier for the capability to connect any component of the cutting tool together, except assembly items, on the machine side. Code: CCMS
        /// </summary>
        public const string ConnectionCodeMachineSide = "Identifier for the capability to connect any component of the cutting tool together, except assembly items, on the machine side. Code: CCMS";
    }
}
