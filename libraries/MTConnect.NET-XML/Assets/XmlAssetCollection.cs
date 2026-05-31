// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.Xml.CuttingTools;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTConnect.Assets.Xml
{
    /// <summary>
    /// XML serialization helper for the <c>Assets</c> container of an
    /// MTConnectAssets document. Holds the strongly-typed assets and writes or
    /// reads them as their type-named child elements, dispatching to the
    /// per-type asset surrogates.
    /// </summary>
    public class XmlAssetCollection
    {
        private readonly bool _indentOutput = false;


        /// <summary>
        /// The assets contained in the collection.
        /// </summary>
        [XmlArray("Assets")]
        [XmlArrayItem(typeof(XmlCuttingToolAsset), ElementName = "CuttingTool")]
        public List<IAsset> Assets { get; set; }


        /// <summary>
        /// Creates an empty collection.
        /// </summary>
        public XmlAssetCollection() { Assets = new List<IAsset>(); }

        /// <summary>
        /// Creates a collection containing the single given asset, recording
        /// the indentation preference for output.
        /// </summary>
        public XmlAssetCollection(IAsset asset, bool indentOutput = false)
        {
            _indentOutput = indentOutput;
            Assets = new List<IAsset>();
            if (asset != null) Assets.Add(asset);
        }

        /// <summary>
        /// Creates a collection containing the given assets, recording the
        /// indentation preference for output.
        /// </summary>
        public XmlAssetCollection(IEnumerable<IAsset> assets, bool indentOutput = false)
        {
            _indentOutput = indentOutput;
            Assets = new List<IAsset>();
            if (!assets.IsNullOrEmpty()) Assets.AddRange(assets);
        }


        #region "Xml Serialization"

        /// <summary>
        /// Writes each contained asset to <paramref name="writer"/> by
        /// dispatching to its concrete asset surrogate.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            if (!Assets.IsNullOrEmpty())
            {
                for (var i = 0; i < Assets.Count; i++)
                {
                    var asset = Assets[i];
                    XmlAsset.WriteXml(writer, asset);


                    //var xml = XmlAsset.ToXml(asset, _indentOutput);
                    //if (!string.IsNullOrEmpty(xml))
                    //{
                    //    if (_indentOutput)
                    //    {
                    //        writer.WriteWhitespace(XmlFunctions.NewLine);

                    //        // Manually Indent
                    //        var lines = xml.Split(XmlFunctions.NewLineCharacter);
                    //        foreach (var line in lines)
                    //        {
                    //            writer.WriteWhitespace(XmlFunctions.Tab);
                    //            writer.WriteWhitespace(XmlFunctions.Tab);
                    //            writer.WriteRaw(line);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        writer.WriteRaw(xml);
                    //    }
                    //}
                }
            }
        }

        //public void WriteXml(XmlWriter writer)
        //{
        //    if (!Assets.IsNullOrEmpty())
        //    {
        //        for (var i = 0; i < Assets.Count; i++)
        //        {
        //            var asset = Assets[i];
        //            var xml = XmlAsset.ToXml(asset, _indentOutput);
        //            if (!string.IsNullOrEmpty(xml))
        //            {
        //                if (_indentOutput)
        //                {
        //                    writer.WriteWhitespace(XmlFunctions.NewLine);

        //                    // Manually Indent
        //                    var lines = xml.Split(XmlFunctions.NewLineCharacter);
        //                    foreach (var line in lines)
        //                    {
        //                        writer.WriteWhitespace(XmlFunctions.Tab);
        //                        writer.WriteWhitespace(XmlFunctions.Tab);
        //                        writer.WriteRaw(line);
        //                    }
        //                }
        //                else
        //                {
        //                    writer.WriteRaw(xml);
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Reads the asset child elements from <paramref name="reader"/>,
        /// resolving each element name to its asset type and deserializing it
        /// into the collection.
        /// </summary>
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
                                    var t = Asset.GetAssetType(child.Name);
                                    if (t != null)
                                    {
                                        // Create new XmlSerializer using the Type defined above
                                        var serializer = new XmlSerializer(t);

                                        // Create new Asset
                                        var asset = (IAsset)serializer.Deserialize(new XmlNodeReader(child));
                                        asset.Type = child.Name;
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

        /// <summary>
        /// Returns <c>null</c>; the collection does not advertise an inline XML
        /// schema, as required by <see cref="IXmlSerializable"/>.
        /// </summary>
        public XmlSchema GetSchema()
        {
            return (null);
        }

        #endregion

    }
}