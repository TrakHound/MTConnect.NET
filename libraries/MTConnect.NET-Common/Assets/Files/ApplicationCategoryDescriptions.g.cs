// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.Files
{
    public static class ApplicationCategoryDescriptions
    {
        /// <summary>
        /// Files regarding the fully assembled product.
        /// </summary>
        public const string ASSEMBLY = "Files regarding the fully assembled product.";
        
        /// <summary>
        /// Device related files.
        /// </summary>
        public const string DEVICE = "Device related files.";
        
        /// <summary>
        /// Files relating to the handling of material.
        /// </summary>
        public const string HANDLING = "Files relating to the handling of material.";
        
        /// <summary>
        /// Files related to the quality inspection.
        /// </summary>
        public const string INSPECTION = "Files related to the quality inspection.";
        
        /// <summary>
        /// Files relating to equipment maintenance.
        /// </summary>
        public const string MAINTENANCE = "Files relating to equipment maintenance.";
        
        /// <summary>
        /// Files relating to a part.
        /// </summary>
        public const string PART = "Files relating to a part.";
        
        /// <summary>
        /// Files related to the manufacturing process.
        /// </summary>
        public const string PROCESS = "Files related to the manufacturing process.";
        
        /// <summary>
        /// Files related to the setup of a process.
        /// </summary>
        public const string SETUP = "Files related to the setup of a process.";


        public static string Get(ApplicationCategory value)
        {
            switch (value)
            {
                case ApplicationCategory.ASSEMBLY: return "Files regarding the fully assembled product.";
                case ApplicationCategory.DEVICE: return "Device related files.";
                case ApplicationCategory.HANDLING: return "Files relating to the handling of material.";
                case ApplicationCategory.INSPECTION: return "Files related to the quality inspection.";
                case ApplicationCategory.MAINTENANCE: return "Files relating to equipment maintenance.";
                case ApplicationCategory.PART: return "Files relating to a part.";
                case ApplicationCategory.PROCESS: return "Files related to the manufacturing process.";
                case ApplicationCategory.SETUP: return "Files related to the setup of a process.";
            }

            return null;
        }
    }
}