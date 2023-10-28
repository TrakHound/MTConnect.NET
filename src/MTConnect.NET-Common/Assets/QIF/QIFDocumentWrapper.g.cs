// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1622119541205_751450_1761

namespace MTConnect.Assets.QIF
{
    /// <summary>
    /// Asset that carries the QIF Document.
    /// </summary>
    public class QIFDocumentWrapper : Asset, IQIFDocumentWrapper
    {
        public new const string DescriptionText = "Asset that carries the QIF Document.";


        /// <summary>
        /// QIF Document as given by the QIF standard.
        /// </summary>
        public MTConnect.Assets.QIF.IQIFDocument QIFDocument { get; set; }
        
        /// <summary>
        /// Contained QIF Document type as defined in the QIF Standard.
        /// </summary>
        public MTConnect.Assets.QIF.QIFDocumentType QifDocumentType { get; set; }
    }
}