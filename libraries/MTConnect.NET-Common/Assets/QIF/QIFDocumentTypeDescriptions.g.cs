// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.QIF
{
    /// <summary>
    /// Description text for each <see cref="QIFDocumentType"/> value as defined by the MTConnect Standard.
    /// </summary>
    public static class QIFDocumentTypeDescriptions
    {
        /// <summary>
        /// 
        /// </summary>
        public const string MEASUREMENT_RESOURCE = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string PLAN = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string PRODUCT = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string RESULTS = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string RULES = "";
        
        /// <summary>
        /// 
        /// </summary>
        public const string STATISTICS = "";


        /// <summary>
        /// Returns the MTConnect Standard description text for the specified <see cref="QIFDocumentType"/> value, or <c>null</c> when none is defined.
        /// </summary>
        public static string Get(QIFDocumentType value)
        {
            switch (value)
            {
                case QIFDocumentType.MEASUREMENT_RESOURCE: return "";
                case QIFDocumentType.PLAN: return "";
                case QIFDocumentType.PRODUCT: return "";
                case QIFDocumentType.RESULTS: return "";
                case QIFDocumentType.RULES: return "";
                case QIFDocumentType.STATISTICS: return "";
            }

            return null;
        }
    }
}