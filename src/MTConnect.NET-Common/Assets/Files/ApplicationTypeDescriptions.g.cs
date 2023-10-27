// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    public static class ApplicationTypeDescriptions
    {
        /// <summary>
        /// Generic data.
        /// </summary>
        public const string DATA = "Generic data.";
        
        /// <summary>
        /// Computer aided design files or drawings.
        /// </summary>
        public const string DESIGN = "Computer aided design files or drawings.";
        
        /// <summary>
        /// Documentation regarding a category of file.
        /// </summary>
        public const string DOCUMENTATION = "Documentation regarding a category of file.";
        
        /// <summary>
        /// User instructions regarding the execution of a task.
        /// </summary>
        public const string INSTRUCTIONS = "User instructions regarding the execution of a task.";
        
        /// <summary>
        /// Data related to the history of a machine or process.
        /// </summary>
        public const string LOG = "Data related to the history of a machine or process.";
        
        /// <summary>
        /// Machine instructions to perform a process.
        /// </summary>
        public const string PRODUCTION_PROGRAM = "Machine instructions to perform a process.";


        public static string Get(ApplicationType value)
        {
            switch (value)
            {
                case ApplicationType.DATA: return "Generic data.";
                case ApplicationType.DESIGN: return "Computer aided design files or drawings.";
                case ApplicationType.DOCUMENTATION: return "Documentation regarding a category of file.";
                case ApplicationType.INSTRUCTIONS: return "User instructions regarding the execution of a task.";
                case ApplicationType.LOG: return "Data related to the history of a machine or process.";
                case ApplicationType.PRODUCTION_PROGRAM: return "Machine instructions to perform a process.";
            }

            return null;
        }
    }
}