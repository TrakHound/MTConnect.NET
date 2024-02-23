// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1622119541205_751450_1761

namespace MTConnect.Assets.QIF
{
    /// <summary>
    /// Asset that carries the QIF Document.
    /// </summary>
    public partial class QIFDocumentWrapperAsset : Asset, IQIFDocumentWrapperAsset
    {
        public new const string DescriptionText = "Asset that carries the QIF Document.";


        /// <summary>
        /// QIF Document as given by the QIF standard.
        /// </summary>
        public string QIFDocument { get; set; }
        
        /// <summary>
        /// Contained QIF Document type as defined in the QIF Standard.
        /// </summary>
        public MTConnect.Assets.QIF.QIFDocumentType QifDocumentType { get; set; }
    }
}