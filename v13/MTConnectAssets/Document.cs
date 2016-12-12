// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MTConnect.v13.MTConnectAssets.CuttingTools;

namespace MTConnect.v13.MTConnectAssets
{
    [XmlRoot("MTConnectAssets", Namespace = NAMESPACE)]
    public class Document
    {
        [XmlIgnore]
        public const string NAMESPACE = "urn:mtconnect.org:MTConnectAssets:1.3";

        public Document() { }

        public static Document Create(string xml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Document));
                using (var textReader = new StringReader(xml))
                using (var xmlReader = XmlReader.Create(textReader))
                {
                    return (Document)serializer.Deserialize(xmlReader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        [XmlElement("Header")]
        public Headers.MTConnectAssetsHeader Header { get; set; }

        [XmlArray("Assets")]
        [XmlArrayItem("CuttingTool", typeof(CuttingTool))]
        public List<Asset> Assets { get; set; }
    }
}
