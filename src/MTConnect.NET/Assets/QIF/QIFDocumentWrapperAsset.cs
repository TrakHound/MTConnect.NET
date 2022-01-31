// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.QIF
{
    /// <summary>
    /// QIFDocumentWrapper is an Asset that carries the Quality Information Framework (QIF) Document.
    /// </summary>
    [XmlRoot("QIFDocumentWrapper")]
    public class QIFDocumentWrapperAsset : Asset<QIFDocumentWrapperAsset>
    {
        public const string TypeId = "QIFDocumentWrapper";


        /// <summary>
        /// The contained QIF Document type as defined in the QIF Standard.
        /// </summary>
        [XmlAttribute("qifDocumentType")]
        public string QifDocumentType { get; set; }

        /// <summary>
        /// The QIF Document as defined by the QIF standard.
        /// </summary>
        [XmlElement("qifDocument")]
        public string QifDocument { get; set; }
    }
}
