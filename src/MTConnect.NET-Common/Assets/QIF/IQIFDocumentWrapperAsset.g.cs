// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.QIF
{
    /// <summary>
    /// Asset that carries the QIF Document.
    /// </summary>
    public partial interface IQIFDocumentWrapperAsset : IAsset
    {
        /// <summary>
        /// QIF Document as given by the QIF standard.
        /// </summary>
        MTConnect.Assets.QIF.IQIFDocument QIFDocument { get; }
        
        /// <summary>
        /// Contained QIF Document type as defined in the QIF Standard.
        /// </summary>
        MTConnect.Assets.QIF.QIFDocumentType QifDocumentType { get; }
    }
}