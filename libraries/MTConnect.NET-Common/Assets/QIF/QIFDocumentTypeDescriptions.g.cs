// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.QIF
{
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