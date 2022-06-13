// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.QIF
{
    /// <summary>
    /// QIFDocumentWrapper is an Asset that carries the Quality Information Framework (QIF) Document.
    /// </summary>
    [XmlRoot("QIFDocumentWrapper")]
    public class QIFDocumentWrapperAsset : Asset
    {
        public const string TypeId = "QIFDocumentWrapper";


        /// <summary>
        /// The contained QIF Document type as defined in the QIF Standard.
        /// </summary>
        [XmlAttribute("qifDocumentType")]
        [JsonPropertyName("qifDocumentType")]
        public string QifDocumentType { get; set; }

        /// <summary>
        /// The QIF Document as defined by the QIF standard.
        /// </summary>
        [XmlElement("qifDocument")]
        [JsonPropertyName("qifDocument")]
        public string QifDocument { get; set; }


        public override IAsset Process(Version mtconnectVersion)
        {
            if (mtconnectVersion != null && mtconnectVersion >= MTConnectVersions.Version18)
            {
                return this;
            }

            return null;
        }

        public override AssetValidationResult IsValid(Version mtconnectVersion)
        {
            var message = "";
            var result = true;

            if (string.IsNullOrEmpty(QifDocument))
            {
                message = "QIFDocument is Required";
                result = false;
            }

            return new AssetValidationResult(result, message);
        }
    }
}
