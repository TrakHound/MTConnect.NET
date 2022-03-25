// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    /// <summary>
    /// The Asset Information Model associates each electronic MTConnectAssets document with a unique
    /// identifier and allows for some predefined mechanisms to find, create, request, updated, and delete these
    /// electronic documents in a way that provides for consistency across multiple pieces of equipment.
    /// </summary>
    [XmlRoot("MTConnectAssets")]
    public class XmlAssetsDocument
    {
        /// <summary>
        /// Contains the Header information in an MTConnect Assets XML document
        /// </summary>
        [XmlElement("Header")]
        public IMTConnectAssetsHeader Header { get; set; }

        /// <summary>
        /// An XML container that consists of one or more types of Asset XML elements.
        /// </summary>
        [XmlElement("Assets")]
        public XmlAssetCollection AssetCollection { get; set; }


        public XmlAssetsDocument() { }

        public XmlAssetsDocument(IAssetsResponseDocument assetsDocument)
        {
            if (assetsDocument != null)
            {
                Header = assetsDocument.Header;

                // Add Assets
                if (!assetsDocument.Assets.IsNullOrEmpty())
                {
                    AssetCollection = new XmlAssetCollection
                    {
                        Assets = assetsDocument.Assets.ToList()
                    };
                }
                else AssetCollection = new XmlAssetCollection();
            }
        }


        public AssetsResponseDocument ToAssetsDocument()
        {
            var assetsDocument = new AssetsResponseDocument();

            assetsDocument.Header = Header;

            // Add Assets
            if (AssetCollection != null && !AssetCollection.Assets.IsNullOrEmpty())
            {
                assetsDocument.Assets = AssetCollection.Assets.ToList();
            }
            else assetsDocument.Assets = new List<IAsset>();

            return assetsDocument;
        }
    }
}
