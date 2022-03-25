// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Assets
{
    public class XmlAssetCollection : IXmlSerializable
    {
        [XmlIgnore]
        public List<IAsset> Assets { get; set; }


        public XmlAssetCollection() { Assets = new List<IAsset>(); }

        public XmlAssetCollection(IAsset asset)
        {
            Assets = new List<IAsset>();
            if (asset != null) Assets.Add(asset);
        }

        public XmlAssetCollection(IEnumerable<IAsset> assets)
        {
            Assets = new List<IAsset>();
            if (!assets.IsNullOrEmpty()) Assets.AddRange(assets);
        }


        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            if (!Assets.IsNullOrEmpty())
            {
                foreach (var asset in Assets)
                {
                    var xml = asset.ToXml();
                    if (!string.IsNullOrEmpty(xml))
                    {
                        writer.WriteRaw(xml);
                    }
                }
            }
        }

        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read Child Elements
                using (var inner = reader.ReadSubtree())
                {
                    while (inner.Read())
                    {
                        if (inner.NodeType == XmlNodeType.Element)
                        {
                            // Create a copy of each Child Node so we can change the name to "Asset" and deserialize it
                            // (Seems like a dirty way to do this but until an XmlAttribute can be found to ignore the Node's name/type
                            // and to always deserialize as a Asset)
                            var doc = new XmlDocument();
                            var node = doc.ReadNode(inner);

                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Element)
                                {
                                    //XmlNode childNode = null;

                                    var t = Asset.GetAssetType(child.Name);
                                    if (t != null)
                                    {
                                        // Create new XmlSerializer using the Type defined above
                                        var serializer = new XmlSerializer(t);

                                        // Create new Asset
                                        var asset = (IAsset)serializer.Deserialize(new XmlNodeReader(child));
                                        asset.Type = child.Name;
                                        asset.Xml = child.OuterXml;
                                        Assets.Add(asset);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            // Advance Reader
            reader.Skip();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion
    }
}
