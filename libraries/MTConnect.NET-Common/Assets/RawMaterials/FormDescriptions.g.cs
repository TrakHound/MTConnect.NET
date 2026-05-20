// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.RawMaterials
{
    /// <summary>
    /// Description text for each <see cref="Form"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class FormDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public const string BAR = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string BLOCK = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string CASTING = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string FILAMENT = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string GAS = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string GEL = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string LIQUID = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string POWDER = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string SHEET = "";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="Form"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(Form value)
        {
            switch (value)
            {
                case Form.BAR: return "";
                case Form.BLOCK: return "";
                case Form.CASTING: return "";
                case Form.FILAMENT: return "";
                case Form.GAS: return "";
                case Form.GEL: return "";
                case Form.LIQUID: return "";
                case Form.POWDER: return "";
                case Form.SHEET: return "";
            }

            return null;
        }
    }
}