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
        /// The MTConnect version of the document the assets are written into, which decides where
        /// version-dependent elements go; <c>null</c> uses the 2.6+ layout. Not serialized.
        /// </summary>
        [XmlIgnore]
        public System.Version MTConnectVersion { get; set; }


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
        /// dispatching to its concrete asset surrogate, laid out for
        /// <see cref="MTConnectVersion"/>.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            if (!Assets.IsNullOrEmpty())
            {
                for (var i = 0; i < Assets.Count; i++)
                {
                    XmlAsset.WriteXml(writer, Assets[i], MTConnectVersion);
                }
            }
        }

        /// <summary>
        /// Reads the asset child elements from <paramref name="reader"/>,
        /// resolving each element name to its asset type and deserializing it
        /// into the collection.
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            try
            {
                // Read Child Elements: each asset element is deserialized directly from the reader
                // (its element name selects the asset type and matches that type's root element)
                using (var inner = reader.ReadSubtree())
                {
                    if (inner.Read() && inner.NodeType == XmlNodeType.Element && !inner.IsEmptyElement)
                    {
                        var depth = inner.Depth + 1;
                        inner.Read();

                        while (!inner.EOF)
                        {
                            if (inner.NodeType == XmlNodeType.Element && inner.Depth == depth)
                            {
                                var name = inner.Name;
                                var t = Asset.GetAssetType(name);
                                if (t != null)
                                {
                                    IAsset asset;
                                    using (var assetReader = inner.ReadSubtree())
                                    {
                                        asset = (IAsset)XmlAsset.GetSerializer(t).Deserialize(assetReader);
                                    }

                                    if (asset != null)
                                    {
                                        asset.Type = name;
                                        Assets.Add(asset);
                                    }

                                    inner.Read(); // Past the asset's end element
                                }
                                else
                                {
                                    inner.Skip();
                                }
                            }
                            else
                            {
                                inner.Read();
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