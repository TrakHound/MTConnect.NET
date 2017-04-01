// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace MTConnect.MTConnectAssets
{
    public class AssetCollection : IXmlSerializable
    {
        [XmlIgnore]
        public List<Asset> Assets { get; set; }


        public AssetCollection() { Assets = new List<Asset>(); }

        #region "Xml Serialization"

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString("");
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
                                    XmlNode childNode = null;
                                    Type t;

                                    if (child.Name == "CuttingTool")
                                    {
                                        t = typeof(CuttingTools.CuttingTool);
                                        childNode = child;
                                    }
                                    else
                                    {
                                        t = typeof(Asset);

                                        // Create a new Node with the name of "Asset"
                                        var copy = doc.CreateNode(XmlNodeType.Element, "Asset", null);

                                        // Copy Attributes
                                        foreach (XmlAttribute attribute in child.Attributes)
                                        {
                                            var attr = doc.CreateAttribute(attribute.Name);
                                            attr.Value = attribute.Value;
                                            copy.Attributes.Append(attr);
                                        }

                                        // Copy Text
                                        copy.InnerText = child.InnerText;
                                        copy.InnerXml = child.InnerXml;
                                        childNode = copy;
                                    }

                                    // Create new XmlSerializer using the Type defined above
                                    var serializer = new XmlSerializer(t);

                                    // Create new Asset
                                    var asset = (Asset)serializer.Deserialize(new XmlNodeReader(childNode));
                                    asset.Type = child.Name;
                                    asset.Xml = child.OuterXml;
                                    Assets.Add(asset);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
