// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.CuttingTools
{
    public static class ToolLifeDescriptions
    {
        /// <summary>
        /// Indicates if the tool life counts from zero to maximum or maximum to zero.
        /// </summary>
        public const string CountDirection = "Indicates if the tool life counts from zero to maximum or maximum to zero.";
        
        /// <summary>
        /// Initial life of the tool when it is new.
        /// </summary>
        public const string Initial = "Initial life of the tool when it is new.";
        
        /// <summary>
        /// End of life limit for the tool.
        /// </summary>
        public const string Limit = "End of life limit for the tool.";
        
        /// <summary>
        /// Type of tool life being accumulated.
        /// </summary>
        public const string Type = "Type of tool life being accumulated.";
        
        /// <summary>
        /// Value of ToolLife.
        /// </summary>
        public const string Value = "Value of ToolLife.";
        
        /// <summary>
        /// Point at which a tool life warning will be raised.
        /// </summary>
        public const string Warning = "Point at which a tool life warning will be raised.";
    }
}